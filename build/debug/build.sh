#!/bin/bash
cd ../..
git submodule init
git submodule update
xbuild build/CoiniumServ.sln /p:Configuration="Debug"
mono contrib/xunit/xunit.console.clr4.x86.exe src/Tests/bin/Debug/CoiniumServ.Tests.dll
