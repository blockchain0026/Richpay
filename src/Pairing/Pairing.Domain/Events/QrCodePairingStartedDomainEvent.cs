using MediatR;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class QrCodePairingStartedDomainEvent : INotification
    {
        public QrCodePairingStartedDomainEvent(QrCode qrCode, int pairingStatusId)
        {
            QrCode = qrCode;
            PairingStatusId = pairingStatusId;
        }

        public QrCode QrCode { get; set; }
        public int PairingStatusId { get; set; }
    }
}
