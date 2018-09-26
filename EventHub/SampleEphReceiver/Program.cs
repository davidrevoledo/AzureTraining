// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SampleEphReceiver
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.EventHubs;
    using Microsoft.Azure.EventHubs.Processor;

    public class Program
    {
        private const string EventHubConnectionString = @"Endpoint=sb://eventhubazuretraining.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=dse2CjYBqLxr+ZPK6y01RY3zG1m5z+zPCHfvar0Hb7g=";
        private const string EventHubName = "hub";
        private const string StorageContainerName = "container";

        private static readonly string StorageConnectionString =
            @"DefaultEndpointsProtocol=https;AccountName=davidstorage1234;AccountKey=rTjwFWW9IB+C1ki7CTI4F6vc1qbw60kxSJuN40VG0dVgiG3vvvcLM7Y8vG2nxLASuZbfYYmhVOP6k/KRrRuplA==;EndpointSuffix=core.windows.net";

        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            Console.WriteLine("Registering EventProcessor...");

            var eventProcessorHost = new EventProcessorHost(EventHubName,
                PartitionReceiver.DefaultConsumerGroupName,
                EventHubConnectionString,
                StorageConnectionString,
                StorageContainerName);

            // Registers the Event Processor Host and starts receiving messages
            await eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>();

            Console.WriteLine("Receiving. Press enter key to stop worker.");
            Console.ReadLine();

            // Disposes of the Event Processor Host
            await eventProcessorHost.UnregisterEventProcessorAsync();
        }
    }
}
