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
using Coinium.Rpc.Service;

namespace Coinium.Server.Stratum.Config
{
    public class StratumServerConfig:IStratumServerConfig
    {
        public bool Valid { get; private set; }

        public string Name { get; private set; }

        public bool Enabled { get; private set; }

        public string BindInterface { get; private set; }

        public Int32 Port { get; private set; }

        public Int32 Diff { get; private set; }

        public StratumServerConfig(dynamic config)
        {
            if (config == null)
            {
                Valid = false;
                return;
            }

            Name = RpcServiceNames.Stratum;
            Enabled = config.enabled;
            BindInterface = !string.IsNullOrEmpty(config.bind) ? config.bind : "0.0.0.0";
            Port = config.port;
            Diff = config.diff;

            Valid = true;
        }
    }
}
