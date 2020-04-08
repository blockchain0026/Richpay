using MediatR;
using Pairing.Domain.Model.ShopGateways;
using Pairing.Domain.Model.ShopSettingsDomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class ShopGatewayCreatedDomainEvent : INotification
    {
        public ShopGatewayCreatedDomainEvent(ShopGateway shopGateway, int shopGatewayTypeId, int paymentChannelId, int paymentSchemeId, int? fourthPartyGatewayId, AlipayPreference alipayPreference, DateTime dateCreated)
        {
            ShopGateway = shopGateway;
            ShopGatewayTypeId = shopGatewayTypeId;
            PaymentChannelId = paymentChannelId;
            PaymentSchemeId = paymentSchemeId;
            FourthPartyGatewayId = fourthPartyGatewayId;
            AlipayPreference = alipayPreference;
            DateCreated = dateCreated;
        }

        public ShopGateway ShopGateway { get; }
        public int ShopGatewayTypeId { get; }
        public int PaymentChannelId { get; }
        public int PaymentSchemeId { get; }
        public int? FourthPartyGatewayId { get; }
        public AlipayPreference AlipayPreference { get; }
        public DateTime DateCreated { get; }
    }
}
