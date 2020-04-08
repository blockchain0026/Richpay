using Microsoft.EntityFrameworkCore;
using Pairing.Domain.Model.ShopGateways;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pairing.Infrastructure.Repositories
{
    public class ShopGatewayRepository
     : IShopGatewayRepository
    {
        private readonly PairingContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public ShopGatewayRepository(PairingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ShopGateway Add(ShopGateway shopGateway)
        {
            return _context.ShopGateways.Add(shopGateway).Entity;

        }

        public void Update(ShopGateway shopGateway)
        {
            _context.Entry(shopGateway).State = EntityState.Modified;
        }

        public async Task<ShopGateway> GetByShopGatewayIdAsync(int shopGatewayId)
        {
            var shopGateway = await _context
                .ShopGateways
                .Include(s => s.AlipayPreference)
                .FirstOrDefaultAsync(b => b.Id == shopGatewayId);

            if (shopGateway == null)
            {
                shopGateway = _context
                            .ShopGateways
                            .Local
                            .FirstOrDefault(b => b.Id == shopGatewayId);
            }
            /*
            if (shopGateway != null)
            {
                await _context.Entry(shopGateway)
                    .Reference(b => b.ShopGatewayType).LoadAsync();
                await _context.Entry(shopGateway)
                    .Reference(b => b.PaymentChannel).LoadAsync();
                await _context.Entry(shopGateway)
                    .Reference(b => b.PaymentScheme).LoadAsync();
            }*/

            //No need to load enums. (The domain use another way to load.  )

            return shopGateway;

        }

        public async Task<IEnumerable<ShopGateway>> GetByShopIdAsync(string shopId)
        {
            var shopGateways = _context
                                .ShopGateways
                                .Include(s => s.AlipayPreference)
                                .Where(b => b.ShopId == shopId);
            /*if (shopGateway == null)
            {
                shopGateway = _context
                            .ShopGateways
                            .Local
                            .FirstOrDefault(b => b.Id == shopGatewayId);
            }
            if (shopGateway != null)
            {
                await _context.Entry(shopGateway)
                    .Reference(b => b.ShopGatewayType).LoadAsync();
                await _context.Entry(shopGateway)
                    .Reference(b => b.PaymentChannel).LoadAsync();
                await _context.Entry(shopGateway)
                    .Reference(b => b.PaymentScheme).LoadAsync();
            }*/

            //No need to load enums. (The domain use another way to load.  )

            return shopGateways;
        }

        public void Delete(ShopGateway shopGateway)
        {
            if (shopGateway != null)
            {
                _context.ShopGateways.Remove(shopGateway);
            }
        }
    }
}
