using MediatR;
using Pairing.Domain.Model.ShopSettingsDomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class ShopSettingsCreatedDomainEvent : INotification
    {
        public ShopSettingsCreatedDomainEvent(ShopSettings shopSettings, string shopId)
        {
            ShopSettings = shopSettings;
            ShopId = shopId;
        }

        public ShopSettings ShopSettings { get; }
        public string ShopId { get; }
    }
}
