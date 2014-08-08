#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     http://www.coiniumserv.com - https://github.com/CoiniumServ/CoiniumServ
// 
//     This software is dual-licensed: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//    
//     For the terms of this license, see licenses/gpl_v3.txt.
// 
//     Alternatively, you can license this software under a commercial
//     license or white-label it as set out in licenses/commercial.txt.
// 
#endregion

using System;
using System.Dynamic;
using Newtonsoft.Json;

namespace CoiniumServ.Statistics
{
    public class PerAlgorithm:IPerAlgorithm
    {
        public string Json { get; private set; }
        public string Name { get; private set; }
        public int WorkerCount { get; private set; }
        public ulong Hashrate { get; private set; }

        private readonly dynamic _response;

        public PerAlgorithm(string algorithm)
        {
            Name = algorithm;

            _response = new ExpandoObject();
        }

        public void Reset()
        {
            Hashrate = 0;
            WorkerCount = 0;
        }

        public void Recache(UInt64 hashrate, int workerCount)
        {
            // recache data.
            Hashrate = hashrate;
            WorkerCount = workerCount;

            // recache json response.
            _response.hashrate = Hashrate;
            _response.workers = WorkerCount;

            Json = JsonConvert.SerializeObject(_response);
        }

        public object GetResponseObject()
        {
            return _response;
        }
    }
}
