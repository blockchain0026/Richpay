using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.ShopAgents
{
    public interface IShopAgentQueries
    {
        Task<ShopAgent> GetShopAgent(string shopAgentId);
        Task<List<ShopAgent>> GetShopAgents(int pageIndex, int take, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetShopAgentsTotalCount(string searchString = null);

        Task<List<ShopAgent>> GetDownlines(int pageIndex, int take, string shopAgentId = null, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetDownlinesTotalCount(string shopAgentId = null, string searchString = null);

        List<ShopAgent> GetPendingReviews(int pageIndex, int take, string uplineId = null, string searchString = "", string sortField = "", string direction = "desc");
        Task<int> GetPendingReviewsTotalCount(string uplineId = null, string searchString = null);

        Task UpdateBalanceAsync(string userId, decimal balance);

        ShopAgent Add(ShopAgent shopAgent);
        void Update(ShopAgent shopAgent);
        void Delete(ShopAgent shopAgent);
        Task SaveChangesAsync();
        void UpdateBalance(string userId, decimal balance);
        void UpdateBaseInfo(string userId, string fullName, string nickname, string phoneNumber, string email, string wechat, string qq);
    }
}
