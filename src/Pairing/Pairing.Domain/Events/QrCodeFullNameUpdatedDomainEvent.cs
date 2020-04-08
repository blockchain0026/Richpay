using MediatR;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class QrCodeFullNameUpdatedDomainEvent : INotification
    {
        public QrCodeFullNameUpdatedDomainEvent(QrCode qrCode, string fullName)
        {
            QrCode = qrCode;
            FullName = fullName;
        }

        public QrCode QrCode { get; set; }
        public string FullName { get; set; }
    }
}
