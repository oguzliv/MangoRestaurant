namespace Mango.Services.PaymentAPI.Messagin
{
    public interface IAzureServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}
