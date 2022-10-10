using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace STS.Function.DurableFunctionTrigger;

public static class CommonTrigger
{
    [FunctionName("Start")]
    public static async Task<HttpResponseMessage> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")]
        HttpRequestMessage req,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
    {
        // Function input comes from the request content.
        var instanceId = await starter.StartNewAsync("ChainingTrigger");
        // var instanceId = await starter.StartNewAsync("FanInFanOutTrigger");
        // var instanceId = await starter.StartNewAsync("MonitorJobStatus");
        // var instanceId = await starter.StartNewAsync("ApprovalWorkflow");
    
        log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

        return starter.CreateCheckStatusResponse(req, instanceId);
    }
}