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

using System.Collections.Generic;
using Coinium.Core.Coin.Daemon;
using Coinium.Core.Mining.Pool.Config;
using Coinium.Core.Server.Stratum.Config;
using Coinium.Core.Server.Vanilla.Config;
using Serilog;

namespace Coinium.Core.Mining.Pool
{
    public class PoolManager:IPoolManager
    {
        private readonly List<IPool> _pools = new List<IPool>();
        //private Dictionary<string, PoolConfig> _configs; 

        public PoolManager()
        {
            Log.Verbose("PoolManager() init..");
        }

        public void Run()
        {
            // we should be loading pools from configs here - this.LoadPoolsConfig();    
            
            var stratumServerConfig = new StratumServerConfig("0.0.0.0", 3333);
            var vanillaServerConfig = new VanillaServerConfig(3334);
            var daemonConfig = new DaemonConfig("http://127.0.0.1:9334", "devel", "develpass");
            var poolConfig = new PoolConfig(stratumServerConfig, vanillaServerConfig, daemonConfig);

            this.AddPool(poolConfig);
        }

        public IList<IPool> GetPools()
        {
            return this._pools;
        }

        public IPool AddPool(IPoolConfig poolConfig)
        {
            var pool = new Pool(poolConfig);
            this._pools.Add(pool);

            return pool;
        }

        //private void LoadPoolsConfig()
        //{
        //    this._configs=new Dictionary<string, PoolConfig>();

        //    var poolsConfigFolder = string.Format("{0}/Conf/Pools", FileHelpers.AssemblyRoot);
        //    var files = FileHelpers.GetFilesByExtensionRecursive(poolsConfigFolder, ".conf");

        //    foreach (var file in files)
        //    {
        //        this._configs.Add(file, new PoolConfig(file));
        //    }
        //}

        private static readonly PoolManager _instance = new PoolManager();

        /// <summary>
        /// Singleton instance of WalletManager.
        /// </summary>
        public static PoolManager Instance { get { return _instance; } }
    }
}
