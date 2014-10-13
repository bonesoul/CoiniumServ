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
using System.Globalization;
using CoiniumServ.Utils.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoiniumServ.Daemon.Converters
{
    /// <summary>
    /// Custom json converter for time fields which can parse int32 unix-time for bitcoin variants and utc time-string for peercoin variants.
    /// </summary>
    public class TimeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            DateTime dateTime;
            Int32 unixTime;

            var token = JToken.Load(reader); // read the difficulty token.

            // bitcoin variants use an Int32 for time.            
            if (int.TryParse(token.ToString(), out unixTime)) // try parsing as int32
                return unixTime;

            // peercoin variants use a date-time string.
            if (DateTime.TryParseExact(token.ToString(), "yyyy-MM-dd HH:mm:ss UTC", CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal, out dateTime)) // try parsing as utc string.
                return dateTime.ToUnixTimestamp();

            return existingValue; // return the default value.
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}
