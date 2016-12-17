# compile the project in release mode.
$dotNetVersion = "4.0"
$regKey = "HKLM:\software\Microsoft\MSBuild\ToolsVersions\$dotNetVersion"
$regProperty = "MSBuildToolsPath"
$msbuildExe = join-path -path (Get-ItemProperty $regKey).$regProperty -childpath "msbuild.exe"
&$msbuildExe ../../build/CoiniumServ.sln /p:Configuration=Release /t:rebuild /p:DebugSymbols=false /p:DebugType=None /p:AllowedReferenceRelatedFileExtensions=none

# run the tests.
../../contrib/xunit/xunit.console.clr4.x86.exe ../../src/Tests/bin/Release/CoiniumServ.Tests.dll