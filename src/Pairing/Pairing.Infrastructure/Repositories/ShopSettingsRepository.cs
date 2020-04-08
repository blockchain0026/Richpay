using Microsoft.EntityFrameworkCore;
using Pairing.Domain.Model.ShopSettingsDomainModel;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pairing.Infrastructure.Repositories
{
    public class ShopSettingsRepository
     : IShopSettingsRepository
    {
        private readonly PairingContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public ShopSettingsRepository(PairingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ShopSettings Add(ShopSettings shopSettings)
        {
            return _context.ShopSettings.Add(shopSettings).Entity;

        }

        public void Update(ShopSettings shopSettings)
        {
            _context.Entry(shopSettings).State = EntityState.Modified;
        }

        public async Task<ShopSettings> GetByShopSettingsIdAsync(int shopSettingsId)
        {
            var shopSettings = await _context
                                .ShopSettings
                                .FirstOrDefaultAsync(b => b.Id == shopSettingsId);
            if (shopSettings == null)
            {
                shopSettings = _context
                            .ShopSettings
                            .Local
                            .FirstOrDefault(b => b.Id == shopSettingsId);
            }
            if (shopSettings != null)
            {
                await _context.Entry(shopSettings)
                    .Collection(b => b.OrderAmountOptions).LoadAsync();
            }

            return shopSettings;
        }

        public async Task<ShopSettings> GetByShopIdAsync(string shopId)
        {
            var shopSettings = await _context
                    .ShopSettings
                    .FirstOrDefaultAsync(b => b.ShopId == shopId);

            if (shopSettings == null)
            {
                shopSettings = _context
                            .ShopSettings
                            .Local
                            .FirstOrDefault(b => b.ShopId == shopId);
            }
            if (shopSettings != null)
            {
                await _context.Entry(shopSettings)
                    .Collection(b => b.OrderAmountOptions).LoadAsync();
            }

            return shopSettings;
        }

        public void Delete(ShopSettings shopSettings)
        {
            if (shopSettings != null)
            {
                _context.ShopSettings.Remove(shopSettings);
            }
        }
    }
}
