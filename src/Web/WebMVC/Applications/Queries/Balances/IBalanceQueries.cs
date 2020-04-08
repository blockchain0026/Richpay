using Distributing.Domain.Model.Commissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.Balances
{
    public interface IBalanceQueries
    {
        Task<decimal?> GetAvailableBalanceByUserId(string userId);

        Task<Balance> GetBalanceFromUserAsync(string userId);

        Task<IEnumerable<Balance>> GetBalancesFromTraderUsersAsync(IEnumerable<ApplicationUser> users, int? pageIndex, int? take, string sortField = "", string direction = SortDirections.Descending);
    }
}
