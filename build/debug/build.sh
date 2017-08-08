#!/bin/bash
cd ../..
git submodule init
git submodule update
mozroots --import --ask-remove
xbuild src/CoiniumServ/CoiniumServ.sln /p:Configuration="Debug"
mono contrib/xunit/xunit.console.clr4.x86.exe src/Tests/bin/Debug/CoiniumServ.Tests.dll
