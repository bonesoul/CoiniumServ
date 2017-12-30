using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoiniumServ.Daemon.Responses
{
    public class SoftForksData
    {
        public string status { get; set; }
        public long startTime { get; set; }
        public long timeout { get; set; }
        public long since { get; set; }
    }
}
