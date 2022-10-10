using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Ordering.API.Services;

namespace Ordering.API;

public class OrderTrigger
{
    private readonly IOrderingService _orderingService;
    
    public OrderTrigger(IOrderingService orderingService)
    {
        _orderingService = orderingService;
    }
    
    [FunctionName("GetOrders")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders")] HttpRequest req,
        ExecutionContext context,
        ILogger log)
    {
        log.LogInformation("GetOrders function processed a request");

        await _orderingService.Init(context);

        var orders = await _orderingService.GetOrderAsync();

        return new OkObjectResult(orders);
    }
}