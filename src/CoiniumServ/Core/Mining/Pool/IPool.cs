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
        IMiningServer StratumServer { get; }

        IRPCService StratumRpcService { get; }

        IMiningServer VanillaServer { get; }

        IRPCService VanillaRpcService { get; }

        IDaemonClient DaemonClient { get; }

        IMinerManager MinerManager { get; }

        IJobManager JobManager { get; }

        IShareManager ShareManager { get; }

        UInt64 InstanceId { get; }

        void Start();

        void Stop();
    }
}
