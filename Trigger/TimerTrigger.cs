using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace STS.Function;

public static class TimerTrigger
{
    [Disable]
    [FunctionName("TimerTrigger")]
    public static async Task RunAsync([TimerTrigger("5,8,10 * * * * *")] TimerInfo myTimer, ILogger log)
    {
        log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");
        
    }
}