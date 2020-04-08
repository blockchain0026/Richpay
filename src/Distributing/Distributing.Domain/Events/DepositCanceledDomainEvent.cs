using Distributing.Domain.Model.Deposits;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class DepositCanceledDomainEvent:INotification
    {
        public DepositCanceledDomainEvent(Deposit deposit, int depositStatusId, DateTime dateFinished)
        {
            Deposit = deposit;
            DepositStatusId = depositStatusId;
            DateFinished = dateFinished;
        }

        public Deposit Deposit { get; }
        public int DepositStatusId { get; }
        public DateTime DateFinished { get; }
    }
}
