using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

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