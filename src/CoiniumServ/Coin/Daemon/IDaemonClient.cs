using Coinium.Coin.Daemon.Config;
using Coinium.Coin.Daemon.Responses;

namespace Coinium.Coin.Daemon
{
    public interface IDaemonClient
    {
        BlockTemplate GetBlockTemplate(params object[] @params);

        Work Getwork();

        bool Getwork(string data);

        ValidateAddress ValidateAddress(string walletAddress);

        void Initialize(IDaemonConfig daemonConfig);
    }
}
