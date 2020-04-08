using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Pairing.Domain.Model.CloudDevices;
using Pairing.Domain.Model.QrCodes;
using Pairing.Domain.Model.Shared;
using Pairing.Domain.Model.ShopSettingsDomainModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Util.Tools.QrCode.QrCoder;
using WebMVC.Applications.DomainServices.PairingDomain;
using WebMVC.Applications.Queries.Balances;
using WebMVC.Applications.Queries.Commission;
using WebMVC.Applications.Queries.QrCodes;
using WebMVC.Models;
using WebMVC.Models.Queries;
using WebMVC.Models.Roles;

namespace WebMVC.Infrastructure.Services.Implementations
{
    public class QrCodeService : IQrCodeService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IQrCodeQueries _qrCodeQueries;
        private readonly ICommissionQueries _commissionQueries;
        private readonly IBalanceQueries _balanceQueries;

        private readonly ICloudDeviceRepository _cloudDeviceRepository;
        private readonly IQrCodeRepository _qrCodeRepository;
        private readonly IShopSettingsRepository _shopSettingsRepository;


        private readonly IDateTimeService _dateTimeService;
        private readonly ISystemConfigurationService _systemConfigurationService;

        public QrCodeService(UserManager<ApplicationUser> userManager, IQrCodeQueries qrCodeQueries, ICommissionQueries commissionQueries, IBalanceQueries balanceQueries, ICloudDeviceRepository cloudDeviceRepository, IQrCodeRepository qrCodeRepository, IShopSettingsRepository shopSettingsRepository, IDateTimeService dateTimeService, ISystemConfigurationService systemConfigurationService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _qrCodeQueries = qrCodeQueries ?? throw new ArgumentNullException(nameof(qrCodeQueries));
            _commissionQueries = commissionQueries ?? throw new ArgumentNullException(nameof(commissionQueries));
            _balanceQueries = balanceQueries ?? throw new ArgumentNullException(nameof(balanceQueries));
            _cloudDeviceRepository = cloudDeviceRepository ?? throw new ArgumentNullException(nameof(cloudDeviceRepository));
            _qrCodeRepository = qrCodeRepository ?? throw new ArgumentNullException(nameof(qrCodeRepository));
            _shopSettingsRepository = shopSettingsRepository ?? throw new ArgumentNullException(nameof(shopSettingsRepository));
            _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
            _systemConfigurationService = systemConfigurationService ?? throw new ArgumentNullException(nameof(systemConfigurationService));
        }


        #region Query

        public async Task<QrCodeEntry> GetQrCode(string searchByUserId, int qrCodeId)
        {
            //Validate user is exist.
            var user = await ValidateUserExist(searchByUserId).ConfigureAwait(false);

            await ValidateUserPermissionOnQrCodeAsync(user, qrCodeId).ConfigureAwait(false);


            //Get QR code data.
            var result = await _qrCodeQueries.GetQrCodeEntryAsync(
                qrCodeId
                ).ConfigureAwait(false);


            if (result is null)
            {
                throw new InvalidOperationException("查无二维码数据。");
            }

            return result;
        }



        public async Task<List<QrCodeEntry>> GetQrCodeEntrys(int pageIndex, int take, string searchByUserId, string searchString = "", string sortField = "",
                        string qrCodeType = null, string paymentChannel = null, string paymentScheme = null, string qrCodeStatus = null, string pairingStatus = null,
                        string direction = SortDirections.Descending)
        {
            var user = await ValidateUserExist(searchByUserId).ConfigureAwait(false);

            if (user.BaseRoleType == BaseRoleType.Manager)
            {
                return await _qrCodeQueries.GetQrCodeEntrysAsync(
                    pageIndex,
                    take,
                    searchString,
                    sortField,
                    qrCodeType,
                    paymentChannel,
                    paymentScheme,
                    qrCodeStatus,
                    pairingStatus,
                    true,
                    direction
                    ).ConfigureAwait(false);
            }
            if (user.BaseRoleType == BaseRoleType.TraderAgent)
            {
                return await _qrCodeQueries.GetQrCodeEntrysByUplineIdAsync(
                    searchByUserId,
                    pageIndex,
                    take,
                    searchString,
                    sortField,
                    qrCodeType,
                    paymentChannel,
                    paymentScheme,
                    qrCodeStatus,
                    pairingStatus,
                    true,
                    direction
                    ).ConfigureAwait(false);
            }
            if (user.BaseRoleType == BaseRoleType.Trader)
            {
                return await _qrCodeQueries.GetQrCodeEntrysByUserIdAsync(
                    searchByUserId,
                    pageIndex,
                    take,
                    searchString,
                    sortField,
                    qrCodeType,
                    paymentChannel,
                    paymentScheme,
                    qrCodeStatus,
                    pairingStatus,
                    true,
                    direction
                    ).ConfigureAwait(false);
            }

            throw new InvalidOperationException("用户没有二维码权限");
        }

        public async Task<int> GetQrCodeEntrysTotalCount(string searchByUserId, string searchString = null,
            string qrCodeType = null, string paymentChannel = null, string paymentScheme = null, string qrCodeStatus = null, string pairingStatus = null)
        {
            var user = await ValidateUserExist(searchByUserId).ConfigureAwait(false);

            if (user.BaseRoleType == BaseRoleType.Manager)
            {

                return await this._qrCodeQueries.GetQrCodeEntrysTotalCount(
                    searchString,
                    qrCodeType,
                    paymentChannel,
                    paymentScheme,
                    qrCodeStatus,
                    pairingStatus,
                    true
                    ).ConfigureAwait(false);

            }
            if (user.BaseRoleType == BaseRoleType.TraderAgent)
            {
                return await this._qrCodeQueries.GetQrCodeEntrysTotalCountByUplineIdAsync(
                    searchByUserId,
                    searchString,
                    qrCodeType,
                    paymentChannel,
                    paymentScheme,
                    qrCodeStatus,
                    pairingStatus,
                    true
                    ).ConfigureAwait(false);
            }
            if (user.BaseRoleType == BaseRoleType.Trader)
            {
                return await this._qrCodeQueries.GetQrCodeEntrysTotalCountByUserIdAsync(
                    searchByUserId,
                    searchString,
                    qrCodeType,
                    paymentChannel,
                    paymentScheme,
                    qrCodeStatus,
                    pairingStatus,
                    true
                    ).ConfigureAwait(false);
            }

            throw new InvalidOperationException("用户没有二维码权限");
        }

