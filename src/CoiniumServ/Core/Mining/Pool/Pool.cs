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
        private readonly IDaemonClient _daemonClient;
        private readonly IMinerManager _minerManager;
        private readonly IServerFactory _serverFactory;
        private readonly IServiceFactory _serviceFactory;
        private readonly IJobManagerFactory _jobManagerFactory;
        private readonly IShareManagerFactory _shareManagerFactory;
        private readonly IHashAlgorithmFactory _hashAlgorithmFactory;
        private IJobManager _jobManager;
        private IShareManager _shareManager;

        private Dictionary<IMiningServer, IRPCService> _servers;

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
        /// <exception cref="System.ArgumentNullException">config;config.DaemonConfig can not be null!</exception>
        public void Initialize(IPoolConfig config)
        {
            if (config.DaemonConfig == null)
                throw new ArgumentNullException("config", "config.DaemonConfig can not be null!");

            _daemonClient.Initialize(config.DaemonConfig);

            _jobManager = _jobManagerFactory.Get(_daemonClient, _minerManager);
            _jobManager.Initialize(InstanceId);

            _shareManager = _shareManagerFactory.Get(_hashAlgorithmFactory.Get(config.AlgorithmName), _jobManager);

            _servers = new Dictionary<IMiningServer, IRPCService>();
            foreach (var serverConfig in config.ServerConfigs)
            {
                var server = _serverFactory.Get(serverConfig.Name, _minerManager);
                server.Initialize(serverConfig);

                var rpcService = _serviceFactory.Get(serverConfig.Name, _jobManager, _shareManager, _daemonClient);
                
                _servers.Add(server, rpcService);
            }

            // other stuff
            this._timer = new Timer(Timer, null, TimeSpan.Zero, new TimeSpan(0, 0, 0, 10)); // setup a timer to broadcast jobs.
        }

        public void Start()
        {
            if (_servers == null) throw new Exception("At least one server is required. Please check your configuration.");
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
