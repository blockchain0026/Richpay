using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Infrastructure.Services
{
    public interface IDepositService
    {
        //Task<DepositBankAccount> GetDepositBankAccount(int depositBankAccountId);
        Task<DepositBankAccount> GetDepositBankAccount(int depositBankAccountId);
        List<DepositBankAccount> GetDepositBankAccounts(int pageIndex, int take, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetDepositBankAccountsTotalCount(string searchString = null);
        IEnumerable<SelectListItem> GetDepositBankAccountSelectList();

        Task<List<DepositEntry>> GetDepositEntrys(int pageIndex, int take, string searchByUserId, string searchString = "", string sortField = "",
            DateTime? from = null, DateTime? to = null,
            string userType = "", string status = "", string direction = SortDirections.Descending);
        Task<int> GetDepositEntrysTotalCount(string searchByUserId, string searchString = null,
            DateTime? from = null, DateTime? to = null,
            string userType = "", string status = "");
        
        Task<List<DepositEntry>> GetPendingDepositEntrys(int pageIndex, int take, string searchByUserId, string searchString = "", string sortField = "",
            DateTime? from = null, DateTime? to = null,
            string userType = "", string status = "", string direction = SortDirections.Descending);
        Task<int> GetPendingDepositEntrysTotalCount(string searchByUserId, string searchString = null,
            DateTime? from = null, DateTime? to = null,
            string userType = "", string status = "");





        Task CreateDepositBankAccount(string name, string bankName, string accountName, string accountNumber);
        Task DeleteDepositBankAccount(int depositBankAccountId);
        Task DeleteDepositBankAccounts(List<int> ids);


        Task CreateDeposit(string userId, int depositAmount, string description, string createByUserId, int? depositAccountId = null);

        Task VerifyDeposit(int depositId, string verifyByAdminId);
        Task VerifyDeposits(List<int> depositIds, string verifyByAdminId);
        
        Task CancelDeposit(int depositId, string cancelByUserId);
    }
}
