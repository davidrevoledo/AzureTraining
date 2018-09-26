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

            Console.ReadLine();
        }

        private static async Task MainAsync(string[] args)
        {
            await HostProcessor();
        }

        private static async Task HostProcessor()
        {
            Console.WriteLine("Registering EventProcessor...");

            var eventProcessorHost = new EventProcessorHost(EventHubName,
                PartitionReceiver.DefaultConsumerGroupName,
                EventHubConnectionString,
                StorageConnectionString,
                StorageContainerName);


            // Registers the Event Processor Host and starts receiving messages
            await eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>();

            //  MaxBatchSize - this is the maximum size of the collection the user wants to receive in
            // an invocation of ProcessEventsAsync. Note that this is not the minimum, only the maximum.
            // If there are not this many messages to be received the ProcessEventsAsync will execute with as many as were available.

            //  PrefetchCount - this is a value used by the underlying AMQP channel to determine
            //   the upper limit of how many messages the client should receive.This value should be greater than or equal to MaxBatchSize.

            //  InvokeProcessorAfterReceiveTimeout - setting this parameter to true will result in ProcessEventsAsync
            //   being called when the underlying call the receive events on a partition times out. This is useful for taking time based actions during periods of inactivity on the partition.


            //Console.WriteLine("Receiving. Press enter key to stop worker.");
            Console.ReadLine();

            // Disposes of the Event Processor Host
            await eventProcessorHost.UnregisterEventProcessorAsync();
        }
    }
}
