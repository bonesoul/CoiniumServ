using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using Serilog;

namespace Coinium.Servers.Web.Modules
{
    public class Index : NancyModule
    {
        public Index()
        {
            //Get["/"] = @params => "coinium";

            Get["/getwork"] = @params =>
            {
                return "test";
            };
        }
    }
}
