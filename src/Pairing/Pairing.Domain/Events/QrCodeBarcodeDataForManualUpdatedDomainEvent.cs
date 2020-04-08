using MediatR;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class QrCodeBarcodeDataForManualUpdatedDomainEvent : INotification
    {
        public QrCodeBarcodeDataForManualUpdatedDomainEvent(QrCode qrCode, BarcodeDataForManual barcodeDataForManual)
        {
            QrCode = qrCode ?? throw new ArgumentNullException(nameof(qrCode));
            BarcodeDataForManual = barcodeDataForManual ?? throw new ArgumentNullException(nameof(barcodeDataForManual));
        }

        public QrCode QrCode { get; set; }
        public BarcodeDataForManual BarcodeDataForManual { get; set; }
    }
}
