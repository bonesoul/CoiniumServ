using Coinium.Coin.Algorithms;
using Coinium.Jobs;

namespace Coinium.Share
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
