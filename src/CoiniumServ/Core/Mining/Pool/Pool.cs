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
using Coinium.Core.Mining.Jobs;
using Coinium.Core.Mining.Miner;
using Coinium.Core.Mining.Share;
using Coinium.Core.Server;
using Coinium.Net.Server;

namespace Coinium.Core.Mining.Pool
{
    /// <summary>
    /// Contains pool services and server.
    /// </summary>
    public class Pool:IPool
    {
        // dependencies.        
        private readonly IMiningServer _server;
        private readonly IMinerManager _minerManager;
        private readonly IJobManager _jobManager;
        private readonly IShareManager _shareManager;

        public IMiningServer Server { get { return this._server; } }

        public IMinerManager MinerManager {get {return this._minerManager;}}

        public IJobManager JobManager { get { return this._jobManager; } }

        public Pool(IMiningServer server, IMinerManager minerManager, IJobManager jobManager)
        {
            // setup our dependencies.
            this._server = server;
            this._minerManager = minerManager;
            this._jobManager = jobManager;

            this.Server.Pool = this;
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
