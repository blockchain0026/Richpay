using Distributing.Domain.Model.Distributions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{

    public class DistributionCreatedDomainEvent : INotification
    {
        public DistributionCreatedDomainEvent(Distribution distribution, int distributionTypeId, int commissionId, Order order, int balanceId, decimal distributedAmount, string downlineUserId)
        {
            Distribution = distribution;
            DistributionTypeId = distributionTypeId;
            CommissionId = commissionId;
            Order = order;
            BalanceId = balanceId;
            DistributedAmount = distributedAmount;
            DownlineUserId = downlineUserId;
        }

        public Distribution Distribution { get; }
        public int DistributionTypeId { get; }
        public int CommissionId { get; }
        public Order Order { get; }
        public int BalanceId { get; }
        public decimal DistributedAmount { get; }
        public string DownlineUserId { get; } //For UI Purpose.
    }
}
