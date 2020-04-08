using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Distributions;
using Distributing.Domain.Model.Frozens;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class UnfrozenDomainEvent : INotification
    {
        public UnfrozenDomainEvent(Frozen frozen, Balance balance, BalanceRecord balanceRecord, int frozenStatusId, DateTime dateUnfroze)
        {
            Frozen = frozen ?? throw new ArgumentNullException(nameof(frozen));
            Balance = balance ?? throw new ArgumentNullException(nameof(balance));
            BalanceRecord = balanceRecord ?? throw new ArgumentNullException(nameof(balanceRecord));
            FrozenStatusId = frozenStatusId;
            DateUnfroze = dateUnfroze;
        }

        public Frozen Frozen { get; }
        public Balance Balance { get; }

        public BalanceRecord BalanceRecord { get; }
        public int FrozenStatusId { get; }
        public DateTime DateUnfroze { get; }
    }
}
