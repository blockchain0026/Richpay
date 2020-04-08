using Distributing.Domain.Model.Commissions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{

    public class CommissionRebateRateUpdatedDomainEvent : INotification
    {
        public CommissionRebateRateUpdatedDomainEvent(Commission commission, decimal rateRebateAlipay, decimal rateRebateWechat)
        {
            Commission = commission;
            RateRebateAlipay = rateRebateAlipay;
            RateRebateWechat = rateRebateWechat;
        }

        public Commission Commission { get; }
        public decimal RateRebateAlipay { get; }
        public decimal RateRebateWechat { get; }
    }
}
