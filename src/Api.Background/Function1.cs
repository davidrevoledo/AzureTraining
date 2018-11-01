using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Api.Background
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task Run(
            [QueueTrigger("invoice-request")]string myQueueItem,
            TraceWriter log)
        {
            // send queue message 

            await CrazyMethod1();
            await CrazyMethod2();
            await CrazyMethod3();

            throw new Exception();
        }

        [FunctionName("Function1")]
        public static async Task GenerateInvoice(
            [QueueTrigger("invoice-request-step2")]string myQueueItem)
        {
            await Task.Delay(10000);
        }

        private static async Task CrazyMethod1()
        {
            await Task.Delay(10000);
        }
        private static async Task CrazyMethod2()
        {
            await Task.Delay(10000);
        }
        private static async Task CrazyMethod3()
        {
            await Task.Delay(10000);
        }
    }
}
