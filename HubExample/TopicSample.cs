using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;

namespace ServiceBusSample
{
    public class TopicSample
    {
        private const string ServiceBusConnectionString =
            "Endpoint=sb://busexample1.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=LIdsRM4kAfDsj1Bu6RmOonkW5kJrfGYv0g9dwL1NCEo=";

        private const string TopicName = "topicsample";
        private const string SubscriptionName = "subcription1";

        private static ITopicClient topicClient;
        private static ISubscriptionClient subscriptionClient;

        internal async Task Invoke()
        {
            const int numberOfMessages = 10;

            // Send messages.
            await SendMessagesAsync(numberOfMessages);

            await RegisterOnMessageHandlerAndReceiveMessages();

            Console.ReadKey();
        }

        private async Task RegisterOnMessageHandlerAndReceiveMessages()
        {
            var client = new ManagementClient(ServiceBusConnectionString);

            if (!await client.SubscriptionExistsAsync(TopicName, SubscriptionName))
                await client.CreateSubscriptionAsync(TopicName, SubscriptionName);

            subscriptionClient = new SubscriptionClient(ServiceBusConnectionString, TopicName, SubscriptionName);

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            // Register the function that processes messages.
            subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);

            Console.ReadKey();

            await subscriptionClient.CloseAsync();
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

        private static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            // Process the message.
            Console.WriteLine(
                $"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private static async Task SendMessagesAsync(int numberOfMessagesToSend)
        {
            topicClient = new TopicClient(ServiceBusConnectionString, TopicName);

            var client = new ManagementClient(ServiceBusConnectionString);

            if (!await client.TopicExistsAsync(TopicName))
                await client.CreateTopicAsync(TopicName);

            try
            {
                for (var i = 0; i < numberOfMessagesToSend; i++)
                {
                    // Create a new message to send to the topic.
                    var messageBody = $"Message {i}";
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                    // Write the body of the message to the console.
                    Console.WriteLine($"Sending message: {messageBody}");

                    // Send the message to the topic.
                    await topicClient.SendAsync(message);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }

            await topicClient.CloseAsync();
        }
    }
}