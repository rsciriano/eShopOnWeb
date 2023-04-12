using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using BgTasks2.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BgTasks2;

public class UpdateProductReservedQuantity
{
    private readonly ILogger<UpdateProductReservedQuantity> _logger;

    public UpdateProductReservedQuantity(ILogger<UpdateProductReservedQuantity> log)
    {
        _logger = log;
    }

    [FunctionName("UpdateProductReservedQuantity")]
    public async Task Run(
        [ServiceBusTrigger("reserved-quantity-update-requested", Connection = "ServiceBusConnection", IsSessionsEnabled = true)]
        OrderedItem @event,
        
        [Sql(commandText: "select [Id], [ReservedQuantity] from dbo.CatalogStock where [Id] > @OrderId",
                commandType: System.Data.CommandType.Text,
                parameters: "@OrderId={OrderId}",
                connectionStringSetting: "CatalogConnection")]
        IEnumerable<CatalogStock> stockItems,

        [Sql("dbo.CatalogStock", connectionStringSetting: "CatalogConnection")]
        IAsyncCollector<CatalogStock> collector
    )
    {
        foreach (var item in stockItems)
        {
            item.ReservedQuantity += @event.Units;
            await collector.AddAsync(item);
        }

        await collector.FlushAsync();
    }
}
