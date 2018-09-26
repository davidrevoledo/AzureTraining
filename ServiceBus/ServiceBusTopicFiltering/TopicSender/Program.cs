using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace TopicSender
{
    class Program
    {
        private const string connectionString =
            @"Endpoint=sb://servicebusazure134.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Pp7n9VNZak7DEoGZ8CWjV0UCl9iKilrqhZ0Sncd7Zec=";

        const string topic = "topicfilter";
        const string sub = "priorityfilter10";

        static void Main(string[] args)
        {
            MainAsync().GetAwaiter()
                .GetResult();

            Console.ReadLine();
        }

        private static async Task MainAsync()
        {
            var messagingFactory = MessagingFactory.CreateFromConnectionString(connectionString);

            var client = new ManagementClient(connectionString);
            if (!await client.SubscriptionExistsAsync(topic, sub))
            {
                await client.CreateSubscriptionAsync(topic, sub);
            }

            // Receive messages from subscriptions.
            var receivedMessages = 0;

            // Create subscription client.
            var subscriptionClient =
                messagingFactory.CreateSubscriptionClient(topic, sub, ReceiveMode.ReceiveAndDelete);

            //await subscriptionClient.RemoveRuleAsync("sql");

            var filter = new SqlFilter("sys.Label LIKE '%Bus%'");
            await subscriptionClient.AddRuleAsync("priority", filter);

            await Task.Delay(10);

            // Create client for the topic.
            var topicClient = messagingFactory.CreateTopicClient(topic);
            Console.WriteLine("\nSending orders to topic.");

            // Now we can start sending orders.
            await Task.WhenAll(
                SendOrder(topicClient, new Order()),
                SendOrder(topicClient, new Order { Color = "blue", Quantity = 5, Priority = "low" }),
                SendOrder(topicClient, new Order { Color = "red", Quantity = 10, Priority = "high" }),
                SendOrder(topicClient, new Order { Color = "yellow", Quantity = 5, Priority = "low" }),
                SendOrder(topicClient, new Order { Color = "blue", Quantity = 10, Priority = "low" }),
                SendOrder(topicClient, new Order { Color = "blue", Quantity = 5, Priority = "high" }),
                SendOrder(topicClient, new Order { Color = "blue", Quantity = 10, Priority = "low" }),
                SendOrder(topicClient, new Order { Color = "red", Quantity = 5, Priority = "low" }),
                SendOrder(topicClient, new Order { Color = "red", Quantity = 10, Priority = "low" })
            );


            await Task.Delay(2000);

            // Create a receiver from the subscription client and receive all messages.
            Console.WriteLine("\nReceiving messages from subscription {0}.", sub);

            while (true)
            {
                var receivedMessage = await subscriptionClient.ReceiveAsync(TimeSpan.Zero);
                if (receivedMessage != null)
                {
                    foreach (var prop in receivedMessage.Properties)
                    {
                        Console.Write("{0}={1},", prop.Key, prop.Value);
                    }
                    Console.WriteLine("CorrelationId={0}", receivedMessage.CorrelationId);

                    receivedMessage.Dispose();
                    receivedMessages++;
                }
            }
        }

        private static async Task SendOrder(TopicClient topicClient, Order order)
        {
            var message = new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(order))))
            {
                CorrelationId = order.Priority,
                Label = order.Color,
                Properties =
                {
                    { "color", order.Color },
                    { "quantity", order.Quantity },
                    { "priority", order.Priority }
                }
            };
            await topicClient.SendAsync(message);

            Console.WriteLine("Sent order with Color={0}, Quantity={1}, Priority={2}", order.Color, order.Quantity, order.Priority);
        }
    }
}
