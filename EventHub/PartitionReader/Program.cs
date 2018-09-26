using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace PartitionReader
{
    public class Program
    {
        private const string EventHubConnectionString = @"Endpoint=sb://eventhubazuretraining.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=dse2CjYBqLxr+ZPK6y01RY3zG1m5z+zPCHfvar0Hb7g=";
        private const string EventHubName = "hub";
        private const string StorageContainerName = "container";

        private static readonly string StorageConnectionString =
            @"DefaultEndpointsProtocol=https;AccountName=davidstorage1234;AccountKey=rTjwFWW9IB+C1ki7CTI4F6vc1qbw60kxSJuN40VG0dVgiG3vvvcLM7Y8vG2nxLASuZbfYYmhVOP6k/KRrRuplA==;EndpointSuffix=core.windows.net;TransportType=Amqp";

        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();

            Console.ReadLine();
        }

        private static async Task MainAsync(string[] args)
        {
            await Receiver();
        }

        private static async Task Receiver()
        {
            var factory = MessagingFactory.CreateFromConnectionString(EventHubConnectionString);
            var client = factory.CreateEventHubClient(EventHubName);
            var group = client.GetDefaultConsumerGroup();

            var host = group.CreateReceiver("1");
            
            while (true)
            {
                var messages = await host.ReceiveAsync(maxCount: 1000);

                foreach (var message in messages)
                {
                    Console.WriteLine(message.PartitionKey);
                }
            }
        }
    }
}
