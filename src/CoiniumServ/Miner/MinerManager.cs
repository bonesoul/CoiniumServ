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
using System.Linq;
using Coinium.Net.Server.Sockets;

namespace Coinium.Miner
{
    public class MinerManager : IMinerManager
    {
        private readonly Dictionary<Int32, IMiner> _miners = new Dictionary<Int32, IMiner>();
        private int _counter = 0; // counter for assigining unique id's to miners.

        public IList<IMiner> GetAll()
        {
            return _miners.Values.ToList();
        }

        public IMiner GetMiner(Int32 id)
        {
            return _miners.ContainsKey(id) ? _miners[id] : null;
        }

        public T Create<T>() where T : IMiner
        {
            var instance = Activator.CreateInstance(typeof(T), new object[] { _counter++ }); // create an instance of the miner.
            var miner = (IMiner)instance;
            _miners.Add(miner.Id, miner); // add it to our collection.

            return (T)miner;
        }

        public T Create<T>(IConnection connection) where T : IMiner
        {
            var instance = Activator.CreateInstance(typeof(T), new object[] { _counter++, connection });  // create an instance of the miner.
            var miner = (IMiner)instance;
            _miners.Add(miner.Id, miner); // add it to our collection.           

            return (T)miner;
        }
    }
}
