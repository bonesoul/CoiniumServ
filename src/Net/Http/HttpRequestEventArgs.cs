using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Coinium.Net.Http
{
    public sealed class HttpRequestEventArgs : EventArgs
    {
        public string Data { get; private set; }
        public StringWriter Writer { get; private set; }

        public HttpRequestEventArgs(string data, StringWriter writer)
        {
            this.Data = data;
            this.Writer = writer;
        }

        public override string ToString()
        {
            return this.Data.ToString();
        }
    }
}
