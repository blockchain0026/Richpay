using Pairing.Domain.Model.ShopGateways;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.ShopGateways
{
    public interface IShopGatewayQueries
    {
        Task<ShopGatewayEntry> GetShopGatewayEntryAsync(int shopGateWayId);

        Task<ShopGatewayEntry> GetMatchedShopGatewayAsync(string shopId, string paymentChannel, string paymentScheme);


        Task<List<ShopGatewayEntry>> GetShopGatewayEntrysAsync(int? pageIndex, int? take, string searchString = null, string sortField = null,
            string shopGatewayType = null, string paymentChannel = null, string paymentScheme = null,
            string direction = SortDirections.Descending);

        Task<int> GetShopGatewayEntrysTotalCount(string searchString = null, string shopGatewayType = null, string paymentChannel = null, string paymentScheme = null);


        Task<List<ShopGatewayEntry>> GetShopGatewayEntrysByShopIdAsync(
            string shopId, int? pageIndex, int? take, string searchString = null, string sortField = null,
            string shopGatewayType = null, string paymentChannel = null, string paymentScheme = null,
            string direction = SortDirections.Descending);

        Task<int> GetShopGatewayEntrysTotalCountByShopIdAsync(string shopId, string searchString = null,
            string shopGatewayType = null, string paymentChannel = null, string paymentScheme = null
            );

        ShopGatewayEntry MapFromEntity(ShopGateway entity);

        ShopGatewayEntry Add(ShopGatewayEntry shopGatewayEntry);
        void Update(ShopGatewayEntry shopGatewayEntry);
        void Delete(ShopGatewayEntry shopGatewayEntry);
        Task SaveChangesAsync();
    }

}
