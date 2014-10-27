#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     http://www.coiniumserv.com - https://github.com/CoiniumServ/CoiniumServ
// 
//     This software is dual-licensed: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//    
//     For the terms of this license, see licenses/gpl_v3.txt.
// 
//     Alternatively, you can license this software under a commercial
//     license or white-label it as set out in licenses/commercial.txt.
// 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using CoiniumServ.Accounts;
using CoiniumServ.Persistance.Layers;
using CoiniumServ.Pools;
using CoiniumServ.Server.Mining.Getwork;
using CoiniumServ.Server.Mining.Stratum;
using CoiniumServ.Server.Mining.Stratum.Sockets;
using Serilog;

namespace CoiniumServ.Mining
{
    public class MinerManager : IMinerManager
    {        
        public int Count { get { return _miners.Count(kvp => kvp.Value.Authenticated); } }

        public IList<IMiner> Miners { get { return _miners.Values.ToList(); } }

        public event EventHandler MinerAuthenticated;

        private readonly Dictionary<int, IMiner> _miners;        

        private int _counter = 0; // counter for assigining unique id's to miners.

        private readonly IPoolConfig _poolConfig;

        private readonly IStorageLayer _storageLayer;

        private readonly IAccountManager _accountManager;

        private readonly ILogger _logger;

        /// <summary>
        /// Used for locking miners list.
        /// </summary>
        private readonly object _minersLock = new object();

        public MinerManager(IPoolConfig poolConfig, IStorageLayer storageLayer, IAccountManager accountManager)
        {
            _poolConfig = poolConfig;
            _storageLayer = storageLayer;
            _accountManager = accountManager;

            _miners = new Dictionary<int, IMiner>();
            _logger = Log.ForContext<MinerManager>().ForContext("Component", poolConfig.Coin.Name);
        }

        public IMiner GetMiner(Int32 id)
        {
            return _miners.ContainsKey(id) ? _miners[id] : null;
        }

        public IMiner GetByConnection(IConnection connection)
        {
            return (from pair in _miners  // returned the miner associated with the given connection.
                let client = (IClient) pair.Value 
                where client.Connection == connection 
                select pair.Value).FirstOrDefault();
        }

        public T Create<T>(IPool pool) where T : IGetworkMiner
        {
            var @params = new object[]
            {
                _counter++,
                pool,
                this
            };

            var instance = Activator.CreateInstance(typeof(T), @params); // create an instance of the miner.
            var miner = (IGetworkMiner)instance;

            lock(_minersLock) // lock the list before we modify the collection.
                _miners.Add(miner.Id, miner); // add it to our collection.

            return (T)miner;
        }

        public T Create<T>(UInt32 extraNonce, IConnection connection, IPool pool) where T : IStratumMiner
        {
            var @params = new object[]
            {
                _counter++, 
                extraNonce, 
                connection, 
                pool,
                this,
                _storageLayer
            };

            var instance = Activator.CreateInstance(typeof(T), @params);  // create an instance of the miner.
            var miner = (IStratumMiner)instance;
            
            lock (_minersLock) // lock the list before we modify the collection.
                _miners.Add(miner.Id, miner); // add it to our collection.           

            return (T)miner;
        }

        public void Remove(IConnection connection)
        {
            // find the miner associated with the connection.
            var miner = (from pair in _miners
                let client = (IClient) pair.Value
                where client.Connection == connection
                select pair.Value).FirstOrDefault();

            if (miner == null) // make sure the miner exists
                return;

            lock (_minersLock) // lock the list before we modify the collection.
                _miners.Remove(miner.Id); // remove the miner.
        }

        public void Authenticate(IMiner miner)
        {
            // if username validation is not on just authenticate the miner, else ask the current storage layer to do so.
            miner.Authenticated = !_poolConfig.Miner.ValidateUsername || _storageLayer.Authenticate(miner);

            _logger.Debug(
                miner.Authenticated ? "Authenticated miner: {0:l} [{1:l}]" : "Miner authentication failed: {0:l} [{1:l}]",
                miner.Username, ((IClient) miner).Connection.RemoteEndPoint);

            if (!miner.Authenticated) 
                return;

            if (miner is IStratumMiner) // if we are handling a stratum-miner, apply stratum specific stuff.
            {
                var stratumMiner = (IStratumMiner) miner;
                stratumMiner.SetDifficulty(_poolConfig.Stratum.Diff); // set the initial difficulty for the miner and send it.
                stratumMiner.SendMessage(_poolConfig.Meta.MOTD); // send the motd.
            }

            miner.Account = _accountManager.GetAccountByUsername(miner.Username); // query the user.
            if (miner.Account == null) // if the user doesn't exists.
            {
                _accountManager.AddAccount(new Account(miner)); // create a new one.
                miner.Account = _accountManager.GetAccountByUsername(miner.Username); // re-query the newly created record.
            }

            OnMinerAuthenticated(new MinerEventArgs(miner)); // notify listeners about the new authenticated miner.            
        }

        // todo: consider exposing this event by miner object itself.
        protected virtual void OnMinerAuthenticated(MinerEventArgs e)
        {
            var handler = MinerAuthenticated;

            if (handler != null)
                handler(this, e);
        }
    }
}
