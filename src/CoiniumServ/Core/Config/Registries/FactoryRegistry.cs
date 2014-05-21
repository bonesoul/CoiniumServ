using Coinium.Core.Coin.Algorithms;
using Coinium.Core.Mining.Pool;
using Coinium.Core.Server;
using Ninject;

namespace Coinium.Core.Config.Registries
{
    public class FactoryRegistry : IRegistry
    {
        private readonly IKernel _kernel;

        public FactoryRegistry(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void RegisterInstances()
        {
            _kernel.Bind<IHashAlgorithmFactory>().To<HashAlgorithmFactory>().InSingletonScope();
            _kernel.Bind<IPoolFactory>().To<PoolFactory>().InSingletonScope();
            _kernel.Bind<IServerFactory>().To<ServerFactory>().InSingletonScope();
            _kernel.Bind<IServiceFactory>().To<ServiceFactory>().InSingletonScope();
        }
    }
}
