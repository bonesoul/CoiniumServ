using Coinium.Coin.Configs;
using Coinium.Common.Context;
using Nancy.TinyIoc;
using Serilog;

namespace Coinium.Mining.Pool.Config
{
    public class PoolConfigFactory : IPoolConfigFactory
    {
        /// <summary>
        /// The _application context
        /// </summary>
        private readonly IApplicationContext _applicationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolConfigFactory"/> class.
        /// </summary>
        /// <param name="applicationContext">The application context.</param>
        public PoolConfigFactory(IApplicationContext applicationContext)
        {
            Log.Debug("PoolConfigFactory() init..");
            _applicationContext = applicationContext;
        }

        /// <summary>
        /// Gets the specified read configuration.
        /// </summary>
        /// <param name="readConfig">The read configuration.</param>
        /// <returns></returns>
        public IPoolConfig Get(dynamic readConfig)
        {
            var @params = new NamedParameterOverloads { { "coinConfigFactory", _applicationContext.Container.Resolve<ICoinConfigFactory>() }, { "config", readConfig } };
            return _applicationContext.Container.Resolve<IPoolConfig>(@params);
        }
    }
}
