namespace Coinium.Core.Server
{
    public interface IServerFactory
    {
        IMiningServer Get(string serviceName);
    }
}
