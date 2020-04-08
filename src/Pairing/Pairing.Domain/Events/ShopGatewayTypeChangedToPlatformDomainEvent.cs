using MediatR;
using Pairing.Domain.Model.ShopGateways;
using Pairing.Domain.Model.ShopSettingsDomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class ShopGatewayTypeChangedToPlatformDomainEvent : INotification
    {
        public ShopGatewayTypeChangedToPlatformDomainEvent(ShopGateway shopGateway, int shopGatewayTypeId, AlipayPreference alipayPreference)
        {
            ShopGateway = shopGateway;
            ShopGatewayTypeId = shopGatewayTypeId;
            AlipayPreference = alipayPreference;
        }

        public ShopGateway ShopGateway { get; }
        public int ShopGatewayTypeId { get; }
        public AlipayPreference AlipayPreference { get; }
    }
}
