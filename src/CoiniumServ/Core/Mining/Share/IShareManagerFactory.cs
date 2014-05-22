using Coinium.Core.Coin.Algorithms;
using Coinium.Core.Mining.Jobs;

namespace Coinium.Core.Mining.Share
{
    public interface IShareManagerFactory
    {
        /// <summary>
        /// Gets the specified daemon client.
        /// </summary>
        /// <param name="hashAlgorithm">The hash algorithm.</param>
        /// <param name="jobManager">The job manager.</param>
        /// <returns></returns>
        IShareManager Get(IHashAlgorithm hashAlgorithm, IJobManager jobManager);
    }
}
