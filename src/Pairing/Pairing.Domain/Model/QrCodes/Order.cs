using Pairing.Domain.Exceptions;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Model.QrCodes
{
    public class Order : ValueObject
    {
        public string TrackingNumber { get; private set; }
        public string ShopOrderId { get; private set; }
        public string PaymentChannel { get; private set; }
        public string PaymentScheme { get; private set; }
        public decimal Amount { get; private set; }
        public string ShopId { get; private set; }
        public decimal RateRebate { get; private set; }
        public DateTime DateCreated { get; private set; }
        public DateTime? DateFinished { get; private set; }

        public Order(string trackingNumber,string shopOrderId, string paymentChannel, string paymentScheme, decimal amount, string shopId, decimal rateRebate, DateTime dateCreated, DateTime? dateFinished)
        {
            //Checking the tracking number is valid.
            TrackingNumber = !string.IsNullOrEmpty(trackingNumber) ? trackingNumber :
                throw new PairingDomainException("The tracking number must be provided" + ". At Order()");

            //Checking the tracking number is valid.
            ShopOrderId = !string.IsNullOrEmpty(shopOrderId) ? shopOrderId :
                throw new PairingDomainException("The shop order Id must be provided" + ". At Order()");

            //Checking the payment channel and payment scheme is valid.
            this.ValidatePaymentChannelAndScheme(paymentChannel, paymentScheme);
            PaymentChannel = paymentChannel;
            PaymentScheme = paymentScheme;

            //Checking the value is positive and the order amount is larger than or equal to 1.
            if (amount < 1)
            {
                throw new PairingDomainException("The order value must be positive" + ". At Order()");
            }
            else if (decimal.Round(amount, 0) != amount)
            {
                throw new PairingDomainException("The order amount must be an integer" + ". At Order()");
            }
            Amount = amount;

            //Checking the shop Id is valid.
            ShopId = !string.IsNullOrEmpty(shopId) ? shopId :
                throw new PairingDomainException("The shop Id must be provided" + ". At Order()");

            //Checking the rebate rates greater than 0 and less than 1.
            if (rateRebate < 0 || rateRebate >= 1)
            {
                throw new PairingDomainException("The rebate rate must greater than 0 and less than 1" + ". At Order()");
            }
            //Checking the rebate rates has only 3 points.
            if (decimal.Round(rateRebate, 3) != rateRebate)
            {
                throw new PairingDomainException("The rebate rate can only has 3 points" + ". At Order()");
            }
            RateRebate = rateRebate;


            DateCreated = dateCreated;
            DateFinished = dateFinished;
        }



        private void ValidatePaymentChannelAndScheme(string paymentChannel, string paymentScheme)
        {
            try
            {
                var paymentChannelVO = QrCodes.PaymentChannel.FromName(paymentChannel);
                var paymentSchemeVO = QrCodes.PaymentScheme.FromName(paymentScheme);
            }
            catch (PairingDomainException ex)
            {
                Console.Write(ex);

                throw new PairingDomainException("Invalid payment channel or payment scheme given" + ". At Order()");
            }
        }


        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return TrackingNumber;
            yield return PaymentChannel;
            yield return PaymentScheme;
            yield return Amount;
            yield return ShopId;
            yield return RateRebate;
            yield return DateCreated;
            yield return DateFinished;
        }
    }
}
