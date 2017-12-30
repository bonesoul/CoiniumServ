using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoiniumServ.Daemon.Responses
{
    public class MiningInfoData
    {
        public long blocks { get; set; }
        public long currentblockweight { get; set; }
        public long currentblocktx { get; set; }
        public double difficulty { get; set; }
        public double networkhashps { get; set; }
        public long pooledtx { get; set; }
        public string chain { get; set; }
        public string warnings { get; set; }
    }
}
