using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace STS.Function.DurableFunctionTrigger;

public static class AggregatorTrigger
{
    [FunctionName("AddFromQueue")]
    public static Task AddFromQueue([QueueTrigger("durable-function-trigger")] string input,
        [DurableClient] IDurableEntityClient client)
    {
        // OperationName-Key-Value
        var values = input.Split('-');
        var entityId = new EntityId("CounterFunctionBased", values[1]);
        int amount = int.Parse(values[2]);

        return client.SignalEntityAsync(entityId, values[0], amount);
    }
    
    [FunctionName("CounterFunctionBased")]
    public static void Counter([EntityTrigger] IDurableEntityContext ctx, ILogger logger)
    {
        switch (ctx.OperationName.ToLowerInvariant())
        {
            case "add":
                ctx.SetState(ctx.GetState<int>() + ctx.GetInput<int>());
                break;
            case "reset":
                ctx.SetState(0);
                break;
            case "get":
                ctx.Return(ctx.GetState<int>());
                break;
        }

        var test = ctx.GetState<int>();
        logger.LogInformation($"Total is {test}");
    }
}