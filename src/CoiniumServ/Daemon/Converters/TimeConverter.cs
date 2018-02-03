#region License
// 
//     MIT License
//
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2017, CoiniumServ Project
//     Hüseyin Uslu, shalafiraistlin at gmail dot com
//     https://github.com/bonesoul/CoiniumServ
// 
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//     
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
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
