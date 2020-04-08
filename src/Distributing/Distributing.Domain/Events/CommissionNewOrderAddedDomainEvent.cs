using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Distributions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class CommissionNewOrderAddedDomainEvent : INotification
    {
        public CommissionNewOrderAddedDomainEvent(Commission commission, Order order, string downlineUserId)
        {
            Commission = commission;
            Order = order;
            DownlineUserId = downlineUserId;
        }

        public Commission Commission { get; }
        public Order Order { get; }
        public string DownlineUserId { get; }
    }
}
