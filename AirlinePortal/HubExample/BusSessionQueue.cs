using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;

namespace ServiceBusSample
{
    public class BusSessionQueue
    {
        // standard
        private const string ServiceBusConnectionString =
            "Endpoint=sb://standardbus1.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;TransportType=Amqp;SharedAccessKey=JBKJmPMvCGFz4sz86Xz2InIZvypSEfGOMh7W/6XZqdA=";

        private const string QueueName = "squeue";
        private static IQueueClient queueClient;

        internal async Task Invoke()
        {
            const int numberOfMessages = 10;
            queueClient =
                new QueueClient(ServiceBusConnectionString, QueueName, ReceiveMode.ReceiveAndDelete); // Default Peek Lock

            //Send messages.
            await SendMessagesAsync(numberOfMessages);

            RegisterOnMessageHandlerAndReceiveMessages();

            Console.ReadKey();

            await queueClient.CloseAsync();

        }


        private static void RegisterOnMessageHandlerAndReceiveMessages()
        {
            queueClient.RegisterSessionHandler(ProcessMessagesAsync, new SessionHandlerOptions(ExceptionReceivedHandler)
            {
                //MaxConcurrentSessions = 2
            });
        }

        private static async Task ProcessMessagesAsync(IMessageSession session, Message message, CancellationToken cancellationToken)
        {
            // Process the message.
            Console.WriteLine(
                $"Received message: SessionId{session.SessionId} SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            await session.SetStateAsync(Encoding.UTF8.GetBytes("completed:10%;stage:stockMovement;tempOutput:foo"));
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


        private static async Task SendMessagesAsync(int numberOfMessagesToSend)
        {
            var session = "1";

            try
            {
                for (var i = 0; i < numberOfMessagesToSend; i++)
                {
                    // Create a new message to send to the queue.
                    var messageBody = $"Message {i}";
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                    message.SessionId = session;

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