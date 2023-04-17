using System.Collections.Generic;
using System.Linq;
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
    public void Run(
        [ServiceBusTrigger("reserved-quantity-update-requested", Connection = "ServiceBusConnection", IsSessionsEnabled = true)]
        OrderedItem @event,
        
        [Sql(commandText: "select [Id], [ReservedQuantity] from dbo.CatalogStock where [Id] = @CatalogItemId",
                commandType: System.Data.CommandType.Text,
                parameters: "@CatalogItemId={CatalogItemId}",
                connectionStringSetting: "CatalogConnection")]
        IEnumerable<CatalogStock> stockItems,

        [Sql("dbo.CatalogStock", connectionStringSetting: "CatalogConnection")]
        ICollector<CatalogStock> collector
    )
    {
        var stockEntry = stockItems.SingleOrDefault();
        if ( stockEntry == null )
        {
            stockEntry = new CatalogStock { Id = @event.CatalogItemId };
        }

        stockEntry.ReservedQuantity += @event.Units;
        collector.Add(stockEntry);

        _logger.LogInformation(
            "Updated product reserved quantity from order. OrderId={OrderId} | CatalogItemId={CatalogItemId} | OrderedQuantity={OrderedQuantity} | ReservedQuantity={ReservedQuantity}",
            @event.CatalogItemId,
            @event.OrderId,
            @event.Units,
            stockEntry.ReservedQuantity
            );

    }
}
