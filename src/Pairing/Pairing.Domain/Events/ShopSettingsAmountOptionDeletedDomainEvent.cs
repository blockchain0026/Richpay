using MediatR;
using Pairing.Domain.Model.ShopSettingsDomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class ShopSettingsAmountOptionDeletedDomainEvent : INotification
    {
        public ShopSettingsAmountOptionDeletedDomainEvent(ShopSettings shopSettings, decimal amount)
        {
            ShopSettings = shopSettings;
            this.amount = amount;
        }

        public ShopSettings ShopSettings { get; }
        public decimal amount { get; }
    }
}
