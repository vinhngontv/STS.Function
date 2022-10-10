using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Catalog.API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Catalog.API;

public class ProductTrigger
{
    [FunctionName("GetItem")]
    public async Task<IActionResult> GetItemAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get",
            Route = "items/{id}")] HttpRequest req,
        int id,
        ILogger log, ExecutionContext context)
    {

        var catalogItemsFile = Path.Combine(context.FunctionAppDirectory, "catalog.items.json");
        var itemsJson = await File.ReadAllTextAsync(catalogItemsFile);

        var items = JsonConvert.DeserializeObject<List<CatalogItem>>(itemsJson);

        var item = items.FirstOrDefault(i => i.Id == id);

        if (item != null)
            return new OkObjectResult(item);
        else
            return new NotFoundObjectResult("Item  not found");
    }
    
    [FunctionName("GetItems")]
    public async Task<IActionResult> GetItemsAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get",
            Route = "items")] HttpRequest req,
        ILogger log, ExecutionContext context)
    {
    //     var catalogItemsFile = Path.Combine(context.FunctionAppDirectory, "catalog.items.json");
    //     var itemsJson = await File.ReadAllTextAsync(catalogItemsFile);
    //
    //     var items = JsonConvert.DeserializeObject<List<CatalogItem>>(itemsJson);

        return new OkObjectResult(new List<int>
        {
            1,2,3
        });
    }
}