using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AzureStorageQueue
{
    internal class Program
    {
        // Parse the connection string and return a reference to the storage account.
        private static readonly CloudStorageAccount storageAccount 
            = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=storageaccount1234david;AccountKey=KXu5bWOiCTh7d/PWluFOgDjjoJZHrrL2lLQXiGAT1vAHKsVx/sl/B31vJpXmA+N2PnqCmX9cCVvtxn0pBTVdlw==;EndpointSuffix=core.windows.net");
        private static void Main(string[] args)
        {
            MainAsync()
                .GetAwaiter()
                .GetResult();

            Console.ReadLine();
        }

        private static async Task MainAsync()
        {
            //await PushMessage();

            //await RaedMessage();

            await ChangeMessageContent();

            //await GetBatchMessages();
        }

        private static async Task GetBatchMessages()
        {
            var queueClient = storageAccount.CreateCloudQueueClient();

            var queue = queueClient.GetQueueReference("myqueue");

            // Fetch the queue attributes.
            await queue.FetchAttributesAsync();

            // Retrieve the cached approximate message count.
            int? cachedMessageCount = queue.ApproximateMessageCount;

            // Display number of messages.
            Console.WriteLine("Number of messages in queue: " + cachedMessageCount);
        }

        private static async Task ChangeMessageContent()
        {
            var queueClient = storageAccount.CreateCloudQueueClient();

            var queue = queueClient.GetQueueReference("myqueue");

            // Create the queue if it doesn't already exist
            // Get the message from the queue and update the message contents.
            var message = await queue.GetMessageAsync();
            message.SetMessageContent("Updated contents.");

            await queue.UpdateMessageAsync(message,
                TimeSpan.FromSeconds(60.0), // Make it invisible for another 60 seconds.
                MessageUpdateFields.Content | MessageUpdateFields.Visibility);
        }

        private static async Task RaedMessage()
        {
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference("myqueue");

            // Create the queue if it doesn't already exist
            await queue.CreateIfNotExistsAsync();

            var message = await queue.GetMessageAsync();

            await queue.DeleteMessageAsync(message);
        }

        private static async Task PushMessage()
        {
            var queueClient = storageAccount.CreateCloudQueueClient();

            var queue = queueClient.GetQueueReference("myqueue");

            // Create the queue if it doesn't already exist
            await queue.CreateIfNotExistsAsync();

            await queue.AddMessageAsync(new CloudQueueMessage("hola mundo"));
        }
    }
}