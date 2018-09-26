using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace EventGridCallback
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"C# HTTP trigger function begun");
            string response = string.Empty;

            string requestContent = await req.Content.ReadAsStringAsync();
            log.Info($"Received events: {requestContent}");

            //EventGridSubscriber eventGridSubscriber = new EventGridSubscriber();

            //EventGridEvent[] eventGridEvents = eventGridSubscriber.DeserializeEventGridEvents(requestContent);

            //foreach (EventGridEvent eventGridEvent in eventGridEvents)
            //{
            //    if (eventGridEvent.Data is ResourceDeleteSuccessData)
            //    {
            //        var eventData = (ResourceDeleteSuccessData)eventGridEvent.Data;
            //        var responseData = new SubscriptionValidationResponse
            //        {
            //            ValidationResponse = eventData.ResourceGroup
            //        };

            //        return req.CreateResponse(HttpStatusCode.OK, responseData);
            //    }
            //}

            return req.CreateResponse(HttpStatusCode.OK, requestContent);
        }
    }
}
