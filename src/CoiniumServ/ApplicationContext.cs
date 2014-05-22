using Ninject;

namespace Coinium
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
