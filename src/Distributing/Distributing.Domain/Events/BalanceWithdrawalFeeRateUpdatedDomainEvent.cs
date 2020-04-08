using Distributing.Domain.Model.Balances;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class BalanceWithdrawalFeeRateUpdatedDomainEvent : INotification
    {
        public BalanceWithdrawalFeeRateUpdatedDomainEvent(Balance balance, decimal withdrawalCommissionRate)
        {
            Balance = balance;
            WithdrawalCommissionRate = withdrawalCommissionRate;
        }

        public Balance Balance { get; }
        public decimal WithdrawalCommissionRate { get; }
    }
}
