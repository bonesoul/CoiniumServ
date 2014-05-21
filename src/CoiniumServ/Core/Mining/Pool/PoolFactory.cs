using Ninject;

namespace Coinium.Core.Mining.Pool
{
    public class PoolFactory : IPoolFactory
    {
        /// <summary>
        /// The _kernel
        /// </summary>
        private readonly IKernel _kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolFactory"/> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public PoolFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        /// <summary>
        /// Creates the specified bind ip.
        /// </summary>
        /// <param name="bindIp">The bind ip.</param>
        /// <param name="port">The port.</param>
        /// <param name="daemonUrl">The daemon URL.</param>
        /// <param name="daemonUsername">The daemon username.</param>
        /// <param name="daemonPassword">The daemon password.</param>
        /// <returns></returns>
        public IPool Create(string bindIp, int port, string daemonUrl, string daemonUsername, string daemonPassword)
        {
            var pool = _kernel.Get<IPool>();
            pool.Initialize(bindIp, port, daemonUrl, daemonUsername, daemonPassword);
            return pool;
        }
    }
}
