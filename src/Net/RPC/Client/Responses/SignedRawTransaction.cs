/*
 * Coinium project, Copyright (C) 2013, Int6 Studios - All Rights Reserved. - http://www.coinium.org
 *
 */

/* This file is based on https://github.com/BitKoot/BitcoinRpcSharp */

namespace coinium.Net.RPC.Client.Responses
{
    public class SignedRawTransaction
    {
        public string Hex { get; set; }
        public bool Complete { get; set; }
    }
}
