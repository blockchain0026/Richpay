using Pairing.Domain.Exceptions;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Model.QrCodes
{
    public class QrCodeSettings : ValueObject
    {
        public bool AutoPairingBySuccessRate { get; private set; }
        public bool AutoPairingByQuotaLeft { get; private set; }
        public bool AutoPairingByBusinessHours { get; private set; }
        public bool AutoPairingByCurrentConsecutiveFailures { get; private set; }
        public bool AutoPairngByAvailableBalance { get; private set; }
        public decimal SuccessRateThreshold { get; private set; }
        public int SuccessRateMinOrders { get; private set; }
        public decimal QuotaLeftThreshold { get; private set; }
        public int CurrentConsecutiveFailuresThreshold { get; private set; }
        public decimal AvailableBalanceThreshold { get; private set; }

        public QrCodeSettings(bool autoPairingBySuccessRate, bool autoPairingByQuotaLeft, bool autoPairingByBusinessHours, bool autoPairingByCurrentConsecutiveFailures, bool autoPairngByAvailableBalance,
            decimal successRateThreshold, int successRateMinOrders, decimal quotaLeftThreshold, int currentConsecutiveFailuresThreshold, decimal availableBalanceThreshold)
        {
            AutoPairingBySuccessRate = autoPairingBySuccessRate;
            AutoPairingByQuotaLeft = autoPairingByQuotaLeft;
            AutoPairingByBusinessHours = autoPairingByBusinessHours;
            AutoPairingByCurrentConsecutiveFailures = autoPairingByCurrentConsecutiveFailures;
            AutoPairngByAvailableBalance = autoPairngByAvailableBalance;

            //Checking the success rate threshold is less than 1 and larger than or equal to 0.
            if (successRateThreshold < 0.01M || successRateThreshold >= 1)
            {
                throw new PairingDomainException("成功率阈值必须在0.01~1之间" + ". At QrCodeSettings()");
            }
            //Checking the success rate threshold has only two points.
            if (decimal.Round(successRateThreshold, 2) != successRateThreshold)
            {
                throw new PairingDomainException("成功率阈值的浮点数必须小于或等于2位" + ". At QrCodeSettings()");
            }
            SuccessRateThreshold = successRateThreshold;


            //Checking the success rate min orders is larger than or equal to 1.
            if (successRateMinOrders < 1)
            {
                throw new PairingDomainException("最小判断笔数至少须为1" + ". At QrCodeSettings()");
            }
            SuccessRateMinOrders = successRateMinOrders;


            //Checking the quota rate threshold is larger than or equal to 0.
            if (quotaLeftThreshold < 1)
            {
                throw new PairingDomainException("剩余额度阈值必须大于或等于1" + ". At QrCodeSettings()");
            }
            //Checking the quota rate threshold has only 3 points.
            if (decimal.Round(quotaLeftThreshold, 3) != quotaLeftThreshold)
            {
                throw new PairingDomainException("剩余额度阈值的浮点数必须小于或等于3位" + ". At QrCodeSettings()");
            }
            QuotaLeftThreshold = quotaLeftThreshold;

            //Checking the current consecutive failures threshold is larger than or equal to 1.
            if (currentConsecutiveFailuresThreshold < 1)
            {
                throw new PairingDomainException("最小连续失败阈值至少须为1" + ". At QrCodeSettings()");
            }
            CurrentConsecutiveFailuresThreshold = currentConsecutiveFailuresThreshold;


            //Checking the available balance threshold is larger than or equal to 0.
            if (availableBalanceThreshold < 0)
            {
                throw new PairingDomainException("可用余额阈值必须大于或等于1" + ". At QrCodeSettings()");
            }
            //Checking the quota rate threshold has only 3 points.
            if (decimal.Round(availableBalanceThreshold, 3) != availableBalanceThreshold)
            {
                throw new PairingDomainException("可用余额阈值的浮点数必须小于或等于3位" + ". At QrCodeSettings()");
            }
            AvailableBalanceThreshold = availableBalanceThreshold;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return AutoPairingBySuccessRate;
            yield return AutoPairingByQuotaLeft;
            yield return AutoPairingByBusinessHours;
            yield return AutoPairingByCurrentConsecutiveFailures;
            yield return AutoPairngByAvailableBalance;

            yield return SuccessRateThreshold;
            yield return SuccessRateMinOrders;
            yield return QuotaLeftThreshold;
            yield return CurrentConsecutiveFailuresThreshold;
            yield return AvailableBalanceThreshold;
        }
    }
}