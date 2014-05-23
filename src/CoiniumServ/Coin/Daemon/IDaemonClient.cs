using Coinium.Coin.Daemon.Config;
using Coinium.Coin.Daemon.Responses;

namespace Coinium.Coin.Daemon
{
    public interface IDaemonClient
    {
        BlockTemplate GetBlockTemplate();

        BlockTemplate GetBlockTemplate(string blockHex);

        string SubmitBlock(string blockHex);

        Work Getwork();

        bool Getwork(string data);

        ValidateAddress ValidateAddress(string walletAddress);

        void Initialize(IDaemonConfig daemonConfig);
    }
}
