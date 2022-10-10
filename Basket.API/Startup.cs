using Basket.API.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: FunctionsStartup(typeof(Basket.API.Startup))]
namespace Basket.API;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddLogging(loggingBuilder => { loggingBuilder.AddFilter(level => true); });

        builder.Services.AddScoped<IBasketRepository, BasketRepository>();
    }
}