using Pairing.Domain.Events;
using Pairing.Domain.Exceptions;
using Pairing.Domain.Model.CloudDevices;
using Pairing.Domain.Model.Shared;
using Pairing.Domain.Model.ShopSettingsDomainModel;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pairing.Domain.Model.QrCodes
{
    public class QrCode : Entity, IAggregateRoot
    {
        public string UserId { get; private set; }
        public int? CloudDeviceId { get; private set; }

        public QrCodeType QrCodeType { get; private set; }
        public int _qrCodeTypeId;

        public PaymentChannel PaymentChannel { get; private set; }
        public int _paymentChannelId;

        public PaymentScheme PaymentScheme { get; private set; }
        public int _paymentSchemeId;

        public QrCodeStatus QrCodeStatus { get; private set; }
        public int _qrCodeStatusId;

        public PairingStatus PairingStatus { get; private set; }
        public int _pairingStatusId;

        public string CodeStatusDiscription { get; private set; }
        public string PairingStatusDescription { get; private set; }

        public QrCodeSettings QrCodeSettings { get; private set; }

        //public Quota Quota { get; private set; }

        //Quota
        public decimal DailyAmountLimit { get; private set; }
        public decimal OrderAmountUpperLimit { get; private set; }
        public decimal OrderAmountLowerLimit { get; private set; }



        public string FullName { get; private set; }
        public bool IsApproved { get; private set; }
        public Admin ApprovedBy { get; private set; }
        public BarcodeDataForManual BarcodeDataForManual { get; private set; }
        public BarcodeDataForAuto BarcodeDataForAuto { get; private set; }
        public MerchantData MerchantData { get; private set; }
        public TransactionData TransactionData { get; private set; }
        public BankData BankData { get; private set; }

        public PairingData PairingData { get; private set; }

        //Pairing Data
        public bool IsOnline { get; private set; }
        public decimal MinCommissionRate { get; private set; }
        public decimal AvailableBalance { get; private set; }
        public string SpecifiedShopId { get; private set; }
        public decimal QuotaLeftToday { get; private set; }
        public DateTime? DateLastTraded { get; private set; }


        public DateTime DateCreated { get; private set; }


        //private readonly List<QrCodeOrder> _qrCodeOrders;
        //public IReadOnlyCollection<QrCodeOrder> QrCodeOrders => _qrCodeOrders;

        public QrCodeType GetQrCodeType => QrCodeType.From(this._qrCodeTypeId);
        public PaymentChannel GetPaymentChannel => PaymentChannel.From(this._paymentChannelId);
        public PaymentScheme GetPaymentScheme => PaymentScheme.From(this._paymentSchemeId);
        public QrCodeStatus GetQrCodeStatus => QrCodeStatus.From(this._qrCodeStatusId);
        public PairingStatus GetPairingStatus => PairingStatus.From(this._pairingStatusId);

        protected QrCode()
        {
            //_qrCodeOrders = new List<QrCodeOrder>();
        }
        public QrCode(int qrCodeId, int pairingStatusId, QrCodeSettings qrCodeSettings)
        {
            this.Id = qrCodeId;
            this._pairingStatusId = pairingStatusId;
            this.QrCodeSettings = qrCodeSettings;
        }


        public QrCode(string userId, int? cloudDeviceId,
            int qrCodeTypeId, int paymentChannelId, int paymentSchemeId,
            QrCodeSettings qrCodeSettings,
            string fullName, PairingData pairingData, DateTime dateCreated,
            decimal dailyAmountLimit, decimal orderAmountUpperLimit, decimal orderAmountLowerLimit,
            decimal minCommissionRate, decimal availableBalance, string specifiedShopId, decimal quotaLeftToday
            )
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            CloudDeviceId = cloudDeviceId;
            _qrCodeTypeId = qrCodeTypeId;
            _paymentChannelId = paymentChannelId;
            _paymentSchemeId = paymentSchemeId;
            QrCodeSettings = qrCodeSettings ?? throw new ArgumentNullException(nameof(qrCodeSettings));
            //Quota = quota ?? throw new ArgumentNullException(nameof(quota));
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
            PairingData = pairingData ?? throw new ArgumentNullException(nameof(pairingData));

            DailyAmountLimit = dailyAmountLimit;
            OrderAmountUpperLimit = orderAmountUpperLimit;
            OrderAmountLowerLimit = orderAmountLowerLimit;

            IsOnline = false;
            MinCommissionRate = minCommissionRate;
            AvailableBalance = availableBalance;
            SpecifiedShopId = specifiedShopId;
            QuotaLeftToday = quotaLeftToday;

            //Set last traded date to date created. (for speed up pairing performance.)
            DateLastTraded = DateTime.UtcNow;// For performance purpose.


            _qrCodeStatusId = QrCodeStatus.Disabled.Id;
            _pairingStatusId = PairingStatus.NormalDisable.Id;
            IsApproved = false;
            DateCreated = dateCreated;
        }

        public static QrCode FromBarcode(string userId, QrCodeType qrCodeType, PaymentChannel paymentChannel, string fullname,
            IQrCodeSettingService qrCodeSettingService, IQuotaService quotaService, IPairingDataService pairingDataService, IDateTimeService dateTimeService,
            string qrCodeUrl = null, decimal? amount = null, ShopSettings shopSettings = null, CloudDevice cloudDevice = null)
        {
            //Check the user Id is valid;
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new PairingDomainException("The user id must be provided" + ". At QrCode.FromBarcode()");
            }
            if (string.IsNullOrWhiteSpace(fullname) || fullname.Length > 50)
            {
                throw new PairingDomainException("无效姓名" + ". At QrCode.FromBarcode()");
            }

            //Get all required base data
            var qrCodeSettings = qrCodeSettingService.GetDefaultSettings();
            var quotaData = quotaService.GetDefaultQuota();
            var pairingData = pairingDataService.GetDefaultPairingData();

            pairingDataService.GetDefaultPairingData(
                out decimal toppestCommissionRate,
                out decimal availableBalance,
                out string specifiedShopId);

            //Checking all required data for manual pairing.
            if (qrCodeType.Id == QrCodeType.Manual.Id)
            {
                //var barcodeData = BarcodeDataForManual.From(qrCodeUrl, amount, shopSettings);

                var qrCode = new QrCode(
                    userId,
                    null,
                    qrCodeType.Id,
                    paymentChannel.Id,
                    PaymentScheme.Barcode.Id,
                    qrCodeSettings,
                    fullname,
                    pairingData,
                    dateTimeService.GetCurrentDateTime(),
                    quotaData.DailyAmountLimit,
                    quotaData.OrderAmountUpperLimit,
                    quotaData.OrderAmountLowerLimit,
                    toppestCommissionRate + 0.001M,
                    availableBalance,
                    specifiedShopId,
                    quotaData.DailyAmountLimit
                    );

                qrCode.UpdateBarcodeDataForManual(qrCodeUrl, amount, shopSettings);

                return qrCode;
            }

            //Checking all required data for auto pairing.
            if (qrCodeType.Id == QrCodeType.Auto.Id)
            {
                if (cloudDevice == null)
                {
                    throw new PairingDomainException("The cloud device must be provided" + ". At QrCode.FromBarcode()");
                }

                var qrCode = new QrCode(
                    userId,
                    cloudDevice.Id,
                    qrCodeType.Id,
                    paymentChannel.Id,
                    PaymentScheme.Barcode.Id,
                    qrCodeSettings,
                    fullname,
                    pairingData,
                    dateTimeService.GetCurrentDateTime(),
                    quotaData.DailyAmountLimit,
                    quotaData.OrderAmountUpperLimit,
                    quotaData.OrderAmountLowerLimit,
                    toppestCommissionRate + 0.001M,
                    availableBalance,
                    specifiedShopId,
                    quotaData.DailyAmountLimit
                    );

                qrCode.UpdateBarcodeDataForAuto(cloudDevice);

                return qrCode;
            }

            throw new PairingDomainException("错误的二维码类型" + ". At QrCode.FromBarcode()");
        }


        public static QrCode FromMerchant(string userId, QrCodeType qrCodeType, PaymentChannel paymentChannel, string fullname,
            IQrCodeSettingService qrCodeSettingService, IQuotaService quotaService, IPairingDataService pairingDataService, IDateTimeService dateTimeService,
            string appId, string privateKey, string merchantId,
            ShopSettings shopSettings = null, CloudDevice cloudDevice = null, string alipayPublicKey = null, string wechatApiCertificate = null)
        {
            //Checking the user Id is valid;
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new PairingDomainException("The user id must be provided" + ". At QrCode.FromMerchant()");
            }
            //Checking the name is valid.
            if (string.IsNullOrWhiteSpace(fullname) || fullname.Length > 50)
            {
                throw new PairingDomainException("无效姓名" + ". At QrCode.FromMerchant()");
            }

            //Get all required base data
            var qrCodeSettings = qrCodeSettingService.GetDefaultSettings();
            var quotaData = quotaService.GetDefaultQuota();
            var pairingData = pairingDataService.GetDefaultPairingData();


            pairingDataService.GetDefaultPairingData(
                out decimal toppestCommissionRate,
                out decimal availableBalance,
                out string specifiedShopId);

            QrCode qrCode = null;

            //Checking all required data for manual pairing.
            if (qrCodeType.Id == QrCodeType.Manual.Id)
            {
                qrCode = new QrCode(
                    userId,
                    null,
                    qrCodeType.Id,
                    paymentChannel.Id,
                    PaymentScheme.Merchant.Id,
                    qrCodeSettings,
                    fullname,
                    pairingData,
                    dateTimeService.GetCurrentDateTime(),
                    quotaData.DailyAmountLimit,
                    quotaData.OrderAmountUpperLimit,
                    quotaData.OrderAmountLowerLimit,
                    toppestCommissionRate + 0.001M,
                    availableBalance,
                    specifiedShopId,
                    quotaData.DailyAmountLimit
                    );
            }

            //Checking all required data for auto pairing.
            if (qrCodeType.Id == QrCodeType.Auto.Id)
            {
                if (cloudDevice == null)
                {
                    throw new PairingDomainException("The cloud device must be provided" + ". At QrCode.FromMerchant()");
                }

                qrCode = new QrCode(
                    userId,
                    cloudDevice.Id,
                    qrCodeType.Id,
                    paymentChannel.Id,
                    PaymentScheme.Merchant.Id,
                    qrCodeSettings,
                    fullname,
                    pairingData,
                    dateTimeService.GetCurrentDateTime(),
                    quotaData.DailyAmountLimit,
                    quotaData.OrderAmountUpperLimit,
                    quotaData.OrderAmountLowerLimit,
                    toppestCommissionRate + 0.001M,
                    availableBalance,
                    specifiedShopId,
                    quotaData.DailyAmountLimit
                    );
            }

            if (qrCode is null)
            {
                throw new PairingDomainException("错误的二维码类型" + ". At QrCode.FromMerchant()");
            }

            qrCode.UpdateMerchantData(appId, privateKey, merchantId, alipayPublicKey, wechatApiCertificate);

            return qrCode;
        }


        public static QrCode FromTransaction(string userId, QrCodeType qrCodeType, PaymentScheme paymentScheme, string fullname,
            IQrCodeSettingService qrCodeSettingService, IQuotaService quotaService, IPairingDataService pairingDataService, IDateTimeService dateTimeService,
            string alipayUserId,
            ShopSettings shopSettings = null, CloudDevice cloudDevice = null)
        {
            //Checking the user Id is valid;
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new PairingDomainException("The user id must be provided" + ". At QrCode.FromTransaction()");
            }
            //Checking the name is valid.
            if (string.IsNullOrWhiteSpace(fullname) || fullname.Length > 50)
            {
                throw new PairingDomainException("无效姓名" + ". At QrCode.FromTransaction()");
            }

            //Get all required base data
            var qrCodeSettings = qrCodeSettingService.GetDefaultSettings();
            var quotaData = quotaService.GetDefaultQuota();
            var pairingData = pairingDataService.GetDefaultPairingData();

            QrCode qrCode = null;

            pairingDataService.GetDefaultPairingData(
                out decimal toppestCommissionRate,
                out decimal availableBalance,
                out string specifiedShopId);

            //Checking the payment scheme is allowed.
            if (paymentScheme.Id != PaymentScheme.Transaction.Id
                && paymentScheme.Id != PaymentScheme.Envelop.Id
                && paymentScheme.Id != PaymentScheme.EnvelopPassword.Id)
            {
                throw new PairingDomainException("错误的支付通道。此类型二维码仅支持红包、口令红包及支转支。");
            }

            //Checking all required data for manual pairing.
            if (qrCodeType.Id == QrCodeType.Manual.Id)
            {
                qrCode = new QrCode(
                    userId,
                    null,
                    qrCodeType.Id,
                    PaymentChannel.Alipay.Id,
                    paymentScheme.Id,
                    qrCodeSettings,
                    fullname,
                    pairingData,
                    dateTimeService.GetCurrentDateTime(),
                    quotaData.DailyAmountLimit,
                    quotaData.OrderAmountUpperLimit,
                    quotaData.OrderAmountLowerLimit,
                    toppestCommissionRate + 0.001M,
                    availableBalance,
                    specifiedShopId,
                    quotaData.DailyAmountLimit
                    );
            }

            //Checking all required data for auto pairing.
            if (qrCodeType.Id == QrCodeType.Auto.Id)
            {
                if (cloudDevice == null)
                {
                    throw new PairingDomainException("The cloud device must be provided" + ". At QrCode.FromTransaction()");
                }

                qrCode = new QrCode(
                    userId,
                    cloudDevice.Id,
                    qrCodeType.Id,
                    PaymentChannel.Alipay.Id,
                    paymentScheme.Id,
                    qrCodeSettings,
                    fullname,
                    pairingData,
                    dateTimeService.GetCurrentDateTime(),
                    quotaData.DailyAmountLimit,
                    quotaData.OrderAmountUpperLimit,
                    quotaData.OrderAmountLowerLimit,
                    toppestCommissionRate + 0.001M,
                    availableBalance,
                    specifiedShopId,
                    quotaData.DailyAmountLimit
                    );
            }

            if (qrCode is null)
            {
                throw new PairingDomainException("错误的二维码类型" + ". At QrCode.FromTransaction()");
            }

            qrCode.UpdateTransactionData(alipayUserId);

            return qrCode;
        }


        public static QrCode FromBank(string userId, QrCodeType qrCodeType, string fullname,
            IQrCodeSettingService qrCodeSettingService, IQuotaService quotaService, IPairingDataService pairingDataService, IDateTimeService dateTimeService,
            string bankName, string bankMark, string accountName, string cardNumber,
            ShopSettings shopSettings = null, CloudDevice cloudDevice = null)
        {
            //Checking the user Id is valid;
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new PairingDomainException("The user id must be provided" + ". At QrCode.FromBank()");
            }
            //Checking the name is valid.
            if (string.IsNullOrWhiteSpace(fullname) || fullname.Length > 50)
            {
                throw new PairingDomainException("无效姓名" + ". At QrCode.FromBank()");
            }

            //Get all required base data
            var qrCodeSettings = qrCodeSettingService.GetDefaultSettings();
            var quotaData = quotaService.GetDefaultQuota();
            var pairingData = pairingDataService.GetDefaultPairingData();


            pairingDataService.GetDefaultPairingData(
                out decimal toppestCommissionRate,
                out decimal availableBalance,
                out string specifiedShopId);


            QrCode qrCode = null;

            //Checking all required data for manual pairing.
            if (qrCodeType.Id == QrCodeType.Manual.Id)
            {
                qrCode = new QrCode(
                    userId,
                    null,
                    qrCodeType.Id,
                    PaymentChannel.Alipay.Id,
                    PaymentScheme.Bank.Id,
                    qrCodeSettings,
                    fullname,
                    pairingData,
                    dateTimeService.GetCurrentDateTime(),
                    quotaData.DailyAmountLimit,
                    quotaData.OrderAmountUpperLimit,
                    quotaData.OrderAmountLowerLimit,
                    toppestCommissionRate + 0.001M,
                    availableBalance,
                    specifiedShopId,
                    quotaData.DailyAmountLimit
                    );
            }

            //Checking all required data for auto pairing.
            if (qrCodeType.Id == QrCodeType.Auto.Id)
            {
                if (cloudDevice == null)
                {
                    throw new PairingDomainException("The cloud device must be provided" + ". At QrCode.FromBank()");
                }

                qrCode = new QrCode(
                    userId,
                    cloudDevice.Id,
                    qrCodeType.Id,
                    PaymentChannel.Alipay.Id,
                    PaymentScheme.Bank.Id,
                    qrCodeSettings,
                    fullname,
                    pairingData,
                    dateTimeService.GetCurrentDateTime(),
                    quotaData.DailyAmountLimit,
                    quotaData.OrderAmountUpperLimit,
                    quotaData.OrderAmountLowerLimit,
                    toppestCommissionRate + 0.001M,
                    availableBalance,
                    specifiedShopId,
                    quotaData.DailyAmountLimit
                    );
            }

            if (qrCode is null)
            {
                throw new PairingDomainException("错误的二维码类型" + ". At QrCode.FromBank()");
            }

            qrCode.UpdateBankData(bankName, bankMark, accountName, cardNumber);

            return qrCode;
        }



        public void UpdateBarcodeDataForManual(string qrCodeUrl, decimal? amount, ShopSettings shopSettings = null)
        {
            //Checking this's qr code type is manual.
            if (this._qrCodeTypeId != QrCodeType.Manual.Id)
            {
                throw new PairingDomainException("Qr Code's type must be manual to update barcode data for manual" + ". At QrCode.UpdateBarcodeDataForManual()");
            }
            //Checking the qrcode status is disable.
            if (this._qrCodeStatusId != QrCodeStatus.Disabled.Id && this._qrCodeStatusId != QrCodeStatus.AutoDisabled.Id)
            {
                throw new PairingDomainException("二维码处于停用状态时才可进行编辑" + ". At QrCode.UpdateBarcodeDataForManual()");
            }

            //All validate logics are inside the factory function.
            var barcodeData = BarcodeDataForManual.From(qrCodeUrl, amount, shopSettings);

            this.BarcodeDataForManual = barcodeData;

            this.AddDomainEvent(new QrCodeBarcodeDataForManualUpdatedDomainEvent(
                this,
                barcodeData
                ));
        }

        public void UpdateBarcodeDataForAuto(CloudDevice cloudDevice)
        {
            //Checking this's qr code type is auto.
            if (this._qrCodeTypeId != QrCodeType.Auto.Id)
            {
                throw new PairingDomainException("Qr Code's type must be manual to update barcode data for auto" + ". At QrCode.UpdateBarcodeDataForAuto()");
            }
            //Checking the qrcode status is disable.
            if (this._qrCodeStatusId != QrCodeStatus.Disabled.Id && this._qrCodeStatusId != QrCodeStatus.AutoDisabled.Id)
            {
                throw new PairingDomainException("二维码处于停用状态时才可进行编辑" + ". At QrCode.UpdateBarcodeDataForAuto()");
            }

            //All validate logics are inside the factory function.
            var barcodeData = BarcodeDataForAuto.From(cloudDevice);

            this.BarcodeDataForAuto = barcodeData;

            this.AddDomainEvent(new QrCodeBarcodeDataForAutoUpdatedDomainEvent(
                this,
                barcodeData
                ));
        }

        public void UpdateMerchantData(string appId, string privateKey, string merchantId, string alipayPublicKey = null, string wechatApiCertificate = null)
        {
            //Checking the qrcode status is disable.
            if (this._qrCodeStatusId != QrCodeStatus.Disabled.Id && this._qrCodeStatusId != QrCodeStatus.AutoDisabled.Id)
            {
                throw new PairingDomainException("二维码处于停用状态时才可进行编辑" + ". At QrCode.UpdateMerchantData()");
            }

            MerchantData merchantData = null;

            if (this._paymentChannelId == PaymentChannel.Alipay.Id)
            {
                //All validate logics is inside the factory function.
                merchantData = MerchantData.FromAlipay(appId, alipayPublicKey, privateKey, merchantId);
            }
            else if (this._paymentChannelId == PaymentChannel.Wechat.Id)
            {
                merchantData = MerchantData.FromWechat(appId, wechatApiCertificate, privateKey, merchantId);
            }

            if (merchantData == null)
            {
                throw new PairingDomainException("编辑失败，无法建立商家信息" + ". At QrCode.UpdateMerchantData()");
            }


            this.MerchantData = merchantData;

            this.AddDomainEvent(new QrCodeMerchantDataUpdatedDomainEvent(
                this,
                merchantData
                ));
        }

        public void UpdateTransactionData(string alipayUserId)
        {
            //Checking the qrcode status is disable.
            if (this._qrCodeStatusId != QrCodeStatus.Disabled.Id && this._qrCodeStatusId != QrCodeStatus.AutoDisabled.Id)
            {
                throw new PairingDomainException("二维码处于停用状态时才可进行编辑" + ". At QrCode.UpdateTransactionData()");
            }

            //Checking the payment channel is alipay.
            if (this._paymentChannelId != PaymentChannel.Alipay.Id)
            {
                throw new PairingDomainException("微信二维码无法编辑转账信息" + ". At QrCode.UpdateTransactionData()");
            }

            //Checking the payment scheme is envelop, envalop password or transaction.
            if (this._paymentSchemeId != PaymentScheme.Envelop.Id
                && this._paymentSchemeId != PaymentScheme.EnvelopPassword.Id
                && this._paymentSchemeId != PaymentScheme.Transaction.Id)
            {
                throw new PairingDomainException("只有转账、红包和口令红包类型的二维码才能编辑支付宝用户编号" + ". At QrCode.UpdateTransactionData()");
            }

            var transactionData = new TransactionData(alipayUserId);

            this.TransactionData = transactionData;

            this.AddDomainEvent(new QrCodeTransactionDataUpdatedDomainEvent(
                this,
                transactionData
                ));
        }

        public void UpdateBankData(string bankName, string bankMark, string accountName, string cardNumber)
        {
            //Checking the qrcode status is disable.
            if (this._qrCodeStatusId != QrCodeStatus.Disabled.Id && this._qrCodeStatusId != QrCodeStatus.AutoDisabled.Id)
            {
                throw new PairingDomainException("二维码处于停用状态时才可进行编辑" + ". At QrCode.UpdateBankData()");
            }

            //Checking the payment channel is alipay.
            if (this._paymentChannelId != PaymentChannel.Alipay.Id)
            {
                throw new PairingDomainException("微信二维码无法编辑转账信息" + ". At QrCode.UpdateBankData()");
            }

            //Checking the payment scheme is bank.
            if (this._paymentSchemeId != PaymentScheme.Bank.Id)
            {
                throw new PairingDomainException("只有转账、红包和口令红包类型的二维码才能编辑支付宝用户编号" + ". At QrCode.UpdateBankData()");
            }

            var bankData = new BankData(bankName, bankMark, accountName, cardNumber);

            this.BankData = bankData;

            this.AddDomainEvent(new QrCodeBankDataUpdatedDomainEvent(
                this,
                bankData
                ));
        }


        public void UpdateQrCodeSettings(bool autoPairingBySuccessRate, bool autoPairingByQuotaLeft, bool autoPairingByBusinessHours, bool autoPairingByCurrentConsecutiveFailures, bool autoPairngByAvailableBalance,
            decimal successRateThreshold, int successRateMinOrders, decimal quotaLeftThreshold, int currentConsecutiveFailuresThreshold, decimal availableBalanceThreshold)
        {
            var qrCodeSettings = new QrCodeSettings(
                autoPairingBySuccessRate,
                autoPairingByQuotaLeft,
                autoPairingByBusinessHours,
                autoPairingByCurrentConsecutiveFailures,
                autoPairngByAvailableBalance,
                successRateThreshold,
                successRateMinOrders,
                quotaLeftThreshold,
                currentConsecutiveFailuresThreshold,
                availableBalanceThreshold
                );

            this.QrCodeSettings = qrCodeSettings;

            this.AddDomainEvent(new QrCodeSettingsUpdatedDomainEvent(
                this,
                qrCodeSettings
                ));
        }

        public void UpdateQuota(decimal dailyAmountLimit, decimal orderAmountUpperLimit, decimal orderAmountLowerLimit)
        {
            //Checking the quota limit value is correct.
            if (dailyAmountLimit < 0 || orderAmountUpperLimit < 0 || orderAmountLowerLimit < 0
                || orderAmountLowerLimit > orderAmountUpperLimit)
            {
                throw new PairingDomainException("无效的额度限制参数" + ". At QrCode.UpdateQuota()");
            }
            else if (decimal.Round(dailyAmountLimit, 0) != dailyAmountLimit
                || decimal.Round(orderAmountUpperLimit, 0) != orderAmountUpperLimit
                || decimal.Round(orderAmountLowerLimit, 0) != orderAmountLowerLimit)
            {
                throw new PairingDomainException("额度限制参数必须为整数" + ". At QrCode.UpdateQuota()");
            }

            //Get Success Amount Today.
            var successAmount = this.DailyAmountLimit - this.QuotaLeftToday;

            //Update Properties.
            this.DailyAmountLimit = dailyAmountLimit;
            this.OrderAmountLowerLimit = orderAmountLowerLimit;
            this.OrderAmountUpperLimit = orderAmountUpperLimit;

            //Update Quota Left.
            this.QuotaLeftToday = this.DailyAmountLimit - successAmount;
            //If quota left is less than 0, then set it to 0.
            if (this.QuotaLeftToday < 0)
            {
                this.QuotaLeftToday = 0;
            }
            this.AddDomainEvent(new QrCodeQuotaUpdatedDomainEvent(
                this,
                this.DailyAmountLimit,
                this.OrderAmountUpperLimit,
                this.OrderAmountLowerLimit
                ));
        }

        public void UpdateFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName) || fullName.Length > 50)
            {
                throw new PairingDomainException("无效姓名" + ". At QrCode.ValidateFullName()");
            }

            this.FullName = fullName;

            this.AddDomainEvent(new QrCodeFullNameUpdatedDomainEvent(
                this,
                fullName
                ));
        }

        public void SpecifyShop(ShopSettings shopSettings = null)
        {
            //Check this qr code is disable.
            if (!this.IsDisable())
            {
                throw new PairingDomainException("停用二维码后才能修改指定商户" + ". At QrCode.ValidateFullName()");
            }

            this.SpecifiedShopId = shopSettings?.ShopId;

            this.AddDomainEvent(new ShopSettingsSpecifiedShopUpdatedDomainEvent(
                this,
                shopSettings
                ));
        }

        public void Approve(Admin admin)
        {
            //Checking the qr code has not been approved yet.
            if (this.IsApproved)
            {
                throw new PairingDomainException("无法重复审核二维码" + ". At QrCode.ValidateFullName()");
            }

            //Checking the admin is not null.
            if (admin is null)
            {
                throw new PairingDomainException("The admin must be provided" + ". At QrCode.ValidateFullName()");
            }

            this.IsApproved = true;
            this.ApprovedBy = admin;

            this.AddDomainEvent(new QrCodeApprovedDomainEvent(
                this,
                this.IsApproved,
                admin
                ));
        }


        /// <summary>
        /// Called by domain event handler.
        /// </summary>
        /// <param name="balance"></param>
        public void BalanceUpdated(decimal balance)
        {
            //Update available balance.
            this.AvailableBalanceUpdated(balance);

            //If auto pairing by available balance is enabled, and the code is pairing, check it.
            if (this._pairingStatusId == PairingStatus.Pairing.Id)
            {
                if (this.QrCodeSettings.AutoPairngByAvailableBalance)
                {
                    if (this.AvailableBalance < this.QrCodeSettings.AvailableBalanceThreshold)
                    {
                        this.StopPairing(true, $"余额过低。 判断标准:{this.QrCodeSettings.AvailableBalanceThreshold}");
                    }
                }
            }

            //No need to create domain event since this is the end in domain event chain.
        }

        /// <summary>
        /// Called by domain event handler.
        /// </summary>
        /// <param name="rate"></param>
        /*public void ToppestCommissionRateUpdated(decimal rate)
        {
            this.PairingData = this.PairingData.ToppestCommissionRateUpdated(rate);
        }*/

        /// <summary>
        /// Called by domain event handler.
        /// </summary>
        /// <param name="shopSettings"></param>
        public void ShopOrderAmountOptionsUpdated(ShopSettings shopSettings)
        {
            //Check the qr code amount when this is manual barcode.
            if (!this.IsBarcodeAmountValid(shopSettings))
            {
                //The order has specified amount, and the shop doens't has the amount option matched.
                //So disable this qr code.
                this.Disable(true, "商户订单金额选项更新，二维码金额无效。");
            }
        }


        public void Enable(ShopSettings shopSettings = null)
        {
            //Checking this qr code is disable.
            if (this._qrCodeStatusId == QrCodeStatus.Enabled.Id)
            {
                throw new PairingDomainException("请勿重复启用." + ". At QrCode.Enable()");
            }

            //Checking the qrcode is approved
            if (!this.IsApproved)
            {
                throw new PairingDomainException("二维码通过审核后才能启用" + ". At QrCode.Enable()");
            }


            //If this qr code payment scheme is alipay, function will validate it.
            if (!this.IsBarcodeAmountValid(shopSettings))
            {
                throw new PairingDomainException("个码金额错误，请确认金额符合商户金额选项" + ". At QrCode.Enable()");
            }

            this._qrCodeStatusId = QrCodeStatus.Enabled.Id;

            this.AddDomainEvent(new QrCodeEnabledDomainEvent(
                this,
                this._qrCodeStatusId
                ));
        }

        public void Disable(bool isBySystem, string description = null)
        {
            //Checking the qr code is not processing order.
            if (this._pairingStatusId == PairingStatus.ProcessingOrder.Id)
            {
                //It is OK to stop pairing even there are orders in process,
                //because this won's affect the following process connected with oder.
                this.StopPairing(true, "由于二维码被停用，自动停止轮巡。");

                //throw new PairingDomainException("无法在处理订单时停用二维码" + ". At QrCode.Disable()");
            }
            else if (this._pairingStatusId == PairingStatus.Pairing.Id)
            {
                this.StopPairing(true, "由于二维码被停用，自动停止轮巡。");
            }
            else if (this._pairingStatusId == PairingStatus.DisableBySystem.Id)
            {
                //No need to do things.
            }
            else if (this._pairingStatusId == PairingStatus.NormalDisable.Id)
            {
                //No need to do things.
            }
            else
            {
                throw new PairingDomainException("无法辨识配对状态" + ". At QrCode.Disable()");
            }

            if (isBySystem)
            {
                this._qrCodeStatusId = QrCodeStatus.AutoDisabled.Id;
            }
            else
            {
                this._qrCodeStatusId = QrCodeStatus.Disabled.Id;
            }

            CodeStatusDiscription = description;

            this.AddDomainEvent(new QrCodeDisabledDomainEvent(
                this,
                this._qrCodeStatusId
                ));
        }


        /// <summary>
        /// Only call once when the qr code is offline.
        /// Called by domain event handler.
        /// </summary>
        /// <param name="cloudDevice"></param>
        public void Online(CloudDevice cloudDevice)
        {
            //Checking this qr code's type is auto.
            if (this._qrCodeTypeId != QrCodeType.Auto.Id)
            {
                throw new PairingDomainException("Can not go online when the code type is manual" + ". At QrCode.Online()");
            }

            this.PairingData = this.PairingData.Online(cloudDevice);

            //If the code is enabled,
            //and the pairing is not disabled by system, then start auto pairing.
            if (this._qrCodeStatusId == QrCodeStatus.Enabled.Id)
            {
                if (this._pairingStatusId != PairingStatus.DisableBySystem.Id)
                {
                    this.StartPairing();
                }
            }
        }

        /// <summary>
        /// Only call once when the qr code is online.
        /// Called by domain event handler.
        /// </summary>
        /// <param name="cloudDevice"></param>
        public void Offline(CloudDevice cloudDevice)
        {
            //Checking this qr code's type is auto.
            if (this._qrCodeTypeId != QrCodeType.Auto.Id)
            {
                throw new PairingDomainException("Can not go online when the code type is manual" + ". At QrCode.UpdateSuccessRateAndRelatedData()");
            }

            this.PairingData = this.PairingData.Offline(cloudDevice);

            //If the code is pairing, or processing order, disable it.
            if (this._pairingStatusId == PairingStatus.Pairing.Id
                || this._pairingStatusId == PairingStatus.ProcessingOrder.Id
                )
            {
                this.StopPairing(true, "云手机掉线");
            }
        }


        public void StartPairing()
        {
            //Checking this qr code status is enabled.
            if (this._qrCodeStatusId != QrCodeStatus.Enabled.Id)
            {
                throw new PairingDomainException("启用二维码前无法开启轮询配对" + ". At QrCode.StartPairing()");
            }

            //Checking this qr code is not processing order.
            if (this._pairingStatusId == PairingStatus.ProcessingOrder.Id)
            {
                throw new PairingDomainException("二维码处理订单中，请勿更改配对状态" + ". At QrCode.StartPairing()");
            }

            //Checking the qrcode is approved
            if (!this.IsApproved)
            {
                throw new PairingDomainException("二维码通过审核后才能开启配对轮巡" + ". At QrCode.StartPairing()");
            }

            //If this is auto mode, checking the cloud device is prepared.
            if (this._qrCodeTypeId == QrCodeType.Auto.Id)
            {
                if (this.CloudDeviceId == null)
                {
                    throw new PairingDomainException("设置云手机后才可进行自动轮巡" + ". At QrCode.StartPairing()");
                }

                //Checking the payment channel is alipay.
                if (this._paymentChannelId != PaymentChannel.Alipay.Id)
                {
                    throw new PairingDomainException("自动轮询目前仅支援支付宝" + ". At QrCode.StartPairing()");
                }
            }

            //Checking the code data is prepared.
            if (this._paymentSchemeId == PaymentScheme.Barcode.Id)
            {
                if (this.BarcodeDataForAuto is null)
                {
                    throw new PairingDomainException("资料未齐全，无法开启配对" + ". At QrCode.StartPairing()");
                }
            }
            else if (this._paymentSchemeId == PaymentScheme.Merchant.Id)
            {
                if (this.MerchantData is null)
                {
                    throw new PairingDomainException("资料未齐全，无法开启配对" + ". At QrCode.StartPairing()");
                }
            }
            else if (this._paymentSchemeId == PaymentScheme.Transaction.Id)
            {
                if (this.TransactionData is null)
                {
                    throw new PairingDomainException("资料未齐全，无法开启配对" + ". At QrCode.StartPairing()");
                }
            }
            else if (this._paymentSchemeId == PaymentScheme.Envelop.Id)
            {
                if (this.TransactionData is null)
                {
                    throw new PairingDomainException("资料未齐全，无法开启配对" + ". At QrCode.StartPairing()");
                }
            }
            else if (this._paymentSchemeId == PaymentScheme.EnvelopPassword.Id)
            {
                if (this.TransactionData is null)
                {
                    throw new PairingDomainException("资料未齐全，无法开启配对" + ". At QrCode.StartPairing()");
                }
            }
            else
            {
                throw new PairingDomainException("错误的支付方式。目前仅支援:个码、原生码、转账、红包和口令红包" + ". At QrCode.StartPairing()");
            }


            //Checking the pairing environment is valid.
            if (!this.PairingEnvironmentValidation(out string description))
            {
                //this.StopPairing(true, description);
                throw new PairingDomainException("无效的轮询环境:" + description);
            }

            this._pairingStatusId = PairingStatus.Pairing.Id;

            this.AddDomainEvent(new QrCodePairingStartedDomainEvent(
                this,
                this._pairingStatusId
                ));
        }

        public void StopPairing(bool isBySystem, string description = null)
        {
            //If already disabled, disabled again.
            if (this._pairingStatusId == PairingStatus.DisableBySystem.Id || this._pairingStatusId == PairingStatus.NormalDisable.Id)
            {
                if (isBySystem)
                {
                    this._pairingStatusId = PairingStatus.DisableBySystem.Id;
                    this.PairingStatusDescription = description;
                }
                else
                {
                    this._pairingStatusId = PairingStatus.NormalDisable.Id;
                    this.PairingStatusDescription = description;
                }
            }
            else
            {
                //Checking the qr code is pairing.
                /*if (this._pairingStatusId != PairingStatus.Pairing.Id)
                {
                    throw new PairingDomainException("只有配对中的二维码才可停止配对" + ". At QrCode.StopPairing()");
                }*/
                //No need to be pairing status, 
                //because stop pairing won't affect the follow-up process connected with oder.

                if (isBySystem)
                {
                    this._pairingStatusId = PairingStatus.DisableBySystem.Id;
                    this.PairingStatusDescription = description;
                }
                else
                {
                    this._pairingStatusId = PairingStatus.NormalDisable.Id;
                    this.PairingStatusDescription = description;
                }
            }

            this.AddDomainEvent(new QrCodePairingStoppedDomainEvent(
                this,
                this.PairingStatusDescription,
                this._pairingStatusId
                ));
        }

        public void PairWithOrder(Order order)
        {
            //Checking the status is pairing.
            if (this._pairingStatusId != PairingStatus.Pairing.Id)
            {
                throw new PairingDomainException("配对中的二维码才可接单" + ". At QrCode.PairWithOrder()");
            }

            //If this is auto qr code, checking the cloud device status is online.
            if (this._qrCodeTypeId == QrCodeType.Auto.Id)
            {
                if (!this.IsOnline)
                {
                    throw new PairingDomainException("云手机在线的二维码才可接单" + ". At QrCode.PairWithOrder()");
                }
            }

            //Checking the order is provided.
            if (order is null)
            {
                throw new PairingDomainException("错误的订单数据" + ". At QrCode.PairWithOrder()");
            }
            //Checking the payment chennel is matched.
            if (PaymentChannel.FromName(order.PaymentChannel).Id != this._paymentChannelId)
            {
                throw new PairingDomainException("错误的支付类型" + ". At QrCode.PairWithOrder()");
            }
            //Checking the payment chennel is matched.
            if (PaymentScheme.FromName(order.PaymentScheme).Id != this._paymentSchemeId)
            {
                throw new PairingDomainException("错误的支付通道" + ". At QrCode.PairWithOrder()");
            }
            //if this qr code has specified shop, check it.
            if (!string.IsNullOrEmpty(this.SpecifiedShopId))
            {
                if (order.ShopId != this.SpecifiedShopId)
                {
                    throw new PairingDomainException("此二维码不接受此商户订单" + ". At QrCode.PairWithOrder()");
                }
            }


            //Checking the rebate rate is greater than the min commission rate.
            if (order.RateRebate <= this.MinCommissionRate)
            {
                throw new PairingDomainException($"配对失败。此二维码的最低返佣需求为{this.MinCommissionRate}" + ". At QrCode.PairWithOrder()");
            }

            /*
            //Checking the order amount isn't out of limit range.
            if (this.Quota.HasExceededTheOrderAmountLimit(qrCodeOrder))
            {
                throw new PairingDomainException($"配对失败。此二维码的单笔订单金额限制为{this.Quota.OrderAmountLowerLimit}~{this.Quota.OrderAmountUpperLimit}" + ". At QrCode.PairWithOrder()");
            }

            //Checking the daily quota left is enough.
            if (qrCodeOrder.GetAmount() > this.PairingData.QuotaLeftToday)
            {
                throw new PairingDomainException($"配对失败。此二维码的每日成交金额限制为{this.Quota.DailyAmountLimit}，目前剩余{this.PairingData.QuotaLeftToday}" + ". At QrCode.PairWithOrder()");
            }

            //Checking the balance is enough.
            if (order.Amount > this.PairingData.AvailableBalance)
            {
                throw new PairingDomainException($"配对失败。订单金额超过二维码可用余额。目前可用余额{this.PairingData.AvailableBalance}" + ". At QrCode.PairWithOrder()");
            }


            //Add qr code order to lsit.
            //this._qrCodeOrders.Add(qrCodeOrder);
*/
            //Update pairing data.
            this.UpdateDateLastTraded(order.DateCreated);


            //Update pairing status.
            this._pairingStatusId = PairingStatus.ProcessingOrder.Id;

            //No need to update balance. the domain event handler will take care of it.

            this.AddDomainEvent(new QrCodePairedWithOrderDomainEvent(
                this,
                order
                ));
        }

        /// <summary>
        /// Called by domain event handler.
        /// </summary>
        /// <param name="trackingNumber"></param>
        public void OrderSuccess(string trackingNumber, decimal amount, DateTime dateCreated)
        {
            //Checking the status is processing order.
            if (this._pairingStatusId != PairingStatus.ProcessingOrder.Id)
            {
                throw new PairingDomainException("二维码处于错误状态" + ". At QrCode.OrderSuccess()");
            }

            //Update pairing data
            this.PairingData = this.PairingData.UpdateSuccessRateAndRelatedData(true, dateCreated);
            this.UpdateQuotaLeft(amount);


            //Update pairing status.
            if (!this.PairingEnvironmentValidation(out string description))
            {
                //The pairing environment isn't valid, stop pairing.
                this.StopPairing(true, description);

                return;
            }
            else
            {
                if (this._qrCodeTypeId == QrCodeType.Auto.Id)
                {
                    //If this is auto mode,and is online, back to paring status.

                    if (this.IsOnline)
                    {
                        this._pairingStatusId = PairingStatus.Pairing.Id;
                    }
                    else
                    {
                        this.StopPairing(true, "云手机掉线。");
                    }
                }
                else if (this._qrCodeTypeId == QrCodeType.Manual.Id)
                {
                    //Continue pairing after a success order.
                    this._pairingStatusId = PairingStatus.Pairing.Id;
                }
                else
                {
                    throw new PairingDomainException("无法辨认的二维码类型" + ". At QrCode.OrderSuccess()");
                }
            }
            //No need to update balance. the domain event handler will take care of it.
        }

        /// <summary>
        /// Called by domain event handler.
        /// </summary>
        /// <param name="order"></param>
        public void OrderFailed(Order order)
        {
            //Checking the status is processing order.
            if (this._pairingStatusId != PairingStatus.ProcessingOrder.Id)
            {
                throw new PairingDomainException("二维码处于错误状态" + ". At QrCode.OrderSuccess()");
            }

            //Checking the order is provided.
            if (order is null)
            {
                throw new PairingDomainException("错误的订单数据" + ". At QrCode.OrderSuccess()");
            }

            //Update pairing data
            this.PairingData = this.PairingData.UpdateSuccessRateAndRelatedData(false, order.DateCreated);
            this.UpdateQuotaLeft(-order.Amount);

            //Update pairing status.
            if (!this.PairingEnvironmentValidation(out string description))
            {
                //The pairing environment isn't valid, stop pairing.
                this.StopPairing(true, description);

                return;
            }
            else
            {
                if (this._qrCodeTypeId == QrCodeType.Auto.Id)
                {
                    //If this is auto mode,and is online, back to paring status.

                    if (this.IsOnline)
                    {
                        this._pairingStatusId = PairingStatus.Pairing.Id;
                    }
                    else
                    {
                        this.StopPairing(true, "云手机掉线。");
                    }
                }
                else if (this._qrCodeTypeId == QrCodeType.Manual.Id)
                {
                    //Continue pairing after a failed order.

                    //If the current consecutive failures is too high, 
                    //and the auto pairing is enabled,
                    //validation above will disable pairing.

                    this._pairingStatusId = PairingStatus.Pairing.Id;
                }
                else
                {
                    throw new PairingDomainException("无法辨认的二维码类型" + ". At QrCode.OrderSuccess()");
                }
            }

            //No need to update balance. the domain event handler will take care of it.
        }


        public void ResetQuotaLeftToday(Admin admin = null)
        {
            //Update pairing data.
            this.QuotaLeftToday = this.DailyAmountLimit;

            this.AddDomainEvent(new QrCodeQuotaLeftTodayResetDomainEvent(
                this,
                admin));
        }

        public void ResetCurrentConsecutiveFailures(Admin admin)
        {
            //Update pairing data.
            this.PairingData = this.PairingData.ResetCurrentConsecutiveFailures();

            this.AddDomainEvent(new QrCodeCurrentConsecutiveFailuresResetDomainEvent(
                this,
                admin));
        }

        public void ResetSuccessRateAndRelatedData(Admin admin)
        {
            //Update pairing data.
            this.PairingData = this.PairingData.ResetSuccessRateAndRelatedData();

            this.AddDomainEvent(new QrCodeSuccessRateAndRelatedDataResetDomainEvent(
                this,
                admin));
        }


        public string GeneratePaymentCommand(decimal amount)
        {
            string command = string.Empty;
            if (this._paymentChannelId == PaymentChannel.Alipay.Id)
            {
                if (this._paymentSchemeId == PaymentScheme.Barcode.Id)
                {
                    //Get barcode data and composit the link and command.
                    var baseCommand = @"alipays://platformapi/startapp?appId=60000012";
                    var urlParam = "&url=" + this.BarcodeDataForManual.QrCodeUrl;

                    command = baseCommand + urlParam;
                }
                else if (this._paymentSchemeId == PaymentScheme.Merchant.Id)
                {

                }
                else if (this._paymentSchemeId == PaymentScheme.Transaction.Id)
                {
                    var alipayUserId = this.TransactionData.UserId;
                    //var alipayUserId = "2088432193306889";
                    var baseCommand = @"alipays://platformapi/startapp?appId=20000123&actionType=scan";
                    var bizDataParam = @"&biz_data={""s"":""money"",""u"":""" + alipayUserId + @""",""a"":""" + amount + @""",""m"":""你好123456""}";

                    command = baseCommand + bizDataParam;
                    //command = @"alipays://platformapi/startapp?appId=09999988&actionType=toAccount&goBack=NO&amount=1&userId=2088732318934891&memo=你好123456";
                }
                else if (this._paymentSchemeId == PaymentScheme.Envelop.Id)
                {
                    var alipayUserId = this.TransactionData.UserId;
                    //var alipayUserId = "2088432193306889";
                    var baseCommand = @"alipays://platformapi/startapp?appId=88886666&appLaunchMode=3&canSearch=false&chatLoginId=1&chatUserName=123&chatUserType=1&entryMode=personalStage&prevBiz=chat&schemaMode=portalInside&target=personal";
                    var chatUserIdParam = "&chatUserId=" + alipayUserId;
                    var moneyParam = "&money=" + amount;
                    var amountParam = "&amount=" + amount;
                    var remarkParam = "&remark=" + "你好123456";

                    command = baseCommand + chatUserIdParam + moneyParam + amountParam + remarkParam;
                }
                else if (this._paymentSchemeId == PaymentScheme.EnvelopPassword.Id)
                {
                }
                else
                {
                    throw new PairingDomainException("无法辨识二维码支付通道" + ". At QrCode.GeneratePaymentCommand()");
                }
            }
            else if (this._paymentChannelId == PaymentChannel.Wechat.Id)
            {
            }
            else
            {
                throw new PairingDomainException("无法辨识二维码支付类型" + ". At QrCode.GeneratePaymentCommand()");
            }

            return command;
        }


        private bool PairingEnvironmentValidation(out string description)
        {
            var isSuccess = true;
            description = "";

            //If the qr code is disabled, return false.
            if (this._qrCodeStatusId == QrCodeStatus.Disabled.Id
                || this._qrCodeStatusId == QrCodeStatus.AutoDisabled.Id)
            {
                isSuccess = false;
                description += "二维码尚未启用";
            }

            // If the qr code's pairing is stopped by system, return false.
            if (this._pairingStatusId == PairingStatus.DisableBySystem.Id)
            {
                isSuccess = false;
                description += "二维码轮巡达到今日限制，须由管理员清除状态后才可继续轮巡配对";
            }

            //If auto pairing by success rate is enabled, check it.
            if (this.QrCodeSettings.AutoPairingBySuccessRate)
            {
                if (this.PairingData.SuccessRate < this.QrCodeSettings.SuccessRateThreshold)
                {
                    if (this.PairingData.TotalFailures + this.PairingData.TotalSuccess >= this.QrCodeSettings.SuccessRateMinOrders)
                    {
                        isSuccess = false;
                        description += $"成功率过低。判断标准:{this.QrCodeSettings.SuccessRateThreshold * 100}%。最低判断笔数:{this.QrCodeSettings.SuccessRateMinOrders}  \n";
                    }
                }
            }

            //If auto pairing by quota left is enabled, check it.
            if (this.QrCodeSettings.AutoPairingByQuotaLeft)
            {
                if (this.QuotaLeftToday < this.QrCodeSettings.QuotaLeftThreshold)
                {
                    isSuccess = false;
                    description += $"剩余每日额度过低。判断标准:{this.QrCodeSettings.QuotaLeftThreshold} \n";
                }
            }

            //If auto pairing by current consecutive failures is enabled, check it.
            if (this.QrCodeSettings.AutoPairingByCurrentConsecutiveFailures)
            {
                if (this.PairingData.CurrentConsecutiveFailures >= this.QrCodeSettings.CurrentConsecutiveFailuresThreshold)
                {
                    isSuccess = false;
                    description += $"连续失败次数过高。判断标准:{this.QrCodeSettings.QuotaLeftThreshold} \n";
                }
            }

            return isSuccess;
        }

        private bool IsBarcodeAmountValid(ShopSettings shopSettings = null)
        {
            //Check the qr code amount when this is manual barcode.
            if (this._qrCodeTypeId == QrCodeType.Manual.Id && this._paymentSchemeId == PaymentScheme.Barcode.Id)
            {
                //If qr code has specified shop, check the shop settings is provided.
                if (!string.IsNullOrEmpty(this.SpecifiedShopId))
                {
                    if (shopSettings is null)
                    {
                        throw new PairingDomainException("The shop settings must be provided" + ". At QrCode.IsBarcodeAmountValid()");
                    }
                }
                else
                {
                    //The qr code has no specified shop, so no need to validate.
                    return true;
                }
                if (shopSettings.OrderAmountOptions.Any())
                {
                    var forAmount = this.BarcodeDataForManual.Amount;
                    if (forAmount != null)
                    {
                        if (!shopSettings.OrderAmountOptions.Any(o => o.GetAmount() == forAmount))
                        {
                            //The order has specified amount, and the shop doens't has the amount option matched.
                            //So return false.
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void UpdateSuccessRateAndRelatedData(QrCodeOrder qrCodeOrder)
        {
            //Check the pairing is disabled.
            if (this._pairingStatusId != PairingStatus.DisableBySystem.Id
                && this._pairingStatusId != PairingStatus.NormalDisable.Id)
            {
                throw new PairingDomainException("Can not udpate success rate data when the qr code is pairing or processing order." + ". At QrCode.UpdateSuccessRateAndRelatedData()");
            }

            this.PairingData = this.PairingData.UpdateSuccessRateAndRelatedData(qrCodeOrder);
        }

        private bool IsDisable()
        {
            if (this._qrCodeStatusId != QrCodeStatus.Disabled.Id && this._qrCodeStatusId != QrCodeStatus.AutoDisabled.Id)
            {
                return false;
            }
            return true;
        }


        #region Pairing Properties Functions
        private void ToppestCommissionRateUpdated(decimal toppestCommissionRate)
        {
            PairingData.ValidateCommissionRate(toppestCommissionRate);

            //The pairing require at least 0.001 more to earn the money.
            var minCommissionRate = toppestCommissionRate + 0.001M;

            this.MinCommissionRate = minCommissionRate;

        }

        private void AvailableBalanceUpdated(decimal balance)
        {
            PairingData.ValidateBalance(balance);

            this.AvailableBalance = balance;
        }


        private void UpdateQuotaLeft(decimal newOrderAmount)
        {
            //Calculate total success trade amount.
            decimal totalSuccessAmount = this.DailyAmountLimit - this.QuotaLeftToday;

            //If total success amount is negative,
            //that means the quota left is higher than the daily amount limit,
            //the reason is not found for now ,
            //temp fix is to set the quota left today to daily amount limit.
            if (totalSuccessAmount < 0)
            {
                this.QuotaLeftToday = this.DailyAmountLimit;
                return;
            }  

            totalSuccessAmount += newOrderAmount;

            //Calculate quota left.
            var quotaLeft = this.DailyAmountLimit - totalSuccessAmount;

            //If quota left is less than 0, then set it to 0.
            if (quotaLeft < 0)
            {
                quotaLeft = 0;
            }

            this.QuotaLeftToday = quotaLeft;
        }


        private void UpdateDateLastTraded(DateTime dateTime)
        {
            this.DateLastTraded = dateTime;
        }

        #endregion
    }
}
