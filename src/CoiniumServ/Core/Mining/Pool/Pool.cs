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
using Coinium.Core.Coin.Daemon;
using Coinium.Core.Mining.Jobs;
using Coinium.Core.Mining.Miner;
using Coinium.Core.Mining.Pool.Config;
using Coinium.Core.Mining.Share;
using Coinium.Core.RPC;
using Coinium.Core.RPC.Service;
using Coinium.Core.Server;
using Coinium.Core.Server.Stratum;
using Coinium.Core.Server.Stratum.Config;
using Coinium.Core.Server.Vanilla;
using Coinium.Core.Server.Vanilla.Config;
using Ninject;
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
        private readonly IJobManager _jobManager;
        private readonly IShareManager _shareManager;
        private readonly IServerFactory _serverFactory;
        private readonly IServiceFactory _serviceFactory;

        private Dictionary<IMiningServer, IRPCService> _servers;
        public IDictionary<IMiningServer, IRPCService> Servers { get { return _servers; } }

        public IDaemonClient DaemonClient { get { return _daemonClient; } }
        public IMinerManager MinerManager { get { return _minerManager; } }
        public IJobManager JobManager { get { return _jobManager; } }
        public IShareManager ShareManager { get { return _shareManager; } }

        /// <summary>
        /// Instance id of the pool.
        /// </summary>
        public ulong InstanceId { get; private set; }

        private Timer _timer;

        public Pool(IServerFactory serverFactory, IServiceFactory serviceFactory, IDaemonClient client, IMinerManager minerManager, IJobManager jobManager, IShareManager shareManager)
        {
            _daemonClient = client;
            _minerManager = minerManager;
            _jobManager = jobManager;
            _shareManager = shareManager;
            _serverFactory = serverFactory;
            _serviceFactory = serviceFactory;
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
            _minerManager.Initialize(this);
            _jobManager.Initialize(this, InstanceId);
            _shareManager.Initialize(this);


            _servers = new Dictionary<IMiningServer, IRPCService>();
            foreach (var serverConfig in config.ServerConfigs)
            {
                var server = _serverFactory.Get(serverConfig.Name);
                server.Initialize(this, serverConfig);

                var rpcService = _serviceFactory.Get(serverConfig.Name);
                rpcService.Initialize(this);
                
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
