using Coinium.Coin.Algorithms;
using Coinium.Coin.Daemon;
using Coinium.Mining.Jobs;

namespace Coinium.Mining.Share
{
    public interface IShareManagerFactory
    {
        /// <summary>
        /// Gets the specified daemon client.
        /// </summary>
        /// <param name="hashAlgorithm">The hash algorithm.</param>
        /// <param name="jobManager">The job manager.</param>
        /// <param name="daemonClient">The daemon client.</param>
        /// <returns></returns>
        IShareManager Get(IHashAlgorithm hashAlgorithm, IJobManager jobManager, IDaemonClient daemonClient);
    }
}
