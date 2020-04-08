using Pairing.Domain.Exceptions;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Model.QrCodes
{
    public class QrCodeOrder
    : Entity
    {
        // DDD Patterns comment
        // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (Instead of properties and property collections)
        private DateTime _dateCreated;
        private DateTime? _dateFinished;
        private decimal _amount;
        private bool _isSuccess;
        private bool _isFailed;

        public string OrderTrackingNumber { get; private set; }

        protected QrCodeOrder() { }


        public QrCodeOrder(DateTime dateCreated, decimal amount, string orderTrackingNumber)
        {
            if (string.IsNullOrWhiteSpace(orderTrackingNumber))
            {
                throw new PairingDomainException("The order tracking number must be provided" + ". At QrCodeOrder()");
            }
            if (amount <= 0)
            {
                throw new PairingDomainException("The order amount must be larger than 0." + ". At QrCodeOrder()");
            }

            _dateCreated = dateCreated;
            _dateFinished = null;
            _amount = amount;

            _isSuccess = false;
            _isFailed = false;

            OrderTrackingNumber = orderTrackingNumber;
        }

        public bool IsSuccess() => _isSuccess;
        public bool IsFailed() => _isFailed;

        public decimal GetAmount() => _amount;
        public DateTime GetDateCreated() => _dateCreated;

        public bool IsOrderDateCreatedBefore(DateTime dateTime)
        {
            if (dateTime == null)
            {
                throw new PairingDomainException("The date time must be provided" + ". At QrCodeOrder.IsOrderDateBefore()");
            }
            var result = DateTime.Compare(dateTime, this._dateCreated);

            //if dateTime is later than _dateCreated, the result is 1.
            if (result == 1)
            {
                return true;
            }

            return false;
        }

        public void Success()
        {
            /*
            //Checking the order id matched.
            if (string.IsNullOrEmpty(order.TrackingNumber) || order.TrackingNumber != this.OrderTrackingNumber)
            {
                throw new PairingDomainException("The order tracking number doesn't match." + ". At QrCodeOrder.Success()");
            }

            //Checking the amount is equal.
            if (order.Amount != this._amount)
            {
                throw new PairingDomainException("The order aount doesn't match." + ". At QrCodeOrder.Success()");
            }
            */
            //Checking the order is processing.
            if (!this.IsProcessing())
            {
                throw new PairingDomainException("The order already finished." + ". At QrCodeOrder.Success()");
            }

            this._isSuccess = true;
            this._isFailed = false;


            //this._dateFinished = order.DateFinished ?? throw new PairingDomainException("The finish date must be provided." + ". At QrCodeOrder.Success()");
            this._dateFinished = DateTime.UtcNow; //Need to improve.
        }

        public void Failed(Order order)
        {
            //Checking the order is processing.
            if (!this.IsProcessing())
            {
                throw new PairingDomainException("The order already finished." + ". At QrCodeOrder.Failed()");
            }

            this._isFailed = true;
            this._isSuccess = false;

            this._dateFinished = order.DateFinished ?? throw new PairingDomainException("The finish date must be provided." + ". At QrCodeOrder.Failed()");
        }


        private bool IsProcessing()
        {
            return !this._isFailed && !this._isSuccess;
        }
    }


}
