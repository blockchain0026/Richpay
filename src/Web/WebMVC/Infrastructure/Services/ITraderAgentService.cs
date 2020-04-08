using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.Balances;
using WebMVC.Models;
using WebMVC.Models.Queries;
using WebMVC.ViewModels;

namespace WebMVC.Infrastructure.Services
{
    public interface ITraderAgentService
    {
        Task<TraderAgent> GetTraderAgent(string traderAgentId, string searchByUplineId = null);
        Task<List<TraderAgent>> GetTraderAgents(int pageIndex, int take, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetTraderAgentsTotalCount(string searchString = null);

        List<TraderAgent> GetPendingReviewTraderAgents(int pageIndex, int take,  string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetPendingReviewTraderAgentsTotalCount(string searchString = null);

        List<TraderAgent> GetPendingReviewDownlines(int pageIndex, int take, string searchByUplineId, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetPendingReviewDownlinesTotalCount(string searchByUplineId, string searchString = null);

        Task<TradingCommission> GetTradingCommissionFromTraderAgentId(string traderAgentId);

        Task<List<TraderAgent>> GetDownlines(int pageIndex, int take, string topUserId = null, string searchByUplineId = null, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetDownlinesTotalCount(string topUserId = null, string searchString = null, string searchByUplineId = null);


        Task<List<BankbookRecord>> GetBankbookRecords(int pageIndex, int take, string traderAgentId, string searchByUplineId = null, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetBankbookRecordsTotalCount(string traderAgentId, string searchByUplineId = null, string searchString = null);
        Task<int> GetAwaitingUnfrozeByAdminTotalCount(string userId, string searchByUplineId = null, string searchString = null);
        Task<List<FrozenRecord>> GetAwaitingUnfrozeByAdmin(string userId, int pageIndex, int take, string searchByUplineId = null, string searchString = "", string sortField = "", string direction = SortDirections.Descending);

        Task<bool> IsUplineOf(string traderAgentId, string downlineId);


        Task CreateTraderAgents(TraderAgent traderAgent, string password, string createByTraderAgentId = null);
        Task UpdateTraderAgent(TraderAgent traderAgent, string password = null);
        Task UpdateTraderAgentStatus(List<AccountStatus> accounts);
        Task DeleteTraderAgent(string traderAgentId = null, string traderAgentUsername = null);
        Task DeleteTraderAgents(List<ApplicationUser> users);
        Task DeleteTraderAgents(List<string> traderAgentIds);
        Task ReviewTraderAgents(List<AccountReview> accountReviews);
        Task SubmitTransfer(string fromTraderAgentId, string toTraderId, string transferByUserId, decimal amount);
        Task ChangeBalance(string type, string userId, decimal amount, string description, string createByUserId);
        Task Unfreeze(int frozenId, string unfrozeByUserId);
    }
}
