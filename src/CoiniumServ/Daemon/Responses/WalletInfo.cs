using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoiniumServ.Daemon.Responses
{
    public class WalletInfo
    {
        public string walletname { get; set; }
        public int walletversion { get; set; }
        public decimal balance { get; set; }
        public double unconfirmed_balance { get; set; }
        public double immature_balance { get; set; }
        public long txcount { get; set; }
        public long keypoololdest { get; set; }
        public long keypoolsize { get; set; }
        public long keypoolsize_hd_internal { get; set; }
        public decimal paytxfee { get; set; }
        public string hdmasterkeyid { get; set; }
    }
}
