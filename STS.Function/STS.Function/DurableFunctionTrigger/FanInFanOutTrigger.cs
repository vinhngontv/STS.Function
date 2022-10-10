using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace STS.Function.DurableFunctionTrigger;

public class FanInFanOutTrigger
{
    [FunctionName("FanInFanOutTrigger")]
    public async Task RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var parallelTasks = new List<Task<int>>();

        // Get a list of N work items to process in parallel.
        var workBatch = await context.CallActivityAsync<object[]>("F1", null);
        foreach (var t in workBatch)
        {
            var task = context.CallActivityAsync<int>("F2", t);
            parallelTasks.Add(task);
        }

        await Task.WhenAll(parallelTasks);

        // Aggregate all N outputs and send the result to F3.
        var sum = parallelTasks.Sum(t => t.Result);
        await context.CallActivityAsync("F3", sum);
    }
    
    [FunctionName("F1")]
    public int[] F1([ActivityTrigger] int num, ILogger log)
    {
        return new[] { 1, 2, 3 };
    }
    
    [FunctionName("F2")]
    public int F2([ActivityTrigger] int num, ILogger log)
    {
        log.LogInformation($"====Your number is {num}");
        return num;
    }
    
    [FunctionName("F3")]
    public int F3([ActivityTrigger] int num, ILogger log)
    {
        log.LogInformation($"====Total is {num}");
        return num;
    }
}