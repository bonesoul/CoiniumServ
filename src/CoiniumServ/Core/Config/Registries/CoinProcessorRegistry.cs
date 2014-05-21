using Coinium.Common.Constants;
using Coinium.Core.Coin.Processors;
using Ninject;

namespace Coinium.Core.Config.Registries
{
    public class CoinProcessorRegistry : IRegistry
    {
        private readonly IKernel _kernel;

        public CoinProcessorRegistry(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void RegisterInstances()
        {
            _kernel.Bind<ICoinProcessorFactory>().To<CoinProcessorFactory>();
            _kernel.Bind<ICoinProcessor>().To<ScryptCoinProcessor>().Named(CoinProcessorNames.Scrypt);
        }
    }
}
