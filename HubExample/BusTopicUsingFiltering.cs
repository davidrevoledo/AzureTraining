//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.ServiceBus.Messaging;
//using Newtonsoft.Json;

//namespace ServiceBusSample
//{
//    class BusTopicUsingFiltering
//    {
//        private const string ServiceBusConnectionString =
//            "Endpoint=sb://busexample1.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=LIdsRM4kAfDsj1Bu6RmOonkW5kJrfGYv0g9dwL1NCEo=";

//        public BusTopicUsingFiltering()
//        {
//            ExecuteAsync()
//                .GetAwaiter()
//                .GetResult();
//        }

//        private async Task ExecuteAsync()
//        {
//            var messagingFactory = MessagingFactory.CreateFromConnectionString(ServiceBusConnectionString);

//            // Create subscription client.
//            var subscriptionClient =
//                messagingFactory.CreateSubscriptionClient("", "", ReceiveMode.ReceiveAndDelete);

//            var receivedMessage = await subscriptionClient.ReceiveAsync(TimeSpan.Zero);

//            var message = new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(order))))
//            {
//                CorrelationId = order.Priority,
//                Label = order.Color,
//                Properties =
//                {
//                    { "color", order.Color },
//                    { "quantity", order.Quantity },
//                    { "priority", order.Priority }
//                }
//            };
//            await topicClient.SendAsync(message);
//        }
//    }
//}
