using MediatR;
using Pairing.Domain.Model.ShopGateways;
using Pairing.Domain.Model.ShopSettingsDomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{

    public class ShopGatewayAlipayPreferenceUpdatedDomainEvent : INotification
    {
        public ShopGatewayAlipayPreferenceUpdatedDomainEvent(ShopGateway shopGateway, AlipayPreference alipayPreference)
        {
            ShopGateway = shopGateway;
            AlipayPreference = alipayPreference;
        }

        public ShopGateway ShopGateway { get; }
        public AlipayPreference AlipayPreference { get; }
    }
}
