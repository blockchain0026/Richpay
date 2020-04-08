using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Frozens;
using Distributing.Domain.Model.Withdrawals;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class BalanceWithdrawalCreatedDomainEvent : INotification
    {
        public BalanceWithdrawalCreatedDomainEvent(Balance balance, decimal amount, DateTime dateCreated, Withdrawal withdrawal, decimal balanceBefore, decimal balanceAfter)
        {
            Balance = balance;
            Amount = amount;
            DateCreated = dateCreated;
            Withdrawal = withdrawal;
            BalanceBefore = balanceBefore;
            BalanceAfter = balanceAfter;
        }

        public Balance Balance { get; }
        public decimal Amount { get; }
        public DateTime DateCreated { get; }
        public Withdrawal Withdrawal { get; }
        public decimal BalanceBefore { get; }
        public decimal BalanceAfter { get; }
    }
}
