using System;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Newtonsoft.Json;

namespace EventProducer
{
    internal class Program
    {
        private const string connectionString =
            @"Endpoint=sb://servicebusazure134.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Pp7n9VNZak7DEoGZ8CWjV0UCl9iKilrqhZ0Sncd7Zec=";

        private const string queueName = "transactionQueue";

        private static MessageSender sender;

        private static void Main(string[] args)
        {
            MainAsync()
                .GetAwaiter()
                .GetResult();

            Console.ReadLine();
        }

        private static async Task MainAsync()
        {
            // await SentMessages();
            try
            {
                sender = new MessageSender(connectionString, queueName);

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await Operation1();

                    await Operation2();

                    //throw new Exception();

                    scope.Complete();

                    // send messages for 1/2/3 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Messages not sent !");
            }
        }

        private static async Task SentMessages()
        {
            var sender = new MessageSender(connectionString, queueName);

            dynamic data = new[]
            {
                new {step = 1, title = "Shop"},
                new {step = 2, title = "Unpack"},
                new {step = 3, title = "Prepare"}
            };

            for (var i = 0; i < data.Length; i++)
            {
                var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data[i])))
                {
                    SessionId = data[i].step.ToString(),
                    ContentType = "application/json",
                    Label = "RecipeStep",
                    MessageId = i.ToString(),
                    TimeToLive = TimeSpan.FromMinutes(2)
                };
                await sender.SendAsync(message);
                lock (Console.Out)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Message sent: Session {0}, MessageId = {1}", message.SessionId,
                        message.MessageId);
                    Console.ResetColor();
                }
            }
        }

        private static async Task Operation2()
        {
            // logic here
            var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject("message operation 2")))
            {
                ContentType = "application/json",
                Label = "RecipeStep",
                TimeToLive = TimeSpan.FromMinutes(2),
                PartitionKey = "1"
            };
            await sender.SendAsync(message);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Message sent operation 2");
        }

        private static async Task Operation1()
        {
            // logic here
            var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject("message operation 1")))
            {
                ContentType = "application/json",
                Label = "RecipeStep",
                TimeToLive = TimeSpan.FromMinutes(2),
                PartitionKey = "1"
            };
            await sender.SendAsync(message);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Message sent operation 1");
        }
    }
}