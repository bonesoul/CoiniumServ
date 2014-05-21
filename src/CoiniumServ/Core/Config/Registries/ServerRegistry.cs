using Coinium.Common.Attributes;
using Coinium.Common.Constants;
using Coinium.Core.Coin.Processors;
using Coinium.Core.Mining;
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
            _kernel.Bind<IServer>().To<VanillaServer>().WhenClassHas<DefaultInstanceAttribute>().InSingletonScope();
            _kernel.Bind<IServer>().To<StratumServer>().WhenClassHas<DefaultInstanceAttribute>().InSingletonScope();
        }
    }
}
