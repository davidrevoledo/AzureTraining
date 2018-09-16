using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace InvoiceV3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public InvoiceController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            var invoiceRequest = await GenerateInvoiceRequest();

            return Ok($"Your Invoice is being processed here is your reference number {invoiceRequest}");
        }

        private async Task<string> GenerateInvoiceRequest()
        {
            var invoiceRequest = Guid.NewGuid().ToString("N");

            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_configuration["StorageKey"]);

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
    }
}