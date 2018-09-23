using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace FunctionsV3
{
    public static class Function1
    {
        [FunctionName("InvoiceProcessingTrigger")]
        public static async Task InvoiceProcessingTrigger([QueueTrigger("invoice-requests")] string request,
            DurableOrchestrationClient starter,
            TraceWriter log)
        {
            await starter.StartNewAsync("InvoiceProcessing", request);
        }

        [FunctionName("Request")]
        public static async Task<HttpResponseMessage> Request([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            var invoiceRequest = await GenerateInvoiceRequest();


            var message = $"Your Invoice is being processed here is your reference number {invoiceRequest}";

            return req.CreateResponse(HttpStatusCode.OK, message);
        }

        private static async Task<string> GenerateInvoiceRequest()
        {
            var invoiceRequest = Guid.NewGuid().ToString("N");

            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("");

            // Create the queue client.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue.
            CloudQueue queue = queueClient.GetQueueReference("invoice-requests");

            // Create the queue if it doesn't already exist.
            await queue.CreateIfNotExistsAsync();

            // Create a message and add it to the queue.
            CloudQueueMessage message = new CloudQueueMessage(invoiceRequest);

            await queue.AddMessageAsync(message);

            return invoiceRequest;
        }

        [FunctionName("InvoiceProcessing")]
        public static async Task InvoiceProcessing(DurableOrchestrationContext ctx, TraceWriter log)
        {
            try
            {
                var request = ctx.GetInput<string>();

                var invoiceNumber = await ctx.CallActivityAsync<string>("GenerateInvoice", request);
                await ctx.CallActivityAsync<string>("StockMovement", request);
                await ctx.CallActivityAsync("PrepareLogisticDelivery", request);

                log.Info($"Invoice has been generated {invoiceNumber} for invoice request {request}");
            }
            catch (Exception)
            {
                // error handling/compensation goes here
            }
        }

        [FunctionName("GenerateInvoice")]
        public static async Task<string> GenerateInvoice([ActivityTrigger] string request)
        {
            var task = Task.Delay(50);

            await task;

            return Guid.NewGuid().ToString("N");
        }

        [FunctionName("StockMovement")]
        public static async Task StockMovement([ActivityTrigger] string request)
        {
            var task = Task.Delay(50);

            await task;
        }

        [FunctionName("PrepareLogisticDelivery")]
        public static async Task PrepareLogisticDelivery([ActivityTrigger] string request)
        {
            var task = Task.Delay(50);

            await task;
        }
    }
}