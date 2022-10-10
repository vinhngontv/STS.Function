using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace STS.Function;

public static class CosmosDBTrigger
{
    [Disable]
    [FunctionName("CosmosDBTrigger")]
    public static async Task RunAsync([CosmosDBTrigger(
            databaseName: "sts",
            collectionName: "user",
            ConnectionStringSetting = "CosmosDBConnectionString",
            MaxItemsPerInvocation = 1,
            LeaseCollectionName = "leases")]
        IReadOnlyList<Document> input, ILogger log)
    {
        if (input is { Count: > 0 })
        {
            log.LogInformation("Documents modified " + input.Count);
            log.LogInformation("First document Id " + input[0].Id);
            
        }
    }
}