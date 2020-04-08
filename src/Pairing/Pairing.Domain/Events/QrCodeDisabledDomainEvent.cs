using MediatR;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class QrCodeDisabledDomainEvent : INotification
    {
        public QrCodeDisabledDomainEvent(QrCode qrCode, int qrCodeStatusId)
        {
            QrCode = qrCode;
            QrCodeStatusId = qrCodeStatusId;
        }

        public QrCode QrCode { get; set; }
        public int QrCodeStatusId { get; set; }
    }
}
