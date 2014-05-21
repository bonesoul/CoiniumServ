using Ninject;

namespace Coinium.Core.Config.Registries
{
    public class Registry : IRegistry
    {
        private readonly IKernel _kernel;

        public Registry(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void RegisterInstances()
        {
            _kernel.Bind<IRegistry>().To<ManagerRegistry>();
            _kernel.Bind<IRegistry>().To<ServerRegistry>();
            _kernel.Bind<IRegistry>().To<ClassRegistry>();
            _kernel.Bind<IRegistry>().To<FactoryRegistry>();
        }
    }
}
