#!/bin/bash
git submodule init
git submodule update
mozroots --import --ask-remove
cd ..
xbuild CoiniumServ.sln
