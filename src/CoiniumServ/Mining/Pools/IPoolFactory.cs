using Coinium.Mining.Pools.Config;

namespace Coinium.Mining.Pools
{
    public interface IPoolFactory
    {
        /// <summary>
        /// Creates the specified bind ip.
        /// </summary>
        /// <param name="poolConfig">The pool configuration.</param>
        /// <returns></returns>
        IPool Create(IPoolConfig poolConfig);
    }
}
