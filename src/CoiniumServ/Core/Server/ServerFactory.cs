using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;

namespace Coinium.Core.Server
{
    public class ServerFactory : IServerFactory
    {
        private readonly IKernel _kernel;

        public ServerFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        /// <summary>
        /// Gets the specified service name.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns></returns>
        public IMiningServer Get(string serviceName)
        {
            return _kernel.Get<IMiningServer>(serviceName);
        }
    }
}
