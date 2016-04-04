CoiniumServ 
[![Build Status](https://travis-ci.org/int6/CoiniumServ.svg?branch=develop)](https://travis-ci.org/CoiniumServ/CoiniumServ) [![Build status](https://ci.appveyor.com/api/projects/status/3x349ig9dt14943t)](https://ci.appveyor.com/project/raistlinthewiz/coiniumserv) [![Documentation Status](https://readthedocs.org/projects/coiniumserv/badge/?version=latest)](https://readthedocs.org/projects/coiniumserv/?badge=latest) [![Project Stats](https://www.openhub.net/p/CoiniumServ/widgets/project_thin_badge.gif)](https://www.openhub.net/p/CoiniumServ)
 
**CoiniumServ** is a high performance, extremely efficient, platform-agnostic, easy to setup pool server implementation. It features stratum and vanilla services, reward, payment, share processors, vardiff & ban managers, user-friendly embedded web-server & front-end and a full-stack API.

Even better multi-pool & auto-exchange module is being developed which once completed will allow you to setup pools that can pay users in any crypto-currency that can be traded over exchanges.

CoiniumServ was created to be used for Coinium.org mining pool network at first hand. You can check [some of pools](https://github.com/int6/CoiniumServ/wiki/Pools) of the pools running CoiniumServ.

### Screenshots

##### Console

![CoiniumServ running over mono & ubuntu](http://i.imgur.com/HvaPVrZ.png)

##### Embedded web frontend

![Embedded web frontend](http://i.imgur.com/oOF8lQ0.png)

### Status

Latest release: [v0.2.4 beta](https://github.com/int6/CoiniumServ/releases/tag/v0.2.4-beta)

### Getting Started

Start by checking our [Getting Started](https://github.com/int6/CoiniumServ/wiki/Getting-Started) guide for installation instructions for *nix and Windows.

### Documentation

* [Wiki](https://github.com/int6/CoiniumServ/wiki/)
* [FAQ](https://github.com/int6/CoiniumServ/wiki/FAQ)

### User Support

Start by reading our [FAQ](https://github.com/int6/CoiniumServ/wiki/FAQ) and [wiki](https://github.com/int6/CoiniumServ/wiki/). If you need further help, join us over our IRC channel. You can also use our [issues](https://github.com/int6/CoiniumServ/issues) page to report bugs.

##### Where to get support?

* IRC (**irc.freenode.net**):
  - **#coiniumserv** [user support](http://webchat.freenode.net/?channels=%23coiniumserv&prompt=1&uio=OT10cnVlde)
  - **#coiniumserv-dev** [dev talk](http://webchat.freenode.net/?channels=%23coiniumserv-dev&prompt=1&uio=OT10cnVlde)
* [Bitcointalk.org](https://bitcointalk.org/index.php?topic=604476.0)

##### Binary distributions

You can purchase [binary distribution](http://www.int6ware.com/shop/coiniumserv/coiniumserv/) for easier setup and meanwhile support the development of the project.

### Support the project

You can support the development of the project with different methods;

[![Bountysource](https://api.bountysource.com/badge/team?team_id=760&style=bounties_received)](https://www.bountysource.com/teams/coinium/issues?utm_source=Coinium&utm_medium=shield&utm_campaign=TEAM_BADGE_1) [![tip for next commit](http://tip4commit.com/projects/760.svg)](http://tip4commit.com/projects/760)  [![Gratipay](http://img.shields.io/badge/gratipay-donate-brightgreen.svg)](https://gratipay.com/on/github/CoiniumServ)

##### Donations

You can contribute the development of the project by donating; 

* BTC: `18qqrtR4xHujLKf9oqiCsjmwmH5vGpch4D`
* LTC: `LMXfRb3w8cMUBfqZb6RUkFTPaT6vbRozPa`
* DOGE: `DM8FW8REMHj3P4xtcMWDn33ccjikCWJnQr`
* RDD: `Rb9kcLs96VDHTmiXVjcWC2RBsfCJ73UQyr`

If you would like to automatically donate a percentage of your pool's earning to support the project, check the [donation setup](https://github.com/int6/CoiniumServ/wiki/Donation) guide.

##### Donors

Here is a list of our generous donors that keep the project ongoing;

* [reddapi.com](https://www.reddapi.com)

### Features
* __Platform Agnostic__; unlike other pool-servers, CoiniumServ doesn't dictate platforms and can run on anything including Windows, Linux or MacOS.
* __High Performance__; Designed to be fast & efficient, CoiniumServ can handle dozens of pools together.
* __Modular & Flexible__; Designed to be modular since day one so that you can implement your very own ideas.
* __Free & Open-Source__; Best of all CoiniumServ is open source and free-to-use. You can get it running for free in minutes.
* __Easy to Setup__; We got your back covered with our [guides & how-to's](https://github.com/int6/CoiniumServ/wiki), [irc channel](http://webchat.freenode.net/?channels=%23coiniumserv&prompt=1&uio=OT10cnVlde). On top that, we do also provide [paid support & consulting services](https://github.com/int6/CoiniumServ#consulting).

##### General

* Multiple pools & ports
* Multiple coin daemon connections
* Supports POW (proof-of-work) coins
* Supports POS (proof-of-stake) coins
 
##### Auto Exchange & Multipool
* Being developed, stay tuned!

##### Algorithms

* __Scrypt__, __SHA256d__, __X11__, __X13__, X14, X15, X17, Blake, Fresh, Fugue, Groestl, Keccak, NIST5, Scrypt-OG, Scrypt-N, SHA1, SHAvite3, Skein, Qubit

##### Protocols

* Stratum
 * show_message support
 * block template support
 * generation transaction support
 * transaction message (txMessage) support
* Getwork [experimental]

##### Storage Layers

* Hybrid mode (redis + mysql)
* [MPOS](https://github.com/MPOS/php-mpos) compatibility (mysql)

##### Embedded Web Server

* Customizable front-end
* Full stack json-api

##### Addititional Features

* ✔ Vardiff support
* ✔ Ban manager (that can handle miners flooding with invalid shares)
* ✔ Share & Payment processor, Job Manager

### Development

##### Model

* We have implemented extensive [tests](https://github.com/int6/CoiniumServ/tree/develop/src/Tests) for all important functionality and never merge in code that breaks tests and stuff. Yet again, when a new functionality is introduced we also expect proper tests to be implemented within the PR. In simple words, most probably you won't notice any functionality-breaking changes within the repository.
* A strict ruleset for the [Development Model](https://github.com/int6/CoiniumServ/wiki/Development-Model). You can follow our bleeding-edge [Develop](https://github.com/int6/CoiniumServ) branch or stay with-in the stable [Master](https://github.com/int6/CoiniumServ/tree/master) branch.

##### Contributing

Start reading by these;

* [Developer's Guide](https://github.com/int6/CoiniumServ/wiki/Developer's-Guide)
* [Technical Documentation](https://github.com/int6/CoiniumServ/wiki/Technical-Documentation)

### License

Copyright (C) 2013 - 2015, CoiniumServ Project - http://www.int6ware.com/

This software is dual-licensed: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

For the terms of this license, see [licenses/gpl_v3.txt](https://github.com/int6/CoiniumServ/blob/develop/licenses/gpl_v3.txt).

Alternatively, you can license this software under a commercial
license or white-label it as set out in [licenses/commercial.md](https://github.com/int6/CoiniumServ/blob/develop/licenses/commercial.md).

