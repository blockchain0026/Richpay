using Distributing.Domain.Model.Distributions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class DistributionFinishedDomainEvent : INotification
    {
        public DistributionFinishedDomainEvent(Distribution distribution, decimal distributedAmount, bool isSuccess)
        {
            Distribution = distribution ?? throw new ArgumentNullException(nameof(distribution));
            DistributedAmount = distributedAmount;
            IsSuccess = isSuccess;
        }

        public Distribution Distribution { get; }
        public decimal DistributedAmount { get; }
        public bool IsSuccess { get; }
    }
}
