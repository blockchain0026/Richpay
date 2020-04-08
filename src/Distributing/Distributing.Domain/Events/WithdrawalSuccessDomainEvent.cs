using Distributing.Domain.Model.Withdrawals;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class WithdrawalSuccessDomainEvent : INotification
    {
        public WithdrawalSuccessDomainEvent(Withdrawal withdrawal, int withdrawalStatusId, DateTime dateFinished)
        {
            Withdrawal = withdrawal;
            WithdrawalStatusId = withdrawalStatusId;
            DateFinished = dateFinished;
        }

        public Withdrawal Withdrawal { get; }
        public int WithdrawalStatusId { get; }
        public DateTime DateFinished { get; }
    }
}
