using Distributing.Domain.Model.Commissions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    /// <summary>
    /// Event used when an commission is created
    /// </summary>
    public class CommissionCreatedDomainEvent : INotification
    {
        public CommissionCreatedDomainEvent(Commission commission, string userId, int userTypeId, int? uplineCommissionId, 
            decimal rateAlipay, decimal rateWechat, decimal rateRebateAlipay, decimal rateRebateWechat, bool isEnabled)
        {
            UserId = userId;
            UserTypeId = userTypeId;
            UplineCommissionId = uplineCommissionId;
            RateAlipay = rateAlipay;
            RateWechat = rateWechat;
            RateRebateAlipay = rateRebateAlipay;
            RateRebateWechat = rateRebateWechat;
            IsEnabled = isEnabled;
            Commission = commission;
        }
        public Commission Commission { get; }
        public string UserId { get; }
        public int UserTypeId { get; }
        public int? UplineCommissionId { get; }
        public decimal RateAlipay { get; }
        public decimal RateWechat { get; }
        public decimal RateRebateAlipay { get; }
        public decimal RateRebateWechat { get; }
        public bool IsEnabled { get; }


    }
}
