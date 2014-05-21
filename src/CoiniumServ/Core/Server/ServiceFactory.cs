using Coinium.Core.RPC;
using Ninject;

namespace Coinium.Core.Server
{
    public class ServiceFactory : IServiceFactory
    {
        private readonly IKernel _kernel;

        public ServiceFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        /// <summary>
        /// Gets the specified service name.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns></returns>
        public IRPCService Get(string serviceName)
        {
            return _kernel.Get<IRPCService>(serviceName);
        }
    }
}
