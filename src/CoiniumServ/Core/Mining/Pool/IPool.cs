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
using Coinium.Core.Coin.Daemon;
using Coinium.Core.Mining.Jobs;
using Coinium.Core.Mining.Miner;
using Coinium.Core.Mining.Share;
using Coinium.Core.RPC;
using Coinium.Core.Server;

namespace Coinium.Core.Mining.Pool
{
    public interface IPool
    {
        IMiningServer Server { get; }

        IDaemonClient DaemonClient { get; }

        IMinerManager MinerManager { get; }

        IJobManager JobManager { get; }

        IShareManager ShareManager { get; }

        IRPCService RpcService { get; }

        UInt64 InstanceId { get; }

        /// <summary>
        /// Initializes the specified bind ip.
        /// </summary>
        /// <param name="bindIp">The bind ip.</param>
        /// <param name="port">The port.</param>
        /// <param name="daemonUrl">The daemon URL.</param>
        /// <param name="daemonUsername">The daemon username.</param>
        /// <param name="daemonPassword">The daemon password.</param>
        void Initialize(string bindIp, Int32 port, string daemonUrl, string daemonUsername, string daemonPassword);

        void Start();

        void Stop();
    }
}
