using MediatR;
using Microsoft.eShopWeb.Web.ViewModels;

namespace Microsoft.eShopWeb.Web.Features.OrderDetails;

public class GetOrderDetails : IRequest<OrderDetailViewModel>
{
    public string UserName { get; set; }
    public Guid OrderId { get; set; }

    public GetOrderDetails(string userName, Guid orderId)
    {
        UserName = userName;
        OrderId = orderId;
    }
}
