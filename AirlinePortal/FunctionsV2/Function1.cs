using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace FunctionsV2
{
    public static class Function1
    {
        [FunctionName("InvoiceProcessing")]
        public static async Task Run([QueueTrigger("invoice-requests")]string request,
            TraceWriter log)
        {
            var invoiceNumber = await GenerateInvoice();

            log.Info($"Invoice has been generated {invoiceNumber} for invoice request {request}");
        }

        private static async Task<string> GenerateInvoice()
        {
            var task = Task.Delay(10000);

            await task;

            return Guid.NewGuid().ToString("N");
        }
    }
}
