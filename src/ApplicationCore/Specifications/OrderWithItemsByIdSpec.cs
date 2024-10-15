using System;
using Ardalis.Specification;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Specifications;

public class OrderWithItemsByIdSpec : Specification<Order>
{
    public OrderWithItemsByIdSpec(Guid orderId)
    {
        Query
            .Where(order => order.Id == orderId)
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.ItemOrdered);
    }

    public OrderWithItemsByIdSpec(Guid orderId, string buyerId)
    {
        Query
            .Where(order => order.Id == orderId && order.BuyerId == buyerId)
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.ItemOrdered);
    }

}
