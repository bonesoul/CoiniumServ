using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Net;

namespace Coinium.Net.Http
{
    public sealed class HttpRequestEventArgs : EventArgs
    {
        public string Data { get; private set; }
        public HttpListenerResponse Response { get; private set; }

        public HttpRequestEventArgs(string data, HttpListenerResponse response)
        {
            this.Data = data;
            this.Response = response;
        }

        public override string ToString()
        {
            return this.Data.ToString();
        }
    }
}
