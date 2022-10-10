using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ordering.API.Services;

[assembly: FunctionsStartup(typeof(Ordering.API.Startup))]
namespace Ordering.API;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddLogging(loggingBuilder => { loggingBuilder.AddFilter(level => true); });

        builder.Services.AddScoped<IOrderingService, OrderingService>();
    }
}