using Coinium.Core.Coin.Algorithms;
using Coinium.Core.Context;
using Coinium.Core.Mining.Jobs;
using Ninject;

namespace Coinium.Core.Mining.Share
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
            var hashAlgorithmParam = new Ninject.Parameters.ConstructorArgument("hashAlgorithm", hashAlgorithm);
            var jobManagerParam = new Ninject.Parameters.ConstructorArgument("jobManager", jobManager);

            return _applicationContext.Kernel.Get<IShareManager>(hashAlgorithmParam, jobManagerParam);
        }
    }
}
