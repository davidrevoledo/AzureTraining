using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using ServiceBusSample.Plugins;

namespace ServiceBusSample
{
    public class BusQueueUsingPlugins
    {
        private const string ServiceBusConnectionString =
            "Endpoint=sb://busexample1.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=LIdsRM4kAfDsj1Bu6RmOonkW5kJrfGYv0g9dwL1NCEo=";

        private const string QueueName = "queueExample";
        private static IQueueClient queueClient;

        internal async Task Invoke()
        {
            const int numberOfMessages = 10;
            queueClient =
                new QueueClient(ServiceBusConnectionString, QueueName, ReceiveMode.PeekLock); // Default Peek Lock

            queueClient.RegisterPlugin(new CustomPlugin());

            // Send messages.
            // await SendMessagesAsync(numberOfMessages);

            RegisterOnMessageHandlerAndReceiveMessages();

            Console.ReadKey();

            await queueClient.CloseAsync();
        }


        private static void RegisterOnMessageHandlerAndReceiveMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
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

            // Complete the message so that it is not received again.
            // This can be done only if the queue Client is created in ReceiveMode.PeekLock mode (which is the default).
            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private static async Task SendMessagesAsync(int numberOfMessagesToSend)
        {
            var client = new ManagementClient(ServiceBusConnectionString);

            if (!await client.QueueExistsAsync(QueueName))
                await client.CreateQueueAsync(QueueName);

            try
            {
                for (var i = 0; i < numberOfMessagesToSend; i++)
                {
                    // Create a new message to send to the queue.
                    var messageBody = $"Message {i}";
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                    // Write the body of the message to the console.
                    Console.WriteLine($"Sending message: {messageBody}");

                    // Send the message to the queue.
                    await queueClient.SendAsync(message);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
    }
}