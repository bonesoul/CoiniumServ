using Coinium.Core.Mining.Pool.Config;
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
        /// <param name="poolConfig">The pool configuration.</param>
        /// <returns></returns>
        public IPool Create(IPoolConfig poolConfig)
        {
            var pool = _kernel.Get<IPool>();
            pool.Initialize(poolConfig);
            return pool;
        }
    }
}
