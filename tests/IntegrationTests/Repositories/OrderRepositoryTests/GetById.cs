using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.Infrastructure.Data;
using Microsoft.eShopWeb.UnitTests.Builders;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.eShopWeb.IntegrationTests.Repositories.OrderRepositoryTests;

public class GetById : IClassFixture<DatabaseFixture>
{
    private CatalogContext _catalogContext;
    private EfRepository<Order> _orderRepository;
    private OrderBuilder OrderBuilder { get; } = new OrderBuilder();
    private readonly ITestOutputHelper _output;
    private readonly DatabaseFixture _fixture;

    public GetById(ITestOutputHelper output, DatabaseFixture fixture)
    {
        _output = output;
        _fixture = fixture;
    }

    private async Task InitializeDatabase(string databaseEngine)
    {
        _catalogContext = await _fixture.CreateDbContextAsync<CatalogContext>(databaseEngine);
        _orderRepository = new EfRepository<Order>(_catalogContext);
    }


    [Theory]
    [InlineData(DatabaseEngines.InMemory)]
    [InlineData(DatabaseEngines.SqlServer)]
    [InlineData(DatabaseEngines.CosmosDb)]
    public async Task GetsExistingOrder(string databaseEngine)
    {
        await InitializeDatabase(databaseEngine);

        var existingOrder = OrderBuilder.WithDefaultValues();
        _catalogContext.Orders.Add(existingOrder);
        _catalogContext.SaveChanges();
        Guid orderId = existingOrder.Id;
        _output.WriteLine($"OrderId: {orderId}");

        var orderFromRepo = await _orderRepository.GetByIdAsync(orderId);
        Assert.Equal(OrderBuilder.TestBuyerId, orderFromRepo.BuyerId);

        // Note: Using InMemoryDatabase OrderItems is available. Will be null if using SQL DB.
        // Use the OrderWithItemsByIdSpec instead of just GetById to get the full aggregate
        var firstItem = orderFromRepo.OrderItems.FirstOrDefault();
        Assert.Equal(OrderBuilder.TestUnits, firstItem.Units);
    }
}
