using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC
{
    public class SystemConfigurations
    {
        public SiteBaseInfo SiteBaseInfo { get; set; }
        public Payment Payment { get; set; }
        public SystemNotificationSound SystemNotificationSound { get; set; }
        public PaymentChannel PaymentChannel { get; set; }
        public QrCodeConf QrCodeConf { get; set; }
    }


    public class SiteBaseInfo
    {
        public string SiteTitle { get; set; }
        public string ContactAddress { get; set; }
        public string ContactEmail { get; set; }
        public string ConsumerHotline { get; set; }
        public string ConsumerServiceQQ { get; set; }
        public string ConsumerServiceSkype { get; set; }
        public string BusinessHoursFrom { get; set; }
        public string BusinessHoursTo { get; set; }
    }

    public class Payment
    {
        public int PaymentTimeoutInSecond { get; set; }
        public int SellRequestCountdownInSecond { get; set; }
        public AlipayPaymentTemplate AlipayPaymentTemplate { get; set; }
    }

    public class SystemNotificationSound
    {
        public bool Withdraw { get; set; }
        public bool Deposit { get; set; }
        public bool Member { get; set; }
        public bool QrCode { get; set; }
        public bool NewOrder { get; set; }
        public bool OrderTimeout { get; set; }
    }

    public class UserNotification
    {
        public bool OrderTimeout { get; set; }
        public bool SuccessDeposit { get; set; }
    }

    public class WithdrawalAndDeposit
    {
        public WithdrawalTemplate WithdrawalTemplate { get; set; }
        public string WithdrawalOpenFrom { get; set; }
        public string WithdrawalOpenTo { get; set; }
        public string DepositOpenFrom { get; set; }
        public string DepositOpenTo { get; set; }
    }

    public class PaymentChannel
    {
        public string OpenFrom { get; set; }
        public string OpenTo { get; set; }
        public bool AutoToggle { get; set; }
    }
    
    public class QrCodeConf
    {
        public RiskControlTemplate RiskControlTemplate { get; set; }
    }

    #region inner class
    public class WithdrawalTemplate
    {
        public int EachAmountUpperLimit { get; set; }
        public int EachAmountLowerLimit { get; set; }
        public int DailyAmountLimit { get; set; }
        public int DailyFrequencyLimit { get; set; }
        public int CommissionInThousandth { get; set; }
    }


    public class OpenTimeFormat { 

    }


    public class RiskControlTemplate
    {
        public bool AutoPairingBySuccessRate { get; set; }
        public bool AutoPairingByQuotaLeft { get; set; }
        public bool AutoPairingByBusinessHours { get; set; }
        public bool AutoPairingByCurrentConsecutiveFailures { get; set; }
        public bool AutoPairngByAvailableBalance { get; set; }
        public int SuccessRateThresholdInHundredth { get; set; }
        public int SuccessRateMinOrders { get; set; }
        public int QuotaLeftThreshold { get; set; }
        public int CurrentConsecutiveFailuresThreshold { get; set; }
        public int AvailableBalanceThreshold { get; set; }

        public int DailyAmountLimit { get; set; }
        public int OrderAmountUpperLimit { get; set; }
        public int OrderAmountLowerLimit { get; set; }
    }
    
    public class AlipayPaymentTemplate
    {
        public int SecondsBeforePayment { get; set; }
        public bool IsAmountUnchangeable { get; set; }
        public bool IsAccountUnchangeable { get; set; }
        public bool IsH5RedirectByScanEnabled { get; set; }
        public bool IsH5RedirectByClickEnabled { get; set; }
        public bool IsH5RedirectByPickingPhotoEnabled { get; set; }
    }

    #endregion
}
