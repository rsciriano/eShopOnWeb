using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.Infrastructure.Data.Queries;

public class BasketQueryService : IBasketQueryService
{
    private readonly IBasketRepository _basketRepository;

    public BasketQueryService(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    /// <summary>
    /// This method performs the sum on the database rather than in memory
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public async Task<int> CountTotalBasketItems(string username)
    {
        return _basketRepository.GetTotalBasketItems(username);
    }
}
