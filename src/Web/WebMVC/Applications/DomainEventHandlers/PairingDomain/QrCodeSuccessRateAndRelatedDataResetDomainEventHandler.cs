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
    public class QrCodeSuccessRateAndRelatedDataResetDomainEventHandler
            : INotificationHandler<QrCodeSuccessRateAndRelatedDataResetDomainEvent>
    {
        private readonly IQrCodeQueries _qrCodeQueries;

        public QrCodeSuccessRateAndRelatedDataResetDomainEventHandler(IQrCodeQueries qrCodeQueries)
        {
            _qrCodeQueries = qrCodeQueries ?? throw new ArgumentNullException(nameof(qrCodeQueries));
        }

        public async Task Handle(QrCodeSuccessRateAndRelatedDataResetDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            //Update Qr code entry.
            var qrCode = domainEvent.QrCode;
            var qrCodeVM = await _qrCodeQueries.GetQrCodeEntryAsync(qrCode.Id);

            if (qrCodeVM != null)
            {
                qrCodeVM.PairingInfo.TotalSuccess = qrCode.PairingData.TotalSuccess;
                qrCodeVM.PairingInfo.TotalFailures = qrCode.PairingData.TotalFailures;
                qrCodeVM.PairingInfo.HighestConsecutiveSuccess = qrCode.PairingData.HighestConsecutiveSuccess;
                qrCodeVM.PairingInfo.HighestConsecutiveFailures = qrCode.PairingData.HighestConsecutiveFailures;
                qrCodeVM.PairingInfo.CurrentConsecutiveSuccess = qrCode.PairingData.CurrentConsecutiveSuccess;
                qrCodeVM.PairingInfo.CurrentConsecutiveFailures = qrCode.PairingData.CurrentConsecutiveFailures;
                qrCodeVM.PairingInfo.SuccessRateInPercent = (int)(qrCode.PairingData.SuccessRate * 100);

                _qrCodeQueries.Update(qrCodeVM);

                await _qrCodeQueries.SaveChangesAsync();
            }
        }
    }
}
