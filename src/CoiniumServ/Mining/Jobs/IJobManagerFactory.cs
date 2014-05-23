using Coinium.Coin.Daemon;
using Coinium.Miner;

namespace Coinium.Mining.Jobs
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
