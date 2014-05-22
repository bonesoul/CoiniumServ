using Coinium.Core.Mining.Miner;

namespace Coinium.Core.Server
{
    public interface IServerFactory
    {
        IMiningServer Get(string serviceName, IMinerManager minerManager);
    }
}
