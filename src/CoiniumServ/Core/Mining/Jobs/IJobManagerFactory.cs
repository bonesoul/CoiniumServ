using Coinium.Core.Coin.Daemon;
using Coinium.Core.Mining.Miner;

namespace Coinium.Core.Mining.Jobs
{
    public interface IJobManagerFactory
    {
        /// <summary>
        /// Gets the specified daemon client.
        /// </summary>
        /// <param name="daemonClient">The daemon client.</param>
        /// <param name="minerManager">The miner manager.</param>
        /// <returns></returns>
        IJobManager Get(IDaemonClient daemonClient, IMinerManager minerManager);
    }
}
