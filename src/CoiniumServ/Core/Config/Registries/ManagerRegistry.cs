using Coinium.Common.Constants;
using Coinium.Core.Coin.Processors;
using Coinium.Core.Mining;
using Ninject;

namespace Coinium.Core.Config.Registries
{
    public class ManagerRegistry : IRegistry
    {
        private readonly IKernel _kernel;

        public ManagerRegistry(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void RegisterInstances()
        {
            _kernel.Bind<IMiningManager>().To<MiningManager>().InSingletonScope();
        }
    }
}
