We need to be supporting multiple Database Types:

## Redis
* http://redis.io/clients

## MySQL
* http://dev.mysql.com/downloads/connector/net/
* Would Require us to implement pooling
* Supports Mono 

## PostgreSQL
* http://npgsql.projects.pgfoundry.org/docs/manual/UserManual.html
* Pure C# Code Comes with SSQL and Pooling Built in
* Supports Mono

## SQLite
* https://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki
* Supports Mono

## File Based Logging
* Possible with the existing logging library we are using

## No DB
* Easy, Just return True on Every Query we ever need to make.
