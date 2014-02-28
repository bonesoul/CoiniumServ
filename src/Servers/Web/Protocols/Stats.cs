using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Nancy;
using Serilog;

namespace Coinium.Servers.Web.Modules
{
    public class Stats : NancyModule
    {
        public Stats()
        {
            var banner = string.Format("v{0}", Assembly.GetAssembly(typeof(Program)).GetName().Version);
            Get["/version"] = x => banner;
        }
    }
}
