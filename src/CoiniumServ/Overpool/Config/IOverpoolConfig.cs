using System;
using CoiniumServ.Configuration;

namespace CoiniumServ.Overpool.Config
{
    public interface IOverpoolConfig: IConfig
    {
		/// <summary>
		/// ip/hostname of coin-daemon.
		/// </summary>
		string Host { get; }

		/// <summary>
		/// the port coin daemon is listening on.
		/// </summary>
		Int32 Port { get; }

		/// <summary>
		/// username for rpc connection.
		/// </summary>
		string Username { get; }

		/// <summary>
		/// password for rpc connection.
		/// </summary>
		string Password { get; }

		/// <summary>
		/// Timeout for daemon rpc connections in seconds.
		/// </summary>
		int Timeout { get; }
    }
}
