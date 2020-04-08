using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pairing.Domain.Model.ShopSettingsDomainModel
{
    public interface IShopSettingsRepository : IRepository<ShopSettings>
    {
        ShopSettings Add(ShopSettings shopSettings);
        void Update(ShopSettings shopSettings);
        void Delete(ShopSettings shopSettings);

        Task<ShopSettings> GetByShopSettingsIdAsync(int shopSettingsId);
        Task<ShopSettings> GetByShopIdAsync(string shopId);
    }
}
