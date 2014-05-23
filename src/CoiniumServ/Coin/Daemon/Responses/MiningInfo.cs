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

/* This file is based on https://github.com/BitKoot/BitcoinRpcSharp */

namespace Coinium.Coin.Daemon.Responses
{
    public class MiningInfo
    {
        public int Blocks { get; set; }
        public int CurrentBockSize { get; set; }
        public int CurrentBlockTx { get; set; }
        public double Difficulty { get; set; }
        public string Errors { get; set; }
        public bool Generate { get; set; }
        public int GenProcLimit { get; set; }
        public int HashesPerSec { get; set; }
        public int PooledTx { get; set; }
        public bool Testnet { get; set; }
    }
}
