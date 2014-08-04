#!/bin/bash
cd ..
git submodule init
git submodule update
mozroots --import --ask-remove
xbuild /p:Configuration=Release CoiniumServ.sln
