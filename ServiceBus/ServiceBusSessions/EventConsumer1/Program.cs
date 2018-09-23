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

        private const string queueName = "sessionqueue";

        private static void Main(string[] args)
        {
            //var delay = Task.Delay(2000);
            //delay.Wait();

            Console.WriteLine("Consumer #1");

            var client = new QueueClient(connectionString, queueName);
            client.RegisterSessionHandler(
                async (session, message, cancellationToken) =>
                {
                    if (message.Label != null &&
                        message.ContentType != null &&
                        message.Label.Equals("RecipeStep", StringComparison.InvariantCultureIgnoreCase) &&
                        message.ContentType.Equals("application/json", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var body = message.Body;

                        dynamic recipeStep = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(body));
                        lock (Console.Out)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine(
                                "\t\t\t\tMessage received:  \n\t\t\t\t\t\tSessionId = {0}, \n\t\t\t\t\t\tMessageId = {1}, \n\t\t\t\t\t\tSequenceNumber = {2}," +
                                "\n\t\t\t\t\t\tContent: [ step = {3}, title = {4} ]",
                                message.SessionId,
                                message.MessageId,
                                message.SystemProperties.SequenceNumber,
                                recipeStep.step,
                                recipeStep.title);
                            Console.ResetColor();
                        }

                        await session.CompleteAsync(message.SystemProperties.LockToken);

                        //if (recipeStep.step == 5)
                        //    await session.CloseAsync();
                    }
                    else
                    {
                        await session.DeadLetterAsync(message.SystemProperties
                            .LockToken); //, "BadMessage", "Unexpected message");
                    }
                },
                new SessionHandlerOptions(LogMessageHandlerException)
                {
                    MessageWaitTimeout = TimeSpan.FromSeconds(5),
                    MaxConcurrentSessions = 1,
                    AutoComplete = false
                });

            Console.ReadLine();
        }

        private static Task LogMessageHandlerException(ExceptionReceivedEventArgs e)
        {
            Console.WriteLine("Exception: \"{0}\" {1}", e.Exception.Message, e.ExceptionReceivedContext.EntityPath);
            return Task.CompletedTask;
        }
    }
}