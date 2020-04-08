using Distributing.Domain.Exceptions;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Model.Distributions
{
    public class Order : ValueObject
    {
        public Order(string trackingNumber,string shopOrderId, decimal amount, decimal commissionAmount, string shopId, string traderId,DateTime dateCreated)
        {
            TrackingNumber = !string.IsNullOrEmpty(trackingNumber) ? trackingNumber :
                throw new DistributingDomainException("The tracking number must be provided" + ". At Order()"); ;
            
            ShopOrderId = !string.IsNullOrEmpty(shopOrderId) ? shopOrderId :
                throw new DistributingDomainException("The shop order id must be provided" + ". At Order()"); ;

            //Checking the value is positive and the order amount is larger than or equal to 1.
            if (amount < 1 || commissionAmount < 0)
            {
                throw new DistributingDomainException("The order value must be positive" + ". At Order()");
            }
            else if (decimal.Round(amount, 0) != amount)
            {
                throw new DistributingDomainException("The order amount must be an integer" + ". At Order()");
            }

            Amount = amount;
            CommissionAmount = commissionAmount;

            ShopId = !string.IsNullOrEmpty(shopId) ? shopId :
                throw new DistributingDomainException("The shop Id must be provided" + ". At Order()");
            TraderId = !string.IsNullOrEmpty(traderId) ? traderId :
                throw new DistributingDomainException("The trader Id must be provided" + ". At Order()");
  
            DateCreated = dateCreated;
        }

        public string TrackingNumber { get; private set; }
        public string ShopOrderId { get; private set; }
        public decimal Amount { get; private set; }
        public decimal CommissionAmount { get; private set; }
        public string ShopId { get; private set; }
        public string TraderId { get; private set; }
        public DateTime DateCreated { get; private set; }


        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return TrackingNumber;
            yield return Amount;
            yield return CommissionAmount;
            yield return ShopId;
            yield return TraderId;
            yield return DateCreated;
        }
    }
}
