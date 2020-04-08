using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.Frozen
{
    public interface IFrozenQueries
    {
        Task<List<FrozenRecord>> GetFrozenRecordsByUserIdAsync(string userId, int? pageIndex, int? take, string searchString = null, string sortField = null, int? typeFilter = null, int? statusFilter =null, string direction = SortDirections.Descending);
        Task<List<FrozenRecord>> GetFrozenRecordsByBalanceIdAsync(int balanceId, int? pageIndex, int? take, string searchString = null, string sortField = null, int? typeFilter =null, int? statusFilter = null, string direction = SortDirections.Descending);        
        decimal GetUserCurrentFrozenAmountAsync(string userId);
        Task<int> GetFrozenRecordsTotalCountByUserId(string userId, string searchString = null , int? typeFilter = null, int? statusFilter = null);

    }
}
