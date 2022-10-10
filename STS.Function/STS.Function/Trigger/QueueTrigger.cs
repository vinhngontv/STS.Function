using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace STS.Function;

public static class QueueTrigger
{
    [Disable]
    [FunctionName("QueueTrigger")]
    [StorageAccount("QueueTriggerStorage")]
    public static async Task RunAsync([QueueTrigger("%QueueName%")] string myQueueItem, ILogger log)
    {
        log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        
    }
}