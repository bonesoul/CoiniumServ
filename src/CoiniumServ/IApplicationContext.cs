using Ninject;

namespace Coinium
{
    public interface IApplicationContext
    {
        IKernel Kernel { get;  }

        void Initialize(IKernel kernel);
    }
}
