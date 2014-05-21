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
using System.Threading;
using Coinium.Core.Coin.Daemon;
using Coinium.Core.Mining.Jobs;
using Coinium.Core.Mining.Miner;
using Coinium.Core.Mining.Share;
using Coinium.Core.RPC;
using Coinium.Core.Server;
using Coinium.Core.Server.Stratum;
using Serilog;

namespace Coinium.Core.Mining.Pool
{
    /// <summary>
    /// Contains pool services and server.
    /// </summary>
    public class Pool : IPool
    {
        // dependencies.        
        private readonly IMiningServer _server;
        private readonly IDaemonClient _daemonClient;
        private readonly IMinerManager _minerManager;
        private readonly IJobManager _jobManager;
        private readonly IShareManager _shareManager;
        private readonly IRPCService _rpcService;

        public IMiningServer Server { get { return _server; } }
        public IDaemonClient DaemonClient { get { return _daemonClient; } }
        public IMinerManager MinerManager { get { return _minerManager; } }
        public IJobManager JobManager { get { return _jobManager; } }
        public IShareManager ShareManager { get { return _shareManager; } }
        public IRPCService RpcService { get { return _rpcService; } }

        /// <summary>
        /// Instance id of the pool.
        /// </summary>
        public ulong InstanceId { get; private set; }

        private Timer _timer;

        public Pool(IMiningServer server, IDaemonClient client, IMinerManager minerManager, IJobManager jobManager, IShareManager shareManager, IRPCService rpcService)
        {
            _server = server;
            _daemonClient = client;
            _minerManager = minerManager;
            _jobManager = jobManager;
            _shareManager = shareManager;
            _rpcService = rpcService;

            this.GenerateInstanceId();
        }

        /// <summary>
        /// Initializes the specified bind ip.
        /// </summary>
        /// <param name="bindIp">The bind ip.</param>
        /// <param name="port">The port.</param>
        /// <param name="daemonUrl">The daemon URL.</param>
        /// <param name="daemonUsername">The daemon username.</param>
        /// <param name="daemonPassword">The daemon password.</param>
        public void Initialize(string bindIp, Int32 port, string daemonUrl, string daemonUsername, string daemonPassword)
        {
            _server.Initialize(this, bindIp, port);
            _daemonClient.Initialize(daemonUrl, daemonUsername, daemonPassword);
            _minerManager.Initialize(this);
            _jobManager.Initialize(this, InstanceId);
            _rpcService.Initialize(this);
            _shareManager.Initialize(this);

            // other stuff
            this._timer = new Timer(Timer, null, TimeSpan.Zero, new TimeSpan(0, 0, 0, 10)); // setup a timer to broadcast jobs.
        }

        public void Start()
        {
            this._server.Start();
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
