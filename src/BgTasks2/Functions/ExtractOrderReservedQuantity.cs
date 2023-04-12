using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using BgTasks2.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BgTasks2.Functions;

public class ExtractOrderReservedQuantity
{
    private readonly ILogger<ExtractOrderReservedQuantity> _logger;

    public ExtractOrderReservedQuantity(ILogger<ExtractOrderReservedQuantity> log)
    {
        _logger = log;
    }

    [FunctionName("ExtractOrderReservedQuantity")]
    public async Task Run(
        [ServiceBusTrigger("order-payment-succeeded", "ReservedQuantityUpdater2", Connection = "ServiceBusConnection")]
        OrderPaymentSucceeded @event,

        [Sql(commandText: "select [OrderId], [ItemOrdered_CatalogItemId] as CatalogItemId, [Units] from dbo.OrderItems where [OrderId] = @OrderId",
                commandType: System.Data.CommandType.Text,
                parameters: "@OrderId={OrderId}",
                connectionStringSetting: "CatalogConnection")]
        IEnumerable<OrderedItem> orderedItems,

        [ServiceBus("reserved-quantity-update-requested", Connection = "ServiceBusConnection")]
        IAsyncCollector<ServiceBusMessage> collector
    )
    {
        foreach (var item in orderedItems)
        {
            var message = new ServiceBusMessage(item.ToJson());
            message.SessionId = item.CatalogItemId.ToString();
            await collector.AddAsync(message);
        }
        await collector.FlushAsync();
    }
}
