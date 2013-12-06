/*
 * Coinium project, Copyright (C) 2013, Int6 Studios - All Rights Reserved. - http://www.coinium.org
 *
 */

/* This file is based on https://github.com/BitKoot/BitcoinRpcSharp */

namespace coinium.Net.RPC.Client.Responses
{
    public class MultisigAddress
    {
        public string Address { get; set; }
        public string RedeemScript { get; set; }
    }
}
