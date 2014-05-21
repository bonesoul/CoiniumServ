using Coinium.Common.Attributes;
using Coinium.Common.Constants;
using Coinium.Core.RPC;
using Coinium.Core.Server;
using Coinium.Core.Server.Stratum;
using Coinium.Core.Server.Vanilla;
using Coinium.Net.Server;
using Ninject;

namespace Coinium.Core.Config.Registries
{
    public class ServerRegistry : IRegistry
    {
        private readonly IKernel _kernel;

        public ServerRegistry(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void RegisterInstances()
        {
            _kernel.Bind<IMiningServer>().To<VanillaServer>().Named(RPCServiceNames.Vanilla);
            _kernel.Bind<IMiningServer>().To<StratumServer>().Named(RPCServiceNames.Stratum);

            _kernel.Bind<IRPCService>().To<VanillaService>().Named(RPCServiceNames.Vanilla);
            _kernel.Bind<IRPCService>().To<StratumService>().Named(RPCServiceNames.Stratum);
        }
    }
}
