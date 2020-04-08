using Distributing.Domain.Model.Balances;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class BalanceUnfrozenDomainEvent : INotification
    {
        public BalanceUnfrozenDomainEvent(Balance balance, decimal amountUnfreeze, decimal amountAvailable)
        {
            Balance = balance;
            AmountUnfreeze = amountUnfreeze;
            AmountAvailable = amountAvailable;
        }

        public Balance Balance { get; }
        public decimal AmountUnfreeze { get; }
        public decimal AmountAvailable { get; }
    }
}
