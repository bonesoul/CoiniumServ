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
using System.Linq;
using System.Net;
using System.Net.Sockets;
using CoiniumServ.Utils.Extensions;
using Serilog;

namespace CoiniumServ.Server.Mining.Stratum.Sockets
{
    public class SocketServer : ISocketServer, IDisposable
    {
        /// <summary>
        /// The IP address of the interface the server binded.
        /// </summary>
        public string BindInterface { get; protected set; }

        /// <summary>
        /// The listening port for the server.
        /// </summary>
        public int Port { get; protected set; }

        /// <summary>
        /// Is server currently listening for connections?
        /// </summary>
        public bool IsListening { get; private set; }

        /// <summary>
        /// Listener socket.
        /// </summary>
        protected Socket Listener;

        /// <summary>
        /// List of connections.
        /// </summary>
        protected List<IConnection> Connections = new List<IConnection>();

        /// <summary>
        /// Used for locking connections list.
        /// </summary>
        protected object ConnectionLock = new object();

        // connection event handlers.
        public delegate void ConnectionEventHandler(object sender, ConnectionEventArgs e);
        public delegate void BannedConnectionEventHandler(object sender, BannedConnectionEventArgs e);
        public delegate void ConnectionDataEventHandler(object sender, ConnectionDataEventArgs e);

        // connection events.
        public event ConnectionEventHandler ClientConnected;
        public event ConnectionEventHandler ClientDisconnected;
        public event BannedConnectionEventHandler BannedConnection;
        public event ConnectionDataEventHandler DataReceived;
        public event ConnectionDataEventHandler DataSent;

        protected ILogger _logger;

        /// <summary>
        /// Is the instance disposed?
        /// </summary>
        private bool _disposed;

        #region listener & accept callbacks.

        /// <summary>
        /// Start listening on given interface and port.
        /// </summary>
        /// <param name="bindIP">The interface IP to listen for connections.</param>
        /// <param name="port">The port to listen for connections.</param>
        /// <returns></returns>
        protected virtual bool Listen(string bindIP, int port)
        {
            // Check if the server instance has been already disposed.
            if (_disposed) 
                throw new ObjectDisposedException(GetType().Name, "Server instance has been already disposed.");

            // Check if the server is already listening.
            if (IsListening)
                throw new InvalidOperationException(string.Format("Server is already listening on {0}:{1}", bindIP, port));

            try
            {
                // Create new TCP socket and set socket options.
                Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Setup our options:
                // * NoDelay - true - don't use packet coalescing
                // * DontLinger - true - don't keep sockets around once they've been disconnected            
                Listener.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                Listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

                // try the actual bind.
                Listener.Bind(new IPEndPoint(IPAddress.Parse(bindIP), port));
                Port = port;

                // Start listening for incoming connections.
                Listener.Listen(int.MaxValue); // let the maximum amount of accept backlog - we are basically leaving it to OS to determine the value. 
                                               // by setting the maximum available value, we can make sure that we can handle large amounts of concurrent
                                               // connection requests (maybe after a server restart).
                
                // http://blog.stephencleary.com/2009/05/using-socket-as-server-listening-socket.html
                // The “backlog” parameter to Socket.Listen is how many connections the OS may accept on behalf of the application. This is not 
                // the total number of active connections; it is only how many connections will be established if the application “gets behind”. 
                // Once connections are Accepted, they move out of the backlog queue and no longer “count” against the backlog limit.

                // The .NET docs fail to mention that int.MaxValue can be used to invoke the “dynamic backlog” feature (Windows Server systems only), 
                // essentially leaving it up to the OS. It is tempting to set this value very high (e.g., always passing int.MaxValue), but this would 
                // hurt system performance (on non-server machines) by pre-allocating a large amount of scarce resources. This value should be set to a 
                // reasonable amount (usually between 2 and 5), based on how many connections one is realistically expecting and how quickly they can be 
                // Accepted.

                IsListening = true;

                // Begin accepting any incoming connections asynchronously.
                Listener.BeginAccept(AcceptCallback, null);

                return true;
            }
            catch (SocketException exception)
            {
                _logger.Fatal("{0} can not bind on {1}, server shutting down.. Reason: {2}", GetType().Name, bindIP, exception);
                Shutdown();
                return false;
            }
        }

        /// <summary>
        /// Accept callbacks from listener socket.
        /// </summary>
        /// <param name="result"></param>
        private void AcceptCallback(IAsyncResult result)
        {
            if (Listener == null) 
                return;

            try
            {
                var socket = Listener.EndAccept(result); // Finish accepting the incoming connection.
                var banned = IsBanned(socket);

                if (banned)
                {
                    var endpoint = socket.RemoteEndPoint;
                    socket.Disconnect(true);
                    OnBannedConnection(new BannedConnectionEventArgs(endpoint));
                }
                else
                {
                    var connection = new Connection(this, socket); // Track the connection.

                    lock (ConnectionLock)
                        Connections.Add(connection); // Add the new connection to the active connections list.

                    OnClientConnection(new ConnectionEventArgs(connection)); // Raise the ClientConnected event.
                    connection.BeginReceive(ReceiveCallback, connection); // Begin receiving on the new connection connection.
                }
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "Can not accept connection");
            }
            finally
            {
                 // no matter we were able to accept last connection request, make sure we continue to listen for new connections.
                Listener.BeginAccept(AcceptCallback, null);
            }
        }

        public virtual bool IsBanned(Socket socket)
        {
            return false;
        }

        #endregion

        #region recieve callback

