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

using System.IO;
using Coinium.Coin.Configs;
using Coinium.Coin.Daemon.Config;
using Coinium.Server.Stratum.Config;
using Coinium.Server.Vanilla.Config;

namespace Coinium.Mining.Pool.Config
{
    /// <summary>
    /// Reads and serves the configuration for a pool.
    /// </summary>
    public class PoolConfig : IPoolConfig
    {
        public bool Valid { get; private set; }

        public bool Enabled { get; private set; }

        public ICoinConfig Coin { get; private set; }

        public IStratumServerConfig Stratum { get; private set; }

        public IVanillaServerConfig Vanilla { get; private set; }

        /// <summary>
        /// Gets the daemon configuration.
        /// </summary>
        /// <value>
        /// The daemon configuration.
        /// </value>
        public IDaemonConfig Daemon { get; private set; }

        public PoolConfig(dynamic config)
        {
            if (config == null)
            {
                this.Valid = false;
                return;
            }

            this.Enabled = config.enabled ? config.enabled : false;

            var coinName = Path.GetFileNameWithoutExtension(config.coin);
            this.Coin = CoinConfigFactory.GetConfig(coinName);

            if (this.Coin == null)
            {
                this.Valid = false;
                return;
            }

            this.Stratum = new StratumServerConfig(config.stratum);
            this.Vanilla = new VanillaServerConfig(config.vanilla);

            if (this.Stratum == null && this.Vanilla == null)
            {
                this.Valid = false;
                return;
            }

            this.Daemon = new DaemonConfig(config.daemon);

            if (this.Daemon == null)
            {
                this.Valid = false;
                return;
            }

            this.Valid = true;
        }
    }
}
