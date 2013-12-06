/*
 * Coinium project, Copyright (C) 2013, Int6 Studios - All Rights Reserved. - http://www.coinium.org
 *
 */

/* This file is based on https://github.com/BitKoot/BitcoinRpcSharp */

using System.Collections.Generic;

namespace coinium.Net.RPC.Client.Responses
{
    public class AddedNodeInfo
    {
        public string AddedNode { get; set; }
        public bool Connected { get; set; }
        public List<NodeAddress> Addresses { get; set; }
    }
}
