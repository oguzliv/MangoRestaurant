using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Repository;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Services.OrderAPI.Messagin
{
    public class RabbitMQOrderPaymentConsumer : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly OrderRepository _orderRepo;
        string queueName = "";

        //private const string ExchangeName = "PublisherSubscriberPaymentUpdate_Exchange";
        private const string ExchangeName = "DirectPaymentUpdate_Exchange";
        private const string PaymentOrderQueue = "PaymentOrderQueue";

        public RabbitMQOrderPaymentConsumer(OrderRepository orderRepo)
        {
            _orderRepo = orderRepo;

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            //_channel.QueueDeclare(queue: "orderpaymentprocesstopic", false, false, false, arguments: null);
            //_channel.ExchangeDeclare(ExchangeName,ExchangeType.Direct);
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(PaymentOrderQueue, false, false, false, null);
            _channel.QueueBind(PaymentOrderQueue, ExchangeName, "PaymentOrder");

        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                UpdatePaymentResultMessage updatePaymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(content);
                HandleMessage(updatePaymentResultMessage).GetAwaiter().GetResult();
                _channel.BasicAck(ea.DeliveryTag, false);

            };
            _channel.BasicConsume(PaymentOrderQueue, false,consumer);
            return Task.CompletedTask;
        }

        private async Task HandleMessage(UpdatePaymentResultMessage updatePaymentResultMessage)
        {

            try
            {
                await _orderRepo.UpdateOrderPaymentStatus(updatePaymentResultMessage.OrderId, updatePaymentResultMessage.Status);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
