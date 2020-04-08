using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.IpWhitelists;
using WebMVC.Models;
using WebMVC.Models.Queries;
using WebMVC.ViewModels;

namespace WebMVC.Infrastructure.Services
{
    public interface IShopService
    {
        Task<Shop> GetShop(string shopId, string searchByUplineId = null);
        Task<List<Shop>> GetShops(int pageIndex, int take, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetShopsTotalCount(string searchString = null);

        List<Shop> GetPendingReviewShops(int pageIndex, int take, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetPendingReviewShopsTotalCount(string searchString = null);

        Task<List<Shop>> GetDownlines(int pageIndex, int take, string searchByUplineId, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetDownlinesTotalCount(string searchByUserId, string searchString = null);

        List<Shop> GetPendingReviewDownlines(int pageIndex, int take, string searchByUplineId, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetPendingReviewDownlinesTotalCount(string searchByUplineId, string searchString = null);

        Task<RebateCommission> GetRebateCommissionFromShopAgentId(string shopAgentId);

        Task<List<BankbookRecord>> GetBankbookRecords(int pageIndex, int take, string shopId, string searchByUplineId = null, string searchString = "", string sortField = "", string direction = SortDirections.Descending);

        Task<int> GetBankbookRecordsTotalCount(string shopId, string searchByUplineId = null, string searchString = null);

        Task<int> GetAwaitingUnfrozeByAdminTotalCount(string userId, string searchByUplineId = null, string searchString = null);

        Task<List<FrozenRecord>> GetAwaitingUnfrozeByAdmin(string userId, int pageIndex, int take, string searchByUplineId = null, string searchString = "", string sortField = "", string direction = SortDirections.Descending);

        Task<List<ShopGatewayEntry>> GetShopGateways(int pageIndex, int take, string searchUserId, string shopId, string searchString = "", string sortField = "",
            string shopGatewayType = null, string paymentChannel = null, string paymentScheme = null,
            string direction = SortDirections.Descending);

        Task<int> GetShopGatewaysTotalCount(string searchByUserId, string shopId, string searchString = null,
            string shopGatewayType = null, string paymentChannel = null, string paymentScheme = null);


        Task<List<decimal>> GetShopOrderAmountOpitions(string searchByUserId, string shopId);

        Task CreateShop(Shop shop, string password, string createByShopAgentId = null);
        Task UpdateShop(Shop shop, string password = null, string updateByShopAgentId = null);
        Task UpdateShopStatus(List<AccountStatus> accounts, string updateByShopAgentId = null);
        Task DeleteShop(string shopId = null, string shopUsername = null, string deleteByShopAgentId = null);
        Task DeleteShops(List<ApplicationUser> users, string deleteByShopAgentId = null);
        Task DeleteShops(List<string> shopIds, string deleteByShopAgentId = null);


        Task ReviewShops(List<AccountReview> accountReviews);
        Task ChangeBalance(string type, string userId, decimal amount, string description, string createByUserId);
        Task Unfreeze(int frozenId, string unfrozeByUserId);


        /// <summary>
        /// Only manager have permission to create shop gateway.
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="paymentChannel"></param>
        /// <param name="paymentScheme"></param>
        /// <param name="gatewayNumber"></param>
        /// <param name="name"></param>
        /// <param name="secondsBeforePayment"></param>
        /// <param name="isAmountUnchangeable"></param>
        /// <param name="isAccountUnchangeable"></param>
        /// <param name="isH5RedirectByScanEnabled"></param>
        /// <param name="isH5RedirectByClickEnabled"></param>
        /// <param name="isH5RedirectByPickingPhotoEnabled"></param>
        /// <returns></returns>
        Task CreatePlatformShopGateway(string shopId, string paymentChannel, string paymentScheme,
            string gatewayNumber, string name,
            int secondsBeforePayment, bool isAmountUnchangeable, bool isAccountUnchangeable, bool isH5RedirectByScanEnabled, bool isH5RedirectByClickEnabled, bool isH5RedirectByPickingPhotoEnabled);

        /// <summary>
        /// Only manager have permission to update alipay prefernce of shop gateway.
        /// </summary>
        /// <param name="shopGatewayId"></param>
        /// <param name="secondsBeforePayment"></param>
        /// <param name="isAmountUnchangeable"></param>
        /// <param name="isAccountUnchangeable"></param>
        /// <param name="isH5RedirectByScanEnabled"></param>
        /// <param name="isH5RedirectByClickEnabled"></param>
        /// <param name="isH5RedirectByPickingPhotoEnabled"></param>
        /// <returns></returns>
        Task UpdateShopGatewayAlipayPreference(int shopGatewayId,
            int secondsBeforePayment, bool isAmountUnchangeable, bool isAccountUnchangeable,
            bool isH5RedirectByScanEnabled, bool isH5RedirectByClickEnabled, bool isH5RedirectByPickingPhotoEnabled);

        /// <summary>
        /// Only manager have permission to delete shop gateway.
        /// </summary>
        /// <param name="shopGatewayId"></param>
        /// <returns></returns>
        void DeleteShopGateway(int shopGatewayId);


        Task AddAmountOption(string shopId, decimal amount);
        Task DeleteAmountOption(string shopId, decimal amount);
        List<string> GetIpWhitelistByShopId(string shopId);
    }
}
