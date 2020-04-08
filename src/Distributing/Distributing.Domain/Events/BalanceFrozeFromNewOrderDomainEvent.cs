using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Frozens;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class BalanceFrozeFromNewOrderDomainEvent : INotification
    {
        public BalanceFrozeFromNewOrderDomainEvent(Balance balance, decimal amountFrozen, decimal amountAvailable, Frozen frozen)
        {
            Balance = balance;
            AmountFrozen = amountFrozen;
            AmountAvailable = amountAvailable;
            Frozen = frozen;
        }

        public Balance Balance { get; }
        public decimal AmountFrozen { get; }
        public decimal AmountAvailable { get; }
        public Frozen Frozen { get; }
    }
}
