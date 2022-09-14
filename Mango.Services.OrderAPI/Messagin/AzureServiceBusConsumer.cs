using Azure.Messaging.ServiceBus;
using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Repository;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.OrderAPI.Messagin
{
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly OrderRepository _orderRepository;
        private readonly IConfiguration _configuration;
        private readonly string serviceBusConnectionString;
        private readonly string subscriptionName;
        private readonly string checkoutMessageTopic;
        private ServiceBusProcessor checkoutProcessor;

        public AzureServiceBusConsumer(OrderRepository orderRepository, IConfiguration configuration)
        {
            _orderRepository = orderRepository;
            _configuration = configuration;

            serviceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");
            subscriptionName = configuration.GetValue<string>("SubscriptionName");
            checkoutMessageTopic = configuration.GetValue<string>("CheckoutMessageTopic");

            var client = new ServiceBusClient(serviceBusConnectionString);

            checkoutProcessor = client.CreateProcessor(checkoutMessageTopic,subscriptionName);
        }
        public async Task Start()
        {
            checkoutProcessor.ProcessMessageAsync += OnCheckoutMessageReceived;
            checkoutProcessor.ProcessErrorAsync += ErrorHandler;
            await checkoutProcessor.StartProcessingAsync();
        }
        public async Task Stop()
        {
            await checkoutProcessor.StopProcessingAsync();
            await checkoutProcessor.DisposeAsync();
        }
        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        private async Task OnCheckoutMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);

            OrderHeader orderHeader = new OrderHeader
            {
                UserId = checkoutHeaderDto.UserId,
                FirstName = checkoutHeaderDto.FirstName,
                LastName = checkoutHeaderDto.LastName,
                OrderDetails = new List<OrderDetails>(),
                CardNumber = checkoutHeaderDto.CardNumber,
                CouponCode = checkoutHeaderDto.CouponCode,
                CVV = checkoutHeaderDto.CVV,
                DiscountTotal = checkoutHeaderDto.DiscountTotal,
                Email = checkoutHeaderDto.Email,
                ExpiryMonthYear = checkoutHeaderDto.ExpiryMonthYear,
                OrderTime = DateTime.UtcNow,
                OrderTotal = checkoutHeaderDto.OrderTotal,
                PaymentStatus = false,
                Phone = checkoutHeaderDto.Phone,
                PickeupDateTime = checkoutHeaderDto.PickeupDateTime
            };

            foreach(var detail in checkoutHeaderDto.CartDetails)
            {
                OrderDetails orderDetails = new OrderDetails
                {
                    ProductId = detail.ProductId,
                    ProductName = detail.Product.Name,
                    Price = detail.Product.Price,
                    Count = detail.Count
                };
                orderHeader.CartTotalItems += detail.Count;
                orderHeader.OrderDetails.Add(orderDetails);

            }
            await _orderRepository.AddOrder(orderHeader);

        }
    }
}
