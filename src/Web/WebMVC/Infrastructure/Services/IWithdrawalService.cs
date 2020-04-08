using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Infrastructure.Services
{
    public interface IWithdrawalService
    {
        List<WithdrawalBankOption> GetWithdrawalBankOptions(int pageIndex, int take, string searchString = "", string sortField = "", string direction = SortDirections.Descending);
        Task<int> GetWithdrawalBankOptionsTotalCount(string searchString = null);
        IEnumerable<SelectListItem> GetWithdrawalBankOptionSelectList();

        Task<List<WithdrawalEntry>> GetWithdrawalEntrys(int pageIndex, int take, string searchByUserId, string searchString = "", string sortField = "",
            DateTime? from = null, DateTime? to = null,
            string userType = "",string direction = SortDirections.Descending);
        Task<int> GetWithdrawalEntrysTotalCount(string searchByUserId, string searchString = null,
            DateTime? from = null, DateTime? to = null,
            string userType = "");
        
        Task<List<WithdrawalEntry>> GetPendingWithdrawalEntrys(int pageIndex, int take, string searchByUserId, string searchString = "", string sortField = "",
            DateTime? from = null, DateTime? to = null,
            string userType = "", string status = "", string direction = SortDirections.Descending);
        Task<int> GetPendingWithdrawalEntrysTotalCount(string searchByUserId, string searchString = null,
            DateTime? from = null, DateTime? to = null,
            string userType = "", string status = "");





        Task CreateWithdrawalBankOption(string bankName);
        Task DeleteWithdrawalBankOption(int withdrawalBankOptionId);
        Task DeleteWithdrawalBankOptions(List<int> ids);



        Task CreateWithdrawal(string userId, int withdrawalAmount, int withdrawalBankOptionId, string accountName, string accountNumber, string description, string createByUserId);
        
        Task ApproveWithdrawal(int withdrawalId, string aprrovedByAdminId);
        Task ApproveWithdrawals(List<int> withdrawalIds, string aprrovedByAdminId);

        Task ConfirmWithdrawalPaymentReceived(int withdrawalId, string confirmByUserId);

        Task ForceWithdrawalSuccess(int withdrawalId, string forcedByAdminId);
        Task ForceWithdrawalsSuccess(List<int> withdrawalIds, string forcedByAdminId);

        Task CancelWithdrawal(int withdrawalId, string cancelByUserId);

        Task ApproveCancellation(int withdrawalId, string aprrovedByAdminId);
        Task ApproveCancellations(List<int> withdrawalIds, string aprrovedByAdminId);
    }
}
