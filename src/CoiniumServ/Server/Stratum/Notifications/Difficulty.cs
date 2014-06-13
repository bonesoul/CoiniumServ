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
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Coinium.Server.Stratum.Notifications
{
    [JsonArray]
    public class Difficulty : IEnumerable<object>
    {
        /// <summary>
        /// Difficulty for jobs.
        /// </summary>
        [JsonIgnore]
        public UInt32 Diff { get; private set; }

        /// <summary>
        /// Creates a new instance of JobNotification.
        /// </summary>
        /// <param name="difficulty"></param>
        public Difficulty(UInt32 difficulty)
        {
            Diff = difficulty;
        }

        public IEnumerator<object> GetEnumerator()
        {
            var data = new List<object>
            {
                Diff
            };

            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
