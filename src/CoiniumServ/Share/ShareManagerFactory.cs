using Coinium.Coin.Algorithms;
using Coinium.Common.Context;
using Coinium.Jobs;
using Nancy.TinyIoc;

namespace Coinium.Share
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
            _applicationContext = applicationContext;
        }

        /// <summary>
        /// Gets the specified daemon client.
        /// </summary>
        /// <param name="hashAlgorithm">The hash algorithm.</param>
        /// <param name="jobManager">The job manager.</param>
        /// <returns></returns>
        public IShareManager Get(IHashAlgorithm hashAlgorithm, IJobManager jobManager)
        {
            var @params = new NamedParameterOverloads() {{"hashAlgorithm", hashAlgorithm}, {"jobManager", jobManager}};

            return _applicationContext.Container.Resolve<IShareManager>(@params);
        }
    }
}
