using System.Reflection;
using Nancy;

namespace Coinium.Servers.Web.Modules
{
    public class Version : NancyModule
    {
        public Version()
        {
            var banner = string.Format("v{0}", Assembly.GetAssembly(typeof (Program)).GetName().Version);
            Get["/version"] = x => banner;
        }
    }
}
