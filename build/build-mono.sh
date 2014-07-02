#!/bin/bash
git submodule init
git submodule update
mozroots --import --ask-remove
xbuild CoiniumServ.sln
