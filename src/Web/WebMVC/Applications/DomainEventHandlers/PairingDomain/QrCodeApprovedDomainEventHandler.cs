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
    public class QrCodeApprovedDomainEventHandler
            : INotificationHandler<QrCodeApprovedDomainEvent>
    {
        private readonly IQrCodeQueries _qrCodeQueries;

        public QrCodeApprovedDomainEventHandler(IQrCodeQueries qrCodeQueries)
        {
            _qrCodeQueries = qrCodeQueries ?? throw new ArgumentNullException(nameof(qrCodeQueries));
        }

        public async Task Handle(QrCodeApprovedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            //Update Qr code entry.
            var qrCode = domainEvent.QrCode;
            var qrCodeVM = await _qrCodeQueries.GetQrCodeEntryAsync(qrCode.Id);

            if (qrCodeVM != null)
            {
                qrCodeVM.IsApproved = domainEvent.QrCode.IsApproved;
                qrCodeVM.ApprovedByAdminId = domainEvent.Admin.AdminId;
                qrCodeVM.ApprovedByAdminName = domainEvent.Admin.Name;

                _qrCodeQueries.Update(qrCodeVM);

                await _qrCodeQueries.SaveChangesAsync();
            }
        }
    }
}
