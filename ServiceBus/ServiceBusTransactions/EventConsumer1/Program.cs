using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace EventConsumer1
{
    internal class Program
    {
        private const string connectionString =
            @"Endpoint=sb://servicebusazure134.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Pp7n9VNZak7DEoGZ8CWjV0UCl9iKilrqhZ0Sncd7Zec=";

        private const string queueName = "transactionQueue";

        private static void Main(string[] args)
        {
            Console.WriteLine("Consumer ready ! ");

            var client = new QueueClient(connectionString, queueName);

            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            var messageHandlerOptions = new MessageHandlerOptions(LogMessageHandlerException)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = true
            };

            client.RegisterMessageHandler((message, token) =>
            {
                Console.WriteLine($"message received ! {message.MessageId}");
                return Task.CompletedTask;

            }, messageHandlerOptions);

            Console.ReadLine();
        }

        private static Task LogMessageHandlerException(ExceptionReceivedEventArgs e)
        {
            Console.WriteLine("Exception: \"{0}\" {1}", e.Exception.Message, e.ExceptionReceivedContext.EntityPath);
            return Task.CompletedTask;
        }
    }
}