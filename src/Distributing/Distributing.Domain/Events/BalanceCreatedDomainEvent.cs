using Distributing.Domain.Model.Balances;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class BalanceCreatedDomainEvent : INotification
    {
        public BalanceCreatedDomainEvent(Balance balance, string userId, int userTypeId, WithdrawalLimit withdrawalLimit, decimal withdrawalCommissionRate, decimal depositCommissionRate)
        {
            Balance = balance;
            UserId = userId;
            UserTypeId = userTypeId;
            WithdrawalLimit = withdrawalLimit;
            WithdrawalCommissionRate = withdrawalCommissionRate;
            DepositCommissionRate = depositCommissionRate;
        }

        public Balance Balance { get; }
        public string UserId { get; }
        public int UserTypeId { get; }
        public WithdrawalLimit WithdrawalLimit { get; }
        public decimal WithdrawalCommissionRate { get; }
        public decimal DepositCommissionRate { get; }
    }
}
