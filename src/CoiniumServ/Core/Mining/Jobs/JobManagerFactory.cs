using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coinium.Core.Coin.Daemon;
using Coinium.Core.Mining.Miner;
using Ninject;

namespace Coinium.Core.Mining.Jobs
{
    public class JobManagerFactory : IJobManagerFactory
    {
        /// <summary>
        /// The _kernel
        /// </summary>
        private readonly IKernel _kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobManagerFactory"/> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public JobManagerFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        /// <summary>
        /// Gets the specified daemon client.
        /// </summary>
        /// <param name="daemonClient">The daemon client.</param>
        /// <param name="minerManager">The miner manager.</param>
        /// <returns></returns>
        public IJobManager Get(IDaemonClient daemonClient, IMinerManager minerManager)
        {
            var daemonClientParam = new Ninject.Parameters.ConstructorArgument("daemonClient", daemonClient);
            var minerManagerParam = new Ninject.Parameters.ConstructorArgument("minerManager", minerManager);

            return _kernel.Get<IJobManager>(daemonClientParam, minerManagerParam);
        }
    }
}
