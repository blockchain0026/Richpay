using MediatR;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class QrCodeSettingsUpdatedDomainEvent : INotification
    {
        public QrCodeSettingsUpdatedDomainEvent(QrCode qrCode, QrCodeSettings qrCodeSettings)
        {
            QrCode = qrCode;
            QrCodeSettings = qrCodeSettings;
        }

        public QrCode QrCode { get; set; }
        public QrCodeSettings QrCodeSettings { get; set; }
    }
}
