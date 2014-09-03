##### [v0.1.4 alpha](https://github.com/CoiniumServ/CoiniumServ/releases/tag/v0.1.4-alpha) - 03.09.2014

**Storage**
* Downgraded csredis package at it was causing problems with mono.

##### [v0.1.3 alpha](https://github.com/CoiniumServ/CoiniumServ/releases/tag/v0.1.3-alpha) - 03.09.2014

**Storage**
* Implemented storage layers support; hybrid-storage (redis+mysql) and mpos compatibility (mysql).
* Major changes in storage configuration - you need to update per-pool configuration files.
* Added migration support, CoiniumServ will manage required tables on it's own in hybrid-storage mode.
 
**Coin**
* Fixed a bug with coin's that was returning non-standard version reply in getinfo().
* Added initial proof-of-stake coin support.
* Added automatic detection support for proof-of-stake coins.

**Statistics & API**
* Re-developed statistics & api sub-system from stratch.
* Pool api now can expose more details.
* Fixed a bug where authenticated miner count was reported incorrectly.
 
**Web**
* Improved index page layout.

**Misc**
* Updated dependency packages.
* File path handling improvements.
* Fixed app.config.

##### [v0.1.2 alpha](https://github.com/CoiniumServ/CoiniumServ/releases/tag/v0.1.2-alpha) - 14.08.2014

**Payments**
* Fixed a major bug in payment processor which was preventing payments to miners.
* Fixed a bug in statistics manager.

##### [v0.1.1 alpha](https://github.com/CoiniumServ/CoiniumServ/releases/tag/v0.1.1-alpha) - 10.08.2014
**Mining**
* Improved SocketServiceContext and removed unnecessary overhead.
* New block found message is no more sent to miners as the miner software can already determine itself.
* Added TxMessage support.
 
**Web**
* Added /pool/{slug}/round and /pool/{slug}/workers pages.
* Respectively added /api/pool/{slug}/round and /api/pool/{slug}/workers pages.
* Pools drop down in web front-end now correctly work.

**Bug fixes**
* Fixed a major bug with stratum service where multiple json-rpc calls couldn't be parsed.
* Fixed a bug effecting block submissions with some coins that does not include an address field in transaction outputs.
* Fixed a bug with stratum-miner packet logger where it was emitting normal log messages.
* Config, log and web front-end source files are no more searching in current working directory but looked for relatively to assembly location.
* Updated some log messages to be more verbose.
* LogManager can now handle unauthorized access exceptions.
* Misc vanilla miner fixes.
* Applied a fix to json-prettifier, it'll now handle exceptions.
* ShareManager and PaymenProcessor will now also honor orphan blocks with -1 confirmations.
* Double.parse() and float.parse() calls will now correctly use CultureInfo.InvariantCulture - basically fixing potential problems with non en-US systems.


##### [v0.1.0 alpha](https://github.com/CoiniumServ/CoiniumServ/releases/tag/v0.1.0-alpha) - 08.08.2014

Initial release which is fully functional with a basic feature set.
