using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Nancy;

namespace coinium.Net.Web.Modules
{
    public class TestModule : NancyModule
    {
        public TestModule()
        {
            var banner = string.Format("Coinium - v{0} ready..", Assembly.GetAssembly(typeof (Program)).GetName().Version);
            Get["/"] = x => { return banner; };
        }
    }
}
