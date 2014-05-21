using Coinium.Core.RPC;

namespace Coinium.Core.Server
{
    public interface IServiceFactory
    {
        IRPCService Get(string serviceName);
    }
}
