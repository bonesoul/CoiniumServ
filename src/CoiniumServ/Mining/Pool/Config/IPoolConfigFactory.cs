namespace Coinium.Mining.Pool.Config
{
    /// <summary>
    /// Returns an instance of IPoolConfig based on the parsed config file
    /// </summary>
    public interface IPoolConfigFactory
    {
        /// <summary>
        /// Gets the specified read configuration.
        /// </summary>
        /// <param name="readConfig">The read configuration.</param>
        /// <returns></returns>
        IPoolConfig Get(dynamic readConfig);
    }
}
