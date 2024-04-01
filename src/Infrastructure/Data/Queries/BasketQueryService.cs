﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.Infrastructure.Data.Queries;

public class BasketQueryService : IBasketQueryService
{
    private readonly CatalogContext _dbContext;

    public BasketQueryService(CatalogContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// This method performs the sum on the database rather than in memory
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public async Task<int> CountTotalBasketItems(string username)
    {
        if (_dbContext.Database.IsCosmos())
        {
            var userBaskets = await _dbContext.Baskets
                .Where(basket => basket.BuyerId == username)
                .ToListAsync();

            var totalItems = userBaskets
                .SelectMany(item => item.Items)
                .Sum(sum => sum.Quantity);

            return totalItems;

        }
        else
        {
            var totalItems = await _dbContext.Baskets
                .Where(basket => basket.BuyerId == username)
                .SelectMany(item => item.Items)
                .SumAsync(sum => sum.Quantity);

            return totalItems;

        }
    }
}
