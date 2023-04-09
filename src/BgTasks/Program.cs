using Microsoft.Azure.Functions.Worker;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Services;
using Microsoft.eShopWeb.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(builder =>
    {
        // Is added by package "Microsoft.Azure.Functions.Worker.ApplicationInsights".
        // Documented here because it is still preview: https://github.com/Azure/azure-functions-dotnet-worker/pull/944#issue-1282987627
        builder
            .AddApplicationInsights()
            .AddApplicationInsightsLogger();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<IInventoryService, InventoryService>();
    })
    .Build();

await host.RunAsync();