        public async Task<List<QrCodeEntry>> GetPendingQrCodeEntrys(int pageIndex, int take, string searchByUserId, string searchString = "", string sortField = "",
            string qrCodeType = null, string paymentChannel = null, string paymentScheme = null,
            string direction = SortDirections.Descending)
        {
            var user = await ValidateUserExist(searchByUserId).ConfigureAwait(false);

            if (user.BaseRoleType == BaseRoleType.Manager)
            {
                return await _qrCodeQueries.GetQrCodeEntrysAsync(
                    pageIndex,
                    take,
                    searchString,
                    sortField,
                    qrCodeType,
                    paymentChannel,
                    paymentScheme,
                    null,
                    null,
                    false,
                    direction
                    ).ConfigureAwait(false);
            }
            if (user.BaseRoleType == BaseRoleType.TraderAgent)
            {
                return await _qrCodeQueries.GetQrCodeEntrysByUplineIdAsync(
                    searchByUserId,
                    pageIndex,
                    take,
                    searchString,
                    sortField,
                    qrCodeType,
                    paymentChannel,
                    paymentScheme,
                    null,
                    null,
                    false,
                    direction
                    ).ConfigureAwait(false);
            }
            if (user.BaseRoleType == BaseRoleType.Trader)
            {
                return await _qrCodeQueries.GetQrCodeEntrysByUserIdAsync(
                    searchByUserId,
                    pageIndex,
                    take,
                    searchString,
                    sortField,
                    qrCodeType,
                    paymentChannel,
                    paymentScheme,
                    null,
                    null,
                    false,
                    direction
                    ).ConfigureAwait(false);
            }

            throw new InvalidOperationException("用户没有二维码权限");
        }

        public async Task<int> GetPendingQrCodeEntrysTotalCount(string searchByUserId, string searchString = null,
            string qrCodeType = null, string paymentChannel = null, string paymentScheme = null)
        {
            var user = await ValidateUserExist(searchByUserId).ConfigureAwait(false);

            if (user.BaseRoleType == BaseRoleType.Manager)
            {

                return await this._qrCodeQueries.GetQrCodeEntrysTotalCount(
                    searchString,
                    qrCodeType,
                    paymentChannel,
                    paymentScheme,
                    null,
                    null,
                    false
                    ).ConfigureAwait(false);

            }
            if (user.BaseRoleType == BaseRoleType.TraderAgent)
            {
                return await this._qrCodeQueries.GetQrCodeEntrysTotalCountByUplineIdAsync(
                    searchByUserId,
                    searchString,
                    qrCodeType,
                    paymentChannel,
                    paymentScheme,
                    null,
                    null,
                    false
                    ).ConfigureAwait(false);
            }
            if (user.BaseRoleType == BaseRoleType.Trader)
            {
                return await this._qrCodeQueries.GetQrCodeEntrysTotalCountByUserIdAsync(
                    searchByUserId,
                    searchString,
                    qrCodeType,
                    paymentChannel,
                    paymentScheme,
                    null,
                    null,
                    false
                    ).ConfigureAwait(false);
            }

            throw new InvalidOperationException("用户没有二维码权限");
        }



        public async Task<BarcodeInfoForManual> GetBarcodeInfoForManual(string searchByUserId, int qrCodeId)
        {
            //Validate user is exist.
            var user = await ValidateUserExist(searchByUserId).ConfigureAwait(false);

            await ValidateUserPermissionOnQrCodeAsync(user, qrCodeId).ConfigureAwait(false);


            //Get QR code data.
            var result = await _qrCodeQueries.GetBarcodeInfoForManualAsync(
                qrCodeId
                ).ConfigureAwait(false);


            if (result is null)
            {
                throw new InvalidOperationException("查无二维码数据。");
            }

            return result;
        }

        public async Task<BarcodeInfoForAuto> GetBarcodeInfoForAutoAsync(string searchByUserId, int qrCodeId)
        {
            var user = await ValidateUserExist(searchByUserId).ConfigureAwait(false);

            await ValidateUserPermissionOnQrCodeAsync(user, qrCodeId).ConfigureAwait(false);


            //Get QR code data.
            var result = await _qrCodeQueries.GetBarcodeInfoForAutoAsync(
                qrCodeId
                ).ConfigureAwait(false);


            if (result is null)
            {
                throw new InvalidOperationException("查无二维码数据。");
            }

            return result;
        }

        public async Task<MerchantInfo> GetMerchantInfoAsync(string searchByUserId, int qrCodeId)
        {
            var user = await ValidateUserExist(searchByUserId).ConfigureAwait(false);

            await ValidateUserPermissionOnQrCodeAsync(user, qrCodeId).ConfigureAwait(false);


            //Get QR code data.
            var result = await _qrCodeQueries.GetMerchantInfoAsync(
                qrCodeId
                ).ConfigureAwait(false);


            if (result is null)
            {
                throw new InvalidOperationException("查无二维码数据。");
            }

            return result;
        }

        public async Task<TransactionInfo> GetTransactionInfoAsync(string searchByUserId, int qrCodeId)
        {
            var user = await ValidateUserExist(searchByUserId).ConfigureAwait(false);

            await ValidateUserPermissionOnQrCodeAsync(user, qrCodeId).ConfigureAwait(false);


            //Get QR code data.
            var result = await _qrCodeQueries.GetTransactionInfoAsync(
                qrCodeId
                ).ConfigureAwait(false);


            if (result is null)
            {
                throw new InvalidOperationException("查无二维码数据。");
            }

            return result;
        }

        public async Task<BankInfo> GetBankInfoAsync(string searchByUserId, int qrCodeId)
        {
            var user = await ValidateUserExist(searchByUserId).ConfigureAwait(false);

            await ValidateUserPermissionOnQrCodeAsync(user, qrCodeId).ConfigureAwait(false);


            //Get QR code data.
            var result = await _qrCodeQueries.GetBankInfoAsync(
                qrCodeId
                ).ConfigureAwait(false);


            if (result is null)
            {
                throw new InvalidOperationException("查无二维码数据。");
            }

            return result;
        }

        public async Task<byte[]> GenerateQrCode(string searchByUserId, int qrCodeId, decimal? amount)
        {
            //Validate user is exist.
            var user = await ValidateUserExist(searchByUserId).ConfigureAwait(false);

            await ValidateUserPermissionOnQrCodeAsync(user, qrCodeId).ConfigureAwait(false);


            //Get QR code payment command.
            var qrCode = await this._qrCodeRepository.GetByQrCodeIdAsync(qrCodeId);
            var paymentCommand = qrCode.GeneratePaymentCommand(amount ?? 1);


            if (string.IsNullOrEmpty(paymentCommand))
            {
                throw new InvalidOperationException("生成失败");
            }

            //Encode.
            byte[] result = new QrCoderService().CreateQrCode(paymentCommand);


            return result;
        }

