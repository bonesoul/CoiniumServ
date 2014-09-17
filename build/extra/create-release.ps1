$dotNetVersion = "4.0"
$regKey = "HKLM:\software\Microsoft\MSBuild\ToolsVersions\$dotNetVersion"
$regProperty = "MSBuildToolsPath"
$msbuildExe = join-path -path (Get-ItemProperty $regKey).$regProperty -childpath "msbuild.exe"
&$msbuildExe ../CoiniumServ.sln /p:Configuration=Release /t:rebuild /p:DebugSymbols=false /p:DebugType=None /p:AllowedReferenceRelatedFileExtensions=none
cp ../../src/CoiniumServ/bin/Nancy.ViewEngines.Razor.dll ../../bin/Release/Nancy.ViewEngines.Razor.dll