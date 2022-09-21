using Mango.Service.Email.Messages;
using Mango.Service.Email.Repository;
using Newtonsoft.Json;
using PaymentProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Services.EmailAPI.Messagin
{
    public class RabbitMQEmailConsumer : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly EmailRepository _emailRepo;
        string queueName = "";

        //private const string ExchangeName = "PublisherSubscriberPaymentUpdate_Exchange";
        private const string ExchangeName = "DirectPaymentUpdate_Exchange";
        private const string PaymentEmailQueue = "PaymentEmailQueue";

        public RabbitMQEmailConsumer(EmailRepository emailRepo)
        {
            _emailRepo = emailRepo;

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            //_channel.QueueDeclare(queue: "orderpaymentprocesstopic", false, false, false, arguments: null);
            _channel.ExchangeDeclare(ExchangeName,ExchangeType.Direct);
            _channel.QueueDeclare(PaymentEmailQueue,false,false,false,null);
            _channel.QueueBind(PaymentEmailQueue, ExchangeName, "PaymentEmail");
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
            _channel.BasicConsume(PaymentEmailQueue, false,consumer);
            return Task.CompletedTask;
        }

        private async Task HandleMessage(UpdatePaymentResultMessage updatePaymentResultMessage)
        {

            try
            {
                await _emailRepo.SendAndLogEmail(updatePaymentResultMessage);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
