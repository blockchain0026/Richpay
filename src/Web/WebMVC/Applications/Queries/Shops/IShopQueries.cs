using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.Shops
{
    public interface IShopQueries
    {
        Task<Shop> GetShop(string shopId);
        Task<List<Shop>> GetShops(int pageIndex, int take, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetShopsTotalCount(string searchString = null);

        Task<List<Shop>> GetDownlines(int pageIndex, int take, string shopAgentId, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetDownlinesTotalCount(string shopAgentId, string searchString = null);

        List<Shop> GetPendingReviews(int pageIndex, int take, string uplineId = null, string searchString = "", string sortField = "", string direction = "desc");
        Task<int> GetPendingReviewsTotalCount(string uplineId = null, string searchString = null);
        Task UpdateBalanceAsync(string userId, decimal balance);
        Shop Add(Shop shop);
        void Update(Shop shop);
        void Delete(Shop shop);
        Task SaveChangesAsync();
        void UpdateBalance(string userId, decimal balance);
        void UpdateBaseInfo(string userId, string fullName, string siteAddress, string phoneNumber, string email, string wechat, string qq);
    }
}
