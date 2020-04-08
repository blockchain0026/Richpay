using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Distributions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class BalanceDistributedDomainEvent : INotification
    {
        public BalanceDistributedDomainEvent(Balance balance, Distribution distribution,int qrCodeId)
        {
            Balance = balance;
            Distribution = distribution;
            QrCodeId = qrCodeId;
        }

        public Balance Balance { get; }
        public Distribution Distribution { get; }
        public int QrCodeId { get; }
    }
}
