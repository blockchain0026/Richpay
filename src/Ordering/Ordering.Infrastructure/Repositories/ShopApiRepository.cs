using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Model.ShopApis;
using Ordering.Domain.SeedWork;
using Ordering.Infrastructure;
using Ordering.Domain.Model.ShopApis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Repositories
{
    public class ShopApiRepository
      : IShopApiRepository
    {
        private readonly OrderingContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public ShopApiRepository(OrderingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ShopApi Add(ShopApi shopApi)
        {
            return _context.ShopApis.Add(shopApi).Entity;

        }

        public void Update(ShopApi shopApi)
        {
            _context.Entry(shopApi).State = EntityState.Modified;
        }

        public async Task<ShopApi> GetByShopApiIdAsync(int shopApiId)
        {
            var shopApi = await _context
                                .ShopApis
                                .FirstOrDefaultAsync(b => b.Id == shopApiId);
            if (shopApi == null)
            {
                shopApi = _context
                            .ShopApis
                            .Local
                            .FirstOrDefault(b => b.Id == shopApiId);
            }
            if (shopApi != null)
            {
                await _context.Entry(shopApi)
                    .Collection(b => b.IpWhitelists).LoadAsync();
            }

            return shopApi;
        }

        public async Task<ShopApi> GetByShopIdAsync(string shopId)
        {
            var shopApi = await _context
                    .ShopApis
                    .Where(b => b.ShopId == shopId)
                    .FirstOrDefaultAsync();

            if (shopApi == null)
            {
                shopApi = _context
                            .ShopApis
                            .Local
                            .FirstOrDefault(b => b.ShopId == shopId);
            }
            if (shopApi != null)
            {
                await _context.Entry(shopApi)
                    .Collection(b => b.IpWhitelists).LoadAsync();
            }

            return shopApi;
        }

        public async Task<string> GetApiKeyByShopIdAsync(string shopId)
        {
            var apiKey = await _context
                    .ShopApis
                    .Where(b => b.ShopId == shopId)
                    .Select(b => b.ApiKey)
                    .FirstOrDefaultAsync();

            return apiKey;
        }

        public void Delete(ShopApi shopApi)
        {
            if (shopApi != null)
            {
                _context.ShopApis.Remove(shopApi);
            }
        }
    }
}
