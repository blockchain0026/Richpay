using MediatR;
using Ordering.Domain.Model.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.Events
{
    public class OrderPaymentConfirmedByTraderDomainEvent : INotification
    {
        public OrderPaymentConfirmedByTraderDomainEvent(Order order, string traderId)
        {
            Order = order;
            TraderId = traderId;
        }

        public Order Order { get; }
        public string TraderId { get; }
    }
}
