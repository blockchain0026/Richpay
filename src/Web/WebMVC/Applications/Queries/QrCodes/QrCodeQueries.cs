using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pairing.Domain.Model.QrCodes;
using Pairing.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Data;
using WebMVC.Models;
using WebMVC.Models.Roles;
using WebMVC.Extensions;
using Z.EntityFramework.Plus;
using WebMVC.Applications.CacheServices;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.QrCodes
{
    public class QrCodeQueries : IQrCodeQueries
    {
        private readonly PairingContext _pairingContext;
        private readonly ApplicationDbContext _applicationContext;
        private readonly IQrCodeCacheService _qrCodeCacheService;
        private readonly UserManager<ApplicationUser> _userManager;

        public QrCodeQueries(PairingContext pairingContext, ApplicationDbContext applicationContext, IQrCodeCacheService qrCodeCacheService, UserManager<ApplicationUser> userManager)
        {
            _pairingContext = pairingContext ?? throw new ArgumentNullException(nameof(pairingContext));
            _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
            _qrCodeCacheService = qrCodeCacheService ?? throw new ArgumentNullException(nameof(qrCodeCacheService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<QrCodeEntry> GetQrCodeEntryAsync(int qrCodeId)
        {
            return await _applicationContext.QrCodeEntrys.Where(w => w.QrCodeId == qrCodeId).FirstOrDefaultAsync();
        }


        public async Task<List<QrCodeEntry>> GetQrCodeEntrysAsync(int? pageIndex, int? take, string searchString = null, string sortField = null,
            string qrCodeType = null, string paymentChannel = null, string paymentScheme = null, string qrCodeStatus = null, string pairingStatus = null, bool isApproved = true,
            string direction = SortDirections.Descending)
        {
            var result = new List<QrCodeEntry>();

            IQueryable<QrCodeEntry> qrCodeEntrys = null;

            qrCodeEntrys = _applicationContext.QrCodeEntrys
                .AsNoTracking()
                .Include(q => q.PairingInfo)
                .Where(q => q.QrCodeType.Contains(qrCodeType ?? string.Empty)
                && q.PaymentChannel.Contains(paymentChannel ?? string.Empty)
                && q.PaymentScheme.Contains(paymentScheme ?? string.Empty)
                && q.QrCodeStatus.Contains(qrCodeStatus ?? string.Empty)
                && q.PairingStatus.Contains(pairingStatus ?? string.Empty)
                && q.IsApproved == isApproved
                );

            IQueryable<QrCodeEntry> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = qrCodeEntrys
                    .Include(w => w.QrCodeEntrySetting)
                    .Include(w => w.PairingInfo)
                    .Where(w => w.QrCodeId.ToString() == searchString
                    || w.UserId == searchString
                    //|| w.CodeStatusDiscription.Contains(searchString)
                    //|| w.PairingStatusDescription.Contains(searchString)
                    || w.FullName.Contains(searchString)
                    );
            }
            else
            {
                searchResult = qrCodeEntrys;
            }

            result = await this.GetSortedRecords(
                searchResult,
                pageIndex,
                take,
                sortField,
                direction
                );

            return result;
        }

        public async Task<int> GetQrCodeEntrysTotalCount(string searchString = null,
            string qrCodeType = null, string paymentChannel = null, string paymentScheme = null,
            string qrCodeStatus = null, string pairingStatus = null, bool isApproved = true)
        {

            IQueryable<QrCodeEntry> qrCodeEntrys = null;

            qrCodeEntrys = _applicationContext.QrCodeEntrys
                .Where(q => q.QrCodeType.Contains(qrCodeType ?? string.Empty)
                && q.PaymentChannel.Contains(paymentChannel ?? string.Empty)
                && q.PaymentScheme.Contains(paymentScheme ?? string.Empty)
                && q.QrCodeStatus.Contains(qrCodeStatus ?? string.Empty)
                && q.PairingStatus.Contains(pairingStatus ?? string.Empty)
                && q.IsApproved == isApproved
                );


            if (!string.IsNullOrEmpty(searchString))
            {
                return await qrCodeEntrys
                    .Include(w => w.QrCodeEntrySetting)
                    .Include(w => w.PairingInfo)
                    .Where(w => w.QrCodeId.ToString() == searchString
                    || w.UserId == searchString
                    || w.FullName.Contains(searchString)
                    ).CountAsync();
            }
            else
            {
                return await qrCodeEntrys.CountAsync();
            }
        }


        public async Task<List<QrCodeEntry>> GetQrCodeEntrysByUserIdAsync(string userId, int? pageIndex, int? take, string searchString = null, string sortField = null,
            string qrCodeType = null, string paymentChannel = null, string paymentScheme = null, string qrCodeStatus = null, string pairingStatus = null, bool isApproved = true,
            string direction = SortDirections.Descending)
        {
            var result = new List<QrCodeEntry>();

            IQueryable<QrCodeEntry> qrCodeEntrys = null;

            qrCodeEntrys = _applicationContext.QrCodeEntrys
                .Where(q => q.UserId == userId
                && q.QrCodeType.Contains(qrCodeType ?? string.Empty)
                && q.PaymentChannel.Contains(paymentChannel ?? string.Empty)
                && q.PaymentScheme.Contains(paymentScheme ?? string.Empty)
                && q.QrCodeStatus.Contains(qrCodeStatus ?? string.Empty)
                && q.PairingStatus.Contains(pairingStatus ?? string.Empty)
                && q.IsApproved == isApproved
                );

            IQueryable<QrCodeEntry> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = qrCodeEntrys
                    .Include(w => w.QrCodeEntrySetting)
                    .Include(w => w.PairingInfo)
                    .Where(w => w.QrCodeId.ToString() == searchString
                    || w.UserId == searchString
                    || w.FullName.Contains(searchString)
                    );
            }
            else
            {
                searchResult = qrCodeEntrys;
            }

            result = await this.GetSortedRecords(
                searchResult,
                pageIndex,
                take,
                sortField,
                direction
                );

            return result;
        }

        public async Task<int> GetQrCodeEntrysTotalCountByUserIdAsync(string userId, string searchString = null,
            string qrCodeType = null, string paymentChannel = null, string paymentScheme = null, string qrCodeStatus = null, string pairingStatus = null, bool isApproved = true)
        {
            IQueryable<QrCodeEntry> qrCodeEntrys = null;

            qrCodeEntrys = _applicationContext.QrCodeEntrys
                .Where(q => q.UserId == userId
                && q.QrCodeType.Contains(qrCodeType ?? string.Empty)
                && q.PaymentChannel.Contains(paymentChannel ?? string.Empty)
                && q.PaymentScheme.Contains(paymentScheme ?? string.Empty)
                && q.QrCodeStatus.Contains(qrCodeStatus ?? string.Empty)
                && q.PairingStatus.Contains(pairingStatus ?? string.Empty)
                && q.IsApproved == isApproved
                );


            if (!string.IsNullOrEmpty(searchString))
            {
                return await qrCodeEntrys
                    .Include(w => w.QrCodeEntrySetting)
                    .Include(w => w.PairingInfo)
                    .Where(w => w.QrCodeId.ToString() == searchString
                    || w.UserId == searchString
                    || w.FullName.Contains(searchString)
                    ).CountAsync();
            }
            else
            {
                return await qrCodeEntrys.CountAsync();
            }
        }



        public async Task<List<QrCodeEntry>> GetQrCodeEntrysByUplineIdAsync(string uplineId, int? pageIndex, int? take, string searchString = null, string sortField = null,
            string qrCodeType = null, string paymentChannel = null, string paymentScheme = null, string qrCodeStatus = null, string pairingStatus = null, bool isApproved = true,
            string direction = SortDirections.Descending)
        {
            var result = new List<QrCodeEntry>();

            IQueryable<QrCodeEntry> qrCodeEntrys = null;

            qrCodeEntrys = _applicationContext.QrCodeEntrys
                .Where(q => q.UplineUserId == uplineId
                && q.QrCodeType.Contains(qrCodeType ?? string.Empty)
                && q.PaymentChannel.Contains(paymentChannel ?? string.Empty)
                && q.PaymentScheme.Contains(paymentScheme ?? string.Empty)
                && q.QrCodeStatus.Contains(qrCodeStatus ?? string.Empty)
                && q.PairingStatus.Contains(pairingStatus ?? string.Empty)
                && q.IsApproved == isApproved
                );

            IQueryable<QrCodeEntry> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = qrCodeEntrys
                    .Include(w => w.QrCodeEntrySetting)
                    .Include(w => w.PairingInfo)
                    .Where(w => w.QrCodeId.ToString() == searchString
                    || w.UserId == searchString
                    || w.FullName.Contains(searchString)
                    );
            }
            else
            {
                searchResult = qrCodeEntrys;
            }

            result = await this.GetSortedRecords(
                searchResult,
                pageIndex,
                take,
                sortField,
                direction
                );

            return result;
        }

        public async Task<int> GetQrCodeEntrysTotalCountByUplineIdAsync(string uplineId, string searchString = null,
            string qrCodeType = null, string paymentChannel = null, string paymentScheme = null, string qrCodeStatus = null, string pairingStatus = null, bool isApproved = true)
        {
            IQueryable<QrCodeEntry> qrCodeEntrys = null;

            qrCodeEntrys = _applicationContext.QrCodeEntrys
                .Where(q => q.UplineUserId == uplineId
                && q.QrCodeType.Contains(qrCodeType ?? string.Empty)
                && q.PaymentChannel.Contains(paymentChannel ?? string.Empty)
                && q.PaymentScheme.Contains(paymentScheme ?? string.Empty)
                && q.QrCodeStatus.Contains(qrCodeStatus ?? string.Empty)
                && q.PairingStatus.Contains(pairingStatus ?? string.Empty)
                && q.IsApproved == isApproved
                );


            if (!string.IsNullOrEmpty(searchString))
            {
                return await qrCodeEntrys
                    .Include(w => w.QrCodeEntrySetting)
                    .Include(w => w.PairingInfo)
                    .Where(w => w.QrCodeId.ToString() == searchString
                    || w.UserId == searchString
                    || w.FullName.Contains(searchString)
                    ).CountAsync();
            }
            else
            {
                return await qrCodeEntrys.CountAsync();
            }
        }




        public async Task<BarcodeInfoForManual> GetBarcodeInfoForManualAsync(int qrCodeId)
        {
            var result = await _pairingContext.QrCodes
                .Where(q => q.Id == qrCodeId)
                .Select(b => new BarcodeInfoForManual
                {
                    QrCodeId = b.Id,
                    UserId = b.UserId,
                    //QrCodeUrl = b.BarcodeDataForManual != null ? b.BarcodeDataForManual.QrCodeUrl : null,
                    //Amount = b.BarcodeDataForManual != null ? b.BarcodeDataForManual.Amount : null
                    QrCodeUrl = b.BarcodeDataForManual.QrCodeUrl,
                    Amount = b.BarcodeDataForManual.Amount
                })
                .FirstOrDefaultAsync();

            return result;
        }


        public async Task<BarcodeInfoForAuto> GetBarcodeInfoForAutoAsync(int qrCodeId)
        {
            var result = await _pairingContext.QrCodes
                .Where(q => q.Id == qrCodeId)
                .Select(b => new BarcodeInfoForAuto
                {
                    QrCodeId = b.Id,
                    UserId = b.UserId,
                    CloudDeviceUsername = b.BarcodeDataForAuto.CloudDeviceUsername,
                    CloudDevicePassword = b.BarcodeDataForAuto.CloudDevicePassword,
                    CloudDeviceNumber = b.BarcodeDataForAuto.CloudDeviceNumber
                })
                .FirstOrDefaultAsync();

            return result;
        }


        public async Task<MerchantInfo> GetMerchantInfoAsync(int qrCodeId)
        {
            var result = await _pairingContext.QrCodes
                .Where(q => q.Id == qrCodeId)
                .Select(b => new MerchantInfo
                {
                    QrCodeId = b.Id,
                    UserId = b.UserId,
                    AppId = b.MerchantData.AppId,
                    AlipayPublicKey = b.MerchantData.AlipayPublicKey,
                    WechatApiCertificate = b.MerchantData.WechatApiCertificate,
                    PrivateKey = b.MerchantData.PrivateKey,
                    MerchantId = b.MerchantData.MerchantId
                })
                .FirstOrDefaultAsync();

            return result;
        }


        public async Task<TransactionInfo> GetTransactionInfoAsync(int qrCodeId)
        {
            var result = await _pairingContext.QrCodes
                .Where(q => q.Id == qrCodeId)
                .Select(b => new TransactionInfo
                {
                    QrCodeId = b.Id,
                    UserId = b.UserId,
                    AlipayUserId = b.TransactionData.UserId
                })
                .FirstOrDefaultAsync();

            return result;
        }


        public async Task<BankInfo> GetBankInfoAsync(int qrCodeId)
        {
            var result = await _pairingContext.QrCodes
                .Where(q => q.Id == qrCodeId)
                .Select(b => new BankInfo
                {
                    QrCodeId = b.Id,
                    UserId = b.UserId,
                    BankName = b.BankData.BankName,
                    BankMark = b.BankData.BankMark,
                    AccountName = b.BankData.AccountName,
                    CardNumber = b.BankData.CardNumber
                })
                .FirstOrDefaultAsync();

            return result;
        }




        private async Task<List<QrCodeEntry>> GetSortedRecords(
            IQueryable<QrCodeEntry> qrCodeEntrys,
            int? pageIndex, int? take, string sortField = null, string direction = SortDirections.Descending)
        {
            var result = new List<QrCodeEntry>();

            if (pageIndex != null && take != null)
            {
                if (!string.IsNullOrEmpty(sortField))
                {
                    if (sortField == "DateCreated")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await qrCodeEntrys
                               .OrderBy(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await qrCodeEntrys
                               .OrderByDescending(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "DateLastTraded")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await qrCodeEntrys
                               .OrderBy(f => f.DateLastTraded)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await qrCodeEntrys
                               .OrderByDescending(f => f.DateLastTraded)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "QrCodeId")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await qrCodeEntrys
                               .OrderBy(f => f.QrCodeId)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await qrCodeEntrys
                               .OrderByDescending(f => f.QrCodeId)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "Username")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await qrCodeEntrys
                               .OrderBy(f => f.Username)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await qrCodeEntrys
                               .OrderByDescending(f => f.Username)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "CloudDeviceId")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await qrCodeEntrys
                               .OrderBy(f => f.CloudDeviceId)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await qrCodeEntrys
                               .OrderByDescending(f => f.CloudDeviceId)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "QrCodeStatus")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await qrCodeEntrys
                                .OrderBy(f => f.QrCodeStatus)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await qrCodeEntrys
                               .OrderByDescending(f => f.QrCodeStatus)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "PairingStatus")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await qrCodeEntrys
                                .OrderBy(f => f.PairingStatus)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await qrCodeEntrys
                               .OrderByDescending(f => f.PairingStatus)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "IsOnline")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await qrCodeEntrys
                                .OrderBy(f => f.IsOnline)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await qrCodeEntrys
                               .OrderByDescending(f => f.IsOnline)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "SpecifiedShop")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await qrCodeEntrys
                                .OrderBy(f => f.SpecifiedShopUsername)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await qrCodeEntrys
                               .OrderByDescending(f => f.SpecifiedShopUsername)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "QuotaLeftToday")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await qrCodeEntrys
                                .OrderBy(f => f.QuotaLeftToday)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await qrCodeEntrys
                               .OrderByDescending(f => f.QuotaLeftToday)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "CurrentConsecutiveFailures")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await qrCodeEntrys
                                .OrderBy(f => f.PairingInfo.CurrentConsecutiveFailures)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await qrCodeEntrys
                               .OrderByDescending(f => f.PairingInfo.CurrentConsecutiveFailures)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "SuccessRateInPercent")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await qrCodeEntrys
                                .OrderBy(f => f.PairingInfo.SuccessRateInPercent)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await qrCodeEntrys
                               .OrderByDescending(f => f.PairingInfo.SuccessRateInPercent)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await qrCodeEntrys
                               .OrderBy(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await qrCodeEntrys
                               .OrderByDescending(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                }
                else
                {
                    result = await qrCodeEntrys
                       .OrderByDescending(f => f.DateCreated)
                       .Skip((int)take * (int)pageIndex)
                       .Take((int)take)
                       .ToListAsync();
                }
            }
            else
            {
                result = await qrCodeEntrys.ToListAsync();
            }

            return result;
        }



        public async Task<QrCodeEntry> MapFromEntity(QrCode entity, ApplicationUser userToAddQrCode)
        {
            //Check the entity and user is not null.
            if (entity is null)
            {
                throw new ArgumentNullException("The qr code entity must be provided.");
            }
            if (userToAddQrCode is null)
            {
                throw new ArgumentNullException("The application user to add qr code must be provided.");
            }

            //Check the upline user is exist.
            var uplineUser = await _userManager.Users
                .Where(u => u.Id == userToAddQrCode.UplineId)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FullName
                })
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (uplineUser == null)
            {
                throw new KeyNotFoundException("找无用户上级。");
            }

            //If the qr code has specified shop, check the shop user exist.
            string specifiedShopId = entity.SpecifiedShopId;

            var shopUser = await _userManager.Users
                .Where(u => u.Id == specifiedShopId)
                .Select(u => new
                {
                    u.BaseRoleType,
                    u.Id,
                    u.UserName,
                    u.FullName
                })
                .FirstOrDefaultAsync()
                .ConfigureAwait(false); ;

            if (!string.IsNullOrEmpty(specifiedShopId))
            {
                if (shopUser is null || shopUser.BaseRoleType != BaseRoleType.Shop)
                {
                    throw new KeyNotFoundException("找无指定商户。");
                }
            }

            //Build qr code view model.
            var qrCodeVM = new QrCodeEntry
            {
                QrCodeId = entity.Id,
                UserId = userToAddQrCode.Id,
                Username = userToAddQrCode.UserName,
                UserFullName = userToAddQrCode.FullName,
                UplineUserId = uplineUser.Id,
                UplineUserName = uplineUser.UserName,
                UplineFullName = uplineUser.FullName,
                CloudDeviceId = entity.CloudDeviceId,
                QrCodeType = entity.GetQrCodeType.Name,
                PaymentChannel = entity.GetPaymentChannel.Name,
                PaymentScheme = entity.GetPaymentScheme.Name,
                QrCodeStatus = entity.GetQrCodeStatus.Name,
                PairingStatus = entity.GetPairingStatus.Name,
                CodeStatusDescription = entity.CodeStatusDiscription,
                PairingStatusDescription = entity.PairingStatusDescription,
                QrCodeEntrySetting = new QrCodeEntrySetting
                {
                    AutoPairingBySuccessRate = entity.QrCodeSettings.AutoPairingBySuccessRate,
                    AutoPairingByQuotaLeft = entity.QrCodeSettings.AutoPairingByQuotaLeft,
                    AutoPairingByBusinessHours = entity.QrCodeSettings.AutoPairingByBusinessHours,
                    AutoPairingByCurrentConsecutiveFailures = entity.QrCodeSettings.AutoPairingByCurrentConsecutiveFailures,
                    AutoPairngByAvailableBalance = entity.QrCodeSettings.AutoPairngByAvailableBalance,
                    SuccessRateThresholdInHundredth = (int)(entity.QrCodeSettings.SuccessRateThreshold * 100),
                    SuccessRateMinOrders = entity.QrCodeSettings.SuccessRateMinOrders,
                    QuotaLeftThreshold = entity.QrCodeSettings.QuotaLeftThreshold,
                    CurrentConsecutiveFailuresThreshold = entity.QrCodeSettings.CurrentConsecutiveFailuresThreshold,
                    AvailableBalanceThreshold = entity.QrCodeSettings.AvailableBalanceThreshold,
                },
                DailyAmountLimit = (int)entity.DailyAmountLimit,
                OrderAmountUpperLimit = (int)entity.OrderAmountUpperLimit,
                OrderAmountLowerLimit = (int)entity.OrderAmountLowerLimit,
                FullName = entity.FullName,
                IsApproved = entity.IsApproved,
                ApprovedByAdminId = entity.ApprovedBy?.AdminId,
                ApprovedByAdminName = entity.ApprovedBy?.Name,

                IsOnline = entity.IsOnline,
                MinCommissionRateInThousandth = (int)(entity.MinCommissionRate * 1000),
                AvailableBalance = entity.AvailableBalance,
                SpecifiedShopId = shopUser?.Id,
                SpecifiedShopUsername = shopUser?.UserName,
                SpecifiedShopFullName = shopUser?.FullName,
                QuotaLeftToday = entity.QuotaLeftToday,
                DateLastTraded = entity.DateLastTraded?.ToFullString(),

                PairingInfo = new PairingInfo
                {
                    TotalSuccess = entity.PairingData.TotalSuccess,
                    TotalFailures = entity.PairingData.TotalFailures,
                    HighestConsecutiveSuccess = entity.PairingData.HighestConsecutiveSuccess,
                    HighestConsecutiveFailures = entity.PairingData.HighestConsecutiveFailures,
                    CurrentConsecutiveSuccess = entity.PairingData.CurrentConsecutiveSuccess,
                    CurrentConsecutiveFailures = entity.PairingData.CurrentConsecutiveFailures,
                    SuccessRateInPercent = (int)(entity.PairingData.SuccessRate * 100)
                },
                DateCreated = entity.DateCreated.ToFullString()
            };

            return qrCodeVM;
        }


        public async Task UpdateQrCodeEntryForOrderCompleted(int qrCodeId, string pairingStatus, string pairingStatusDescription, decimal availableBalance)
        {
            /*var user = await _applicationContext.OrderEntrys
                .Include(t => t.Balance)
                .Where(u => u.TraderId == userId)
                .Select(u => new
                {
                    u.TraderId,
                    u.Balance
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();*/
            var originalQrCode = await _applicationContext.QrCodeEntrys
                .Include(t => t.PairingInfo)
                .Where(u => u.QrCodeId == qrCodeId)
                .Select(u => new
                {
                    u.QrCodeId,
                    u.PairingInfo
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var toUpdate = new QrCodeEntry
            {
                QrCodeId = qrCodeId,
                PairingInfo = originalQrCode.PairingInfo
            };

            _applicationContext.QrCodeEntrys.Attach(toUpdate);
            _applicationContext.Entry(toUpdate).Reference(b => b.PairingInfo).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.PairingStatus).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.PairingStatusDescription).IsModified = true;

            //Update available balance.
            toUpdate.AvailableBalance = availableBalance;

            toUpdate.PairingStatus = pairingStatus;
            toUpdate.PairingStatusDescription = pairingStatusDescription;
        }

        public async Task UpdateQrCodeEntryForOrderCompleted(int qrCodeId, string pairingStatus, string pairingStatusDescription, decimal availableBalance,
            int totalSuccess, int totalFailures, int highestConsecutiveSuccess, int highestConsecutiveFailures,
            int currentConsecutiveSuccess, int currentConsecutiveFailures, int successRateInPercent,
            decimal quotaLeftToday)
        {
            /*var originalQrCode = await _applicationContext.QrCodeEntrys
                .Include(t => t.PairingInfo)
                .Where(u => u.QrCodeId == qrCodeId)
                .Select(u => new
                {
                    u.QrCodeId,
                    u.PairingInfo
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();*/

            var toUpdate = new QrCodeEntry
            {
                QrCodeId = qrCodeId,
                PairingInfo = new PairingInfo()
            };

            _applicationContext.QrCodeEntrys.Attach(toUpdate);
            _applicationContext.Entry(toUpdate).Reference(b => b.PairingInfo).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.PairingStatus).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.PairingStatusDescription).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.AvailableBalance).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.QuotaLeftToday).IsModified = true;

            //Update available balance.
            toUpdate.AvailableBalance = availableBalance;

            toUpdate.PairingStatus = pairingStatus;
            toUpdate.PairingStatusDescription = pairingStatusDescription;

            //Update success data and quota left data.
            toUpdate.PairingInfo.TotalSuccess = totalSuccess;
            toUpdate.PairingInfo.TotalFailures = totalFailures;
            toUpdate.PairingInfo.HighestConsecutiveSuccess = highestConsecutiveSuccess;
            toUpdate.PairingInfo.HighestConsecutiveFailures = highestConsecutiveFailures;
            toUpdate.PairingInfo.CurrentConsecutiveSuccess = currentConsecutiveSuccess;
            toUpdate.PairingInfo.CurrentConsecutiveFailures = currentConsecutiveFailures;
            toUpdate.PairingInfo.SuccessRateInPercent = successRateInPercent;

            toUpdate.QuotaLeftToday = quotaLeftToday;
        }

        public async Task UpdateQrCodeEntryForOrderCreated(int qrCodeId, decimal availableBalance,
            string pairingStatus, string pairingStatusDescription, string dateLastTraded = null)
        {
            /*var originalQrCode = await _applicationContext.QrCodeEntrys
                .Include(t => t.PairingInfo)
                .Where(u => u.QrCodeId == qrCodeId)
                .Select(u => new
                {
                    u.QrCodeId,
                    u.PairingInfo
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();*/

            var toUpdate = new QrCodeEntry
            {
                QrCodeId = qrCodeId,
                //PairingInfo = originalQrCode.PairingInfo
            };

            _applicationContext.QrCodeEntrys.Attach(toUpdate);
            //_applicationContext.Entry(toUpdate).Reference(b => b.PairingInfo).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.PairingStatus).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.PairingStatusDescription).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.AvailableBalance).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.DateLastTraded).IsModified = true;

            //Update available balance.
            toUpdate.AvailableBalance = availableBalance;

            //Update pairing status.
            toUpdate.PairingStatus = pairingStatus;
            toUpdate.PairingStatusDescription = pairingStatusDescription;

            //Update last traded date.
            if (!string.IsNullOrEmpty(dateLastTraded))
            {
                toUpdate.DateLastTraded = dateLastTraded;
            }


            /*_applicationContext.QrCodeEntrys.Where(q => q.QrCodeId == qrCodeId)
                     .Update(x => new QrCodeEntry()
                     {
                         AvailableBalance = availableBalance,
                         PairingStatus = pairingStatus,
                         PairingStatusDescription = pairingStatusDescription,
                         DateLastTraded = dateLastTraded
                     });*/
        }



        public QrCodeEntry Add(QrCodeEntry qrCodeEntry)
        {
            return _applicationContext.QrCodeEntrys.Add(qrCodeEntry).Entity;
        }

        public void Update(QrCodeEntry qrCodeEntry)
        {
            _applicationContext.Entry(qrCodeEntry).State = EntityState.Modified;
        }

        public void Delete(QrCodeEntry qrCodeEntry)
        {
            if (qrCodeEntry != null)
            {
                _applicationContext.QrCodeEntrys.Remove(qrCodeEntry);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _applicationContext.SaveChangesAsync();
        }
    }
}