        #endregion


        #region Command

        public async Task CreateBarcode(string createByUserId, string userId, int? cloudDeviceId, string qrCodeType, string paymentChannel,
            QrCodeEntrySetting qrCodeEntrySetting, int dailyAmountLimit, int orderAmountUpperLimit, int orderAmountLowerLimit,
            string fullName, string specifiedShopId,
            string qrCodeUrl = null, decimal? amount = null)
        {
            //Validate user permission.
            var createByUser = await ValidateUserExist(createByUserId).ConfigureAwait(false);
            var userToAddQrCode = await ValidateUserExist(userId).ConfigureAwait(false);

            ValidateUserPermissionOnCreateQrCode(createByUser, userToAddQrCode);


            //Build default services.
            var qrCodeSettingService = BuildQrCodeSettingServiceFrom(qrCodeEntrySetting);

            var quotaService = new FakeQuotaService(
                dailyAmountLimit,
                orderAmountUpperLimit,
                orderAmountLowerLimit
                );

            var pairingDataService = await this.BuildPairingDataServiceFromAsync(
                userToAddQrCode,
                paymentChannel,
                specifiedShopId)
                .ConfigureAwait(false);


            //Get cloud device
            CloudDevice cloudDevice = null;
            if (qrCodeType == QrCodeTypeOption.Auto.Value)
            {
                cloudDevice = await this._cloudDeviceRepository
                    .GetByCloudDeviceIdAsync((int)cloudDeviceId)
                    .ConfigureAwait(false);
            }

            //Get shop settings
            var shopSettings = await _shopSettingsRepository
                .GetByShopIdAsync(specifiedShopId)
                .ConfigureAwait(false);


            //Create Qr code.
            var qrCode = QrCode.FromBarcode(
                userId,
                QrCodeType.FromName(qrCodeType),
                Pairing.Domain.Model.QrCodes.PaymentChannel.FromName(paymentChannel),
                fullName,
                qrCodeSettingService,
                quotaService,
                pairingDataService,
                this._dateTimeService,
                qrCodeUrl,
                amount,
                shopSettings,
                cloudDevice
                );

            var qrCodeCreated = _qrCodeRepository.Add(qrCode);

            //Save changes and execute related domain events.
            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();


            //Create view data here because it may slow down the performence if create in domain handler.
            var qrCodeVM = await _qrCodeQueries
                .MapFromEntity(qrCodeCreated, userToAddQrCode).ConfigureAwait(false);

            _qrCodeQueries.Add(qrCodeVM);

            await _qrCodeQueries
                .SaveChangesAsync()
                .ConfigureAwait(false);
        }


        public async Task CreateMerchantCode(string createByUserId, string userId, int? cloudDeviceId, string qrCodeType, string paymentChannel,
            QrCodeEntrySetting qrCodeEntrySetting, int dailyAmountLimit, int orderAmountUpperLimit, int orderAmountLowerLimit,
            string fullName, string specifiedShopId,
            string appId, string privateKey, string merchantId, string alipayPublicKey, string wechatApiCertificate)
        {
            //Validate user permission.
            var createByUser = await ValidateUserExist(createByUserId).ConfigureAwait(false);
            var userToAddQrCode = await ValidateUserExist(userId).ConfigureAwait(false);

            ValidateUserPermissionOnCreateQrCode(createByUser, userToAddQrCode);


            //Build default services.
            var qrCodeSettingService = BuildQrCodeSettingServiceFrom(qrCodeEntrySetting);

            var quotaService = new FakeQuotaService(
                dailyAmountLimit,
                orderAmountUpperLimit,
                orderAmountLowerLimit
                );

            var pairingDataService = await this.BuildPairingDataServiceFromAsync(
                userToAddQrCode,
                paymentChannel,
                specifiedShopId)
                .ConfigureAwait(false);


            //Get cloud device
            CloudDevice cloudDevice = null;
            if (qrCodeType == QrCodeTypeOption.Auto.Value)
            {
                cloudDevice = await this._cloudDeviceRepository
                    .GetByCloudDeviceIdAsync((int)cloudDeviceId)
                    .ConfigureAwait(false);
            }

            //Get shop settings
            var shopSettings = await _shopSettingsRepository
                .GetByShopIdAsync(specifiedShopId)
                .ConfigureAwait(false);


            //Create Qr code.
            var qrCode = QrCode.FromMerchant(
                userId,
                QrCodeType.FromName(qrCodeType),
                Pairing.Domain.Model.QrCodes.PaymentChannel.FromName(paymentChannel),
                fullName,
                qrCodeSettingService,
                quotaService,
                pairingDataService,
                this._dateTimeService,
                appId,
                privateKey,
                merchantId,
                shopSettings,
                cloudDevice,
                alipayPublicKey,
                wechatApiCertificate
                );

            var qrCodeCreated = _qrCodeRepository.Add(qrCode);

            //Save changes and execute related domain events.
            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();



            //Create view data here because it may slow down the performence if create in domain handler.
            var qrCodeVM = await _qrCodeQueries
                .MapFromEntity(qrCodeCreated, userToAddQrCode).ConfigureAwait(false);

            _qrCodeQueries.Add(qrCodeVM);

            await _qrCodeQueries.SaveChangesAsync();
        }


