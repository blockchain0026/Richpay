using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Frozens;
using Distributing.Domain.Model.Roles;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class BalanceFrozeByAdminDomainEvent : INotification
    {
        public BalanceFrozeByAdminDomainEvent(Balance balance, decimal amountFrozen, decimal amountAvailable, Admin byAdmin, Frozen frozen)
        {
            Balance = balance;
            AmountFrozen = amountFrozen;
            AmountAvailable = amountAvailable;
            ByAdmin = byAdmin;
            Frozen = frozen;
        }

        public Balance Balance { get; }
        public decimal AmountFrozen { get; }
        public decimal AmountAvailable { get; }
        public Admin ByAdmin { get; }

        public Frozen Frozen { get; }
    }
}
