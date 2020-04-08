using MediatR;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{

    public class QrCodeBankDataUpdatedDomainEvent : INotification
    {
        public QrCodeBankDataUpdatedDomainEvent(QrCode qrCode, BankData bankData)
        {
            QrCode = qrCode;
            BankData = bankData;
        }
        public QrCode QrCode { get; set; }
        public BankData BankData { get; set; }
    }
}
