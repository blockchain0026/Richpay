using Distributing.Domain.Model.Roles;
using Distributing.Domain.Model.Withdrawals;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class WithdrawalCanceledDomainEvent : INotification
    {
        public WithdrawalCanceledDomainEvent(Withdrawal withdrawal, int withdrawalStatusId, Admin approvedBy, DateTime dateFinished)
        {
            Withdrawal = withdrawal;
            WithdrawalStatusId = withdrawalStatusId;
            ApprovedBy = approvedBy;
            DateFinished = dateFinished;
        }

        public Withdrawal Withdrawal { get; }
        public int WithdrawalStatusId { get; }
        public Admin ApprovedBy { get; }
        public DateTime DateFinished { get; }
    }
}
