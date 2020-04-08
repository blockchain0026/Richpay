using Distributing.Domain.Model.Commissions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class CommissionOrderCompletedDomainEvent : INotification
    {
        public CommissionOrderCompletedDomainEvent(Commission commission, string orderTrackingNumber, bool isSuccess, decimal distributedAmount)
        {
            Commission = commission;
            OrderTrackingNumber = orderTrackingNumber;
            IsSuccess = isSuccess;
            DistributedAmount = distributedAmount;
        }

        public Commission Commission { get; }
        public string OrderTrackingNumber { get; }
        public bool IsSuccess { get; }
        public decimal DistributedAmount { get; }
    }
}
