using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Basket.API.Model;
using Basket.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Basket.API;

public class Cart
{
    private readonly IBasketRepository _basketRepository;
    
    public Cart(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }
    
    [FunctionName("GetBasket")]
    public async Task<IActionResult> GetAsync([HttpTrigger(AuthorizationLevel.Function, "get", 
        Route = "baskets/{id:guid}")] HttpRequest req, string id, ILogger log)
    {
        log.LogInformation("GetBasket API processed a request");

        if (!Guid.TryParse(id, out var basketId))
            return new BadRequestObjectResult("Bad request");

        var customerBasket = await _basketRepository.GetBasketAsync(basketId);

        if (customerBasket != null)
            return new OkObjectResult(customerBasket);
        else
            return new NotFoundObjectResult("Basket not found");
        
    }
    
    
    [FunctionName("UpdateBasket")]
    public async Task<IActionResult> UpdateAsync(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "baskets")] HttpRequestMessage req,
        ILogger log)
    {
        log.LogInformation("UpdateBasket API processed a request");

        var basket = await req.Content.ReadAsAsync<CustomerBasket>();

        var customerBasket = await _basketRepository.UpdateBasketAsync(basket);

        return new OkObjectResult(customerBasket);
    }
    
    [FunctionName("DeleteBasket")]
    public async Task<IActionResult> DeleteAsync(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "baskets/{id}")] HttpRequestMessage req,
        string id,
        ILogger log)
    {
        log.LogInformation("DeleteBasket function processed a request");

        if (!Guid.TryParse(id, out var basketId))
            return new BadRequestObjectResult("Bad request");

        var customerBasket = await _basketRepository.GetBasketAsync(basketId);

        if (customerBasket != null)
        {
            await _basketRepository.DeleteBasketAsync(basketId);
            return new OkObjectResult("Basket deleted");
        }
        else
            return new NotFoundObjectResult("Basket not found");
    }
}