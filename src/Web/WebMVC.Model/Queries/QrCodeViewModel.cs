using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Models.Queries
{
    public class QrCodeEntry
    {
        //Base Info
        public int QrCodeId { get; set; }
        public string FullName { get; set; }

        public string UserId { get; set; }
        public string Username { get; set; }
        public string UserFullName { get; set; }

        public string UplineUserId { get; set; }
        public string UplineUserName { get; set; }
        public string UplineFullName { get; set; }

        public int? CloudDeviceId { get; set; }

        public string QrCodeType { get; set; }

        public string PaymentChannel { get; set; }
        public string PaymentScheme { get; set; }

        public bool IsApproved { get; set; }

        public string ApprovedByAdminId { get; set; }
        public string ApprovedByAdminName { get; set; }

        public string QrCodeStatus { get; set; }
        public string PairingStatus { get; set; }

        public string CodeStatusDescription { get; set; }
        public string PairingStatusDescription { get; set; }

        public string DateCreated { get; set; }


        //配置
        public QrCodeEntrySetting QrCodeEntrySetting { get; set; }

        public int DailyAmountLimit { get; set; }
        public int OrderAmountUpperLimit { get; set; }
        public int OrderAmountLowerLimit { get; set; }



        public bool IsOnline { get; set; }//
        public int MinCommissionRateInThousandth { get; set; }
        public decimal AvailableBalance { get; set; }
        public string SpecifiedShopId { get; set; }//
        public string SpecifiedShopUsername { get; set; }//
        public string SpecifiedShopFullName { get; set; }//
        public decimal QuotaLeftToday { get; set; }//
        public string DateLastTraded { get; set; }//

        public PairingInfo PairingInfo { get; set; }

    }

    public class QrCodeEntrySetting
    {
        public bool AutoPairingBySuccessRate { get; set; }
        public bool AutoPairingByQuotaLeft { get; set; }
        public bool AutoPairingByBusinessHours { get; set; }
        public bool AutoPairingByCurrentConsecutiveFailures { get; set; }
        public bool AutoPairngByAvailableBalance { get; set; }
        public int SuccessRateThresholdInHundredth { get; set; }
        public int SuccessRateMinOrders { get; set; }
        public decimal QuotaLeftThreshold { get; set; }
        public int CurrentConsecutiveFailuresThreshold { get; set; }
        public decimal AvailableBalanceThreshold { get; set; }
    }

    public class PairingInfo
    {
        public int TotalSuccess { get; set; }
        public int TotalFailures { get; set; }
        public int HighestConsecutiveSuccess { get; set; }
        public int HighestConsecutiveFailures { get; set; }
        public int CurrentConsecutiveSuccess { get; set; }
        public int CurrentConsecutiveFailures { get; set; }//
        public int SuccessRateInPercent { get; set; }//
    }


    public class BarcodeInfoForManual
    {
        public int QrCodeId { get; set; }
        public string UserId { get; set; }

        public string QrCodeUrl { get; set; }
        public decimal? Amount { get; set; }
    }

    public class BarcodeInfoForAuto
    {
        public int QrCodeId { get; set; }
        public string UserId { get; set; }

        public string CloudDeviceUsername { get; set; }
        public string CloudDevicePassword { get; set; }
        public string CloudDeviceNumber { get; set; }
    }

    public class MerchantInfo
    {
        public int QrCodeId { get; set; }
        public string UserId { get; set; }

        public string AppId { get; set; }
        public string AlipayPublicKey { get; set; }
        public string WechatApiCertificate { get; set; }
        public string PrivateKey { get; set; }
        public string MerchantId { get; set; }
    }

    public class TransactionInfo
    {
        public int QrCodeId { get; set; }
        public string UserId { get; set; }
        public string AlipayUserId { get; set; }
    }

    public class BankInfo
    {
        public int QrCodeId { get; set; }
        public string UserId { get; set; }

        public string BankName { get; set; }
        public string BankMark { get; set; }
        public string AccountName { get; set; }
        public string CardNumber { get; set; }
    }


    public static class PaymentChannelOption
    {
        public static class Alipay
        {
            public const string Title = "支付宝";
            public const string Value = "Alipay";
        }
        public static class Wechat
        {
            public const string Title = "微信";
            public const string Value = "Wechat";
        }
    }

    public static class QrCodeTypeOption
    {
        public static class Auto
        {
            public const string Title = "自动回调";
            public const string Value = "Auto";
        }
        public static class Manual
        {
            public const string Title = "手动回调";
            public const string Value = "Manual";
        }
    }
}
