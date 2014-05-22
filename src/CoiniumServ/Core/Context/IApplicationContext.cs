using Nancy.TinyIoc;

namespace Coinium.Core.Context
{
    public interface IApplicationContext
    {
        TinyIoCContainer Container { get; }

        void Initialize(TinyIoCContainer container);
    }
}
