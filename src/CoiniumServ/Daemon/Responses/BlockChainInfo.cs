using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoiniumServ.Daemon.Responses
{
    class BlockChainInfo
    {
        public string chain { get; set; }
        public long blocks { get; set; }
        public long headers { get; set; }
        public string bestblockhash { get; set; }
        public double difficulty { get; set; }
        public long mediantime { get; set; }
        public double verificationprogress { get; set; }
        public bool initialblockdownload { get; set; }
        public string chainwork { get; set; }
        public long size_on_disk { get; set; }
        public bool pruned { get; set; }
        public BIP9Softforks bip9_softforks { get; set; }
        public string warnings { get; set; }
    }
}
