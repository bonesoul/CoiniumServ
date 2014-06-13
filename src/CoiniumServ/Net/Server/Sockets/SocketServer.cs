/*
 *   CoiniumServ - crypto currency pool software - https://github.com/CoiniumServ/CoiniumServ
 *   Copyright (C) 2013 - 2014, Coinium Project - http://www.coinium.org
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Coinium.Common.Extensions;
using Serilog;

namespace Coinium.Net.Server.Sockets
{
    public class SocketServer : IServer, IDisposable
    {
        /// <summary>
        /// The IP address of the interface the server binded.
        /// </summary>
        public string BindIP { get; protected set; }

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
        public delegate void ConnectionDataEventHandler(object sender, ConnectionDataEventArgs e);

        // connection events.
        public event ConnectionEventHandler OnConnect;
        public event ConnectionEventHandler OnDisconnect;
        public event ConnectionDataEventHandler DataReceived;
        public event ConnectionDataEventHandler DataSent;

        /// <summary>
        /// Is the instance disposed?
        /// </summary>
        private bool _disposed;

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
                Listener.Listen(10);
                IsListening = true;

                // Begin accepting any incoming connections asynchronously.
                Listener.BeginAccept(AcceptCallback, null);

                return true;
            }
            catch (SocketException exception)
            {
                Log.Fatal("{0} can not bind on {1}, server shutting down.. Reason: {2}", GetType().Name, bindIP, exception);
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
                var connection = new Connection(this, socket); // Track the connection.

                lock (ConnectionLock) Connections.Add(connection); // Add the new connection to the activec connections list.

                OnClientConnection(new ConnectionEventArgs(connection)); // Raise the OnConnect event.

                connection.BeginReceive(ReceiveCallback, connection); // Begin receiving on the new connection connection.
                Listener.BeginAccept(AcceptCallback, null); // Continue receiving other incoming connection asynchronously.
            }
            catch (NullReferenceException) { } // we recive this after issuing server-shutdown, just ignore it.
            //catch (Exception exception)
            //{
            //    Log.Error("Can not accept connection: {0}", exception);
            //}
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
                    else
                        Log.Debug("Connection closed:" + connection.Client);
                }
                else
                {
                    RemoveConnection(connection, true); // Connection was lost.
                }
            }
            catch (SocketException e)
            {
                RemoveConnection(connection, true); // An error occured while receiving, connection has disconnected.
                Log.Error(e, "ReceiveCallback");
            }
            catch (Exception e)
            {
                RemoveConnection(connection, true); // An error occured while receiving, the connection may have been disconnected.
                Log.Error(e, "ReceiveCallback");
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
                RemoveConnection(connection, true); // An error occured while sending, connection has disconnected.
                Log.Error(socketException, "Send");
            }
            catch (Exception e)
            {
                RemoveConnection(connection, true); // An error occured while sending, it is possible that the connection has a problem.
                Log.Error(e, "Send");
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

        public virtual void DisconnectAll()
        {
            lock (ConnectionLock)
            {
                foreach (var connection in Connections.Cast<Connection>()) // Check if the connection is connected.
                {
                    // Disconnect and raise the OnDisconnect event.
                    connection.Disconnect();
                    //                    connection.Socket.Disconnect(false);
                    OnClientDisconnect(new ConnectionEventArgs(connection));
                }

                Connections.Clear();
            }
        }

        private void RemoveConnection(Connection connection, bool raiseEvent)
        {
            if (connection == null)
                return;

            // The whole Server.Disconnect vs Server.RemoveConnection vs Connection.Disconnect is a complete mess.
            // Trying to modify the code so everything gets cleaned up properly without causing an infinite recursion.
            connection.Disconnect();

            // Remove the connection from the dictionary and raise the OnDisconnection event.
            lock (ConnectionLock)
            {
                if (Connections.Contains(connection))
                    Connections.Remove(connection);
            }

            if (raiseEvent)
                NotifyRemoveConnection(connection);
        }

        private void NotifyRemoveConnection(Connection connection)
        {
            OnClientDisconnect(new ConnectionEventArgs(connection));
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
            if (!IsListening) return;

            // Close the listener socket.
            if (Listener != null)
            {
                Listener.Close();
                Listener = null;
            }

            // Disconnect the clients.
            foreach (var connection in Connections.ToList()) // use ToList() so we don't get collection modified exception there
            {
                connection.Disconnect();
            }

            Listener = null;
            IsListening = false;
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

        #region events

        protected virtual void OnClientConnection(ConnectionEventArgs e)
        {
            var handler = OnConnect;
            if (handler != null) 
                handler(this, e);
        }

        protected virtual void OnClientDisconnect(ConnectionEventArgs e)
        {
            var handler = OnDisconnect;
            if (handler != null) 
                handler(this, e);
        }

        protected virtual void OnDataReceived(ConnectionDataEventArgs e)
        {
            var handler = DataReceived;
            if (handler != null) 
                handler(this, e);
        }

        protected virtual void OnDataSent(ConnectionDataEventArgs e)
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
