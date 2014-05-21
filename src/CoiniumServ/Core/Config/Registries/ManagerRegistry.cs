using Coinium.Core.Mining.Jobs;
using Coinium.Core.Mining.Miner;
using Coinium.Core.Mining.Pool;
using Coinium.Core.Mining.Share;
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
            _kernel.Bind<IShareManager>().To<ShareManager>();
            _kernel.Bind<IMinerManager>().To<MinerManager>();
            _kernel.Bind<IJobManager>().To<JobManager>();
            _kernel.Bind<IPoolManager>().To<PoolManager>();
        }
    }
}
