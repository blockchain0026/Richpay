using MediatR;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class QrCodeMerchantDataUpdatedDomainEvent : INotification
    {
        public QrCodeMerchantDataUpdatedDomainEvent(QrCode qrCode, MerchantData merchantData)
        {
            QrCode = qrCode ?? throw new ArgumentNullException(nameof(qrCode));
            MerchantData = merchantData ?? throw new ArgumentNullException(nameof(merchantData));
        }

        public QrCode QrCode { get; set; }
        public MerchantData MerchantData { get; set; }
    }
}
