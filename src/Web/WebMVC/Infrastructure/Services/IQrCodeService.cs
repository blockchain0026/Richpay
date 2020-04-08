using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Infrastructure.Services
{
    public interface IQrCodeService
    {
        Task<QrCodeEntry> GetQrCode(string searchByUserId, int qrCodeId);


        Task<List<QrCodeEntry>> GetQrCodeEntrys(int pageIndex, int take, string searchByUserId, string searchString = "", string sortField = "",
                        string qrCodeType = null, string paymentChannel = null, string paymentScheme = null, string qrCodeStatus = null, string pairingStatus = null,
                        string direction = SortDirections.Descending);
        Task<int> GetQrCodeEntrysTotalCount(string searchByUserId, string searchString = null,
            string qrCodeType = null, string paymentChannel = null, string paymentScheme = null, string qrCodeStatus = null, string pairingStatus = null);


        Task<List<QrCodeEntry>> GetPendingQrCodeEntrys(int pageIndex, int take, string searchByUserId, string searchString = "", string sortField = "",
            string qrCodeType = null, string paymentChannel = null, string paymentScheme = null,
            string direction = SortDirections.Descending);
        Task<int> GetPendingQrCodeEntrysTotalCount(string searchByUserId, string searchString = null,
            string qrCodeType = null, string paymentChannel = null, string paymentScheme = null);


        Task<BarcodeInfoForManual> GetBarcodeInfoForManual(string searchByUserId, int qrCodeId);

        Task<BarcodeInfoForAuto> GetBarcodeInfoForAutoAsync(string searchByUserId, int qrCodeId);

        Task<MerchantInfo> GetMerchantInfoAsync(string searchByUserId, int qrCodeId);

        Task<TransactionInfo> GetTransactionInfoAsync(string searchByUserId, int qrCodeId);

        Task<BankInfo> GetBankInfoAsync(string searchByUserId, int qrCodeId);

        Task<byte[]> GenerateQrCode(string searchByUserId, int qrCodeId, decimal? amount);

        //Task<IEnumerable<SelectListItem>> GetBankOptionSelectList();



        Task CreateBarcode(string createByUserId, string userId, int? cloudDeviceId, string qrCodeType, string paymentChannel,
            QrCodeEntrySetting qrCodeEntrySetting, int dailyAmountLimit, int orderAmountUpperLimit, int orderAmountLowerLimit,
            string fullName, string specifiedShopId,
            string qrCodeUrl = null, decimal? amount = null);

        Task CreateMerchantCode(string createByUserId, string userId, int? cloudDeviceId, string qrCodeType, string paymentChannel,
            QrCodeEntrySetting qrCodeEntrySetting, int dailyAmountLimit, int orderAmountUpperLimit, int orderAmountLowerLimit,
            string fullName, string specifiedShopId,
            string appId, string privateKey, string merchantId, string alipayPublicKey, string wechatApiCertificate);

        Task CreateTransactionCode(string createByUserId, string userId, int? cloudDeviceId, string qrCodeType, string paymentScheme,
            QrCodeEntrySetting qrCodeEntrySetting, int dailyAmountLimit, int orderAmountUpperLimit, int orderAmountLowerLimit,
            string fullName, string specifiedShopId,
            string alipayUserId);

        Task CreateBankCode(string createByUserId, string userId, int? cloudDeviceId, string qrCodeType,
            QrCodeEntrySetting qrCodeEntrySetting, int dailyAmountLimit, int orderAmountUpperLimit, int orderAmountLowerLimit,
            string fullName, string specifiedShopId,
            string bankName, string bankMark, string accountName, string cardNumber);

        Task UpdateBarcodeDataForManual(string updateByUserId, int qrCodeId, string qrCodeUrl, decimal? amount);
        Task UpdateBarcodeDataForAuto(string updateByUserId, int qrCodeId, int cloudDeviceId);
        Task UpdateMerchantData(string updateByUserId, int qrCodeId, string appId, string privateKey, string merchantId, string alipayPublicKey, string wechatApiCertificate);
        Task UpdateTransactionData(string updateByUserId, int qrCodeId, string alipayUserId);
        Task UpdateBankData(string updateByUserId, int qrCodeId, string bankName, string bankMark, string accountName, string cardNumber);
        Task UpdateQrCodeSettings(string updateByUserId, int qrCodeId, QrCodeEntrySetting qrCodeEntrySetting);
        Task UpdateQuota(string updateByUserId, int qrCodeId, int dayAmountLimit, int orderAmountUpperLimit, int orderAmountLowerLimit);

        Task UpdateBaseInfo(string updateByUserId, int qrCodeId, string fullName, string shopId);



        Task ApproveQrCode(int qrCodeId, string aprrovedByAdminId);
        Task ApproveQrCodes(List<int> qrCodeIds, string aprrovedByAdminId);

        Task RejectQrCode(int qrCodeId, string rejectByAdminId);
        Task RejectQrCodes(List<int> qrCodeIds, string rejectByAdminId);

        Task EnableQrCode(int qrCodeId, string enabledByAdminId);
        Task EnableQrCodes(List<int> qrCodeIds, string enabledByAdminId);

        Task DisableQrCode(int qrCodeId, string disabledByAdminId, string description);
        Task DisableQrCodes(List<int> qrCodeIds, string disabledByAdminId, string description);

        Task StartPairingQrCode(int qrCodeId, string startedByUserId);
        Task StartPairingQrCodes(List<int> qrCodeIds, string startedByUserId);

        Task StopPairingQrCode(int qrCodeId, string stopedByUserId);
        Task StopPairingQrCodes(List<int> qrCodeIds, string stopedByUserId);

        Task ResetRiskControlData(string resetByAdminId, int qrCodeId, bool resetQuotaLeftToday, bool resetCurrentConsecutiveFailures, bool resetSuccessRateAndRelatedData);


        string DecodeQrCodeData(IFormFile qrCodeFile);
    }
}
