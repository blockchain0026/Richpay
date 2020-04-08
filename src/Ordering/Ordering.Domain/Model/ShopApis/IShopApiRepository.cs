using Ordering.Domain.Model.ShopApis;
using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Model.ShopApis
{
    public interface IShopApiRepository : IRepository<ShopApi>
    {
        ShopApi Add(ShopApi shopApi);
        void Update(ShopApi shopApi);
        void Delete(ShopApi shopApi);

        Task<ShopApi> GetByShopApiIdAsync(int shopApiId);
        Task<ShopApi> GetByShopIdAsync(string shopId);
        Task<string> GetApiKeyByShopIdAsync(string shopId);
    }
}
