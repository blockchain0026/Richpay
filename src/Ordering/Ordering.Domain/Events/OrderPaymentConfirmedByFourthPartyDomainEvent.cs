using MediatR;
using Ordering.Domain.Model.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.Events
{
    public class OrderPaymentConfirmedByFourthPartyDomainEvent : INotification
    {
        public OrderPaymentConfirmedByFourthPartyDomainEvent(Order order, string fourthPartyId)
        {
            Order = order;
            FourthPartyId = fourthPartyId;
        }

        public Order Order { get; }
        public string FourthPartyId { get; }
    }
}
