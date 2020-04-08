using MediatR;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class QrCodePairedWithOrderDomainEvent : INotification
    {
        public QrCodePairedWithOrderDomainEvent(QrCode qrCode, Order order)
        {
            QrCode = qrCode;
            Order = order;
        }

        public QrCode QrCode { get; set; }
        public Order Order { get; set; }
    }
}
