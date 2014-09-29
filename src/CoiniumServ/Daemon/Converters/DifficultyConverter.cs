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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoiniumServ.Daemon.Converters
{
    /// <summary>
    /// Custom json converter for difficulty fields in getinfo() and getmininginfo() that can handle both pow and pos coins.
    /// </summary>
    public class DifficultyConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader); // read the difficulty token.

            // POW coins return difficulty as a double, but POS coins instead return a custom data structure.
            // So we need a custom json-converter here to be able to handle both.
            // { "proof-of-work" : 41867.16992903, "proof-of-stake" : 0.00390625, "search-interval" : 0 }

            if (token.HasValues) // if token has sub-values, then it's a POS-difficulty structure.
            {
                var diffToken = token.SelectToken("proof-of-work"); // get the proof-of-work difficulty token.
                return diffToken.ToObject<double>(); // return it's value.
            }

            return token.ToObject<double>(); // else it's a POW-difficulty which is just a plain double.
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}
