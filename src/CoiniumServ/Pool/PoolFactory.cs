using Coinium.Common.Context;
using Coinium.Pool.Config;

namespace Coinium.Pool
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
            var pool = _applicationContext.Container.Resolve<IPool>();
            pool.Initialize(poolConfig);
            return pool;
        }
    }
}
