using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Applications.DomainServices.PairingDomain
{
    public class FakeQrCodeSettingService : IQrCodeSettingService
    {
        public bool AutoPairingBySuccessRate { get; set; }
        public bool AutoPairingByQuotaLeft { get; set; }
        public bool AutoPairingByBusinessHours { get; set; }
        public bool AutoPairingByCurrentConsecutiveFailures { get; set; }
        public bool AutoPairngByAvailableBalance { get; set; }
        public decimal SuccessRateThreshold { get; set; }
        public int SuccessRateMinOrders { get; set; }
        public decimal QuotaLeftThreshold { get; set; }
        public int CurrentConsecutiveFailuresThreshold { get; set; }
        public decimal AvailableBalanceThreshold { get; set; }

        public QrCodeSettings GetDefaultSettings()
        {
            return new QrCodeSettings(
                AutoPairingBySuccessRate,
                AutoPairingByQuotaLeft,
                AutoPairingByBusinessHours,
                AutoPairingByCurrentConsecutiveFailures,
                AutoPairngByAvailableBalance,
                SuccessRateThreshold,
                SuccessRateMinOrders,
                QuotaLeftThreshold,
                CurrentConsecutiveFailuresThreshold,
                AvailableBalanceThreshold
                );
        }
    }
}
