namespace CoiniumServ.Server.Web.Service
{
    public interface IJsonService
    {
        string ServiceResponse { get; }

        void Recache();
    }
}
