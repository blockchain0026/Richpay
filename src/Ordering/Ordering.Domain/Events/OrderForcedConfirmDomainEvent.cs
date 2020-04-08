using MediatR;
using Ordering.Domain.Model.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.Events
{
    public class OrderForcedConfirmDomainEvent : INotification
    {
        public OrderForcedConfirmDomainEvent(Order order, string orderStatusDescription)
        {
            Order = order;
            OrderStatusDescription = orderStatusDescription;
        }

        public Order Order { get; }
        public string OrderStatusDescription { get; }
    }
}
