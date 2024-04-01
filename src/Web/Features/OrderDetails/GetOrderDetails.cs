using MediatR;
using Microsoft.eShopWeb.Web.ViewModels;

namespace Microsoft.eShopWeb.Web.Features.OrderDetails;

public class GetOrderDetails : IRequest<OrderDetailViewModel>
{
    public string UserName { get; set; }
    public string OrderId { get; set; }

    public GetOrderDetails(string userName, string orderId)
    {
        UserName = userName;
        OrderId = orderId;
    }
}
