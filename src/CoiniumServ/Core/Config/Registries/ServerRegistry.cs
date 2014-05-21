using Coinium.Common.Attributes;
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
            _kernel.Bind<IServer>().To<VanillaServer>().WhenClassHas<DefaultInstanceAttribute>();
            _kernel.Bind<IMiningServer>().To<StratumServer>(); // Only needed if there are multiple bindings for the interface: .WhenClassHas<DefaultInstanceAttribute>();
            _kernel.Bind<IRPCService>().To<StratumService>();
        }
    }
}
