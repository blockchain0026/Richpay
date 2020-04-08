using Distributing.Domain.Model.Balances;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class BalanceWithdrawalLimitUpdatedDomainEvent : INotification
    {
        public BalanceWithdrawalLimitUpdatedDomainEvent(Balance balance, WithdrawalLimit withdrawalLimit)
        {
            Balance = balance;
            WithdrawalLimit = withdrawalLimit;
        }

        public Balance Balance { get; }
        public WithdrawalLimit WithdrawalLimit { get; }
    }
}
