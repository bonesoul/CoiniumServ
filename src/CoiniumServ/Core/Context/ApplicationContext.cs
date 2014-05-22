using Ninject;

namespace Coinium.Core.Context
{
    public class ApplicationContext : IApplicationContext
    {
        public IKernel Kernel { get; private set; }

        public void Initialize(IKernel kernel)
        {
            Kernel = kernel;
        }
    }
}
