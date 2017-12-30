using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoiniumServ.Daemon.Responses
{
    public class NetworkInfo
    {
        public long version { get; set; }
        public string subversion { get; set; }
        public int protocolversion { get; set; }
        public string localservices { get; set; }
        public bool localrelay { get; set; }
        public long timeoffset { get; set; }
        public long networkactive { get; set; }
        public long connections { get; set; }
        public NetworkData[] networks {get; set;}
    }
}