        private void ReceiveCallback(IAsyncResult result)
        {
            var connection = result.AsyncState as Connection; // Get the connection connection passed to the callback.
            
            if (connection == null)                 
                return;

            try
            {
                var bytesRecv = connection.EndReceive(result); // Finish receiving data from the socket.

                if (bytesRecv > 0)
                {
                    OnDataReceived(new ConnectionDataEventArgs(connection, connection.RecvBuffer.Enumerate(0, bytesRecv))); // Raise the DataReceived event.

                    // Begin receiving again on the socket, if it is connected.
                    if (connection.IsConnected)
                        connection.BeginReceive(ReceiveCallback, connection);
                }
                else
                    RemoveConnection(connection); // Connection was lost.
            }
            catch (SocketException e)
            {
                _logger.Error(e,"SocketException on ReceiveCallback");
                RemoveConnection(connection); // An error occured while receiving, connection has disconnected.
            }
        }

        #endregion

        #region send methods

        public virtual int Send(Connection connection, byte[] buffer, int start, int count, SocketFlags flags)
        {
            if (connection == null) 
                throw new ArgumentNullException("connection");

            if (buffer == null) 
                throw new ArgumentNullException("buffer");

            if (!connection.IsConnected)
                return 0;

            var totalBytesSent = 0;
            var bytesRemaining = buffer.Length;

            try
            {
                while (bytesRemaining > 0) // Ensure we send every byte.
                {
                    int bytesSent = connection.Socket.Send(buffer, start, count, flags);                       

                    if (bytesSent > 0)
                        OnDataSent(new ConnectionDataEventArgs(connection, buffer.Enumerate(totalBytesSent, bytesSent))); // Raise the Data Sent event.

                    // Decrement bytes remaining and increment bytes sent.
                    bytesRemaining -= bytesSent;
                    totalBytesSent += bytesSent;
                }
            }
            catch (SocketException socketException)
            {
                RemoveConnection(connection); // An error occured while sending, connection has disconnected.
                _logger.Error(socketException, "Send");
            }
            catch (Exception e)
            {
                RemoveConnection(connection); // An error occured while sending, it is possible that the connection has a problem.
                _logger.Error(e, "Send");
            }

            return totalBytesSent;
        }
        public virtual int Send(Connection connection, IEnumerable<byte> data, SocketFlags flags)
        {
            if (connection == null) 
                throw new ArgumentNullException("connection");

            if (data == null) 
                throw new ArgumentNullException("data");

            var buffer = data.ToArray();
            return Send(connection, buffer, 0, buffer.Length, SocketFlags.None);
        }

        #endregion

        #region disconnect & shutdown handlers

        public void RemoveConnection(IConnection connection)
        {
            if (connection == null)
                return;

            lock (connection)
            {
                if (connection.IsConnected) // disconnect the client
                {
                    try
                    {
                        connection.Socket.Disconnect(true);
                    } catch(Exception ex){
                        _logger.Error(ex,"Exception on client disconnection");
                    }
                }
            }

            connection.Client = null;

            // Remove the connection from the dictionary and raise the OnDisconnection event.
            lock (ConnectionLock)
            {
                if (Connections.Contains(connection))
                    Connections.Remove(connection);
            }

            OnClientDisconnect(new ConnectionEventArgs(connection)); // raise the ClientDisconnected event.
        }

        /// <summary>
        /// Shuts down the server instance.
        /// </summary>
        public virtual void Shutdown()
        {
            // Check if the server has been disposed.
            if (_disposed) 
                throw new ObjectDisposedException(GetType().Name, "Server has been already disposed.");

            // Check if the server is actually listening.
            if (!IsListening) 
                return;

            // Close the listener socket.
            if (Listener != null)
            {
                Listener.Close();
                Listener = null;
            }

            // Disconnect the clients.
            DisconnectAll();

            Listener = null;
            IsListening = false;
        }

        public virtual void DisconnectAll()
        {
            lock (ConnectionLock)
            {
                foreach (var connection in Connections.ToList()) // using ToList() to get a copy in order to prevent any modified collection exceptions.
                {
                    RemoveConnection(connection);
                }

                Connections.Clear();
            }
        }


        #endregion

        #region service methods

        public IEnumerable<IConnection> GetConnections()
        {
            lock (ConnectionLock)
                foreach (IConnection connection in Connections)
                    yield return connection;
        }

        #endregion

        #region server control
        public virtual bool Start()
        {
            throw new NotImplementedException();
        }

        public virtual bool Stop()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region events

        private void OnClientConnection(ConnectionEventArgs e)
        {
            var handler = ClientConnected;

            if (handler != null) 
                handler(this, e);
        }

        private void OnClientDisconnect(ConnectionEventArgs e)
        {
            var handler = ClientDisconnected;

            if (handler != null) 
                handler(this, e);
        }

        private void OnBannedConnection(BannedConnectionEventArgs e)
        {
            var handler = BannedConnection;

            if (handler != null)
                handler(this, e);
        }

        private void OnDataReceived(ConnectionDataEventArgs e)
        {
            var handler = DataReceived;

            if (handler != null) 
                handler(this, e);
        }

        private void OnDataSent(ConnectionDataEventArgs e)
        {
            var handler = DataSent;

            if (handler != null) 
                handler(this, e);
        }

        #endregion

        #region de-ctor

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                Shutdown(); // Close the listener socket.
                DisconnectAll(); // Disconnect all users.
            }

            // Dispose of unmanaged resources here.

            _disposed = true;
        }

        #endregion
    }
}
