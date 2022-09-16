namespace Mango.Service.Email.Messagin
{
    public interface IAzureServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}
