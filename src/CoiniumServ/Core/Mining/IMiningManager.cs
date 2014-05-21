using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coinium.Net.Server.Sockets;

namespace Coinium.Core.Mining
{
    public interface IMiningManager
    {
        T Create<T>() where T : IMiner;

        T Create<T>(IConnection connection) where T : IMiner;

        bool ProcessShare(IMiner miner, string jobId, string extraNonce2, string nTimeString, string nonceString);
    }
}
