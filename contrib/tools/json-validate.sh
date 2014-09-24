#!/bin/bash
npm install jsonlint -g
for i in $(find ../../src/CoiniumServ/config/coins/*.json -type f); do
    echo "validating $i"
    cat $i | jsonlint -c | grep error
done

