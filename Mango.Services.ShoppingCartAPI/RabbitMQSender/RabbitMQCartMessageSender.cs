using Mango.MessageBus;

namespace Mango.Services.ShoppingCartAPI.RabbitMQSender
{
    public class RabbitMQCartMessageSender : IRabbitMQMessageSender
    {
        public void SendMessage(BaseMessage baseMessage, string queueName)
        {
            throw new NotImplementedException();
        }
    }
}
