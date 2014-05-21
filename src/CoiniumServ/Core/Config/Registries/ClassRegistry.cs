using Coinium.Common.Constants;
using Coinium.Core.Coin.Algorithms;
using Coinium.Core.Coin.Daemon;
using Coinium.Core.Mining.Jobs;
using Coinium.Core.Mining.Miner;
using Coinium.Core.Mining.Pool;
using Coinium.Core.Mining.Share;
using Coinium.Core.RPC;
using Coinium.Core.Server;
using Coinium.Core.Server.Stratum;
using Ninject;

namespace Coinium.Core.Config.Registries
{
    public class ClassRegistry : IRegistry
    {
        private readonly IKernel _kernel;

        public ClassRegistry(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void RegisterInstances()
        {
            _kernel.Bind<IHashAlgorithm>().To<Scrypt>().Named(AlgorithmNames.Scrypt);
            _kernel.Bind<IDaemonClient>().To<DaemonClient>();
            _kernel.Bind<IPool>().To<Pool>();
        }
    }
}