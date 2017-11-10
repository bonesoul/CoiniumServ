using System;
using Serilog;
namespace CoiniumServ.Overpool.Config
{
    public class OverpoolConfig:IOverpoolConfig
    {
		public bool Valid { get; private set; }

		public string Host { get; private set; }

		public int Port { get; private set; }

		public string Username { get; private set; }

		public string Password { get; private set; }

		public int Timeout { get; private set; }

		public OverpoolConfig(dynamic config)
		{
			try
			{
				// load the config data.
				Host = config.host;
				Port = config.port;
				Username = config.username;
				Password = config.password;
				Timeout = config.timeout == 0 ? 5 : config.timeout;
				Valid = true;
			}
			catch (Exception e)
			{
				Valid = false;
				Log.Logger.ForContext<OverpoolConfig>().Error(e, "Error loading daemon configuration");
			}
		}
    }
}
