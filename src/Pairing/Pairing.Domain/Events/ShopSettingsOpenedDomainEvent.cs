using MediatR;
using Pairing.Domain.Model.ShopSettingsDomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{

    public class ShopSettingsOpenedDomainEvent : INotification
    {
        public ShopSettingsOpenedDomainEvent(ShopSettings shopSettings, bool isOpen)
        {
            ShopSettings = shopSettings;
            IsOpen = isOpen;
        }

        public ShopSettings ShopSettings { get; set; }
        public bool IsOpen { get; set; }
    }
}
