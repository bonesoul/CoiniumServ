///////////////////////////////////////////////////////////////////////////////
// Directives
///////////////////////////////////////////////////////////////////////////////

#l "tools/versionUtils.cake"
#l "tools/settingsUtils.cake"
#tool "nuget:?package=NUnit.ConsoleRunner"
#tool "nuget:?package=xunit.runner.console"

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var settings = SettingsUtils.LoadSettings(Context);
var versionInfo = VersionUtils.LoadVersion(Context, settings);

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var solutions = GetFiles(settings.Build.SolutionFilePath);
var solutionPaths = solutions.Select(solution => solution.GetDirectory());

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup((c) =>
{
	// Executed BEFORE the first task.
	settings.Display(c);
	versionInfo.Display(c);
});

Teardown((c) =>
{
	// Executed AFTER the last task.
	Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("CleanAll")
	.Description("Cleans all directories that are used during the build process.")
	.Does(() =>
{
	// Clean solution directories.
	foreach(var path in solutionPaths)
	{
		Information("Cleaning {0}", path);
		CleanDirectories(path + "/**/bin");
		CleanDirectories(path + "/**/obj");
		CleanDirectories(path + "/packages/**/*");
		CleanDirectories(path + "/artifacts/**/*");
		CleanDirectories(path + "/packages");
		CleanDirectories(path + "/artifacts");
	}
	
	var pathTest = MakeAbsolute(Directory(settings.Test.SourcePath)).FullPath;
	Information("Cleaning {0}", pathTest);
	try { CleanDirectories(pathTest + "/**/bin"); } catch {}
	try { CleanDirectories(pathTest + "/**/obj"); } catch {}
});

Task("Clean")
	.Description("Cleans all directories that are used during the build process.")
	.WithCriteria(settings.ExecuteBuild)
	.Does(() =>
{
	// Clean solution directories.
	foreach(var path in solutionPaths)
	{
		Information("Cleaning {0}", path);
		try { CleanDirectories(path + "/**/bin/" + settings.Configuration); } catch {}
		try { CleanDirectories(path + "/**/obj/" + settings.Configuration); } catch {}
	}

	var pathTest = MakeAbsolute(Directory(settings.Test.SourcePath)).FullPath;
	Information("Cleaning {0}", pathTest);
	try { CleanDirectories(pathTest + "/**/bin/" + settings.Configuration); } catch {}
	try { CleanDirectories(pathTest + "/**/obj/" + settings.Configuration); } catch {}
	
});

Task("CleanPackages")
	.Description("Cleans all packages that are used during the build process.")
	.Does(() =>
{
	// Clean solution directories.
	foreach(var path in solutionPaths)
	{
		Information("Cleaning {0}", path);
		CleanDirectories(path + "/packages/**/*");
		CleanDirectories(path + "/packages");
	}
});

Task("Restore")
	.Description("Restores all the NuGet packages that are used by the specified solution.")
	.WithCriteria(settings.ExecuteBuild)
	.Does(() =>
{
	// Restore all NuGet packages.
	foreach(var solution in solutions)
	{
		Information("Restoring {0}...", solution);
		NuGetRestore(solution, new NuGetRestoreSettings { ConfigFile = settings.NuGet.NuGetConfig });
	}
});

Task("Build")
	.Description("Builds all the different parts of the project.")
	.WithCriteria(settings.ExecuteBuild)
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.IsDependentOn("UpdateVersion")
	.Does(() =>
{
	if (settings.Version.AutoIncrementVersion)
	{
		RunTarget("IncrementVersion");
	}

	// Build all solutions.
	foreach(var solution in solutions)
	{
		Information("Building {0}", solution);
		try {
			MSBuild(solution, s =>
				s.SetPlatformTarget(PlatformTarget.MSIL)
					.SetMaxCpuCount(settings.Build.MaxCpuCount)
					.WithProperty("TreatWarningsAsErrors",settings.Build.TreatWarningsAsErrors.ToString())
					.WithTarget("Build")
					.SetConfiguration(settings.Configuration));
		} 
		catch (Exception ex)
		{
			Error("Files to build project: " + solution + ". Error: " + ex.Message);
		}
	}
});

Task("UnitTest")
	.Description("Run unit tests for the solution.")
	.WithCriteria(settings.ExecuteUnitTest)
	.IsDependentOn("Build")
	.Does(() => 
{
	// Run all unit tests we can find.
			
	var assemplyFilePath = string.Format("{0}/**/bin/{1}/{2}", settings.Test.SourcePath, settings.Configuration, settings.Test.AssemblyFileSpec);
	
	Information("Unit Test Files: {0}", assemplyFilePath);
	
	var unitTestAssemblies = GetFiles(assemplyFilePath);
	
	foreach(var uta in unitTestAssemblies)
	{
		Information("Executing Tests for {0}", uta);
		
		switch (settings.Test.Framework)
		{
			case TestFrameworkTypes.NUnit2:
				NUnit(uta.ToString(), new NUnitSettings { });
				break;
			case TestFrameworkTypes.NUnit3:
				NUnit3(uta.ToString(), new NUnit3Settings { Configuration=settings.Configuration });
				break;
			case TestFrameworkTypes.XUnit:
				XUnit(uta.ToString(), new XUnitSettings { OutputDirectory = settings.Test.ResultsPath });
				break;
			case TestFrameworkTypes.XUnit2:
				XUnit2(uta.ToString(), new XUnit2Settings { OutputDirectory = settings.Test.ResultsPath, XmlReportV1 = true });
				break;
		}
	}
});

Task("Package")
	.Description("Packages all nuspec files into nupkg packages.")
	.WithCriteria(settings.ExecutePackage)
	.IsDependentOn("UnitTest")
	.Does(() =>
{
	var artifactsPath = Directory(settings.NuGet.ArtifactsPath);
	var nugetProps = new Dictionary<string, string>() { {"Configuration", settings.Configuration} };
		
	CreateDirectory(artifactsPath);
	
	var nuspecFiles = GetFiles(settings.NuGet.NuSpecFileSpec);
	foreach(var nsf in nuspecFiles)
	{
		Information("Packaging {0}", nsf);
		
		if (settings.NuGet.UpdateVersion) {
			VersionUtils.UpdateNuSpecVersion(Context, settings, versionInfo, nsf.ToString());	
		}
		
		if (settings.NuGet.UpdateLibraryDependencies) {
			VersionUtils.UpdateNuSpecVersionDependency(Context, settings, versionInfo, nsf.ToString());
		}
		
		NuGetPack(nsf, new NuGetPackSettings {
			Version = versionInfo.ToString(),
			ReleaseNotes = versionInfo.ReleaseNotes,
			Symbols = true,
			Properties = nugetProps,
			OutputDirectory = artifactsPath
		});
	}
});

Task("Publish")
	.Description("Publishes all of the nupkg packages to the nuget server. ")
	.IsDependentOn("Package")
	.Does(() =>
{
	var authError = false;
	
	if (settings.NuGet.FeedApiKey.ToLower() == "local")
	{
		settings.NuGet.FeedUrl = Directory(settings.NuGet.FeedUrl).Path.FullPath;
		//Information("Using Local repository: {0}", settings.NuGet.FeedUrl);
	}
		
	Information("Publishing Packages from {0} to {1} for version {2}", settings.NuGet.ArtifactsPath, settings.NuGet.FeedUrl, versionInfo.ToString());

	// Lets get the list of packages (we can skip anything that is not part of the current version being built)
	var nupkgFiles = GetFiles(settings.NuGet.NuGetPackagesSpec).Where(x => x.ToString().Contains(versionInfo.ToString())).ToList();

	Information("\t{0}", string.Join("\n\t", nupkgFiles.Select(x => x.GetFilename().ToString()).ToList()));
	
	foreach (var n in nupkgFiles)
	{
		try
		{		
			NuGetPush(n, new NuGetPushSettings {
				Source = settings.NuGet.FeedUrl,
				ApiKey = settings.NuGet.FeedApiKey,
				ConfigFile = settings.NuGet.NuGetConfig,
				Verbosity = NuGetVerbosity.Normal
			});
		}
		catch (Exception ex)
		{
			Information("\tFailed to published: ", ex.Message);
			
			if (ex.Message.Contains("403")) { authError = true; }
		}
	}
	
	if (authError && settings.NuGet.FeedApiKey == "VSTS")
	{
		Warning("\tYou may need to Configuration Your Credentials.\r\n\t\tCredentialProvider.VSS.exe -Uri {0}", settings.NuGet.FeedUrl);
	}
});

Task("UnPublish")
	.Description("UnPublishes all of the current nupkg packages from the nuget server. Issue: versionToDelete must use : instead of . due to bug in cake")
	.Does(() =>
{
	var v = Argument<string>("versionToDelete", versionInfo.ToString()).Replace(":",".");
	
	var nuspecFiles = GetFiles(settings.NuGet.NuSpecFileSpec);
	foreach(var f in nuspecFiles)
	{
		Information("UnPublishing {0}", f.GetFilenameWithoutExtension());

		var args = string.Format("delete {0} {1} -Source {2} -NonInteractive", 
								f.GetFilenameWithoutExtension(),
								v,
								settings.NuGet.FeedUrl
								);
	
		//if (settings.NuGet.FeedApiKey != "VSTS" ) {
			args = args + string.Format(" -ApiKey {0}", settings.NuGet.FeedApiKey);
		//}
				
		if (!string.IsNullOrEmpty(settings.NuGet.NuGetConfig)) {
			args = args + string.Format(" -Config {0}", settings.NuGet.NuGetConfig);
		}
		
		Information("NuGet Command Line: {0}", args);
		using (var process = StartAndReturnProcess("tools\\nuget.exe", new ProcessSettings {
			Arguments = args
		}))
		{
			process.WaitForExit();
			Information("nuget delete exit code: {0}", process.GetExitCode());
		}
	}
});

Task("UpdateVersion")
	.Description("Updates the version number in the necessary files")
	.Does(() =>
{
	Information("Updating Version to {0}", versionInfo.ToString());
	
	VersionUtils.UpdateVersion(Context, settings, versionInfo);
});

Task("IncrementVersion")
	.Description("Increments the version number and then updates it in the necessary files")
	.Does(() =>
{
	var oldVer = versionInfo.ToString();
	if (versionInfo.IsPreRelease) versionInfo.PreRelease++; else versionInfo.Build++;
	
	Information("Incrementing Version {0} to {1}", oldVer, versionInfo.ToString());
	
	RunTarget("UpdateVersion");	
});

Task("BuildNewVersion")
	.Description("Increments and Builds a new version")
	.IsDependentOn("IncrementVersion")
	.IsDependentOn("Build")
	.Does(() =>
{
});

Task("PublishNewVersion")
	.Description("Increments, Builds, and publishes a new version")
	.IsDependentOn("BuildNewVersion")
	.IsDependentOn("Publish")
	.Does(() =>
{
});

Task("DisplaySettings")
	.Description("Displays All Settings.")
	.Does(() =>
{
	// Settings will be displayed as they are part of the Setup task
});

Task("DisplayHelp")
	.Description("Displays All Settings.")
	.Does(() =>
{
	// Settings will be displayed as they are part of the Setup task
	SettingsUtils.DisplayHelp(Context);
});

///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
	.Description("This is the default task which will be ran if no specific target is passed in.")
	.IsDependentOn("Build");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(settings.Target);