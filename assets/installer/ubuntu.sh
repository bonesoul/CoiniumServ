#!/bin/bash
echo "Making sure git and mono is installed.."
sudo apt-get -y --force-yes install git-core mono-complete
echo "Cloning CoiniumServ.."
git clone https://github.com/CoiniumServ/CoiniumServ.git
cd CoiniumServ
echo "Cloning submodules.."
git submodule init
git submodule update
echo "Building CoiniumServ.."
cd build
./build-mono.sh 
cd ..
echo "CoiniumServ build done.."
