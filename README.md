[![CircleCI](https://circleci.com/gh/bonesoul/CoiniumServ.svg?style=svg)](https://circleci.com/gh/bonesoul/CoiniumServ) [![Documentation Status](https://readthedocs.org/projects/coiniumserv/badge/?version=latest)](https://readthedocs.org/projects/coiniumserv/?badge=latest)
 
**CoiniumServ** is a high performance, extremely efficient, platform-agnostic, easy to setup pool server implementation. It features stratum and vanilla services, reward, payment, share processors, vardiff & ban managers, user-friendly embedded web-server & front-end and a full-stack API.

CoiniumServ was created to be used for Coinium.org mining pool network at first hand. You can check [some of pools](https://github.com/bonesoul/CoiniumServ/wiki/Pools) of the pools running CoiniumServ.

### Screenshots

##### Console

![CoiniumServ running over mono & ubuntu](http://i.imgur.com/HvaPVrZ.png)

##### Embedded web frontend

![Embedded web frontend](http://i.imgur.com/oOF8lQ0.png)

### Status

Latest release: [v0.2.6.2](https://github.com/bonesoul/CoiniumServ/releases/tag/0.2.6.2)

### Getting Started

Start by checking our [Getting Started](https://github.com/bonesoul/CoiniumServ/wiki/Getting-Started) guide for installation instructions for *nix and Windows.

### Documentation

* [Docs](http://coiniumserv.readthedocs.io/en/latest/)
* [Wiki](https://github.com/bonesoul/CoiniumServ/wiki/)
* [FAQ](https://github.com/bonesoul/CoiniumServ/wiki/FAQ)

### User Support

Start by reading our [FAQ](https://github.com/bonesoul/CoiniumServ/wiki/FAQ) and [wiki](https://github.com/bonesoul/CoiniumServ/wiki/). You can also use our [issues](https://github.com/bonesoul/CoiniumServ/issues) page to report bugs.

##### Discussions

* [Bitcointalk.org](https://bitcointalk.org/index.php?topic=604476.0)

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

* __Scrypt__, __SHA256d__, __X11__, __X13__, X14, X15, X17, Blake, Fresh, Fugue, Groestl, Keccak, NIST5, Scrypt-OG, Scrypt-N, SHA1, SHAvite3, Skein, Qubit, C11

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

