using Mango.MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Mango.Services.PaymentAPI.RabbitMQSender
{
    public class RabbitMQPaymentMessageSender : IRabbitMQPaymentMessageSender
    {
        /* can be in appsttings*/
        private readonly string _hostname;
        private readonly string _password;
        private readonly string _username;

        private IConnection _connection;
        //private const string ExchangeName = "PublisherSubscriberPaymentUpdate_Exchange";
        private const string ExchangeName = "DirectPaymentUpdate_Exchange";
        private const string PaymentEmailQueue = "PaymentEmailQueue";
        private const string PaymentOrderQueue = "PaymentOrderQueue";

        public RabbitMQPaymentMessageSender()
        {
            _hostname = "localhost";
            _password = "guest";
            _username = "guest"; 
        }
        public void SendMessage(BaseMessage message)
        {
            if (ConnectionExist())
            {
                using var channel = _connection.CreateModel();
                //channel.QueueDeclare(queue: queueName, false, false, false, arguments: null);
                //channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout, durable: false);
                channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, durable: false);

                channel.QueueDeclare(PaymentEmailQueue,false,false,false,null );
                channel.QueueDeclare(PaymentOrderQueue, false,false,false,null );

                channel.QueueBind(PaymentEmailQueue, ExchangeName, "PaymentEmail");
                channel.QueueBind(PaymentOrderQueue, ExchangeName, "PaymentOrder");
                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);
                //channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
                channel.BasicPublish(exchange: ExchangeName, "PaymentOrder", basicProperties: null, body: body);
                channel.BasicPublish(exchange: ExchangeName, "PaymentEmail", basicProperties: null, body: body);
            }
        }
        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password
                };
                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {

            }
        }
        private bool ConnectionExist()
        {
            if(_connection != null)
            {
                return true;
            }
            CreateConnection();
            return _connection != null;
        }
    }
}
