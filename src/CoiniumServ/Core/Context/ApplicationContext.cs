using Nancy.TinyIoc;

namespace Coinium.Core.Context
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
