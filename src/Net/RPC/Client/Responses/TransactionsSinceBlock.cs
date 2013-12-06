/*
 * Coinium project, Copyright (C) 2013, Int6 Studios - All Rights Reserved. - http://www.coinium.org
 *
 */

/* This file is based on https://github.com/BitKoot/BitcoinRpcSharp */

using System.Collections.Generic;

namespace coinium.Net.RPC.Client.Responses
{
    public class TransactionsSinceBlock
    {
        public List<TransactionSinceBlock> transactions { get; set; }
        public string Lastblock { get; set; }
    }
}
