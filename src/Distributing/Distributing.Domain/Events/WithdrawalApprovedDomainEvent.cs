using Distributing.Domain.Model.Roles;
using Distributing.Domain.Model.Withdrawals;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class WithdrawalApprovedDomainEvent : INotification
    {
        public WithdrawalApprovedDomainEvent(Withdrawal withdrawal, int withdrawalStatusId, Admin approvedBy)
        {
            Withdrawal = withdrawal;
            WithdrawalStatusId = withdrawalStatusId;
            ApprovedBy = approvedBy;
        }

        public Withdrawal Withdrawal { get; }
        public int WithdrawalStatusId { get; }
        public Admin ApprovedBy { get; }
    }
}
