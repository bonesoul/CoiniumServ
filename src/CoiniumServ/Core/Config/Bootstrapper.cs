using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coinium.Core.Config.Registries;
using Ninject;

namespace Coinium.Core.Config
{
    public class Bootstrapper
    {
        private readonly IKernel _kernel;

        public Bootstrapper(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Run()
        {
            var masterRegistry = new Registry(_kernel);
            masterRegistry.RegisterInstances();

            foreach (var registry in _kernel.GetAll<IRegistry>())
            {
                registry.RegisterInstances();
            }
        }
    }
}
