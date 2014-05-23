using Nancy.TinyIoc;

namespace Coinium.Common.Context
{
    public class ApplicationContext : IApplicationContext
    {
        public TinyIoCContainer Container { get; private set; }

        public void Initialize(TinyIoCContainer container)
        {
            Container = container;
        }
    }
}
