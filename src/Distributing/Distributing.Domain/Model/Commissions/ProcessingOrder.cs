using Distributing.Domain.Exceptions;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Model.Commissions
{

    public class ProcessingOrder
        : Entity
    {
        // DDD Patterns comment
        // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (Instead of properties and property collections)
        private decimal _amount;
        private decimal _commissionAmount;

        public string TrackingNumber { get; private set; }

        protected ProcessingOrder() { }

        public ProcessingOrder(string trackingNumber, decimal amount,decimal commissionAmount)
        {
            /*if (string.IsNullOrEmpty(withdrawalId))
            {
                throw new DistributingDomainException("The withdrawal id must be provided" + ". At BalanceWithdrawal()");
            }*/
            if (string.IsNullOrEmpty(trackingNumber))
            {
                throw new DistributingDomainException("The tracking number must be provided" + ". At ProcessingOrder()");
            }

            if (amount <= 0)
            {
                throw new DistributingDomainException("The order amount must be larger than 0." + ". At ProcessingOrder()");
            }
            if (commissionAmount <= 0)
            {
                throw new DistributingDomainException("The commission amount must be larger than 0." + ". At ProcessingOrder()");
            }

            TrackingNumber = trackingNumber;

            _amount = amount;
            _commissionAmount = commissionAmount;
        }

        public decimal GetAmount() => _amount;
        public decimal GetCommissionAmount() => _commissionAmount;
    }
}
