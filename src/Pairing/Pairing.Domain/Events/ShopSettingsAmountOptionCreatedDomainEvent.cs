using MediatR;
using Pairing.Domain.Model.ShopSettingsDomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class ShopSettingsAmountOptionCreatedDomainEvent : INotification
    {
        public ShopSettingsAmountOptionCreatedDomainEvent(ShopSettings shopSettings, decimal amount, DateTime dateCreated)
        {
            ShopSettings = shopSettings;
            Amount = amount;
            DateCreated = dateCreated;
        }

        public ShopSettings ShopSettings { get; }
        public decimal Amount { get; }
        public DateTime DateCreated { get; }
    }
}