        public async Task CreateTransactionCode(string createByUserId, string userId, int? cloudDeviceId, string qrCodeType, string paymentScheme,
            QrCodeEntrySetting qrCodeEntrySetting, int dailyAmountLimit, int orderAmountUpperLimit, int orderAmountLowerLimit,
            string fullName, string specifiedShopId,
            string alipayUserId)
        {
            //Validate user permission.
            var createByUser = await ValidateUserExist(createByUserId).ConfigureAwait(false);
            var userToAddQrCode = await ValidateUserExist(userId).ConfigureAwait(false);

            ValidateUserPermissionOnCreateQrCode(createByUser, userToAddQrCode);


            //Build default services.
            var qrCodeSettingService = BuildQrCodeSettingServiceFrom(qrCodeEntrySetting);

            var quotaService = new FakeQuotaService(
                dailyAmountLimit,
                orderAmountUpperLimit,
                orderAmountLowerLimit
                );

            var pairingDataService = await this.BuildPairingDataServiceFromAsync(
                userToAddQrCode,
                PaymentChannelOption.Alipay.Value, //Online alipay has transaction code option.
                specifiedShopId)
                .ConfigureAwait(false);


            //Get cloud device
            CloudDevice cloudDevice = null;
            if (qrCodeType == QrCodeTypeOption.Auto.Value)
            {
                cloudDevice = await this._cloudDeviceRepository
                    .GetByCloudDeviceIdAsync((int)cloudDeviceId)
                    .ConfigureAwait(false);
            }

            //Get shop settings
            var shopSettings = await _shopSettingsRepository
                .GetByShopIdAsync(specifiedShopId)
                .ConfigureAwait(false);


            //Create Qr code.
            var qrCode = QrCode.FromTransaction(
                userId,
                QrCodeType.FromName(qrCodeType),
                PaymentScheme.FromName(paymentScheme),
                fullName,
                qrCodeSettingService,
                quotaService,
                pairingDataService,
                this._dateTimeService,
                alipayUserId,
                shopSettings,
                cloudDevice
                );

            var qrCodeCreated = _qrCodeRepository.Add(qrCode);

            //Save changes and execute related domain events.
            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();



            //Create view data here because it may slow down the performence if create in domain handler.
            var qrCodeVM = await _qrCodeQueries
                .MapFromEntity(qrCodeCreated, userToAddQrCode).ConfigureAwait(false);

            _qrCodeQueries.Add(qrCodeVM);

            await _qrCodeQueries.SaveChangesAsync();
        }


        public async Task CreateBankCode(string createByUserId, string userId, int? cloudDeviceId, string qrCodeType,
            QrCodeEntrySetting qrCodeEntrySetting, int dailyAmountLimit, int orderAmountUpperLimit, int orderAmountLowerLimit,
            string fullName, string specifiedShopId,
            string bankName, string bankMark, string accountName, string cardNumber)
        {
            //Validate user permission.
            var createByUser = await ValidateUserExist(createByUserId).ConfigureAwait(false);
            var userToAddQrCode = await ValidateUserExist(userId).ConfigureAwait(false);

            ValidateUserPermissionOnCreateQrCode(createByUser, userToAddQrCode);


            //Build default services.
            var qrCodeSettingService = BuildQrCodeSettingServiceFrom(qrCodeEntrySetting);

            var quotaService = new FakeQuotaService(
                dailyAmountLimit,
                orderAmountUpperLimit,
                orderAmountLowerLimit
                );

            var pairingDataService = await this.BuildPairingDataServiceFromAsync(
                userToAddQrCode,
                PaymentChannelOption.Alipay.Value, //Online alipay has transaction code option.
                specifiedShopId)
                .ConfigureAwait(false);


            //Get cloud device
            CloudDevice cloudDevice = null;
            if (qrCodeType == QrCodeTypeOption.Auto.Value)
            {
                cloudDevice = await this._cloudDeviceRepository
                    .GetByCloudDeviceIdAsync((int)cloudDeviceId)
                    .ConfigureAwait(false);
            }

            //Get shop settings
            var shopSettings = await _shopSettingsRepository
                .GetByShopIdAsync(specifiedShopId)
                .ConfigureAwait(false);


            //Create Qr code.
            var qrCode = QrCode.FromBank(
                userId,
                QrCodeType.FromName(qrCodeType),
                fullName,
                qrCodeSettingService,
                quotaService,
                pairingDataService,
                this._dateTimeService,
                bankName,
                bankMark,
                accountName,
                cardNumber,
                shopSettings,
                cloudDevice
                );

            var qrCodeCreated = _qrCodeRepository.Add(qrCode);

            //Save changes and execute related domain events.
            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync().ConfigureAwait(false);



            //Create view data here because it may slow down the performence if create in domain handler.
            var qrCodeVM = await _qrCodeQueries
                .MapFromEntity(qrCodeCreated, userToAddQrCode).ConfigureAwait(false);

            _qrCodeQueries.Add(qrCodeVM);

            await _qrCodeQueries.SaveChangesAsync();
        }



        public async Task UpdateBarcodeDataForManual(string updateByUserId, int qrCodeId, string qrCodeUrl, decimal? amount)
        {
            //Validate user is exist.
            var updateByUser = await ValidateUserExist(updateByUserId).ConfigureAwait(false);

            ///Validate user permission
            var qrCodeEntry = await ValidateUserPermissionOnQrCodeAsync(updateByUser, qrCodeId).ConfigureAwait(false);


            //Get QR code.
            var qrcode = await _qrCodeRepository.GetByQrCodeIdAsync(qrCodeId);

            if (qrcode is null)
            {
                throw new InvalidOperationException("查无二维码数据。");
            }

            //Get shop settings if qr code has specified shop.
            ShopSettings shopSettings = null;
            var shopId = qrcode.SpecifiedShopId;
            if (!string.IsNullOrEmpty(shopId))
            {
                shopSettings = await _shopSettingsRepository.GetByShopIdAsync(shopId);
            }

            qrcode.UpdateBarcodeDataForManual(qrCodeUrl, amount, shopSettings);


            _qrCodeRepository.Update(qrcode);

            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
        }


        public async Task UpdateBarcodeDataForAuto(string updateByUserId, int qrCodeId, int cloudDeviceId)
        {
            //Validate user is exist.
            var updateByUser = await ValidateUserExist(updateByUserId).ConfigureAwait(false);

            ///Validate user permission
            var qrCodeEntry = await ValidateUserPermissionOnQrCodeAsync(updateByUser, qrCodeId).ConfigureAwait(false);


            //Get QR code.
            var qrcode = await _qrCodeRepository.GetByQrCodeIdAsync(qrCodeId);

            if (qrcode is null)
            {
                throw new InvalidOperationException("查无二维码数据。");
            }

            //Get cloud device.
            var cloudDevice = await _cloudDeviceRepository.GetByCloudDeviceIdAsync(cloudDeviceId);

            qrcode.UpdateBarcodeDataForAuto(cloudDevice);


            _qrCodeRepository.Update(qrcode);

            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
        }


        public async Task UpdateMerchantData(string updateByUserId, int qrCodeId, string appId, string privateKey, string merchantId, string alipayPublicKey, string wechatApiCertificate)
        {
            //Validate user is exist.
            var updateByUser = await ValidateUserExist(updateByUserId).ConfigureAwait(false);

            ///Validate user permission
            var qrCodeEntry = await ValidateUserPermissionOnQrCodeAsync(updateByUser, qrCodeId).ConfigureAwait(false);


            //Get QR code.
            var qrcode = await _qrCodeRepository.GetByQrCodeIdAsync(qrCodeId);

            if (qrcode is null)
            {
                throw new InvalidOperationException("查无二维码数据。");
            }


            qrcode.UpdateMerchantData(appId, privateKey, merchantId, alipayPublicKey, wechatApiCertificate);


            _qrCodeRepository.Update(qrcode);

            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
        }


