using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await GenerateInvoiceRequest();

            return Ok($"Tu factura se esta generando numero de solicitud:  {Guid.NewGuid().ToString("N")}");
        }

        private async Task GenerateInvoiceRequest()
        {
            CloudStorageAccount storageAccount
               = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=storageaccount1234david;AccountKey=KXu5bWOiCTh7d/PWluFOgDjjoJZHrrL2lLQXiGAT1vAHKsVx/sl/B31vJpXmA+N2PnqCmX9cCVvtxn0pBTVdlw==;EndpointSuffix=core.windows.net");

            var queueClient = storageAccount.CreateCloudQueueClient();

            var queue = queueClient.GetQueueReference("invoice-request");

            // Create the queue if it doesn't already exist
            await queue.CreateIfNotExistsAsync();

            await queue.AddMessageAsync(new CloudQueueMessage("generate invoice for client : 1"));
        }
    }
}
