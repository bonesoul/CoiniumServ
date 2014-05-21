using System;
namespace Coinium.Core.Mining.Pool
{
    public interface IPoolFactory
    {
        /// <summary>
        /// Creates the specified bind ip.
        /// </summary>
        /// <param name="bindIp">The bind ip.</param>
        /// <param name="port">The port.</param>
        /// <param name="daemonUrl">The daemon URL.</param>
        /// <param name="daemonUsername">The daemon username.</param>
        /// <param name="daemonPassword">The daemon password.</param>
        /// <returns></returns>
        IPool Create(string bindIp, Int32 port, string daemonUrl, string daemonUsername, string daemonPassword);
    }
}
