using Distributing.Domain.Model.Commissions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class CommissionRateUpdatedDomainEvent:INotification
    {
        public CommissionRateUpdatedDomainEvent(Commission commission, decimal rateAlipay, decimal rateWechat)
        {
            Commission = commission;
            RateAlipay = rateAlipay;
            RateWechat = rateWechat;
        }

        public Commission Commission { get; }
        public decimal RateAlipay { get; }
        public decimal RateWechat { get; }
    }
}
