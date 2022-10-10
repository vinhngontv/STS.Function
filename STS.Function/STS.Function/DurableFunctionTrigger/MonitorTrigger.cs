using System;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace STS.Function.DurableFunctionTrigger;

public class MonitorTrigger
{
    [FunctionName("MonitorJobStatus")]
    public async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
    {
        // int jobId = context.GetInput<int>();
        int pollingInterval = GetPollingInterval();
        DateTime expiryTime = GetExpiryTime();

        var i = 1;
        while (context.CurrentUtcDateTime < expiryTime)
        {
            var jobStatus = await context.CallActivityAsync<string>("GetJobStatus", i);
            
            if (jobStatus == "Completed")
            {
                // Perform an action when a condition is met.
                await context.CallActivityAsync("SendAlert", null);
                break;
            }

            // Orchestration sleeps until this time.
            // var nextCheck = context.CurrentUtcDateTime.AddSeconds(pollingInterval);
            // await context.CreateTimer(nextCheck, CancellationToken.None);
            i++;
        }

        // Perform more work here, or let the orchestration end.
    }
    
    [FunctionName("GetJobStatus")]
    public string GetJobStatus([ActivityTrigger] int num, ILogger log)
    {
        var test = num % 8;
        var jobStatus = test == 0 ?"Completed" : "Running";
        
        log.LogInformation($"Process is {jobStatus} - num: {num}");

        return jobStatus;
    }
    
    [FunctionName("SendAlert")]
    public string SendAlert([ActivityTrigger] string num, ILogger log)
    {
        log.LogInformation($"=====SendAlert");
        return "SendAlert";
    }

    private int GetPollingInterval()
    {
        return 20;
    }

    private DateTime GetExpiryTime()
    {
        return DateTime.UtcNow.AddMinutes(10);
    }
}