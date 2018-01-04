#region License
// 
//     MIT License
//
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2017, CoiniumServ Project
//     Hüseyin Uslu, shalafiraistlin at gmail dot com
//     https://github.com/bonesoul/CoiniumServ
// 
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//     
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
// 
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace CoiniumServ.Server.Mining.Stratum.Sockets
{
    [DebuggerDisplay("Remote: {RemoteEndPoint} Connected: {IsConnected}")]
    public class Connection : IConnection
    {
        /// <summary>
        /// Gets or sets bound client.
        /// </summary>
        public IClient Client { get; set; }

        /// <summary>
        /// Gets underlying socket.
        /// </summary>
        public Socket Socket { get; private set; }

        /// <summary>
        /// Returns true if there exists an active connection.
        /// </summary>
        public bool IsConnected
        {
            get { return (Socket != null) && Socket.Connected; }
        }

        /// <summary>
        /// Returns remote endpoint.
        /// </summary>
        public IPEndPoint RemoteEndPoint
        {
            get { return (Socket == null) ? null : Socket.RemoteEndPoint as IPEndPoint; }
        }

        /// <summary>
        /// Returns local endpoint.
        /// </summary>
        public IPEndPoint LocalEndPoint
        {
            get { return Socket.LocalEndPoint as IPEndPoint; }
        }

        /// <summary>
        /// The server instance that connection is bound to.
        /// </summary>
        private readonly SocketServer _server;

        /// <summary>
        /// Default buffer size.
        /// </summary>
        public static readonly int BufferSize = 16 * 1024; // 16 KB   

        /// <summary>
        /// The recieve buffer.
        /// </summary>
        private readonly byte[] _recvBuffer = new byte[BufferSize];

        /// <summary>
        /// Returns the recieve-buffer.
        /// </summary>
        public byte[] RecvBuffer
        {
            get { return _recvBuffer; }
        }

        public Connection(SocketServer server, Socket socket)
        {
            if (server == null)
                throw new ArgumentNullException("server");

            if (socket == null)
                throw new ArgumentNullException("socket");

            _server = server;
            Socket = socket;
        }

        #region recieve methods

        // Read bytes from the Socket into the buffer in a non-blocking call.
        // This allows us to read no more than the specified count number of bytes.\
        // Note that this method should only be called prior to encryption!
        public int Receive(int start, int count)
        {
            return Socket.Receive(_recvBuffer, start, count, SocketFlags.None);
        }

        /// <summary>
        /// Begins recieving data async.
        /// </summary>
        /// <param name="callback">Callback function be called when recv() is complete.</param>
        /// <param name="state">State manager object.</param>
        /// <returns>Returns <see cref="IAsyncResult"/></returns>
        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return Socket.BeginReceive(_recvBuffer, 0, BufferSize, SocketFlags.None, callback, state);
        }

        public int EndReceive(IAsyncResult result)
        {
            return Socket.EndReceive(result);
        }

        #endregion

        #region send methods

        /// <summary>
        /// Sends byte buffer to remote endpoint.
        /// </summary>
        /// <param name="buffer">Byte buffer to send.</param>
        /// <returns>Returns count of sent bytes.</returns>
        public int Send(byte[] buffer)
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");

            return Send(buffer, 0, buffer.Length, SocketFlags.None);
        }

        /// <summary>
        /// Sends an enumarable byte buffer to remote endpoint.
        /// </summary>
        /// <param name="data">Enumrable byte buffer to send.</param>
        /// <returns>Returns count of sent bytes.</returns>
        public int Send(IEnumerable<byte> data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            return Send(data, SocketFlags.None);
        }

        /// <summary>
        /// Sends byte buffer to remote endpoint.
        /// </summary>
        /// <param name="buffer">Byte buffer to send.</param>
        /// <param name="flags">Sockets flags to use.</param>
        /// <returns>Returns count of sent bytes.</returns>
        public int Send(byte[] buffer, SocketFlags flags)
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");

            return Send(buffer, 0, buffer.Length, flags);
        }

        /// <summary>
        /// Sends an enumarable byte buffer to remote endpoint.
        /// </summary>
        /// <param name="data">Enumrable byte buffer to send.</param>
        /// <param name="flags">Sockets flags to use.</param>
        /// <returns>Returns count of sent bytes.</returns>
        public int Send(IEnumerable<byte> data, SocketFlags flags)
        {
            if (data == null) 
                throw new ArgumentNullException("data");

            if (_server == null)
                throw new Exception("[Connection] _server is null in Send");

            return _server.Send(this, data, flags);
        }

        /// <summary>
        /// Sends byte buffer to remote endpoint.
        /// </summary>
        /// <param name="buffer">Byte buffer to send.</param>
        /// <param name="start">Start index to read from buffer.</param>
        /// <param name="count">Count of bytes to send.</param>
        /// <returns>Returns count of sent bytes.</returns>
        public int Send(byte[] buffer, int start, int count)
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");

            return Send(buffer, start, count, SocketFlags.None);
        }

        /// <summary>
        /// Sends byte buffer to remote endpoint.
        /// </summary>
        /// <param name="buffer">Byte buffer to send.</param>
        /// <param name="start">Start index to read from buffer.</param>
        /// <param name="count">Count of bytes to send.</param>
        /// <param name="flags">Sockets flags to use.</param>
        /// <returns></returns>
        public int Send(byte[] buffer, int start, int count, SocketFlags flags)
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");

            if (_server == null)            
                throw new Exception("Connection is not bound to a server instance.");

            return _server.Send(this, buffer, start, count, flags);
        }

        #endregion

        #region disconnect methods

        public void Disconnect()
        {
            _server.RemoveConnection(this);
        }

        #endregion

        /// <summary>
        /// Returns a connection state string.
        /// </summary>
        /// <returns>Connection state string.</returns>
        public override string ToString()
        {
            if (Socket == null)
                return "No Socket!";

            return Socket.RemoteEndPoint != null ? Socket.RemoteEndPoint.ToString() : "Not Connected!";
        }
    }
}
