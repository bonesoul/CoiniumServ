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
using Coinium.Common.Helpers.IO;
using Serilog;

namespace Coinium.Core.Mining.Pool
{
    public class PoolManager
    {
        private Dictionary<string, PoolConfig> _configs; 

        public PoolManager()
        {
            Log.Verbose("PoolManager() init..");
        }

        public void Run()
        {
            this.LoadPoolsConfig();
        }

        private void LoadPoolsConfig()
        {
            this._configs=new Dictionary<string, PoolConfig>();

            var poolsConfigFolder = string.Format("{0}/Conf/Pools", FileHelpers.AssemblyRoot);
            var files = FileHelpers.GetFilesByExtensionRecursive(poolsConfigFolder, ".conf");

            foreach (var file in files)
            {
                this._configs.Add(file, new PoolConfig(file));
            }
        }

        private static readonly PoolManager _instance = new PoolManager();

        /// <summary>
        /// Singleton instance of WalletManager.
        /// </summary>
        public static PoolManager Instance { get { return _instance; } }
    }
}
