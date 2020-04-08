using MediatR;
using Microsoft.AspNetCore.Identity;
using Pairing.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.QrCodes;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.DomainEventHandlers.PairingDomain
{
    /// <summary>
    /// Update view model: update QR code entry.
    /// </summary>
    public class ShopSettingsSpecifiedShopUpdatedDomainEventHandler
            : INotificationHandler<ShopSettingsSpecifiedShopUpdatedDomainEvent>
    {
        private readonly IQrCodeQueries _qrCodeQueries;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShopSettingsSpecifiedShopUpdatedDomainEventHandler(IQrCodeQueries qrCodeQueries, UserManager<ApplicationUser> userManager)
        {
            _qrCodeQueries = qrCodeQueries ?? throw new ArgumentNullException(nameof(qrCodeQueries));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task Handle(ShopSettingsSpecifiedShopUpdatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            //Update Qr code entry.
            var qrCode = domainEvent.QrCode;
            var qrCodeVM = await _qrCodeQueries.GetQrCodeEntryAsync(qrCode.Id);

            if (qrCodeVM != null)
            {
                var user = await _userManager.FindByIdAsync(qrCode.SpecifiedShopId);
                qrCodeVM.SpecifiedShopId = user?.Id;
                qrCodeVM.SpecifiedShopUsername = user?.UserName;
                qrCodeVM.SpecifiedShopId = user?.FullName;

                _qrCodeQueries.Update(qrCodeVM);

                await _qrCodeQueries.SaveChangesAsync();
            }
        }
    }
}
