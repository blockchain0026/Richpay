using MediatR;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class QrCodeTransactionDataUpdatedDomainEvent : INotification
    {
        public QrCodeTransactionDataUpdatedDomainEvent(QrCode qrCode, TransactionData transactionData)
        {
            QrCode = qrCode ?? throw new ArgumentNullException(nameof(qrCode));
            TransactionData = transactionData ?? throw new ArgumentNullException(nameof(transactionData));
        }

        public QrCode QrCode { get; set; }
        public TransactionData TransactionData { get; set; }
    }
}
