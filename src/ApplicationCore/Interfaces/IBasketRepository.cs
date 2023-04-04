using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Specifications;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface IBasketRepository
{
    Task AddAsync(Basket basket, CancellationToken cancellationToken = default);
    Task DeleteAsync(Basket basket, CancellationToken cancellationToken = default);
    Task<Basket> FirstOrDefaultAsync(BasketWithItemsSpecification basketSpec, CancellationToken cancellationToken = default);
    Task<Basket> GetByIdAsync(int basketId, CancellationToken cancellationToken = default);
    int GetTotalBasketItems(string username);
    Task UpdateAsync(Basket basket, CancellationToken cancellationToken = default);
}
