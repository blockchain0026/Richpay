using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Deposits;
using Distributing.Domain.Model.Roles;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class DepositVerifiedDomainEvent : INotification
    {
        public DepositVerifiedDomainEvent(Deposit deposit, Balance balance, int depositStatusId, Admin verifiedBy, DateTime dateFinished)
        {
            Deposit = deposit;
            Balance = balance;
            DepositStatusId = depositStatusId;
            VerifiedBy = verifiedBy;
            DateFinished = dateFinished;
        }

        public Deposit Deposit { get; }
        public Balance Balance { get; }
        public int DepositStatusId { get; }
        public Admin VerifiedBy { get; }
        public DateTime DateFinished { get; }

    }
}
