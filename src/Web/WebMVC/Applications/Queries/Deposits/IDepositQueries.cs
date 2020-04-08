using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.Deposits
{

    public interface IDepositQueries
    {
        Task<DepositEntry> GetDepositEntryAsync(int depositId);

        Task<List<DepositEntry>> GetDepositEntrysByUserIdAsync(string userId, int? pageIndex, int? take, string searchString = null, string sortField = null,
            DateTime? from = null, DateTime? to = null,
            string userType = null, string status = null, bool isInProcess = false, bool onlySelf = true, string direction = SortDirections.Descending);

        Task<int> GetDepositEntrysTotalCountByUserId(string userId, string searchString = null,
            DateTime? from = null, DateTime? to = null,
            string userType = null, string status = null, bool isInProcess = false, bool onlySelf = true);


        Task<List<DepositEntry>> GetDepositEntrysAsync(int? pageIndex, int? take, string searchString = null, string sortField = null,
            DateTime? from = null, DateTime? to = null,
            string userType = null, string status = null, bool isInProcess = false, string direction = SortDirections.Descending);

        Task<int> GetDepositEntrysTotalCount(string searchString = null,
            DateTime? from = null, DateTime? to = null,
            string userType = null, string status = null, bool isInProcess = false);



        Task<DepositBankAccount> GetDepositBankAccount(int depositBankAccountId);
        List<DepositBankAccount> GetDepositBankAccounts(int? pageIndex, int? take, string searchString = null, string sortField = null, string direction = SortDirections.Descending);
        Task<int> GetDepositBankAccountsTotalCount(string searchString = null);

        DepositEntry Add(DepositEntry depositEntry);
        void Update(DepositEntry depositEntry);
        void Delete(DepositEntry depositEntry);
        Task SaveChangesAsync();
        Task<List<DepositEntry>> GetDepositEntrysByUplineIdAsync(string uplineId, int? pageIndex, int? take, string searchString = null, string sortField = null, DateTime? from = null, DateTime? to = null, string userType = null, string status = null, bool isInProcess = false, string direction = "desc");
        Task<int> GetDepositEntrysTotalCountByUplineId(string uplineId, string searchString = null, DateTime? from = null, DateTime? to = null, string userType = null, string status = null, bool isInProcess = false);
    }
}
