using Distributing.Domain.Model.Frozens;
using Distributing.Domain.Model.Roles;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class FrozenCreatedDomainEvent : INotification
    {
        public FrozenCreatedDomainEvent(Frozen frozen, int balanceId, int frozenTypeId, decimal amount, string orderTrackingNumber, int? withdrawalId, Admin byAdmin, DateTime dateFroze, string description)
        {
            Frozen = frozen;
            BalanceId = balanceId;
            FrozenTypeId = frozenTypeId;
            Amount = amount;
            OrderTrackingNumber = orderTrackingNumber;
            WithdrawalId = withdrawalId;
            ByAdmin = byAdmin;
            DateFroze = dateFroze;
            Description = description;
        }

        public Frozen Frozen { get; }
        public int BalanceId { get; }
        public int FrozenTypeId { get; }
        public decimal Amount { get; }
        public string OrderTrackingNumber { get; }
        public int? WithdrawalId { get; }
        public Admin ByAdmin { get; }
        public DateTime DateFroze { get; }
        public string Description { get; }
    }
}
