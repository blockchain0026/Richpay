using MediatR;
using Ordering.Domain.Model.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.Events
{
    public class OrderPairedByPlatformDomainEvent : INotification
    {
        public OrderPairedByPlatformDomainEvent(Order order, PayeeInfo payeeInfo)
        {
            Order = order;
            PayeeInfo = payeeInfo;
        }

        public Order Order { get; }
        public PayeeInfo PayeeInfo { get; }
    }
}
