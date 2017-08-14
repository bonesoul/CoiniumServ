using System;
using System.Globalization;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using StackExchange.Redis;
using System.Collections.Generic;

namespace CoiniumServ.Persistance.Providers.Redis
{

	/// <summary>
	/// Represents a client connection to a Redis server instance
	/// </summary>
	public class RedisClient
	{
		/// <summary>
		/// Get a value indicating whether the Redis client is connected to the server
		/// </summary>
        public bool IsConnected { get { return CM.IsConnected; } }

        public ConnectionMultiplexer CM { get; }
        public IDatabase DB { get; set; }

		/// <summary>
		/// Create a new RedisClient
		/// </summary>
		/// <param name="host">Redis server hostname</param>
		/// <param name="port">Redis server port</param>
		public RedisClient(string host, int port, string password = "", string extraopts = "")			
		{
			// create the connection
			CM = ConnectionMultiplexer.Connect(host 
												+ ((port == 0) ? "" : ":" + port)
												+ (string.IsNullOrEmpty(password) ? "" : (",password="+password))
                                                + (string.IsNullOrEmpty(extraopts) ? "" : (","+extraopts)));					
		}


		internal void Select(int databaseId)
		{
			DB = CM.GetDatabase(databaseId);
		}

		internal void HIncrByFloat(string key, string field, double increment)
		{
			DB.HashIncrement(key, field, increment);
		}

		internal void HIncrBy(string key, string field, int increment)
		{
			DB.HashIncrement(key, field, increment);
		}

        internal void Del(string key)
        {
            DB.KeyDelete(key);
        }		

        internal Dictionary<string,string> HGetAll(string key)
        {
            return DB.HashGetAll(key).ToStringDictionary();
        }

		internal void ZAdd(string key, params Tuple<double, string>[] memberScores)
		{
            SortedSetEntry[] entries = new SortedSetEntry[memberScores.Length];
            for (int i = 0; i < memberScores.Length;++i)
            {
                entries[i] = new SortedSetEntry(memberScores[i].Item2,memberScores[i].Item1);

            }
            DB.SortedSetAdd(key,entries);
		}

		internal void Rename(string key, string newKey)
		{
            DB.KeyRename(key,newKey);
		}

		internal void ZRemRangeByScore(string key, double min, double max)
		{
            DB.SortedSetRemoveRangeByScore(key,min,max);
		}

        internal string[] ZRangeByScore(string key, double min, double max)
		{
            return DB.SortedSetRangeByScore(key, min, max).ToStringArray();
		}
	}
}
