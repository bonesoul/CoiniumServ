using Nancy.TinyIoc;

namespace Coinium.Common.Context
{
    public interface IApplicationContext
    {
        TinyIoCContainer Container { get; }

        void Initialize(TinyIoCContainer container);
    }
}
