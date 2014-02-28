using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;

namespace Coinium.Servers.Web.Modules
{
    public class Index:NancyModule
    {
        public Index()
        {
            Get["/"] = x => "Coinium";
        }        
    }
}
