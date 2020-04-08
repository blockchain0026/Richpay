using Distributing.Domain.Model.Commissions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class CommissionEnabledDomainEvent:INotification
    {
        public CommissionEnabledDomainEvent(Commission commission, bool isEnabled)
        {
            Commission = commission;
            IsEnabled = isEnabled;
        }

        public Commission Commission { get; }
        public bool IsEnabled { get; }
    }
}
