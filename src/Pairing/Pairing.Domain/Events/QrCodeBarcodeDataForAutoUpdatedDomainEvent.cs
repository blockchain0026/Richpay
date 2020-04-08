using MediatR;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class QrCodeBarcodeDataForAutoUpdatedDomainEvent : INotification
    {
        public QrCodeBarcodeDataForAutoUpdatedDomainEvent(QrCode qrCode, BarcodeDataForAuto barcodeDataForAuto)
        {
            QrCode = qrCode ?? throw new ArgumentNullException(nameof(qrCode));
            BarcodeDataForAuto = barcodeDataForAuto ?? throw new ArgumentNullException(nameof(barcodeDataForAuto));
        }

        public QrCode QrCode { get; set; }
        public BarcodeDataForAuto BarcodeDataForAuto { get; set; }
    }
}
