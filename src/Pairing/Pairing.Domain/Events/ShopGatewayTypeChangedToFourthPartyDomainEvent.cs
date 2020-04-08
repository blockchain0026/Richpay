using MediatR;
using Pairing.Domain.Model.FourthPartyGateways;
using Pairing.Domain.Model.ShopGateways;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class ShopGatewayTypeChangedToFourthPartyDomainEvent : INotification
    {
        public ShopGatewayTypeChangedToFourthPartyDomainEvent(ShopGateway shopGateway, int shopGatewayTypeId, FourthPartyGateway fourthPartyGateway)
        {
            ShopGateway = shopGateway;
            ShopGatewayTypeId = shopGatewayTypeId;
            FourthPartyGateway = fourthPartyGateway;
        }

        public ShopGateway ShopGateway { get; }
        public int ShopGatewayTypeId { get; }
        public FourthPartyGateway FourthPartyGateway { get; }
    }
}
