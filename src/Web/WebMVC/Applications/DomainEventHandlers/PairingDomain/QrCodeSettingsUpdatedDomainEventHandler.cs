using MediatR;
using Pairing.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.QrCodes;

namespace WebMVC.Applications.DomainEventHandlers.PairingDomain
{
    /// <summary>
    /// Update view model: update QR code entry.
    /// </summary>
    public class QrCodeSettingsUpdatedDomainEventHandler
            : INotificationHandler<QrCodeSettingsUpdatedDomainEvent>
    {
        private readonly IQrCodeQueries _qrCodeQueries;

        public QrCodeSettingsUpdatedDomainEventHandler(IQrCodeQueries qrCodeQueries)
        {
            _qrCodeQueries = qrCodeQueries ?? throw new ArgumentNullException(nameof(qrCodeQueries));
        }

        public async Task Handle(QrCodeSettingsUpdatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            //Update Qr code entry.
            var qrCode = domainEvent.QrCode;
            var qrCodeVM = await _qrCodeQueries.GetQrCodeEntryAsync(qrCode.Id);

            if (qrCodeVM != null)
            {
                qrCodeVM.QrCodeEntrySetting.AutoPairingBySuccessRate = qrCode.QrCodeSettings.AutoPairingBySuccessRate;
                qrCodeVM.QrCodeEntrySetting.AutoPairingByQuotaLeft = qrCode.QrCodeSettings.AutoPairingByQuotaLeft;
                qrCodeVM.QrCodeEntrySetting.AutoPairingByBusinessHours = qrCode.QrCodeSettings.AutoPairingByBusinessHours;
                qrCodeVM.QrCodeEntrySetting.AutoPairingByCurrentConsecutiveFailures = qrCode.QrCodeSettings.AutoPairingByCurrentConsecutiveFailures;
                qrCodeVM.QrCodeEntrySetting.AutoPairngByAvailableBalance = qrCode.QrCodeSettings.AutoPairngByAvailableBalance;
                qrCodeVM.QrCodeEntrySetting.SuccessRateThresholdInHundredth = (int)(qrCode.QrCodeSettings.SuccessRateThreshold * 100);
                qrCodeVM.QrCodeEntrySetting.SuccessRateMinOrders = qrCode.QrCodeSettings.SuccessRateMinOrders;
                qrCodeVM.QrCodeEntrySetting.QuotaLeftThreshold = qrCode.QrCodeSettings.QuotaLeftThreshold;
                qrCodeVM.QrCodeEntrySetting.CurrentConsecutiveFailuresThreshold = qrCode.QrCodeSettings.CurrentConsecutiveFailuresThreshold;
                qrCodeVM.QrCodeEntrySetting.AvailableBalanceThreshold = qrCode.QrCodeSettings.AvailableBalanceThreshold;

                _qrCodeQueries.Update(qrCodeVM);

                await _qrCodeQueries.SaveChangesAsync();
            }
        }
    }
}