        public async Task UpdateTransactionData(string updateByUserId, int qrCodeId, string alipayUserId)
        {
            //Validate user is exist.
            var updateByUser = await ValidateUserExist(updateByUserId).ConfigureAwait(false);

            ///Validate user permission
            var qrCodeEntry = await ValidateUserPermissionOnQrCodeAsync(updateByUser, qrCodeId).ConfigureAwait(false);


            //Get QR code.
            var qrcode = await _qrCodeRepository.GetByQrCodeIdAsync(qrCodeId);

            if (qrcode is null)
            {
                throw new InvalidOperationException("查无二维码数据。");
            }


            qrcode.UpdateTransactionData(alipayUserId);


            _qrCodeRepository.Update(qrcode);

            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
        }


        public async Task UpdateBankData(string updateByUserId, int qrCodeId, string bankName, string bankMark, string accountName, string cardNumber)
        {
            //Validate user is exist.
            var updateByUser = await ValidateUserExist(updateByUserId).ConfigureAwait(false);

            ///Validate user permission
            var qrCodeEntry = await ValidateUserPermissionOnQrCodeAsync(updateByUser, qrCodeId).ConfigureAwait(false);


            //Get QR code.
            var qrcode = await _qrCodeRepository.GetByQrCodeIdAsync(qrCodeId);

            if (qrcode is null)
            {
                throw new InvalidOperationException("查无二维码数据。");
            }


            qrcode.UpdateBankData(bankName, bankMark, accountName, cardNumber);


            _qrCodeRepository.Update(qrcode);

            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
        }


        public async Task UpdateQrCodeSettings(string updateByUserId, int qrCodeId, QrCodeEntrySetting qrCodeEntrySetting)
        {
            //Validate qr code entry setting is provided.
            if (qrCodeEntrySetting is null)
            {
                throw new ArgumentNullException("The QR code entry setting must be provided.");
            }

            //Validate user is exist.
            var updateByUser = await ValidateUserExist(updateByUserId).ConfigureAwait(false);

            ///Validate user permission
            await ValidateUserPermissionOnQrCodeAsync(updateByUser, qrCodeId).ConfigureAwait(false);


            //Get QR code.
            var qrcode = await _qrCodeRepository.GetByQrCodeIdAsync(qrCodeId);

            if (qrcode is null)
            {
                throw new InvalidOperationException("查无二维码数据。");
            }


            qrcode.UpdateQrCodeSettings(
                qrCodeEntrySetting.AutoPairingBySuccessRate,
                qrCodeEntrySetting.AutoPairingByQuotaLeft,
                qrCodeEntrySetting.AutoPairingByBusinessHours,
                qrCodeEntrySetting.AutoPairingByCurrentConsecutiveFailures,
                qrCodeEntrySetting.AutoPairngByAvailableBalance,
                (decimal)qrCodeEntrySetting.SuccessRateThresholdInHundredth / 100M,
                qrCodeEntrySetting.SuccessRateMinOrders,
                qrCodeEntrySetting.QuotaLeftThreshold,
                qrCodeEntrySetting.CurrentConsecutiveFailuresThreshold,
                qrCodeEntrySetting.AvailableBalanceThreshold
                );


            _qrCodeRepository.Update(qrcode);

            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
        }


