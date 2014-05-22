using Coinium.Core.Coin.Daemon;
using Coinium.Core.Context;
using Coinium.Core.Mining.Miner;
using Nancy.TinyIoc;

namespace Coinium.Core.Mining.Jobs
{
    public class JobManagerFactory : IJobManagerFactory
    {
        /// <summary>
        /// The _kernel
        /// </summary>
        private readonly IApplicationContext _applicationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobManagerFactory"/> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public JobManagerFactory(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        /// <summary>
        /// Gets the specified daemon client.
        /// </summary>
        /// <param name="daemonClient">The daemon client.</param>
        /// <param name="minerManager">The miner manager.</param>
        /// <returns></returns>
        public IJobManager Get(IDaemonClient daemonClient, IMinerManager minerManager)
        {
            var @params = new NamedParameterOverloads() {{"daemonClient", daemonClient}, {"minerManager", minerManager}};

            return _applicationContext.Container.Resolve<IJobManager>(@params);
        }
    }
}
