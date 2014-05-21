using Coinium.Core.Coin.Algorithms;
using Coinium.Core.Mining.Pool;
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
        }
    }
}
