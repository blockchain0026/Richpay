using MediatR;
using Pairing.Domain.Model.QrCodes;
using Pairing.Domain.Model.ShopSettingsDomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class ShopSettingsSpecifiedShopUpdatedDomainEvent : INotification
    {
        public ShopSettingsSpecifiedShopUpdatedDomainEvent(QrCode qrCode, ShopSettings shopSettings)
        {
            QrCode = qrCode;
            ShopSettings = shopSettings;
        }

        public QrCode QrCode { get; set; }
        public ShopSettings ShopSettings { get; set; }
    }
}
