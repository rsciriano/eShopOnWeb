using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using NSubstitute;

namespace Microsoft.eShopWeb.UnitTests.Builders;

public class BasketBuilder
{
    private Basket _basket;
    public string BasketBuyerId => "testbuyerId@test.com";

    public string BasketId => "1";

    public BasketBuilder()
    {
        _basket = WithNoItems();
    }

    public Basket Build()
    {
        return _basket;
    }

    public Basket WithNoItems()
    {
        var basket = new Basket(BasketBuyerId);

        _basket = basket;
        return _basket;
    }

    public Basket WithOneBasketItem()
    {
        var basket = new Basket(BasketBuyerId);
        _basket = basket;
        _basket.AddItem(2, 3.40m, 4);
        return _basket;
    }
}
