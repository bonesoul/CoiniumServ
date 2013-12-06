/*
 * Coinium project, Copyright (C) 2013, Int6 Studios - All Rights Reserved. - http://www.coinium.org
 *
 */

using Serilog;
using coinium.Net.RPC.Responses;

// contains code from: https://github.com/BitKoot/BitcoinRpcSharp

namespace coinium.Net.RPC
{
    public class RPCClient:RPCClientBase
    {
        public RPCClient(string url, string user, string password)
            : base(url, user, password)
        {
            Log.Verbose("Init RPClient()");
        }

        /// <summary>
        /// Returns an object containing various state info.
        /// </summary>
        /// <returns>An object containing some general information.</returns>
        public Info GetInfo()
        {
            return MakeRequest<Info>("getinfo", null);
        }
    }
}
