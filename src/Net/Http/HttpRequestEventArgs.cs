using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Net;
using HttpListenerContext = System.Net.HttpListenerContext;

namespace Coinium.Net.Http
{
    public sealed class HttpRequestEventArgs : EventArgs
    {
        public System.Net.HttpListenerContext Context { get; private set; }

        public HttpRequestEventArgs(HttpListenerContext context)
        {
            this.Context = context;
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}
