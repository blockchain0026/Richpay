using MediatR;
using Ordering.Domain.Model.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.Events
{
    public class OrderPairedByFourthPartyDomainEvent : INotification
    {
        public OrderPairedByFourthPartyDomainEvent(Order order, PayeeInfo payeeInfo)
        {
            Order = order;
            PayeeInfo = payeeInfo;
        }

        public Order Order { get; }
        public PayeeInfo PayeeInfo { get; }
    }
}
