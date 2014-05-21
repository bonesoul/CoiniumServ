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
using Coinium.Core.Mining.Pool.Config;
using Coinium.Core.Mining.Share;
using Coinium.Core.RPC;
using Coinium.Core.Server;
using Coinium.Core.Server.Stratum;
using Coinium.Core.Server.Stratum.Config;
using Coinium.Core.Server.Vanilla;
using Coinium.Core.Server.Vanilla.Config;
using Serilog;

namespace Coinium.Core.Mining.Pool
{
    /// <summary>
    /// Contains pool services and server.
    /// </summary>
    public class Pool:IPool
    {
        public IMiningServer StratumServer { get; private set; }

        public IRPCService StratumRpcService { get; private set; }        

        public IMiningServer VanillaServer { get; private set; }

        public IRPCService VanillaRpcService { get; private set; }

        public IDaemonClient DaemonClient { get; private set; }

        public IMinerManager MinerManager { get; private set; }

        public IJobManager JobManager { get; private set; }

        public IShareManager ShareManager { get; private set; }

        /// <summary>
        /// Instance id of the pool.
        /// </summary>
        public ulong InstanceId { get; private set; }

        private Timer _timer;

        public Pool(IPoolConfig config)
        {
            this.GenerateInstanceId();

            if (config.DaemonConfig == null)
                throw new ArgumentNullException("config", "config.DaemonConfig can not be null!");

            this.DaemonClient = new DaemonClient(config.DaemonConfig);

            if(config.StratumServerConfig == null && config.VanillaServerConfig == null)
                throw new ArgumentNullException("config","At least one server configuration should be provided.");

            if (config.StratumServerConfig != null)
            {
                this.StratumServer = new StratumServer(config.StratumServerConfig);
                this.StratumRpcService = new StratumService();

                this.StratumServer.Pool = this;
                this.StratumRpcService.Pool = this;
            }

            if (config.VanillaServerConfig != null)
            {
                this.VanillaServer = new VanillaServer(config.VanillaServerConfig);
                this.VanillaRpcService = new VanillaService();

                this.VanillaServer.Pool = this;
                this.VanillaRpcService.Pool = this;
            }


            // setup managers.
            this.MinerManager = new MinerManager();
            this.JobManager = new JobManager(this.InstanceId);
            this.ShareManager = new ShareManager();

            // set back references.
            this.MinerManager.Pool = this;
            this.JobManager.Pool = this;
            this.ShareManager.Pool = this;

            // other stuff
            this._timer = new Timer(Timer, null, TimeSpan.Zero, new TimeSpan(0, 0, 0, 10)); // setup a timer to broadcast jobs.
        }

        public void Start()
        {
            if (this.StratumServer != null)
                this.StratumServer.Start();

            if (this.VanillaServer != null)
                this.VanillaServer.Start();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
        private void Timer(object state)
        {
            this.JobManager.Broadcast();
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
