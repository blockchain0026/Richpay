using Distributing.Domain.Model.Balances;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class BalanceDepositFeeRateUpdatedDomainEvent : INotification
    {
        public BalanceDepositFeeRateUpdatedDomainEvent(Balance balance, decimal depositCommissionRate)
        {
            Balance = balance;
            DepositCommissionRate = depositCommissionRate;
        }

        public Balance Balance { get; }
        public decimal DepositCommissionRate { get; }
    }
}
