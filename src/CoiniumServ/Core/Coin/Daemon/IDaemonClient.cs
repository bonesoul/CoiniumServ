using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coinium.Core.Coin.Daemon.Responses;
using Coinium.Core.Mining.Pool;

namespace Coinium.Core.Coin.Daemon
{
    public interface IDaemonClient
    {
        BlockTemplate GetBlockTemplate(params object[] @params);

        Work Getwork();

        ValidateAddress ValidateAddress(string walletAddress);

        void Initialize(string url, string username, string password);
    }
}
