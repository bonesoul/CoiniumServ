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

// source from: http://stackoverflow.com/a/4673210/170181
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Serilog;

namespace Coinium.Net.Server.Http
{
    public class HttpServer : IServer, IDisposable
    {
        /// <summary>
        /// The IP address of the interface the server binded.
        /// </summary>
        public string BindIP { get; private set; }

        /// <summary>
        /// The listening port for the server.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Is server currently listening for connections?
        /// </summary>
        public bool IsListening { get; private set; }

        private HttpListener _listener;
        private Thread _listenerThread;
        private Thread[] _workers;
        private ManualResetEvent _stop, _ready;
        private Queue<HttpListenerContext> _queue;

        /// <summary>
        /// Initializes the specified port.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="maxThreads">The maximum threads.</param>
        /// <exception cref="System.NotSupportedException">HttpListener not supported. Switch to mono provided one.</exception>
        public void Initialize(int port, int maxThreads = 5)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException("HttpListener not supported. Switch to mono provided one.");

            Port = port;

            _workers = new Thread[maxThreads];
            _queue = new Queue<HttpListenerContext>();
            _stop = new ManualResetEvent(false);
            _ready = new ManualResetEvent(false);
            _listener = new HttpListener();
            _listenerThread = new Thread(HandleRequests);
        }

        public bool Start()
        {
            _listener.Prefixes.Add(String.Format(@"http://localhost:{0}/", Port));
            _listener.Start();
            _listenerThread.Start();

            for (int i = 0; i < _workers.Length; i++)
            {
                _workers[i] = new Thread(Worker);
                _workers[i].Start();
            }

            IsListening = true;

            Log.Information("Vanilla server listening on port {0}.", Port);

            return true;
        }

        public bool Stop()
        {
            _stop.Set();
            _listenerThread.Join();
            foreach (Thread worker in _workers)
                worker.Join();

            _listener.Stop();

            return true;
        }

        private void HandleRequests()
        {
            while (_listener.IsListening)
            {
                var context = _listener.BeginGetContext(ContextReady, null);

                if (0 == WaitHandle.WaitAny(new[] {_stop, context.AsyncWaitHandle}))
                    return;
            }
        }

        private void ContextReady(IAsyncResult ar)
        {
            try
            {
                lock (_queue)
                {
                    _queue.Enqueue(_listener.EndGetContext(ar));
                    _ready.Set();
                }
            }
            catch
            {
                return;
            }
        }

        private void Worker()
        {
            WaitHandle[] wait = {_ready, _stop};
            while (0 == WaitHandle.WaitAny(wait))
            {
                HttpListenerContext context;
                lock (_queue)
                {
                    if (_queue.Count > 0)
                        context = _queue.Dequeue();
                    else
                    {
                        _ready.Reset();
                        continue;
                    }
                }

                try
                {
                    ProcessRequest(context);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                }
            }
        }

        public event Action<HttpListenerContext> ProcessRequest;

        public void Dispose()
        {
            Stop();
        }
    }
}
