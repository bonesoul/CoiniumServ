/*
 * Coinium project, Copyright (C) 2013, Int6 Studios - All Rights Reserved. - http://www.coinium.org
 *
 */

using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace coinium.Net.RPC.Server.Responses
{
    [JsonArray]
    public class SubscribeResponse:IEnumerable<object>
    {
        [JsonIgnore]   
        public string UniqueId { get; set; }

        [JsonIgnore]   
        public string ExtraNonce1 { get; set; }

        [JsonIgnore]   
        public int ExtraNonce2 { get; set; }

        public IEnumerator<object> GetEnumerator()
        {
            var data = new List<object>
            {
                new List<string>
                {
                    "mining.notify",
                    this.UniqueId
                },
                this.ExtraNonce1,
                this.ExtraNonce2
            };

            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
