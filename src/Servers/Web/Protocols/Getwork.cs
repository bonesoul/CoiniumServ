using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using Nancy.ModelBinding;

namespace Coinium.Servers.Web.Protocols
{
    public class Getwork : NancyModule 
    {
        public Getwork()
        {
            Post["/"] = @params =>
            {
                var msg = this.Bind<MessageType>();
                return "test";
            };
        }
    }
}
