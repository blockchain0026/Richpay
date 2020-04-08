using Distributing.Domain.Model.Withdrawals;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class WithdrawalAwaitingCancellationDomainEvent : INotification
    {
        public WithdrawalAwaitingCancellationDomainEvent(Withdrawal withdrawal, int withdrawalStatusId)
        {
            Withdrawal = withdrawal;
            WithdrawalStatusId = withdrawalStatusId;
        }

        public Withdrawal Withdrawal { get; }
        public int WithdrawalStatusId { get; }
    }
}