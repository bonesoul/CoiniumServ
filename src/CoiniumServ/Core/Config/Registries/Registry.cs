using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            _kernel.Bind<IRegistry>().To<CoinProcessorRegistry>();
            _kernel.Bind<IRegistry>().To<ManagerRegistry>();
            _kernel.Bind<IRegistry>().To<ServerRegistry>();
        }
    }
}
