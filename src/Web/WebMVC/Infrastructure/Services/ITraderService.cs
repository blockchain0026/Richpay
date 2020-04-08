using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Queries;
using WebMVC.ViewModels;

namespace WebMVC.Infrastructure.Services
{
    public interface ITraderService
    {
        Task<Trader> GetTrader(string traderId, string searchByUplineId = null);
        Task<List<Trader>> GetTraders(int pageIndex, int take, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetTradersTotalCount(string searchString = null);

        List<Trader> GetPendingReviewTraders(int pageIndex, int take, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetPendingReviewTradersTotalCount(string searchString = null);

        Task<List<Trader>> GetDownlines(int pageIndex, int take, string searchByUplineId, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetDownlinesTotalCount(string searchByUserId, string searchString = null);

        List<Trader> GetPendingReviewDownlines(int pageIndex, int take, string searchByUplineId, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetPendingReviewDownlinesTotalCount(string searchByUplineId, string searchString = null);

        Task<TradingCommission> GetTradingCommissionFromTraderAgentId(string traderAgentId);

        Task<List<BankbookRecord>> GetBankbookRecords(int pageIndex, int take, string traderId, string searchByUplineId = null, string searchString = "", string sortField = "", string direction = SortDirections.Descending);

        Task<int> GetBankbookRecordsTotalCount(string traderId, string searchByUplineId = null, string searchString = null);

        Task<int> GetAwaitingUnfrozeByAdminTotalCount(string userId, string searchByUplineId = null, string searchString = null);

        Task<List<FrozenRecord>> GetAwaitingUnfrozeByAdmin(string userId, int pageIndex, int take, string searchByUplineId = null, string searchString = "", string sortField = "", string direction = SortDirections.Descending);


        Task CreateTrader(Trader trader, string password, string createByTraderAgentId = null);
        Task UpdateTrader(Trader trader, string password = null, string updateByTraderAgentId = null);
        Task UpdateTraderStatus(List<AccountStatus> accounts, string updateByTraderAgentId = null);
        Task DeleteTrader(string traderId = null, string traderUsername = null, string deleteByTraderAgentId = null);
        Task DeleteTraders(List<ApplicationUser> users, string deleteByTraderAgentId = null);
        Task DeleteTraders(List<string> traderIds, string deleteByTraderAgentId = null);

        Task ReviewTraders(List<AccountReview> accountReviews);
        Task ChangeBalance(string type, string userId, decimal amount, string description, string createByUserId);
        Task Unfreeze(int frozenId, string unfrozeByUserId);

    }
}
