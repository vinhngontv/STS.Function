using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace STS.Function;

public static class TimerTrigger
{
    [FunctionName("TimerTrigger")]
    public static async Task RunAsync([TimerTrigger("%TimerCron%")] TimerInfo myTimer, ILogger log)
    {
        log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");
        
    }
}