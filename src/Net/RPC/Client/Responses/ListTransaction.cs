/*
 * Coinium project, Copyright (C) 2013, Int6 Studios - All Rights Reserved. - http://www.coinium.org
 *
 */

/* This file is based on https://github.com/BitKoot/BitcoinRpcSharp */

namespace coinium.Net.RPC.Client.Responses
{
    public class ListTransaction
    {
        public string Account { get; set; }
        public string Address { get; set; }
        public string Category { get; set; }
        public double Amount { get; set; }
        public int Confirmations { get; set; }
        public bool Generated { get; set; }
        public string BlockHash { get; set; }
        public int BlockIndex { get; set; }
        public int BlockTime { get; set; }
        public string TxId { get; set; }
        public int Time { get; set; }
        public int TimeReceived { get; set; }
    }
}
