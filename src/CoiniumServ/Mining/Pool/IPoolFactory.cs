using Coinium.Mining.Pool.Config;

namespace Coinium.Mining.Pool
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
