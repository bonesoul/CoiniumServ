/*
 *   Coinium - Crypto Currency Pool Software - https://github.com/CoiniumServ/coinium
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

namespace Coinium.Core.Wallet
{
    /// <summary>
    /// Allows communication with wallets.
    /// </summary>
    public class WalletManager
    {
        public WalletClient Client { get; private set; }

        public WalletManager()
        {
            Log.Verbose("WalletManager() init..");
        }

        public void Run()
        {
            Log.Verbose("Starting wallet-clients..");
            this.Client = new WalletClient("http://127.0.0.1:9334", "devel", "develpass");
            //Log.Verbose("Difficulty: " + this.Client.GetInfo().Difficulty);
        }


        private static readonly WalletManager _instance = new WalletManager();

        /// <summary>
        /// Singleton instance of WalletManager.
        /// </summary>
        public static WalletManager Instance { get { return _instance; } }
    }
}
