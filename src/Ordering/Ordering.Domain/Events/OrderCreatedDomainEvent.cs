using MediatR;
using Ordering.Domain.Model.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.Events
{
    public class OrderCreatedDomainEvent : INotification
    {
        public OrderCreatedDomainEvent(Order order, bool isTestOrder, int orderTypeId, int? expirationTimeInSeconds, int orderPaymentChannelId, int orderPaymentSchemeId, AlipayPagePreference alipayPagePreference, PayeeInfo payeeInfo, decimal amount, DateTime dateCreated)
        {
            Order = order;
            IsTestOrder = isTestOrder;
            OrderTypeId = orderTypeId;
            ExpirationTimeInSeconds = expirationTimeInSeconds;
            OrderPaymentChannelId = orderPaymentChannelId;
            OrderPaymentSchemeId = orderPaymentSchemeId;
            AlipayPagePreference = alipayPagePreference;
            PayeeInfo = payeeInfo;
            Amount = amount;
            DateCreated = dateCreated;
        }

        public Order Order { get; }
        public bool IsTestOrder { get; }
        public int OrderTypeId { get; }
        public int? ExpirationTimeInSeconds { get; }
        public int OrderPaymentChannelId { get; }
        public int OrderPaymentSchemeId { get; }
        public AlipayPagePreference AlipayPagePreference { get; }
        public PayeeInfo PayeeInfo { get; }
        public decimal Amount { get; }
        public DateTime DateCreated { get; }
    }
}
