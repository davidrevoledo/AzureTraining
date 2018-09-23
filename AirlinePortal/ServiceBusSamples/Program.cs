using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;

namespace ServiceBusSamples
{
    class Program
    {
        private const string ServiceBusConnectionString =
            "Endpoint=sb://busexample1234.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=EOBtKNtPdtrd6nsZvMc11wye2T5acbp3XLE8YWT6vI8=";
        private static ITopicClient topicClient;
        private static ISubscriptionClient subscriptionClient;

        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                topicClient = new TopicClient(ServiceBusConnectionString, "topic2");

                while (true)
                {
                    // Create a new message to send to the topic.
                    var messageBody = "Duplicado !";
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody))
                    {
                        MessageId = "2"
                    };

                    await topicClient.SendAsync(message);

                    Console.WriteLine("enviado ! ");
                }
            });

            //Task.Run(async () =>
            //{

            //    var client = new ManagementClient(ServiceBusConnectionString);
            //    if (!await client.SubscriptionExistsAsync("topic1", "sub2"))
            //    {
            //        await client.CreateSubscriptionAsync("topic1", "sub2");
            //    }

            //    subscriptionClient = new SubscriptionClient(ServiceBusConnectionString, "topic1", "sub2");

            //    var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            //    {
            //        MaxConcurrentCalls = 1,
            //    };

            //    // Register the function that processes messages.
            //    subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);

            //});


            Console.ReadLine();
        }

        private static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            // Process the message.
            Console.WriteLine(
                $"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
