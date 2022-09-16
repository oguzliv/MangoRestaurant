using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Service.Email.Messages;
using Mango.Service.Email.Repository;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Service.Email.Messagin
{
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly EmailRepository _emailRepository;
        private readonly IConfiguration _configuration;

        private readonly string serviceBusConnectionString;
        private readonly string subscriptionName;
        private readonly string orderUpdatePaymentResultTopic;

        private ServiceBusProcessor orderUpdatePaymentProcessor;

        public AzureServiceBusConsumer(EmailRepository emailRepository, IConfiguration configuration)
        {
            _emailRepository = emailRepository;
            _configuration = configuration;   

            serviceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");
            subscriptionName = configuration.GetValue<string>("SubscriptionName");
            orderUpdatePaymentResultTopic = configuration.GetValue<string>("OrderUpdatePaymentResultTopic");

            var client = new ServiceBusClient(serviceBusConnectionString);

            orderUpdatePaymentProcessor = client.CreateProcessor(orderUpdatePaymentResultTopic, subscriptionName);
        }
        public async Task Start()
        {

            orderUpdatePaymentProcessor.ProcessMessageAsync += OnOrderUpdateMessageReceived;
            orderUpdatePaymentProcessor.ProcessErrorAsync += ErrorHandler;
            await orderUpdatePaymentProcessor.StartProcessingAsync();
        }
        public async Task Stop()
        {

            await orderUpdatePaymentProcessor.StopProcessingAsync();
            await orderUpdatePaymentProcessor.DisposeAsync();
        }
        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        private async Task OnOrderUpdateMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            UpdatePaymentResultMessage resultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

            try
            {
                await _emailRepository.SendAndLogEmail(resultMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
