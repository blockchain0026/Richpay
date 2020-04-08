using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.Withdrawals
{
    public interface IWithdrawalQueries
    {
        Task<WithdrawalEntry> GetWithdrawalEntryAsync(int withdrawalId);

        Task<List<WithdrawalEntry>> GetWithdrawalEntrysByUserIdAsync(string userId, int? pageIndex, int? take, string searchString = null, string sortField = null,
            DateTime? from = null, DateTime? to = null,
            string userType = null, string status = null, bool isInProcess = false, string direction = SortDirections.Descending);

        Task<int> GetWithdrawalEntrysTotalCountByUserId(string userId, string searchString = null,
            DateTime? from = null, DateTime? to = null,
            string userType = null, string status = null, bool isInProcess = false);

        Task<List<WithdrawalEntry>> GetWithdrawalEntrysAsync(int? pageIndex, int? take, string searchString = null, string sortField = null,
            DateTime? from = null, DateTime? to = null,
            string userType = null, string status = null, bool isInProcess = false, string direction = SortDirections.Descending);

        Task<int> GetWithdrawalEntrysTotalCount(string searchString = null,
            DateTime? from = null, DateTime? to = null,
            string userType = null, string status = null, bool isInProcess = false);



        List<WithdrawalBankOption> GetWithdrawalBankOptions(int? pageIndex, int? take, string searchString = null, string sortField = null, string direction = SortDirections.Descending);

        Task<int> GetWithdrawalBankOptionsTotalCount(string searchString = null);
        //Task<List<WithdrawalBankOption>> GetAllWithdrawalBankOptions();



        WithdrawalEntry Add(WithdrawalEntry withdrawalEntry);
        void Update(WithdrawalEntry withdrawalEntry);
        void Delete(WithdrawalEntry withdrawalEntry);
        Task SaveChangesAsync();
    }
}
