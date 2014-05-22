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
using Coinium.Core.Coin.Daemon;
using Coinium.Core.Server.Config;
using Coinium.Core.Server.Stratum.Config;
using Coinium.Core.Server.Vanilla.Config;
using JsonConfig;

namespace Coinium.Core.Mining.Pool.Config
{
    /// <summary>
    /// Reads and serves the configuration for a pool.
    /// </summary>
    public class PoolConfig : IPoolConfig
    {
        public bool Enabled { get; private set; }
        public IStratumServerConfig StratumConfig { get; private set; }
        public IVanillaServerConfig VanillaConfig { get; private set; }

        /// <summary>
        /// Gets the server configs.
        /// </summary>
        /// <value>
        /// The server configs.
        /// </value>
        public IList<IServerConfig> ServerConfigs { get; private set; }

        /// <summary>
        /// Gets the daemon configuration.
        /// </summary>
        /// <value>
        /// The daemon configuration.
        /// </value>
        public IDaemonConfig DaemonConfig { get; private set; }
        
        /// <summary>
        /// Gets the name of the algorithm.
        /// </summary>
        /// <value>
        /// The name of the algorithm.
        /// </value>
        public string AlgorithmName { get; private set; }

        public PoolConfig(dynamic config)
        {
            this.Enabled = config.Enabled ? config.Enabled : false;                            

            this.StratumConfig = new StratumServerConfig(config.Stratum);
            this.VanillaConfig = new VanillaServerConfig(config.Vanilla);
        }

        public PoolConfig(IServerConfig stratumServerConfig, IServerConfig vanillaServerConfig, IDaemonConfig daemonConfig)
        {
            this.ServerConfigs = new List<IServerConfig>();

            if(stratumServerConfig == null && vanillaServerConfig == null)
                throw new ArgumentException("At least one server configuration needs to be supplied.");

            if (stratumServerConfig != null)
                ServerConfigs.Add(stratumServerConfig);

            if (vanillaServerConfig != null)
                ServerConfigs.Add(vanillaServerConfig);

            this.DaemonConfig = daemonConfig;
        }
    }
}
