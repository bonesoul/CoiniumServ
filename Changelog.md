##### [v0.1.1 alpha - Piri Reis](https://github.com/CoiniumServ/CoiniumServ/releases/tag/v0.1.1-alpha) - 10.08.2014
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


##### [v0.1.0 alpha - Piri Reis](https://github.com/CoiniumServ/CoiniumServ/releases/tag/v0.1.0-alpha) - 08.08.2014

Initial release which is fully functional with a basic feature set.
