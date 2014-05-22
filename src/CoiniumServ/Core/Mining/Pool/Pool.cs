/*
 *   CoiniumServ - crypto currency pool software - https://github.com/CoiniumServ/CoiniumServ
 *   Copyright (C) 2013 - 2014, Coinium Project - http://www.coinium.org
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Threading;
using Coinium.Core.Coin.Algorithms;
using Coinium.Core.Coin.Daemon;
using Coinium.Core.Coin.Daemon.Config;
using Coinium.Core.Mining.Jobs;
using Coinium.Core.Mining.Miner;
using Coinium.Core.Mining.Pool.Config;
using Coinium.Core.Mining.Share;
using Coinium.Core.RPC.Service;
using Coinium.Core.Server;
using Serilog;

namespace Coinium.Core.Mining.Pool
{
    /// <summary>
    /// Contains pool services and server.
    /// </summary>
    public class Pool : IPool
    {
        public IPoolConfig Config { get; private set; }

        private readonly IDaemonClient _daemonClient;
        private readonly IMinerManager _minerManager;
        private readonly IServerFactory _serverFactory;
        private readonly IServiceFactory _serviceFactory;
        private readonly IJobManagerFactory _jobManagerFactory;
        private readonly IShareManagerFactory _shareManagerFactory;
        private readonly IHashAlgorithmFactory _hashAlgorithmFactory;
        private IJobManager _jobManager;
        private IShareManager _shareManager;

        private Dictionary<IMiningServer, IRpcService> _servers;

        /// <summary>
        /// Instance id of the pool.
        /// </summary>
        public ulong InstanceId { get; private set; }

        private Timer _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pool" /> class.
        /// </summary>
        /// <param name="hashAlgorithmFactory">The hash algorithm factory.</param>
        /// <param name="serverFactory">The server factory.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="client">The client.</param>
        /// <param name="minerManager">The miner manager.</param>
        /// <param name="jobManagerFactory">The job manager factory.</param>
        /// <param name="shareManagerFactory">The share manager factory.</param>
        public Pool(IHashAlgorithmFactory hashAlgorithmFactory, IServerFactory serverFactory, IServiceFactory serviceFactory, IDaemonClient client, IMinerManager minerManager, IJobManagerFactory jobManagerFactory, IShareManagerFactory shareManagerFactory)
        {
            _daemonClient = client;
            _minerManager = minerManager;
            _jobManagerFactory = jobManagerFactory;
            _shareManagerFactory = shareManagerFactory;
            _serverFactory = serverFactory;
            _serviceFactory = serviceFactory;
            _hashAlgorithmFactory = hashAlgorithmFactory;
            this.GenerateInstanceId();
        }

        /// <summary>
        /// Initializes the specified bind ip.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <exception cref="System.ArgumentNullException">config;config.Daemon can not be null!</exception>
        public void Initialize(IPoolConfig config)
        {
            this.Config = config;

            // init managers.
            this.InitManagers();

            // init coin daemon.
            this.InitDaemon();

            // init servers
            this.InitServers();

            // other stuff
            this._timer = new Timer(Timer, null, TimeSpan.Zero, new TimeSpan(0, 0, 0, 10)); // setup a timer to broadcast jobs.
        }

        private void InitManagers()
        {
            _jobManager = _jobManagerFactory.Get(_daemonClient, _minerManager);
            _jobManager.Initialize(InstanceId);

            _shareManager = _shareManagerFactory.Get(_hashAlgorithmFactory.Get(this.Config.Coin.Algorithm), _jobManager);
        }

        private void InitDaemon()
        {
            if (this.Config.Daemon == null || this.Config.Daemon.Valid == false)
                Log.Error("Coin daemon configuration is not valid!");

            _daemonClient.Initialize(this.Config.Daemon);
        }

        private void InitServers()
        {
            _servers = new Dictionary<IMiningServer, IRpcService>();

            // we don't need here a server config list as a pool can host only one instance of stratum and one vanilla server.
            // we must be dictative here, using a server list may cause situations we don't want (multiple stratum configs etc..)
            if (this.Config.Stratum != null)
            {
                var stratumServer = _serverFactory.Get("Stratum", _minerManager);
                var stratumService = _serviceFactory.Get("Stratum", _jobManager, _shareManager, _daemonClient);
                stratumServer.Initialize(this.Config.Stratum);

                _servers.Add(stratumServer, stratumService);
            }

            if (this.Config.Vanilla != null)
            {
                var vanillaServer = _serverFactory.Get("Vanilla", _minerManager);
                var vanillaService = _serviceFactory.Get("Vanilla", _jobManager, _shareManager, _daemonClient);

                vanillaServer.Initialize(this.Config.Vanilla);

                _servers.Add(vanillaServer, vanillaService);
            }
        }

        public void Start()
        {
            if (!this.Config.Valid)
            {
                Log.Error("Can't start pool as configuration is not valid.");
                return;
            }                

            foreach (var server in _servers)
            {
                server.Key.Start();
            }
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
        private void Timer(object state)
        {
            _jobManager.Broadcast();
        }

        /// <summary>
        /// Generates an instance Id for the pool that is cryptographically random. 
        /// </summary>
        private void GenerateInstanceId()
        {
            var rndGenerator = System.Security.Cryptography.RandomNumberGenerator.Create(); // cryptographically random generator.
            var randomBytes = new byte[4];
            rndGenerator.GetNonZeroBytes(randomBytes); // create cryptographically random array of bytes.
            this.InstanceId = BitConverter.ToUInt32(randomBytes, 0); // convert them to instance Id.
            Log.Debug("Generated cryptographically random instance Id: {0}", this.InstanceId);
        }
    }
}
