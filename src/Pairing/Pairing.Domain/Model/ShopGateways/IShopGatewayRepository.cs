using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pairing.Domain.Model.ShopGateways
{
    public interface IShopGatewayRepository : IRepository<ShopGateway>
    {
        ShopGateway Add(ShopGateway shopGateway);
        void Update(ShopGateway shopGateway);
        void Delete(ShopGateway shopGateway);

        Task<ShopGateway> GetByShopGatewayIdAsync(int shopGatewayId);
        Task<IEnumerable<ShopGateway>> GetByShopIdAsync(string shopId);
    }
}
