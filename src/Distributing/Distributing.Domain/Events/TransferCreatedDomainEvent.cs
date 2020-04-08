using Distributing.Domain.Model.Transfers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{

    public class TransferCreatedDomainEvent : INotification
    {
        public TransferCreatedDomainEvent(Transfer transfer, string userId, int fromBalanceId, int toBalanceId, decimal amount, int initiatedById, DateTime dateTransferred)
        {
            Transfer = transfer;
            UserId = userId;
            FromBalanceId = fromBalanceId;
            ToBalanceId = toBalanceId;
            Amount = amount;
            InitiatedById = initiatedById;
            DateTransferred = dateTransferred;
        }

        public Transfer Transfer { get; }
        public string UserId { get; }
        public int FromBalanceId { get;  }
        public int ToBalanceId { get; }
        public decimal Amount { get;}
        public int InitiatedById { get; }
        public DateTime DateTransferred { get; }
    }

}
