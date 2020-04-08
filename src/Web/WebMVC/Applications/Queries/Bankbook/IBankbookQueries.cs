using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.Bankbook
{
    public interface IBankbookQueries
    {
        List<BankbookRecord> GetBankbookRecordsByUserIdAsync(
            string userId, int? pageIndex, int? take, string searchString = null, string sortField = null, string direction = SortDirections.Descending);
        List<BankbookRecord> GetBankbookRecordsByBalanceIdAsync(
            int balanceId, int? pageIndex, int? take, string searchString = null, string sortField = null, string direction = SortDirections.Descending);
                
        Task<int> GetBankbookRecordsTotalCountAsync(string userId, string searchString = null);


        BankbookRecord Add(BankbookRecord bankbookRecord);
        void Update(BankbookRecord bankbookRecord);
        void Delete(BankbookRecord bankbookRecord);
        Task SaveChangesAsync();
    }
}
