using MediatR;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class QrCodePairingStoppedDomainEvent : INotification
    {
        public QrCodePairingStoppedDomainEvent(QrCode qrCode, string pairingStatusDescription, int pairingStatusId)
        {
            QrCode = qrCode;
            PairingStatusDescription = pairingStatusDescription;
            PairingStatusId = pairingStatusId;
        }

        public QrCode QrCode { get; set; }
        public string PairingStatusDescription { get; set; }
        public int PairingStatusId { get; set; }
    }
}
