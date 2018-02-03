### v0.2.6.3 beta
* Minor fixes & improvements.

### v0.2.6.2 beta
* Minor fixes.

### v0.2.6.1 beta
* Minor fixes.

### v0.2.5 beta
* Minor improvements.

### [v0.2.4 beta](https://github.com/CoiniumServ/CoiniumServ/releases/tag/v0.2.4-beta)

**Features**
* Implemented basic market data support initially from Cryptsy, Bittrex and Poloniex.
 
**Improvements**
* Marked miner connection, disconnection, share-submission log messages as debug level, so server.log file by default doesn't get spammed with them.
* Improved exception handlers for daemon connections.

**Bug Fixes**
* Fixed a bug in hybrid-storage where Block.Accounted and Payment.Completed fields default values was not set correctly.
* Fixed a bug in generation-transaction - version is now correctly set.
* Fixed a bug in Payment Processor which was causing crashes for invalid addresses.

**Web**
* Added robots.txt
* Added custom.css and custom.js for easier CSS and javascript additions.
* Added analytics.html for easier addition of tracker codes.
* Fixed a bug that was affecting mono based systems where favicon was not correctly rendered.

---

### [v0.2.3 beta](https://github.com/CoiniumServ/CoiniumServ/releases/tag/v0.2.3-beta)

**Web**
* Implemented FAQ page.
* Pool page now renders an notice when pool's daemon connection is un-healthy.
* Pools list & per-pool pages do now show the last found block for the pool.
* /help/termsofservice has been moved to /tos.
* Time zones in embedded front-end are localized & rendered as relative.
* Removed social icons configuration - social icons can be now enabled by editing navbar.cshtml.

---

### [v0.2.2 beta](https://github.com/CoiniumServ/CoiniumServ/releases/tag/v0.2.2-beta)

**Bug Fixes**
* Fixed compilation problems caused by json-rpc package.
* Difficulty in web front-end is now shortened & humanized.
* Added submitblock() detection support - so that coins like USD-e without submitblock() methods are also supported.

---

### [v0.2.1 beta](https://github.com/CoiniumServ/CoiniumServ/releases/tag/v0.2.1-beta)

**Bug Fixes**
* Fixed a bug in stratum service where some miners were unable to connect back after they got disconnected.

---

### [v0.2.0 beta](https://github.com/CoiniumServ/CoiniumServ/releases/tag/v0.2.0-beta)

v0.2.0 is the last major release before our auto-exchange & multi-pool feature with many fixes and improvements. You can find out the details below;

#### What's new?

* Improved support for POW + POS hybrids and peercoin variants.
* Improved & re-developed payment manager.
* New web frontend theme.
* Improved web frontend's functionality.

#### Changelog

**Algorithms**
* Added scrypt-og, sha1, qubit, nist5 and fresh support.

**Coin**
* Improved support for POW + POS hybrid coins.
* Added support for peercoin and variants.
* Improved coin configuration [file format](https://github.com/CoiniumServ/CoiniumServ/blob/master/src/CoiniumServ/config/coins/README.md).
* Added working block explorer's for most coins.

**Payments**
* Re-developed payments manager from stratch with base-work for upcoming auto-exchange feature.
* Multiple payments can now be sent using sendmany() together in once.

**Database**
* Hybrid-storage can now track users, payouts and transactions.

**Web**
* Fixed misc. bugs in embedded web-server, it can now correctly function both in debug and release mode.
* The front-end now uses a brand new bootstrap based theme which is also mobile-friendly.
* Implemented "Getting Started pages for miners.
* Implemented "Mining Software" pages for miners.
* Implemented "Terms of Service" page.
* Front-end can now render algorithms, blocks, payments and transactions in details.
* Added a template option to config.json - website section so additional themes can be used.
* Added partial views to front-end allowing easier edits.
* Added support for social icons to be rendered in frontend.
* Live template edits are now possible in debug mode.
* Embedded web-server can now listen on all available network interfaces.

**Blocks**
* Improved handling of newly found blocks. 
* Fixed an integer overflow bug within in block pooler.

**RPC**
* Improved handling of coin daemon rpc errors.

**Configuration**
* Removed comment lines from json configuration files so they can be validated (using jsonlint.com or so).
* Added timeout option for daemon rpc connections (by default 5 seconds).
* Added node configuration support to config.json stack section.
* Added more error checks for json config file loaders.

**Logging**
* Removed console.log configuration as it'll be always on by default.
* Packet.log configuration is now correctly honored.
 
---

### [v0.1.5 alpha](https://github.com/CoiniumServ/CoiniumServ/releases/tag/v0.1.5-alpha)

**Payments**
* Fixed a bug in hybrid-storage layer where blocks were not correctly set as confirmed once they were actually so.

**Web**
* Fixed a bug in embedded web-server where some users were not able to use the interface.
* Updated web-site tempaltes which reflects newest API changes.

**Storage**
* Fixed a bug in migration-manager where if it couldn't connect to MySQL would cause program to crash & terminate.

**API**
* Improved pool API.
 
**Pools**
* Pools can now determine if connection to coin network is healthy.
* Fixed hashrate calculation bug.

**Jobs**
* JobTracker can now clean expired jobs.
 
**Configuration**
* Moved config.json "website:stats" section to upper level and renamed as "statistics". You have to apply the change to your existing config.json file!

---

### [v0.1.4 alpha](https://github.com/CoiniumServ/CoiniumServ/releases/tag/v0.1.4-alpha)

**Storage**
* Downgraded csredis package at it was causing problems with mono.

---

### [v0.1.3 alpha](https://github.com/CoiniumServ/CoiniumServ/releases/tag/v0.1.3-alpha)

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

---

### [v0.1.2 alpha](https://github.com/CoiniumServ/CoiniumServ/releases/tag/v0.1.2-alpha)

**Payments**
* Fixed a major bug in payment processor which was preventing payments to miners.
* Fixed a bug in statistics manager.

---

### [v0.1.1 alpha](https://github.com/CoiniumServ/CoiniumServ/releases/tag/v0.1.1-alpha)
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

---

### [v0.1.0 alpha](https://github.com/CoiniumServ/CoiniumServ/releases/tag/v0.1.0-alpha)

Initial release which is fully functional with a basic feature set.
