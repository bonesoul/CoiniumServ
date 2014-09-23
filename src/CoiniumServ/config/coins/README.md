# Coin definition file format

Json files in this folder are used for defining coin properties so, CoiniumServ can use them. If you can't find an existing file for a coin, make sure too [here](https://github.com/zone117x/node-open-mining-portal/tree/master/coins) to get a sample config.

Sample file;
```json
{
    "name": "Litecoin",
    "symbol": "LTC",
    "algorithm": "scrypt",
	"site": "https://litecoin.org/",
    "reward": "pow",
	"txMessages": false,
    "blockExplorer": {
        "block": "http://block-explorer.com/block/",
        "tx": "http://block-explorer.com/tx/",
        "address": "http://block-explorer.com/address/"
    },
    "getBlockTemplate": {
        "modeRequired": false
    },
    "node": {
        "magic": "fbc0b6db",
        "testnetMagic": "fcc1b7dc"
    },
	"extra": {
	}
}
```

Fields;
- __name__: name of the coin
- __symbol__: symbol of the coin
- __algorithm__: algorithm used by the coin
- __site__: coins official site
- reward: reward system of the coin - possible values; 
  - pow (proof-of-work) [default]
  - pos (proof-of-work + proof-of-stake hybrid)
- __txMessages__: does the coin use transaction messages? False by default.

__blockExplorer__ options
- block: URL that block hash will be appended.
- tx: URL that x will be appended.
- address: URL that address will be appended.
 
__getBlockTemplate__ options
- "modeRequired": Peercoin variants need a "mode" parameter to be set in in getblocktemplate, so we need to set this true for them. Default: false.

__node__ options

Not in use yet, will be used once embedded daemon-node is developed which will be able to check for block notifications

- __magic__: Coin's magic string.
- __testnetMagic__: Magic string for Testnet.

__extra__ options

Any extra options that will be handled to algorithm itself.
