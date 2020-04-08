using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.QrCodes
{
    public interface IQrCodeQueries
    {
        Task<QrCodeEntry> GetQrCodeEntryAsync(int qrCodeId);

        Task<List<QrCodeEntry>> GetQrCodeEntrysAsync(int? pageIndex, int? take, string searchString = null, string sortField = null,
            string qrCodeType = null, string paymentChannel = null, string paymentScheme = null, string qrCodeStatus = null, string pairingStatus = null, bool isApproved = true,
            string direction = SortDirections.Descending);

        Task<int> GetQrCodeEntrysTotalCount(string searchString = null, string qrCodeType = null, string paymentChannel = null, string paymentScheme = null, string qrCodeStatus = null, string pairingStatus = null, bool isApproved = true);


        Task<List<QrCodeEntry>> GetQrCodeEntrysByUserIdAsync(
            string userId, int? pageIndex, int? take, string searchString = null, string sortField = null,
            string qrCodeType = null, string paymentChannel = null, string paymentScheme = null, string qrCodeStatus = null, string pairingStatus = null, bool isApproved = true,
            string direction = SortDirections.Descending);

        Task<int> GetQrCodeEntrysTotalCountByUserIdAsync(string userId, string searchString = null,
            string qrCodeType = null, string paymentChannel = null, string paymentScheme = null, string qrCodeStatus = null, string pairingStatus = null, bool isApproved = true);
        

        Task<List<QrCodeEntry>> GetQrCodeEntrysByUplineIdAsync(
            string uplineId, int? pageIndex, int? take, string searchString = null, string sortField = null,
            string qrCodeType = null, string paymentChannel = null, string paymentScheme = null, string qrCodeStatus = null, string pairingStatus = null, bool isApproved = true,
            string direction = SortDirections.Descending);

        Task<int> GetQrCodeEntrysTotalCountByUplineIdAsync(string uplineId, string searchString = null,
            string qrCodeType = null, string paymentChannel = null, string paymentScheme = null, string qrCodeStatus = null, string pairingStatus = null, bool isApproved = true);


        Task<QrCodeEntry> MapFromEntity(QrCode entity, ApplicationUser userToAddQrCode);



        Task<BarcodeInfoForManual> GetBarcodeInfoForManualAsync(int qrCodeId);
        Task<BarcodeInfoForAuto> GetBarcodeInfoForAutoAsync(int qrCodeId);
        Task<MerchantInfo> GetMerchantInfoAsync(int qrCodeId);
        Task<TransactionInfo> GetTransactionInfoAsync(int qrCodeId);
        Task<BankInfo> GetBankInfoAsync(int qrCodeId);


        Task UpdateQrCodeEntryForOrderCompleted(int qrCodeId, string pairingStatus, string pairingStatusDescription, decimal availableBalance);
        Task UpdateQrCodeEntryForOrderCompleted(int qrCodeId, string pairingStatus, string pairingStatusDescription, decimal availableBalance,
            int totalSuccess, int totalFailures, int highestConsecutiveSuccess, int highestConsecutiveFailures,
            int currentConsecutiveSuccess, int currentConsecutiveFailures, int successRateInPercent,
            decimal quotaLeftToday);

        QrCodeEntry Add(QrCodeEntry qrCodeEntry);
        void Update(QrCodeEntry qrCodeEntry);
        void Delete(QrCodeEntry qrCodeEntry);
        Task SaveChangesAsync();
        Task UpdateQrCodeEntryForOrderCreated(int qrCodeId, decimal availableBalance, string pairingStatus, string pairingStatusDescription, string dateLastTraded = null);
    }
}
