using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.eShopWeb.ApplicationCore.Events;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.Extensions.Logging;

namespace BgTasks;

public class ReservedQuantityUpdater
{
    private readonly IInventoryService _inventoryService;
    private readonly ILogger<ReservedQuantityUpdater> _logger;

    public ReservedQuantityUpdater(IInventoryService inventoryService,ILogger<ReservedQuantityUpdater> log)
    {
        _inventoryService = inventoryService;
        _logger = log;
    }

    [Function("ReservedQuantityUpdater")]
    public Task Run(
        [ServiceBusTrigger("order-payment-succeeded", "ReservedQuantityUpdater", 
            Connection = "ConnectionStrings:ServiceBusConnection")] 
        OrderPaymentSucceeded @event)
    {
        return _inventoryService.UpdateOrderReservedQuantity(@event.OrderId);
    }
}
