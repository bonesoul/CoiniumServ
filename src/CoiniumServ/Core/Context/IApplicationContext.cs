using Ninject;

namespace Coinium.Core.Context
{
    public interface IApplicationContext
    {
        IKernel Kernel { get;  }

        void Initialize(IKernel kernel);
    }
}