        public async Task UpdateQuota(string updateByUserId, int qrCodeId, int dayAmountLimit, int orderAmountUpperLimit, int orderAmountLowerLimit)
        {
            //Validate user is exist.
            var updateByUser = await ValidateUserExist(updateByUserId).ConfigureAwait(false);

            ///Validate user permission
            var qrCodeEntry = await ValidateUserPermissionOnQrCodeAsync(updateByUser, qrCodeId).ConfigureAwait(false);


            //Get QR code.
            var qrcode = await _qrCodeRepository.GetByQrCodeIdAsync(qrCodeId);

            if (qrcode is null)
            {
                throw new InvalidOperationException("查无二维码数据。");
            }


            qrcode.UpdateQuota(dayAmountLimit, orderAmountUpperLimit, orderAmountLowerLimit);


            _qrCodeRepository.Update(qrcode);

            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task UpdateBaseInfo(string updateByUserId, int qrCodeId, string fullName, string shopId)
        {
            //Validate user is exist.
            var updateByUser = await ValidateUserExist(updateByUserId).ConfigureAwait(false);

            ///Validate user permission
            var qrCodeEntry = await ValidateUserPermissionOnQrCodeAsync(updateByUser, qrCodeId).ConfigureAwait(false);

            //Get Shop Settigs.
            var shopSettings = await _shopSettingsRepository.GetByShopIdAsync(shopId);

            //Get QR code.
            var qrcode = await _qrCodeRepository.GetByQrCodeIdAsync(qrCodeId);

            if (qrcode is null)
            {
                throw new InvalidOperationException("查无二维码数据。");
            }


            qrcode.UpdateFullName(fullName);
            qrcode.SpecifyShop(shopSettings);

            _qrCodeRepository.Update(qrcode);

            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
        }



        public async Task ApproveQrCode(int qrCodeId, string aprrovedByAdminId)
        {
            //Checking the admin is exist.
            var admin = await _userManager.FindByIdAsync(aprrovedByAdminId);
            if (admin == null)
            {
                throw new KeyNotFoundException("No admin found by given admin id.");
            }
            if (admin.BaseRoleType != BaseRoleType.Manager)
            {
                throw new InvalidOperationException("Only admin can approve a QR code.");
            }

            //Checking the qrCode is exist.
            var qrCode = await this._qrCodeRepository.GetByQrCodeIdAsync(qrCodeId);
            if (qrCode == null)
            {
                throw new KeyNotFoundException("No qrCode found by given QR code id.");
            }

            //Approve
            qrCode.Approve(
                new Pairing.Domain.Model.Shared.Admin(admin.Id, admin.UserName)
                );

            _qrCodeRepository.Update(qrCode);
            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task ApproveQrCodes(List<int> qrCodeIds, string aprrovedByAdminId)
        {
            //Checking the admin is exist.
            var admin = await _userManager.FindByIdAsync(aprrovedByAdminId);
            if (admin == null)
            {
                throw new KeyNotFoundException("No admin found by given admin id.");
            }
            if (admin.BaseRoleType != BaseRoleType.Manager)
            {
                throw new InvalidOperationException("Only admin can approve a qrCode.");
            }

            //Checking the list is not null.
            if (qrCodeIds == null)
            {
                throw new ArgumentNullException("The qr code ids must be provided.");
            }

            foreach (var qrCodeId in qrCodeIds)
            {
                //Checking the qrCode is exist.
                var qrCode = await this._qrCodeRepository.GetByQrCodeIdAsync(qrCodeId);
                if (qrCode == null)
                {
                    throw new KeyNotFoundException("No qrCode found by given qrCode id.");
                }

                //Checking the balance is exist.
                /*var balance = await _balanceRepository.GetByBalanceIdAsync(qrCode.BalanceId);
                if (balance == null)
                {
                    throw new KeyNotFoundException($"No balance found.");
                }*/


                qrCode.Approve(
                    new Pairing.Domain.Model.Shared.Admin(admin.Id, admin.UserName)
                    );

                _qrCodeRepository.Update(qrCode);
            }

            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
        }



        public async Task RejectQrCode(int qrCodeId, string rejectByAdminId)
        {
            //Checking the admin is exist.
            var admin = await _userManager.FindByIdAsync(rejectByAdminId);
            if (admin == null)
            {
                throw new KeyNotFoundException("No admin found by given admin id.");
            }
            if (admin.BaseRoleType != BaseRoleType.Manager)
            {
                throw new InvalidOperationException("Only admin can reject a QR code.");
            }

            //Delete qr code.
            await this.DeleteWithoutSaveEntities(qrCodeId);


            //Save all changes.
            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
            await _qrCodeQueries.SaveChangesAsync();
        }

        public async Task RejectQrCodes(List<int> qrCodeIds, string aprrovedByAdminId)
        {
            //Checking the admin is exist.
            var admin = await _userManager.FindByIdAsync(aprrovedByAdminId);
            if (admin == null)
            {
                throw new KeyNotFoundException("No admin found by given admin id.");
            }
            if (admin.BaseRoleType != BaseRoleType.Manager)
            {
                throw new InvalidOperationException("Only admin can approve a qrCode.");
            }

            //Checking the list is not null.
            if (qrCodeIds == null)
            {
                throw new ArgumentNullException("The qr code ids must be provided.");
            }

            //Delte qr codes.
            foreach (var qrCodeId in qrCodeIds)
            {
                await this.DeleteWithoutSaveEntities(qrCodeId);
            }

            //Save all changes.
            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
            await _qrCodeQueries.SaveChangesAsync();
        }



        public async Task EnableQrCode(int qrCodeId, string enabledByAdminId)
        {
            //Checking the admin is exist and validate it.
            var admin = await _userManager.FindByIdAsync(enabledByAdminId);
            if (admin == null)
            {
                throw new KeyNotFoundException("No admin found by given admin id.");
            }
            if (admin.BaseRoleType != BaseRoleType.Manager)
            {
                throw new InvalidOperationException("Only admin can enable a qrCode.");
            }



            //Checking the qrCode is exist.
            var qrCode = await this._qrCodeRepository.GetByQrCodeIdAsync(qrCodeId);
            if (qrCode == null)
            {
                throw new KeyNotFoundException("No qrCode found by given QR code id.");
            }

            //!! Check the trader is enabled. (Part of domain logic)
            var trader = await _userManager.FindByIdAsync(qrCode.UserId);
            if (trader == null)
            {
                throw new InvalidOperationException("No trader found by QR code's info.");
            }
            if (!trader.IsEnabled)
            {
                throw new InvalidOperationException("需启用交易员账号后才可启用所属二维码。");
            }

            //If the qr code is barcode, get the shop settings.
            ShopSettings shopSettings = null;
            if (qrCode.GetPaymentScheme.Id == PaymentScheme.Barcode.Id)
            {
                var shopId = qrCode.SpecifiedShopId;
                if (!string.IsNullOrEmpty(shopId))
                {
                    shopSettings = await this._shopSettingsRepository.GetByShopIdAsync(shopId);
                }
            }

            //Enable
            qrCode.Enable(shopSettings);

            _qrCodeRepository.Update(qrCode);
            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task EnableQrCodes(List<int> qrCodeIds, string enabledByAdminId)
        {
            //Checking the admin is exist.
            var admin = await _userManager.FindByIdAsync(enabledByAdminId);
            if (admin == null)
            {
                throw new KeyNotFoundException("No admin found by given admin id.");
            }
            if (admin.BaseRoleType != BaseRoleType.Manager)
            {
                throw new InvalidOperationException("Only admin can approve a qrCode.");
            }

            //Checking the list is not null.
            if (qrCodeIds == null)
            {
                throw new ArgumentNullException("The qr code ids must be provided.");
            }


            foreach (var qrCodeId in qrCodeIds)
            {
                //Checking the qrCode is exist.
                var qrCode = await this._qrCodeRepository.GetByQrCodeIdAsync(qrCodeId);
                if (qrCode == null)
                {
                    throw new KeyNotFoundException("No qrCode found by given QR code id.");
                }

                //!! Check the trader is enabled. (Part of domain logic)
                var trader = await _userManager.FindByIdAsync(qrCode.UserId);
                if (trader == null)
                {
                    throw new InvalidOperationException("No trader found by QR code's info.");
                }
                if (!trader.IsEnabled)
                {
                    throw new InvalidOperationException("需启用交易员账号后才可启用所属二维码。");
                }

                //If the qr code is barcode, get the shop settings.
                ShopSettings shopSettings = null;
                if (qrCode.GetPaymentScheme.Id == PaymentScheme.Barcode.Id)
                {
                    var shopId = qrCode.SpecifiedShopId;
                    if (!string.IsNullOrEmpty(shopId))
                    {
                        shopSettings = await this._shopSettingsRepository.GetByShopIdAsync(shopId);
                    }
                }

                //Enable
                qrCode.Enable(shopSettings);

                _qrCodeRepository.Update(qrCode);
            }

            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
        }



        public async Task DisableQrCode(int qrCodeId, string disabledByAdminId, string description)
        {
            //Checking the admin is exist and validate it.
            var admin = await _userManager.FindByIdAsync(disabledByAdminId);
            if (admin == null)
            {
                throw new KeyNotFoundException("No admin found by given admin id.");
            }
            if (admin.BaseRoleType != BaseRoleType.Manager)
            {
                throw new InvalidOperationException("Only admin can approve a qrCode.");
            }

            //Checking the qrCode is exist.
            var qrCode = await this._qrCodeRepository.GetByQrCodeIdAsync(qrCodeId);
            if (qrCode == null)
            {
                throw new KeyNotFoundException("No qrCode found by given QR code id.");
            }

            //Disable
            qrCode.Disable(false, description);

            _qrCodeRepository.Update(qrCode);
            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task DisableQrCodes(List<int> qrCodeIds, string disabledByAdminId, string description)
        {
            //Checking the admin is exist.
            var admin = await _userManager.FindByIdAsync(disabledByAdminId);
            if (admin == null)
            {
                throw new KeyNotFoundException("No admin found by given admin id.");
            }
            if (admin.BaseRoleType != BaseRoleType.Manager)
            {
                throw new InvalidOperationException("Only admin can approve a qrCode.");
            }

            //Checking the list is not null.
            if (qrCodeIds == null)
            {
                throw new ArgumentNullException("The qr code ids must be provided.");
            }

            foreach (var qrCodeId in qrCodeIds)
            {
                //Checking the qrCode is exist.
                var qrCode = await this._qrCodeRepository.GetByQrCodeIdAsync(qrCodeId);
                if (qrCode == null)
                {
                    throw new KeyNotFoundException("No qrCode found by given QR code id.");
                }

                //Disable
                qrCode.Disable(false, description);

                _qrCodeRepository.Update(qrCode);
            }

            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
        }


        public async Task StartPairingQrCode(int qrCodeId, string startedByUserId)
        {
            //Checking the user is exist.
            var user = await _userManager.FindByIdAsync(startedByUserId);
            if (user == null)
            {
                throw new KeyNotFoundException("No user found by given user id.");
            }


            //Checking the qrCode is exist.
            var qrCode = await this._qrCodeRepository.GetByQrCodeIdAsync(qrCodeId);
            if (qrCode == null)
            {
                throw new KeyNotFoundException("No QR code found by given QR code id.");
            }

            //Check user's permission.
            if (user.BaseRoleType != BaseRoleType.Manager)
            {
                //If the user is not QR code's owner, throw error.
                if (user.BaseRoleType != BaseRoleType.Trader || user.Id != qrCode.UserId)
                {
                    throw new InvalidOperationException("用户没有此权限。");
                }
            }

            //Start
            qrCode.StartPairing();

            _qrCodeRepository.Update(qrCode);
            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task StartPairingQrCodes(List<int> qrCodeIds, string startedByUserId)
        {
            //Checking the user is exist.
            var user = await _userManager.FindByIdAsync(startedByUserId);
            if (user == null)
            {
                throw new KeyNotFoundException("No user found by given user id.");
            }

            //Checking the list is not null.
            if (qrCodeIds == null)
            {
                throw new ArgumentNullException("The qr code ids must be provided.");
            }


            foreach (var qrCodeId in qrCodeIds)
            {
                //Checking the qrCode is exist.
                var qrCode = await this._qrCodeRepository.GetByQrCodeIdAsync(qrCodeId);
                if (qrCode == null)
                {
                    throw new KeyNotFoundException("No QR code found by given QR code id.");
                }

                //Check user's permission.
                if (user.BaseRoleType != BaseRoleType.Manager)
                {
                    //If the user is not QR code's owner, throw error.
                    if (user.BaseRoleType != BaseRoleType.Trader || user.Id != qrCode.UserId)
                    {
                        throw new InvalidOperationException("用户没有此权限。");
                    }
                }

                //Start
                qrCode.StartPairing();

                _qrCodeRepository.Update(qrCode);
            }

            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
        }


        public async Task StopPairingQrCode(int qrCodeId, string stopedByUserId)
        {
            //Checking the user is exist.
            var user = await _userManager.FindByIdAsync(stopedByUserId);
            if (user == null)
            {
                throw new KeyNotFoundException("No user found by given user id.");
            }


            //Checking the QR code is exist.
            var qrCode = await this._qrCodeRepository.GetByQrCodeIdAsync(qrCodeId);
            if (qrCode == null)
            {
                throw new KeyNotFoundException("No QR code found by given QR code id.");
            }

            //Check user's permission.
            if (user.BaseRoleType != BaseRoleType.Manager)
            {
                //If the user is not QR code's owner, throw error.
                if (user.BaseRoleType != BaseRoleType.Trader || user.Id != qrCode.UserId)
                {
                    throw new InvalidOperationException("用户没有此权限。");
                }
            }

            //Stop
            qrCode.StopPairing(false);

            _qrCodeRepository.Update(qrCode);
            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task StopPairingQrCodes(List<int> qrCodeIds, string stopedByUserId)
        {
            //Checking the user is exist.
            var user = await _userManager.FindByIdAsync(stopedByUserId);
            if (user == null)
            {
                throw new KeyNotFoundException("No user found by given user id.");
            }

            //Checking the list is not null.
            if (qrCodeIds == null)
            {
                throw new ArgumentNullException("The qr code ids must be provided.");
            }


            foreach (var qrCodeId in qrCodeIds)
            {
                //Checking the QR code is exist.
                var qrCode = await this._qrCodeRepository.GetByQrCodeIdAsync(qrCodeId);
                if (qrCode == null)
                {
                    throw new KeyNotFoundException("No QR code found by given QR code id.");
                }

                //Check user's permission.
                if (user.BaseRoleType != BaseRoleType.Manager)
                {
                    //If the user is not QR code's owner, throw error.
                    if (user.BaseRoleType != BaseRoleType.Trader || user.Id != qrCode.UserId)
                    {
                        throw new InvalidOperationException("用户没有此权限。");
                    }
                }

                //Stop
                qrCode.StopPairing(false);

                _qrCodeRepository.Update(qrCode);
            }

            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
        }


        public async Task ResetRiskControlData(string resetByAdminId, int qrCodeId, bool resetQuotaLeftToday, bool resetCurrentConsecutiveFailures, bool resetSuccessRateAndRelatedData)
        {
            //Checking the admin is exist and validate it.
            var admin = await _userManager.FindByIdAsync(resetByAdminId);
            if (admin == null)
            {
                throw new KeyNotFoundException("No admin found by given admin id.");
            }
            if (admin.BaseRoleType != BaseRoleType.Manager)
            {
                throw new InvalidOperationException("Only admin can approve a qrCode.");
            }

            //Checking the qrCode is exist.
            var qrCode = await this._qrCodeRepository.GetByQrCodeIdAsync(qrCodeId);
            if (qrCode == null)
            {
                throw new KeyNotFoundException("No qrCode found by given QR code id.");
            }

            //Reset
            if (resetQuotaLeftToday)
            {
                qrCode.ResetQuotaLeftToday(new Admin(admin.Id, admin.UserName));
            }
            if (resetCurrentConsecutiveFailures)
            {
                qrCode.ResetCurrentConsecutiveFailures(new Admin(admin.Id, admin.UserName));
            }
            if (resetSuccessRateAndRelatedData)
            {
                qrCode.ResetSuccessRateAndRelatedData(new Admin(admin.Id, admin.UserName));
            }


            _qrCodeRepository.Update(qrCode);
            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
        }



        #endregion


        #region Custom Function
        public string DecodeQrCodeData(IFormFile qrCodeFile)
        {
            throw new NotImplementedException();
        }

        private async Task<ApplicationUser> ValidateUserExist(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("No user found by given user id.");
            }

            return user;
        }

        private async Task<QrCodeEntry> ValidateUserPermissionOnQrCodeAsync(ApplicationUser user, int qrCodeId)
        {
            if (user == null)
            {
                throw new KeyNotFoundException("The user must be provided.");
            }


            //Get QR code to do permission check later.
            var qrCodeEntry = await _qrCodeQueries.GetQrCodeEntryAsync(qrCodeId).ConfigureAwait(false);
            if (qrCodeEntry == null)
            {
                throw new InvalidOperationException("查无二维码。");
            }

            //Check user permission.
            if (user.BaseRoleType == BaseRoleType.Manager)
            {
            }
            else if (user.BaseRoleType == BaseRoleType.TraderAgent)
            {
                if (qrCodeEntry.UplineUserId != user.Id)
                {
                    throw new InvalidOperationException("代理只能查看直属交易员的二维码。");
                }
            }
            else if (user.BaseRoleType == BaseRoleType.Trader)
            {
                if (qrCodeEntry.UserId != user.Id)
                {
                    throw new InvalidOperationException("交易员只能查看自己的二维码。");
                }
            }
            else
            {
                throw new InvalidOperationException("用户没有二维码权限。");
            }

            return qrCodeEntry;
        }


        private void ValidateUserPermissionOnCreateQrCode(ApplicationUser createByUser, ApplicationUser userToAddQrCode)
        {
            //If the QR code isn't create by manager, check the QR code is create by trader,
            //and the owner is him.
            if (createByUser.BaseRoleType != BaseRoleType.Manager)
            {
                if (createByUser.BaseRoleType == BaseRoleType.Trader)
                {
                    if (userToAddQrCode.Id != createByUser.Id)
                    {
                        throw new InvalidOperationException($"无效操作，二维码必须添加在自己的账户下。");
                    }
                }
                else if (createByUser.BaseRoleType == BaseRoleType.TraderAgent)
                {
                    if (userToAddQrCode.UplineId != createByUser.Id)
                    {
                        throw new InvalidOperationException($"无效操作，代理仅能为直属交易员添加二维码。");
                    }
                }
                else
                {
                    throw new InvalidOperationException("用户没有此权限。");
                }
            }
        }

        private IQrCodeSettingService BuildQrCodeSettingServiceFrom(QrCodeEntrySetting qrCodeEntrySetting)
        {
            if (qrCodeEntrySetting is null)
            {
                throw new ArgumentNullException("The qr code entry settings must be provided.");
            }

            //Build default QR code settings service.
            var qrCodeSettingService = new FakeQrCodeSettingService
            {
                AutoPairingBySuccessRate = qrCodeEntrySetting.AutoPairingBySuccessRate,
                AutoPairingByQuotaLeft = qrCodeEntrySetting.AutoPairingByQuotaLeft,
                AutoPairingByBusinessHours = qrCodeEntrySetting.AutoPairingByBusinessHours,
                AutoPairingByCurrentConsecutiveFailures = qrCodeEntrySetting.AutoPairingByCurrentConsecutiveFailures,
                AutoPairngByAvailableBalance = qrCodeEntrySetting.AutoPairngByAvailableBalance,
                SuccessRateThreshold = (decimal)qrCodeEntrySetting.SuccessRateThresholdInHundredth / 100M,
                SuccessRateMinOrders = qrCodeEntrySetting.SuccessRateMinOrders,
                QuotaLeftThreshold = qrCodeEntrySetting.QuotaLeftThreshold,
                CurrentConsecutiveFailuresThreshold = qrCodeEntrySetting.CurrentConsecutiveFailuresThreshold,
                AvailableBalanceThreshold = qrCodeEntrySetting.AvailableBalanceThreshold
            };

            return qrCodeSettingService;
        }

        private async Task<IPairingDataService> BuildPairingDataServiceFromAsync(ApplicationUser userToAddQrCode, string paymentChannel, string specifiedShopId)
        {
            if (userToAddQrCode == null)
            {
                throw new ArgumentNullException("The user to add qr code must be provided.");
            }
            var userId = userToAddQrCode.Id;

            var toppestCommission = await _commissionQueries
                .GetToppestCommissionRateFromTradeUserAsync(userId)
                .ConfigureAwait(false);
            if (toppestCommission is null)
            {
                throw new InvalidOperationException("找无最高费率。");
            }

            var availableBalance = await _balanceQueries.GetAvailableBalanceByUserId(userId).ConfigureAwait(false);
            if (availableBalance is null)
            {
                throw new InvalidOperationException("找无用户余额。");
            }

            IPairingDataService pairingDataService = null;

            if (paymentChannel == PaymentChannelOption.Alipay.Value)
            {
                pairingDataService = new FakePairingDataService(
                    toppestCommission.RateAlipayInThousandth / 1000M,
                    (decimal)availableBalance,
                    specifiedShopId
                    );
            }
            else if (paymentChannel == PaymentChannelOption.Wechat.Value)
            {
                pairingDataService = new FakePairingDataService(
                    toppestCommission.RateWechatInThousandth,
                    (decimal)availableBalance,
                    specifiedShopId
                    );
            }
            else
            {
                throw new ArgumentException("无效的支付类型参数");
            }

            return pairingDataService;
        }


        private async Task DeleteWithoutSaveEntities(int qrCodeId)
        {
            //Checking the qr code is exist.
            var qrCode = await this._qrCodeRepository.GetByQrCodeIdAsync(qrCodeId);
            if (qrCode == null)
            {
                throw new KeyNotFoundException("No qr code found by given QR code id.");
            }

            //Checking the qr code is not approved yet.
            if (qrCode.IsApproved)
            {
                throw new InvalidOperationException("无法拒绝审核已审核的二维码。");
            }

            //Delete qr code.
            _qrCodeRepository.Delete(qrCode);


            //Delete qr code view data.
            var qrCodeEntry = await this._qrCodeQueries.GetQrCodeEntryAsync(qrCodeId);
            if (qrCodeEntry != null)
            {
                this._qrCodeQueries.Delete(qrCodeEntry);
            }
        }
        #endregion
    }
}
