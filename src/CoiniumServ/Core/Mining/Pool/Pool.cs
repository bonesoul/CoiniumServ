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

namespace Coinium.Core.Mining.Pool
{
    /// <summary>
    /// Contains pool services and server.
    /// </summary>
    public class Pool:IPool
    {
        // dependencies.        
        private readonly IMiningServer _server;
        private readonly IDaemonClient _daemonClient;
        private readonly IMinerManager _minerManager;
        private readonly IJobManager _jobManager;
        private readonly IShareManager _shareManager;
        private readonly IRPCService _rpcService;

        public IMiningServer Server { get { return this._server; } }

        public IDaemonClient DaemonClient { get { return this._daemonClient; } }

        public IRPCService RpcService { get { return this._rpcService; } }

        public IMinerManager MinerManager {get {return this._minerManager;}}

        public IJobManager JobManager { get { return this._jobManager; } }

        public IShareManager ShareManager { get { return this._shareManager; } }
        

        private Timer _timer;

        public Pool(IMiningServer server, IDaemonClient daemonClient, IRPCService rpcService, IMinerManager minerManager, IJobManager jobManager, IShareManager shareManager)
        {
            // setup our dependencies.
            this._server = server;
            this._daemonClient = daemonClient;
            this._rpcService = rpcService;
            this._minerManager = minerManager;
            this._jobManager = jobManager;
            this._shareManager = shareManager;

            // set back references.
            this.Server.Pool = this;
            this.RpcService.Pool = this;
            this.MinerManager.Pool = this;
            this.JobManager.Pool = this;
            this.ShareManager.Pool = this;

            // other stuff
            this._timer = new Timer(Timer, null, TimeSpan.Zero, new TimeSpan(0, 0, 0, 10)); // setup a timer to broadcast jobs.
        }

        private void Timer(object state)
        {
            this.JobManager.Broadcast();
        }

        public void Start()
        {
            this._server.Start();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
