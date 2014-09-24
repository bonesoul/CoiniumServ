#!/bin/bash
cd ../..
git submodule init
git submodule update
mozroots --import --ask-remove
xbuild build/CoiniumServ.sln /p:Configuration="Release"
mono contrib/xunit/xunit.console.clr4.x86.exe src/Tests/bin/Release/CoiniumServ.Tests.dll
