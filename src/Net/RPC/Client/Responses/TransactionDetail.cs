/*
 * Coinium project, Copyright (C) 2013, Int6 Studios - All Rights Reserved. - http://www.coinium.org
 *
 */

/* This file is based on https://github.com/BitKoot/BitcoinRpcSharp */

namespace coinium.Net.RPC.Client.Responses
{
    public class TransactionDetail
    {
        public string Account { get; set; }
        public string Address { get; set; }
        public string Category { get; set; }
        public double Amount { get; set; }
        public double Fee { get; set; }
    }
}
