namespace Mango.Services.OrderAPI.Messagin
{
    public interface IAzureServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}
