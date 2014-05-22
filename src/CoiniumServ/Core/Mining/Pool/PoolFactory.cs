using Coinium.Core.Context;
using Coinium.Core.Mining.Pool.Config;
using Ninject;

namespace Coinium.Core.Mining.Pool
{
    public class PoolFactory : IPoolFactory
    {

        /// <summary>
        /// The _application context
        /// </summary>
        private readonly IApplicationContext _applicationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolFactory" /> class.
        /// </summary>
        /// <param name="applicationContext">The application context.</param>
        public PoolFactory(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        /// <summary>
        /// Creates the specified bind ip.
        /// </summary>
        /// <param name="poolConfig">The pool configuration.</param>
        /// <returns></returns>
        public IPool Create(IPoolConfig poolConfig)
        {
            var pool = _applicationContext.Kernel.Get<IPool>();
            pool.Initialize(poolConfig);
            return pool;
        }
    }
}
