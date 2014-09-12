# ![Screenshot](http://coinium.org/assets/images/logo/coinium-icon-48.png) CoiniumServ 
[![Build Status](https://travis-ci.org/CoiniumServ/CoiniumServ.svg?branch=develop)](https://travis-ci.org/CoiniumServ/CoiniumServ) [![Build status](https://ci.appveyor.com/api/projects/status/3x349ig9dt14943t)](https://ci.appveyor.com/project/raistlinthewiz/coiniumserv) [![Stories in In Progress](https://badge.waffle.io/CoiniumServ/CoiniumServ.png?label=in%20progress&title=In%20Progress)](http://waffle.io/CoiniumServ/CoiniumServ) [![Project Stats](https://www.openhub.net/p/CoiniumServ/widgets/project_thin_badge.gif)](https://www.openhub.net/p/CoiniumServ)
 
[CoiniumServ](http://www.coiniumserv.com) is a high performance, extremely efficient, platform-agnostic, easy to setup pool server implementation. It features stratum and vanilla services, reward, payment, share processors, vardiff & ban managers, user-friendly embedded web-server & front-end and a full-stack API.

Even better multi-pool & auto-exchange module is being developed which once completed will allow you to setup pools that can pay users in any crypto-currency that can be traded over exchanges.

CoiniumServ was created to be used for [Coinium.org](http://www.coinium.org) mining pool network at first hand. You can check [some of pools](https://github.com/CoiniumServ/CoiniumServ/wiki/Pools) of the pools running CoiniumServ.

![CoiniumServ running over mono & ubuntu](http://i.imgur.com/HvaPVrZ.png)

### User Support

Start by reading our [FAQ](https://github.com/CoiniumServ/CoiniumServ/wiki/FAQ) and [wiki](https://github.com/CoiniumServ/CoiniumServ/wiki/). If you need further help, join us over our user-support channel [#coiniumserv@freenode](http://webchat.freenode.net/?channels=%23coiniumserv&prompt=1&uio=OT10cnVlde).

You can also use our [issues](https://github.com/CoiniumServ/CoiniumServ/issues) page to report bugs.

* Official site: [CoiniumServ.com](http://www.coiniumserv.com)
* [Paid support & consulting options](https://github.com/CoiniumServ/CoiniumServ#consulting)
* [Support forums](http://forum.coinium.org/forum/19-support/)
* IRC (**irc.freenode.net**):
  - **#coiniumserv** [user support](http://webchat.freenode.net/?channels=%23coiniumserv&prompt=1&uio=OT10cnVlde)
  - **#coiniumserv-dev** [dev talk](http://webchat.freenode.net/?channels=%23coiniumserv-dev&prompt=1&uio=OT10cnVlde)
  - **#coinium** [official pools](http://webchat.freenode.net/?channels=%23coinium&prompt=1&uio=OT10cnVlde)
* [Twitter](http://twitter.com/coinium)
* [Bitcointalk.org](https://bitcointalk.org/index.php?topic=604476.0)

### Support the project

You can contribute the development of the project by donating; 

* BTC: `18qqrtR4xHujLKf9oqiCsjmwmH5vGpch4D`
* LTC: `LMXfRb3w8cMUBfqZb6RUkFTPaT6vbRozPa`
* DOGE: `DM8FW8REMHj3P4xtcMWDn33ccjikCWJnQr`
* RDD: `Rb9kcLs96VDHTmiXVjcWC2RBsfCJ73UQyr`

If you would like to automatically donate a percentage of your pool's earning to support the project, check the [donation setup](https://github.com/CoiniumServ/CoiniumServ/wiki/Donation) guide.

###### Donors

Here is a list of our generous donors that keep the project ongoing;

* [reddapi.com](https://www.reddapi.com)

###### Bounties

You can set bounties for the issues [here](https://www.bountysource.com/trackers/401667-coiniumserv). You can set bounties and solve them.

[![Bountysource](https://api.bountysource.com/badge/team?team_id=760&style=bounties_received)](https://www.bountysource.com/teams/coinium/issues?utm_source=Coinium&utm_medium=shield&utm_campaign=TEAM_BADGE_1)

###### Tips

You can send tips and furher support the project or get tips for contributing by commiting.

[![tip for next commit](http://tip4commit.com/projects/760.svg)](http://tip4commit.com/projects/760)

### Status

Latest release: [v0.1.5 alpha](https://github.com/CoiniumServ/CoiniumServ/releases/tag/v0.1.5-alpha)

### Features

###### Platform Agnostic
Can run on these platforms;
* Linux/Unix over [mono](http://www.mono-project.com/).
* MacOS over [mono](http://www.mono-project.com/).
* Windows over [.Net](http://www.microsoft.com/net).

###### Multiplexed Structure
* Multiple pools & ports.
* Multiple coin daemon connections.
* Multiple storage layers.
 
###### Auto Exchange & Multipool
* Being developed, stay tuned!

###### Protocols
* Stratum
 * show_message support
 * block template support
 * generation transaction support
 * transaction message support
* Vanilla (getwork) [experimental]

###### Coins
* proof-of-work (PoW) support
* proof-of-stake (PoS) support

###### Connectivity

* Daemon RPC interface
 
###### Managers

* Job manager
* Ban manager (that can handle miners flooding with invalid shares)

###### Processors

* Share processor
* Payment processor
 
###### Additional

* VarDiff support (variable difficulty support)
* Storage layers support
 * hybrid storage (redis + mysql)
 * MPOS support (mysql)
 
###### Web
* Embedded web-server & front-end
* Full-stack json-API

###### Hashing Algorithms

_Working_
* ✓ Scrypt

_Needs Testing_

* ✓ SHA256
* ✓ multi-algos: X11, X13, X14, X15, X17
* ✓ more: Blake, Fresh, Fugue, Groestl, Keccak, NIST5, Scrypt-OG, SHA1, SHAvite3, Skein, Qubit

_Under Development_

* ✓ Scrypt-Jane, Scrypt-N, Quark, Hefty1 
 
###### Persistance & Storage Layers

CoiniumServ supports storage layer interfaces that you can extend to implement your own persistance logic. By default, it supports two layers; a high performance hybrid layer and mpos compatibility layer.

* __Hybrid Layer__: a custom hybrid layer that utilizes redis + mysql together that is carefully designed for high performance persistance support.
* __MPOS Layer__: a compatibility layer based on mysql that supports MPOS whenever you want payments to be handled by MPOS.

###### Development Model
* We have implemented extensive [tests](https://github.com/CoiniumServ/CoiniumServ/tree/develop/src/Tests) for all important functionality and never merge in code that breaks tests and stuff. Yet again, when a new functionality is introduced we also expect proper tests to be implemented within the PR. In simple words, most probably you won't notice any functionality-breaking changes within the repository.
* A strict ruleset for the [Development Model](https://github.com/CoiniumServ/CoiniumServ/wiki/Development-Model). You can follow our bleeding-edge [Develop](https://github.com/CoiniumServ/CoiniumServ) branch or stay with-in the stable [Master](https://github.com/CoiniumServ/CoiniumServ/tree/master) branch.
   

### Getting Started

Make sure you check our [Getting Started](https://github.com/CoiniumServ/CoiniumServ/wiki/Getting-Started) guide for installation instructions for *nix and Windows.

### Documentation

* [Wiki](https://github.com/CoiniumServ/CoiniumServ/wiki/)
* [FAQ](https://github.com/CoiniumServ/CoiniumServ/wiki/FAQ)
* [Master Plan](https://github.com/CoiniumServ/CoiniumServ/wiki/Master-Plan)

### Contributing

Start reading by these;

* [Developer's Guide](https://github.com/CoiniumServ/CoiniumServ/wiki/Developer's-Guide)
* [Technical Documentation](https://github.com/CoiniumServ/CoiniumServ/wiki/Technical-Documentation)

### Consulting

Additional to free [support](https://github.com/CoiniumServ/CoiniumServ#support) methods, we offer paid remote support & consulting services for whom would like to get professional support. Contact us over [here](http://www.coiniumserv.com/support/consulting/) and we will get back to you to discuss your needs.

### License

Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org - 
http://www.coiniumserv.com

This software is dual-licensed: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

For the terms of this license, see [licenses/gpl_v3.txt](https://github.com/CoiniumServ/CoiniumServ/blob/develop/licenses/gpl_v3.txt).

Alternatively, you can license this software under a commercial
license or white-label it as set out in [licenses/commercial.md](https://github.com/CoiniumServ/CoiniumServ/blob/develop/licenses/commercial.md).

