using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Queries;
using WebMVC.ViewModels;

namespace WebMVC.Infrastructure.Services
{
    public interface IShopAgentService
    {
        Task<ShopAgent> GetShopAgent(string shopAgentId, string searchByUplineId = null);
        Task<List<ShopAgent>> GetShopAgents(int pageIndex, int take, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetShopAgentsTotalCount(string searchString = null);

        List<ShopAgent> GetPendingReviewShopAgents(int pageIndex, int take, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetPendingReviewShopAgentsTotalCount(string searchString = null);

        List<ShopAgent> GetPendingReviewDownlines(int pageIndex, int take, string searchByUplineId, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetPendingReviewDownlinesTotalCount(string searchByUplineId, string searchString = null);

        Task<RebateCommission> GetRebateCommissionFromShopAgentId(string shopAgentId);

        Task<List<ShopAgent>> GetDownlines(int pageIndex, int take, string topUserId = null, string searchByUplineId = null, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetDownlinesTotalCount(string topUserId = null, string searchString = null, string searchByUplineId = null);


        Task<List<BankbookRecord>> GetBankbookRecords(int pageIndex, int take, string shopAgentId, string searchByUplineId = null, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetBankbookRecordsTotalCount(string shopAgentId, string searchByUplineId = null, string searchString = null);
        Task<int> GetAwaitingUnfrozeByAdminTotalCount(string userId, string searchByUplineId = null, string searchString = null);
        Task<List<FrozenRecord>> GetAwaitingUnfrozeByAdmin(string userId, int pageIndex, int take, string searchByUplineId = null, string searchString = "", string sortField = "", string direction = SortDirections.Descending);

        Task<bool> IsUplineOf(string shopAgentId, string downlineId);


        Task CreateShopAgents(ShopAgent shopAgent, string password, string createByShopAgentId = null);
        Task UpdateShopAgent(ShopAgent shopAgent, string password = null);
        Task UpdateShopAgentStatus(List<AccountStatus> accounts);
        Task DeleteShopAgent(string shopAgentId = null, string shopAgentUsername = null);
        Task DeleteShopAgents(List<ApplicationUser> users);
        Task DeleteShopAgents(List<string> shopAgentIds);
        Task ReviewShopAgents(List<AccountReview> accountReviews);
        Task ChangeBalance(string type, string userId, decimal amount, string description, string createByUserId);
        Task Unfreeze(int frozenId, string unfrozeByUserId);
    }
}
