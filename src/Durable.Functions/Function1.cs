using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Durable.Functions
{
    public static class Function1
    {
        [FunctionName("TimerTriggerCSharp")]
        public static async Task TimerTriggerCSharp(
            [TimerTrigger("* * * * * *")]TimerInfo myTimer,
            ILogger log,
            [OrchestrationClient] DurableOrchestrationClient client)
        {
            List<Task> tasks = new List<Task>();


            for (int i = 0; i < 10; i++)
            {
                tasks.Add(client.StartNewAsync("Function1", "david"));
            }

            await Task.WhenAll(tasks);

        }

        [FunctionName("HttpTriggerCSharp")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req,
            ILogger log)
        {
            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            return req.CreateResponse(HttpStatusCode.OK, "Hello ");
        }


        [FunctionName("Request")]
        public static async Task<HttpResponseMessage> Request(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "request")]HttpRequestMessage req,
            [OrchestrationClient] DurableOrchestrationClient starter,
            ILogger log)
        {
            string instanceId = await starter.StartNewAsync("Function1", "david");
            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("Function1")]
        public static async Task Function(
            [OrchestrationTrigger]DurableOrchestrationContext context,
            TraceWriter logTraceWriter)
        {
            var input = context.GetInput<string>();

            await Task.Delay(100000);

            throw new Exception();

            context.SetCustomStatus(new
            {
                progress = 100
            });

            logTraceWriter.Info(input);
        }

    }
}
