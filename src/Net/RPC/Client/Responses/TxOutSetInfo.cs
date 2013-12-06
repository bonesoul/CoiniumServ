/*
 * Coinium project, Copyright (C) 2013, Int6 Studios - All Rights Reserved. - http://www.coinium.org
 *
 */

/* This file is based on https://github.com/BitKoot/BitcoinRpcSharp */

namespace coinium.Net.RPC.Client.Responses
{
    public class TxOutSetInfo
    {
        public int Height { get; set; }
        public string BestBlock { get; set; }
        public int Transactions { get; set; }
        public int TxOuts { get; set; }
        public int BytesSerialized { get; set; }
        public string HashSerialized { get; set; }
        public double TotalAmount { get; set; }
    }
}
