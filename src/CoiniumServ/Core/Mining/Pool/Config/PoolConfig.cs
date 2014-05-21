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
using Coinium.Core.Server.Config;
using Coinium.Core.Server.Stratum.Config;
using Coinium.Core.Server.Vanilla.Config;

namespace Coinium.Core.Mining.Pool.Config
{
    /// <summary>
    /// Reads and serves the configuration for a pool.
    /// </summary>
    public class PoolConfig : IPoolConfig
    {
        public IList<IServerConfig> ServerConfigs { get; private set; }

        public IDaemonConfig DaemonConfig { get; private set; }

        public PoolConfig(IServerConfig stratumServerConfig, IServerConfig vanillaServerConfig, IDaemonConfig daemonConfig)
        {
            ServerConfigs = new List<IServerConfig>();
            if (stratumServerConfig != null)
            {
                ServerConfigs.Add(stratumServerConfig);
            }
            if (vanillaServerConfig != null)
            {
                ServerConfigs.Add(vanillaServerConfig);
            }
            this.DaemonConfig = daemonConfig;
        }

        ///// <summary>
        ///// Filename of the config..
        ///// </summary>
        //public string FileName { get; private set; }

        ///// <summary>
        ///// The parser assigned to the config.
        ///// </summary>
        //private IniConfigSource _parser; // the ini parser.

        ///// <summary>
        ///// The pool section.
        ///// </summary>
        //public PoolSection Pool { get; private set; }

        ///// <summary>
        ///// The wallet section.
        ///// </summary>
        //public WalletSection Wallet { get; private set; }

        ///// <summary>
        ///// Read a new pool-configuration instance from the given file.
        ///// </summary>
        ///// <param name="fileName"></param>
        //public PoolConfig(string fileName)
        //{
        //    this.FileName = fileName;
        //    this.LoadConfig();
        //}

        ///// <summary>
        ///// Loads the config options.
        ///// </summary>
        //private void LoadConfig()
        //{
        //    try
        //    {
        //        this._parser = new IniConfigSource(this.FileName);
        //    }
        //    catch (Exception)
        //    {
        //        Log.Error("Error loading pool config file: {0}", this.FileName);
        //    }
        //    finally
        //    {
        //        this._parser.ExpandKeyValues();
        //        this.Pool = new PoolSection(this._parser);
        //        this.Wallet = new WalletSection(this._parser);
        //    }
        //}

        ///// <summary>
        ///// Pool section
        ///// </summary>
        //public class PoolSection : ConfigSection
        //{
        //    public PoolSection(IniConfigSource source) 
        //        : base(source, "Pool")
        //    { }

        //    public int InstanceId
        //    {
        //        get { return this.GetInt("InstanceId", 31); }
        //    }
        //}

        ///// <summary>
        ///// Wallet section.
        ///// </summary>
        //public class WalletSection : ConfigSection
        //{
        //    public WalletSection(IniConfigSource source)
        //        : base(source, "Wallet")
        //    { }

        //    /// <summary>
        //    /// Main wallet address.
        //    /// </summary>
        //    public string Address
        //    {
        //        get { return this.GetString("Address", string.Empty); }
        //    }

        //    /// <summary>
        //    /// Wallet host.
        //    /// </summary>
        //    public string Host
        //    {
        //        get { return this.GetString("Host", string.Empty); }
        //    }

        //    /// <summary>
        //    /// Wallet port.
        //    /// </summary>
        //    public int Port
        //    {
        //        get { return this.GetInt("Port", 0); }
        //    }

        //    /// <summary>
        //    /// Wallet user.
        //    /// </summary>
        //    public string User
        //    {
        //        get { return this.GetString("User", string.Empty); }
        //    }

        //    /// <summary>
        //    /// Wallet password.
        //    /// </summary>
        //    public string Password
        //    {
        //        get { return this.GetString("Password", string.Empty); }
        //    }        
        //  }        
    }
}
