using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Queries;
using WebMVC.ViewModels;

namespace WebMVC.Applications.Queries.TraderAgents
{
    public interface ITraderAgentQueries
    {
        Task<TraderAgent> GetTraderAgent(string traderAgentId);
        Task<List<TraderAgent>> GetTraderAgents(int pageIndex, int take, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetTraderAgentsTotalCount(string searchString = null);

        Task<List<TraderAgent>> GetDownlines(int pageIndex, int take, string traderAgentId = null, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetDownlinesTotalCount(string traderAgentId = null, string searchString = null);

        List<TraderAgent> GetPendingReviews(int pageIndex, int take, string uplineId = null, string searchString = "", string sortField = "", string direction = "desc");
        Task<int> GetPendingReviewsTotalCount(string uplineId = null, string searchString = null);

        Task UpdateBalanceAsync(string userId, decimal balance);

        TraderAgent Add(TraderAgent traderAgent);
        void Update(TraderAgent traderAgent);
        void Delete(TraderAgent traderAgent);
        Task SaveChangesAsync();
        void UpdateBalance(string userId, decimal balance);
        void UpdateBaseInfo(string userId, string fullName, string nickname, string phoneNumber, string email, string wechat, string qq);
    }
}
