using MediatR;
using Pairing.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.ShopGateways;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.DomainEventHandlers.PairingDomain
{
    /// <summary>
    /// Update view model: Update Shop Gateway Entry.
    /// </summary>
    public class ShopGatewayAlipayPreferenceUpdatedDomainEventHandler
            : INotificationHandler<ShopGatewayAlipayPreferenceUpdatedDomainEvent>
    {
        private readonly IShopGatewayQueries _shopGatewayQueries;

        public ShopGatewayAlipayPreferenceUpdatedDomainEventHandler(IShopGatewayQueries shopGatewayQueries)
        {
            _shopGatewayQueries = shopGatewayQueries ?? throw new ArgumentNullException(nameof(shopGatewayQueries));
        }

        public async Task Handle(ShopGatewayAlipayPreferenceUpdatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var shopGateway = domainEvent.ShopGateway;

            var shopGatewayEntry = await _shopGatewayQueries.GetShopGatewayEntryAsync(shopGateway.Id);

            if (shopGatewayEntry != null)
            {
                shopGatewayEntry.AlipayPreferenceInfo = new AlipayPreferenceInfo
                {
                    SecondsBeforePayment = shopGateway.AlipayPreference.SecondsBeforePayment,
                    IsAmountUnchangeable = shopGateway.AlipayPreference.IsAmountUnchangeable,
                    IsAccountUnchangeable = shopGateway.AlipayPreference.IsAccountUnchangeable,
                    IsH5RedirectByScanEnabled = shopGateway.AlipayPreference.IsH5RedirectByScanEnabled,
                    IsH5RedirectByClickEnabled = shopGateway.AlipayPreference.IsH5RedirectByClickEnabled,
                    IsH5RedirectByPickingPhotoEnabled = shopGateway.AlipayPreference.IsH5RedirectByPickingPhotoEnabled
                };

                _shopGatewayQueries.Update(shopGatewayEntry);
                await _shopGatewayQueries.SaveChangesAsync();
            }
        }
    }
}
