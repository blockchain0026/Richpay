using Distributing.Domain.Model.Roles;
using Distributing.Domain.Model.Withdrawals;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class WithdrawalForcedSuccessDomainEvent : INotification
    {
        public WithdrawalForcedSuccessDomainEvent(Withdrawal withdrawal, int withdrawalStatusId, Admin forcedBy, DateTime dateFinished)
        {
            Withdrawal = withdrawal;
            WithdrawalStatusId = withdrawalStatusId;
            ForcedBy = forcedBy;
            DateFinished = dateFinished;
        }

        public Withdrawal Withdrawal { get; }
        public int WithdrawalStatusId { get; }
        public Admin ForcedBy { get; }
        public DateTime DateFinished { get; }
    }
}
