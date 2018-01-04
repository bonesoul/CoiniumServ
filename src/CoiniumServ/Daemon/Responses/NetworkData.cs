using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoiniumServ.Daemon.Responses
{
    public class NetworkData
    {
        public string name { get; set; }
        public bool limited { get; set; }
        public bool reachable { get; set; }
        public string proxy { get; set; }
        public bool proxy_randomize_credentials { get; set; }
    }
}
