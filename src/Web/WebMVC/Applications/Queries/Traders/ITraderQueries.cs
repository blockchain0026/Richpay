using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.Traders
{
    public interface ITraderQueries
    {
        Task<Trader> GetTrader(string traderId);
        Task<List<Trader>> GetTraders(int pageIndex, int take, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetTradersTotalCount(string searchString = null);

        Task<List<Trader>> GetDownlines(int pageIndex, int take, string traderAgentId, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetDownlinesTotalCount(string traderAgentId, string searchString = null);

        List<Trader> GetPendingReviews(int pageIndex, int take, string uplineId = null, string searchString = "", string sortField = "", string direction = "desc");
        Task<int> GetPendingReviewsTotalCount(string uplineId = null, string searchString = null);
        
        Task UpdateBalanceAsync(string userId, decimal balance);
      
        Trader Add(Trader trader);
        void Update(Trader trader);
        void Delete(Trader trader);
        Task SaveChangesAsync();
        void UpdateBalance(string userId, decimal balance);
        void UpdateBaseInfo(string userId, string fullName, string nickname, string phoneNumber, string email, string wechat, string qq);
    }
}
