using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Services;
using Microsoft.eShopWeb.Infrastructure.Data;
using Microsoft.eShopWeb.UnitTests.Builders;
using Xunit;

namespace Microsoft.eShopWeb.IntegrationTests.Repositories.BasketRepositoryTests;

public class SetQuantities
{
    private readonly BasketInMemoryRepository _basketRepository;
    private readonly BasketBuilder BasketBuilder = new BasketBuilder();

    public SetQuantities()
    {
        /*
        var dbOptions = new DbContextOptionsBuilder<BasketContext>()
            .UseInMemoryDatabase(databaseName: "TestCatalog")
            .Options;
        _basketContext = new BasketContext(dbOptions);
        _basketRepository = new BasketInMemoryRepository(_basketContext);
        */
    }

    [Fact(Skip = "Pending")]
    public async Task RemoveEmptyQuantities()
    {
        var basket = BasketBuilder.WithOneBasketItem();
        var basketService = new BasketService(_basketRepository, null);
        await _basketRepository.AddAsync(basket);
        //_basketContext.SaveChanges();

        await basketService.SetQuantities(BasketBuilder.BasketId, new Dictionary<string, int>() { { BasketBuilder.BasketId.ToString(), 0 } });

        Assert.Equal(0, basket.Items.Count);
    }
}
