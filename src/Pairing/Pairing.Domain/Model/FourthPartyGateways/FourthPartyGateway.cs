using Pairing.Domain.Exceptions;
using Pairing.Domain.Model.QrCodes;
using Pairing.Domain.Model.Shared;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Model.FourthPartyGateways
{
    public class FourthPartyGateway : Entity, IAggregateRoot
    {
        public string UserId { get; private set; }

        public PaymentChannel PaymentChannel { get; private set; }
        private int _paymentChannelId;
        public PaymentScheme PaymentScheme { get; private set; }
        private int _paymentSchemeId;

        public string Name { get; private set; }

        public TimeSpan OpenFrom { get; private set; }
        public TimeSpan OpenTo { get; private set; }

        public bool IsEnabled { get; private set; }

        public PaymentChannel GetPaymentChannel => PaymentChannel.From(this._paymentChannelId);
        public PaymentScheme GetPaymentScheme => PaymentScheme.From(this._paymentSchemeId);

        protected FourthPartyGateway()
        {
        }

        public FourthPartyGateway(string userId, int paymentChannelId, int paymentSchemeId, string name, TimeSpan openFrom, TimeSpan openTo)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            _paymentChannelId = paymentChannelId;
            _paymentSchemeId = paymentSchemeId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            OpenFrom = openFrom;
            OpenTo = openTo;

            IsEnabled = false;
        }



        public void UpdateBusinessHours(TimeSpan openFrom, TimeSpan openTo)
        {
            //Validate time span.
            if (openFrom.Hours < 0 || openFrom.Hours > 24
                || openTo.Hours < 0 || openTo.Hours > 24
                || openFrom.Minutes < 0 || openFrom.Minutes > 59
                || openTo.Minutes < 0 || openTo.Minutes > 59)
            {
                throw new PairingDomainException("无效的营业时间" + ". At ShopGateway.FromPlatForm()");
            }

            var from = new TimeSpan(openFrom.Hours, openFrom.Minutes, 0);
            var to = new TimeSpan(openTo.Hours, openTo.Minutes, 0);

            this.OpenFrom = from;
            this.OpenTo = to;

        }

        public void Enable()
        {
            this.IsEnabled = true;
        }

        public void Disable()
        {
            this.IsEnabled = false;
        }

        public bool IsOutOfBusinessHours(DateTime currentDateTime)
        {
            var openFrom = this.OpenFrom;
            var openTo = this.OpenTo;

            //Ex:
            //   OpenFrom: 10:30, OpenTo: 22:30
            // ->From Today to Today.
            if (openFrom < openTo)
            {

                if (currentDateTime.Hour >= openFrom.Hours && currentDateTime.Hour <= openTo.Hours)
                {
                    //Ex:
                    //   Current: 10:01, OpenFrom: 10:30
                    // -> 01 < 30 
                    // -> return false
                    if (currentDateTime.Hour == openFrom.Hours)
                    {
                        if (currentDateTime.Minute < openFrom.Minutes)
                        {
                            return false;
                        }
                    }

                    //Ex:
                    //   Current: 22:59, OpenTo: 22:30
                    // -> 59 > 30 
                    // -> return false
                    if (currentDateTime.Hour == openTo.Hours)
                    {
                        if (currentDateTime.Minute > openFrom.Minutes)
                        {
                            return false;
                        }
                    }

                    return true;
                }

                return false;
            }

            //Ex:
            //   OpenFrom: 10:30, OpenTo: 00:30
            // ->From Today to Next Day.
            if (openFrom > openTo)
            {
                //Ex:
                //   Current: 10:01, OpenFrom: 10:30, OpenTo: 00:30
                // -> 10 <= 10 && 10 >= 0
                // -> Probably out of the business hours.
                if (currentDateTime.Hour <= openFrom.Hours && currentDateTime.Hour >= openTo.Hours)
                {
                    //Ex:
                    //   Current: 10:59, OpenFrom: 10:30
                    // -> 59 > 30 
                    // -> return true
                    if (currentDateTime.Hour == openFrom.Hours)
                    {
                        if (currentDateTime.Minute > openFrom.Minutes)
                        {
                            return true;
                        }
                    }

                    //Ex:
                    //   Current: 00:16, OpenTo: 00:30
                    // -> 16 <= 30
                    // -> return true
                    if (currentDateTime.Hour == openTo.Hours)
                    {
                        if (currentDateTime.Minute <= openFrom.Minutes)
                        {
                            return true;
                        }
                    }

                    return false;
                }

                return true;
            }

            //Ex:
            //   OpenFrom: 00:00, OpenTo: 00:00
            // ->24 Hours.
            if (openFrom == openTo)
            {
                return true;
            }
            throw new ArgumentOutOfRangeException("Unclear problems.");

        }
    }
}
