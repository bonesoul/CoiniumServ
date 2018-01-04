using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoiniumServ.Daemon.Responses
{
    public class BIP9Softforks
    {
        public SoftForksData csv { get; set; }
        public SoftForksData segwit { get; set; }
        public SoftForksData nversionbips { get; set; }

    }
}
