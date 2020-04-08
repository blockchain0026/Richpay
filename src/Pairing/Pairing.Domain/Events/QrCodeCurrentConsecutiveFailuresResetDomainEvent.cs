using MediatR;
using Pairing.Domain.Model.QrCodes;
using Pairing.Domain.Model.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class QrCodeCurrentConsecutiveFailuresResetDomainEvent : INotification
    {
        public QrCodeCurrentConsecutiveFailuresResetDomainEvent(QrCode qrCode, Admin byAdmin)
        {
            QrCode = qrCode;
            ByAdmin = byAdmin;
        }

        public QrCode QrCode { get; set; }
        public Admin ByAdmin { get; set; }
    }
}
