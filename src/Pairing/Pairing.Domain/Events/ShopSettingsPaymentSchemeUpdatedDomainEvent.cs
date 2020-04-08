using MediatR;
using Pairing.Domain.Model.ShopSettingsDomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class ShopSettingsPaymentSchemeUpdatedDomainEvent : INotification
    {
        public ShopSettingsPaymentSchemeUpdatedDomainEvent(ShopSettings shopSettings, int alipayPaymentSchemeId, int wechatPaymentSchemeId)
        {
            ShopSettings = shopSettings;
            AlipayPaymentSchemeId = alipayPaymentSchemeId;
            WechatPaymentSchemeId = wechatPaymentSchemeId;
        }

        public ShopSettings ShopSettings { get; }
        public int AlipayPaymentSchemeId { get; }
        public int WechatPaymentSchemeId { get; }
    }
}
