using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace STS.Function.DurableFunctionTrigger;

public class HumanInteractionTrigger
{
    [FunctionName("ApprovalWorkflow")]
    public async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        await context.CallActivityAsync("RequestApproval", null);
        using (var timeoutCts = new CancellationTokenSource())
        {
            DateTime dueTime = context.CurrentUtcDateTime.AddHours(72);
            Task durableTimeout = context.CreateTimer(dueTime, timeoutCts.Token);

            Task<bool> approvalEvent = context.WaitForExternalEvent<bool>("ApprovalEvent");
            if (approvalEvent == await Task.WhenAny(approvalEvent, durableTimeout))
            {
                timeoutCts.Cancel();
                await context.CallActivityAsync("ProcessApproval", approvalEvent.Result);
            }
            else
            {
                await context.CallActivityAsync("Escalate", null);
            }
        }
    }
    
    [FunctionName("Approve")]
    public static async Task<IActionResult> Approve(
        [HttpTrigger(AuthorizationLevel.Function, "get")]HttpRequest req,
        [DurableClient] IDurableOrchestrationClient client,
        ILogger log)
    {
        var id = req.Query["id"];
 
        var status = await client.GetStatusAsync(id);
 
        if (status.RuntimeStatus == OrchestrationRuntimeStatus.Running)
        {
            await client.RaiseEventAsync(id, "ApprovalEvent", true);
            return new OkObjectResult("Request was approved.");
        }
 
        return new NotFoundResult();
    }
    
    [FunctionName("Reject")]
    public static async Task<IActionResult> Reject(
        [HttpTrigger(AuthorizationLevel.Function, "get")]HttpRequest req,
        [DurableClient] IDurableOrchestrationClient client,
        ILogger log)
    {
        // additional validation/null check code omitted for brevity
 
        var id = req.Query["id"];
 
        var status = await client.GetStatusAsync(id);
 
        if (status.RuntimeStatus == OrchestrationRuntimeStatus.Running)
        {
            await client.RaiseEventAsync(id, "ApprovalEvent", false);
            return new OkObjectResult("Request was reject.");
        }
 
        return new NotFoundResult();
    }
    
    [FunctionName("RequestApproval")]
    public string RequestApproval([ActivityTrigger] string requestApproval, ILogger log)
    {
        log.LogInformation($"=====Starting Request Approval");
        return "RequestApproval";
    }
    
    [FunctionName("ProcessApproval")]
    public string ProcessApproval([ActivityTrigger] bool approvalEvent, ILogger log)
    {
        log.LogInformation($"=====Process Approval");
        return "ProcessApproval";
    }
    
    [FunctionName("Escalate")]
    public string Escalate([ActivityTrigger] bool approvalEvent, ILogger log)
    {
        log.LogInformation($"=====Escalate");
        return "Escalate";
    }
}