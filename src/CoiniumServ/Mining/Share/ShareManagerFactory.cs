using Coinium.Coin.Algorithms;
using Coinium.Coin.Daemon;
using Coinium.Common.Context;
using Coinium.Mining.Jobs;
using Nancy.TinyIoc;
using Serilog;

namespace Coinium.Mining.Share
{
    public class ShareManagerFactory : IShareManagerFactory
    {
        /// <summary>
        /// The _kernel
        /// </summary>
        private readonly IApplicationContext _applicationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShareManagerFactory" /> class.
        /// </summary>
        /// <param name="applicationContext">The application context.</param>
        public ShareManagerFactory(IApplicationContext applicationContext)
        {
            Log.Debug("ShareManagerFactory() init..");
            _applicationContext = applicationContext;
        }

        /// <summary>
        /// Gets the specified daemon client.
        /// </summary>
        /// <param name="hashAlgorithm">The hash algorithm.</param>
        /// <param name="jobManager">The job manager.</param>
        /// <param name="daemonClient"></param>
        /// <returns></returns>
        public IShareManager Get(IHashAlgorithm hashAlgorithm, IJobManager jobManager, IDaemonClient daemonClient)
        {
            var @params = new NamedParameterOverloads
            {
                {"hashAlgorithm", hashAlgorithm},
                {"jobManager", jobManager},
                {"daemonClient", daemonClient}
            };

            return _applicationContext.Container.Resolve<IShareManager>(@params);
        }
    }
}
