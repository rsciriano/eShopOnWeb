using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Specifications;

namespace Microsoft.eShopWeb.Infrastructure.Data;
public class BasketInMemoryRepository : IBasketRepository
{
    static ConcurrentDictionary<string, Basket> _baskets = new ConcurrentDictionary<string, Basket>();
    static int basketsCount;

    public BasketInMemoryRepository()
    {
    }

    public Task AddAsync(Basket basket, CancellationToken cancellationToken = default)
    {
        lock (_baskets)
        {
            basketsCount++;
            basket.SetNewId(basketsCount);
        }

        if (!_baskets.TryAdd(basket.BuyerId, basket))
        {
            throw new InvalidOperationException();
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Basket basket, CancellationToken cancellationToken = default)
    {
        if (!_baskets.TryRemove(basket.BuyerId, out var basketItem))
        {
            throw new InvalidOperationException();
        }
        return Task.CompletedTask;
    }

    public Task<Basket> FirstOrDefaultAsync(BasketWithItemsSpecification basketSpec, CancellationToken cancellationToken = default)
    {
        var basket = SpecificationEvaluator.Default.GetQuery(_baskets.Values.AsQueryable(), basketSpec).FirstOrDefault();
        return Task.FromResult(basket);

    }

    public Task<Basket> GetByIdAsync(int basketId, CancellationToken cancellationToken = default)
    {
        var basket = _baskets.Values.FirstOrDefault(b => b.Id == basketId);
        return Task.FromResult(basket);
    }

    public int GetTotalBasketItems(string username)
    {
        if (_baskets.TryGetValue(username, out var basket))
        {
            return basket.TotalItems;
        }
        return 0;
    }

    public Task UpdateAsync(Basket basket, CancellationToken cancellationToken = default)
    {
        if (basket == null || !_baskets.TryUpdate(basket.BuyerId, basket, basket))
        {
            throw new InvalidOperationException();
        }
        return Task.CompletedTask;
    }
}
