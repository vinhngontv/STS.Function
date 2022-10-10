using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace STS.Function.DurableFunctionTrigger;

public class ChainingTrigger
{
    [FunctionName("ChainingTrigger")]
    public async Task<int> RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
    {
        var f1 = await context.CallActivityAsync<int>("compute", new Random(10000).Next());
        log.LogInformation($"===Result F1 is {f1}.");
        
        var f2 = await context.CallActivityAsync<int>("compute", f1);        
        log.LogInformation($"===Result F2 is {f2}.");
        
        var f3 = await context.CallActivityAsync<int>("compute", f2);     
        log.LogInformation($"===Result F3 is {f3}.");
        
        return f3;
    }

    [FunctionName("compute")]
    public async Task<int> Compute([ActivityTrigger] int num, ILogger log)
    {
        num += new Random(10000).Next();
        
        await Task.Delay(10 * 1000);
        return num;
    }
}