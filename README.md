[![CircleCI](https://circleci.com/gh/bonesoul/CoiniumServ.svg?style=svg)](https://circleci.com/gh/bonesoul/CoiniumServ) [![Build status](https://ci.appveyor.com/api/projects/status/3x349ig9dt14943t)](https://ci.appveyor.com/project/bonesoul/coiniumserv) [![Documentation Status](https://readthedocs.org/projects/coiniumserv/badge/?version=latest)](https://readthedocs.org/projects/coiniumserv/?badge=latest)
 
**CoiniumServ** is a high performance, extremely efficient, platform-agnostic, easy to setup pool server implementation. It features stratum and vanilla services, reward, payment, share processors, vardiff & ban managers, user-friendly embedded web-server & front-end and a full-stack API.

CoiniumServ was created to be used for Coinium.org mining pool network at first hand. You can check [some of pools](https://github.com/bonesoul/CoiniumServ/wiki/Pools) of the pools running CoiniumServ.

### Buy package
You can buy a compiled version of [CoiniumServ here](https://satoshibox.com/kt337prrdgahwx7jutqa8nb8) if you are having difficulties doing so yourself. You can then just configure & run it.

[![Buy with Bitcoin](http://satoshibox.com/img/button-bitcoin.svg)](https://satoshibox.com/kt337prrdgahwx7jutqa8nb8)
[![Buy with Litecoin](http://satoshibox.com/img/button-litecoin.svg)](https://satoshibox.com/kt337prrdgahwx7jutqa8nb8)
[![Buy with Ethereum](http://satoshibox.com/img/button-ethereum.svg)](https://satoshibox.com/kt337prrdgahwx7jutqa8nb8)
[![Buy with Dash](http://satoshibox.com/img/button-dash.svg)](https://satoshibox.com/kt337prrdgahwx7jutqa8nb8)

**VirusTotal scan results**: [0/56 clean](https://www.virustotal.com/en/file/29a17a38785ae3a535572a08b8dce5dd937718748e9c30f2e0088ed23d157968/analysis/1495791384/)

### Screenshots

##### Console

![CoiniumServ running over mono & ubuntu](http://i.imgur.com/HvaPVrZ.png)

##### Embedded web frontend

![Embedded web frontend](http://i.imgur.com/oOF8lQ0.png)

### Status

Latest release: [v0.2.5 beta](https://github.com/bonesoul/CoiniumServ/releases/tag/v0.2.5-beta)

### Getting Started

Start by checking our [Getting Started](https://github.com/bonesoul/CoiniumServ/wiki/Getting-Started) guide for installation instructions for *nix and Windows.

### Documentation

* [Wiki](https://github.com/bonesoul/CoiniumServ/wiki/)
* [FAQ](https://github.com/bonesoul/CoiniumServ/wiki/FAQ)

### User Support

Start by reading our [FAQ](https://github.com/bonesoul/CoiniumServ/wiki/FAQ) and [wiki](https://github.com/bonesoul/CoiniumServ/wiki/). You can also use our [issues](https://github.com/bonesoul/CoiniumServ/issues) page to report bugs.

##### Discussions

* [Bitcointalk.org](https://bitcointalk.org/index.php?topic=604476.0)

### Support the project

You can support the development of the project with different methods;

[![Bountysource](https://api.bountysource.com/badge/team?team_id=760&style=bounties_received)](https://www.bountysource.com/teams/coinium/issues?utm_source=Coinium&utm_medium=shield&utm_campaign=TEAM_BADGE_1) [![tip for next commit](http://tip4commit.com/projects/760.svg)](http://tip4commit.com/projects/760)  [![Gratipay](http://img.shields.io/badge/gratipay-donate-brightgreen.svg)](https://gratipay.com/on/github/CoiniumServ)

##### Donations

You can contribute the development of the project by donating; 

* BTC: `18qqrtR4xHujLKf9oqiCsjmwmH5vGpch4D`
* LTC: `LMXfRb3w8cMUBfqZb6RUkFTPaT6vbRozPa`
* DOGE: `DM8FW8REMHj3P4xtcMWDn33ccjikCWJnQr`

If you would like to automatically donate a percentage of your pool's earning to support the project, check the [donation setup](https://github.com/bonesoul/CoiniumServ/wiki/Donation) guide.

##### Donors

Here is a list of our generous donors that keep the project ongoing;

* [reddapi.com](https://www.reddapi.com)

### Features
* __Platform Agnostic__; unlike other pool-servers, CoiniumServ doesn't dictate platforms and can run on anything including Windows, Linux or MacOS.
* __High Performance__; Designed to be fast & efficient, CoiniumServ can handle dozens of pools together.
* __Modular & Flexible__; Designed to be modular since day one so that you can implement your very own ideas.
* __Free & Open-Source__; Best of all CoiniumServ is open source and free-to-use. You can get it running for free in minutes.
* __Easy to Setup__; We got your back covered with our [guides & how-to's](https://github.com/bonesoul/CoiniumServ/wiki).

##### General

* Multiple pools & ports
* Multiple coin daemon connections
* Supports POW (proof-of-work) coins
* Supports POS (proof-of-stake) coins

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

* We have implemented extensive [tests](https://github.com/bonesoul/CoiniumServ/tree/develop/src/Tests) for all important functionality and never merge in code that breaks tests and stuff. Yet again, when a new functionality is introduced we also expect proper tests to be implemented within the PR. In simple words, most probably you won't notice any functionality-breaking changes within the repository.
* A strict ruleset for the [Development Model](https://github.com/bonesoul/CoiniumServ/wiki/Development-Model). You can follow our bleeding-edge [Develop](https://github.com/bonesoul/CoiniumServ) branch or stay with-in the stable [Master](https://github.com/bonesoul/CoiniumServ/tree/master) branch.

##### Contributing

Start reading by these;

* [Developer's Guide](https://github.com/bonesoul/CoiniumServ/wiki/Developer's-Guide)
* [Technical Documentation](https://github.com/bonesoul/CoiniumServ/wiki/Technical-Documentation)

