using Microsoft.EntityFrameworkCore;
using Pairing.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Applications.Queries.ShopSettings
{
    public class ShopSettingsQueries : IShopSettingsQueries
    {
        private readonly PairingContext _pairingContext;

        public ShopSettingsQueries(PairingContext pairingContext)
        {
            _pairingContext = pairingContext ?? throw new ArgumentNullException(nameof(pairingContext));
        }

        public async Task<List<decimal>> GetOrderAmountOptionsByShopIdAsync(string shopId)
        {
            List<decimal> result = null;

            var shopSettings = await _pairingContext.ShopSettings
                .Include(s => s.OrderAmountOptions)
                .Where(s => s.ShopId == shopId)
                .FirstOrDefaultAsync();

            if (shopSettings is null)
            {
                throw new KeyNotFoundException("找无商户设定档。");
            }

            foreach (var amountOption in shopSettings.OrderAmountOptions)
            {
                result.Add(amountOption.GetAmount());
            }

            return result.OrderBy(o => o).ToList();
        }
    }
}
