using MediatR;
using Pairing.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.ShopGateways;

namespace WebMVC.Applications.DomainEventHandlers.PairingDomain
{
    /// <summary>
    /// Update view model: Update Shop Gateway Entry.
    /// </summary>
    public class ShopGatewayTypeChangedToFourthPartyDomainEventHandler
            : INotificationHandler<ShopGatewayTypeChangedToFourthPartyDomainEvent>
    {
        private readonly IShopGatewayQueries _shopGatewayQueries;

        public ShopGatewayTypeChangedToFourthPartyDomainEventHandler(IShopGatewayQueries shopGatewayQueries)
        {
            _shopGatewayQueries = shopGatewayQueries ?? throw new ArgumentNullException(nameof(shopGatewayQueries));
        }

        public async Task Handle(ShopGatewayTypeChangedToFourthPartyDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var shopGateway = domainEvent.ShopGateway;

            var shopGatewayEntry = await _shopGatewayQueries.GetShopGatewayEntryAsync(shopGateway.Id);

            if (shopGatewayEntry != null)
            {
                shopGatewayEntry.ShopGatewayType = shopGateway.GetShopGatewayType.Name;
                shopGatewayEntry.FourthPartyGatewayId = shopGateway.FourthPartyGatewayId;

                _shopGatewayQueries.Update(shopGatewayEntry);
                await _shopGatewayQueries.SaveChangesAsync();
            }
        }
    }
}
