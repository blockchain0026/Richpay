using MediatR;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class QrCodeQuotaUpdatedDomainEvent : INotification
    {
        public QrCodeQuotaUpdatedDomainEvent(QrCode qrCode, decimal dailyAmountLimit, decimal orderAmountUpperLimit, decimal orderAmountLowerLimit)
        {
            QrCode = qrCode;
            DailyAmountLimit = dailyAmountLimit;
            OrderAmountUpperLimit = orderAmountUpperLimit;
            OrderAmountLowerLimit = orderAmountLowerLimit;
        }

        public QrCode QrCode { get; set; }


        public decimal DailyAmountLimit { get; set; }
        public decimal OrderAmountUpperLimit { get; set; }
        public decimal OrderAmountLowerLimit { get; set; }

        //public Quota Quota { get; set; }
    }
}
