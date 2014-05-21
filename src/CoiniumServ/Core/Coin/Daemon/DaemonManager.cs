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

using Serilog;

namespace Coinium.Core.Coin.Daemon
{
    /// <summary>
    /// Allows communication with wallets.
    /// </summary>
    public class DaemonManager
    {
        public DaemonClient Client { get; private set; }

        public DaemonManager()
        {
            Log.Verbose("DaemonManager() init..");
        }

        public void Run()
        {
            Log.Verbose("Starting daemon-clients..");
            this.Client = new DaemonClient("http://127.0.0.1:9334", "devel", "develpass");
        }


        private static readonly DaemonManager _instance = new DaemonManager();

        /// <summary>
        /// Singleton instance of WalletManager.
        /// </summary>
        public static DaemonManager Instance { get { return _instance; } }
    }
}
