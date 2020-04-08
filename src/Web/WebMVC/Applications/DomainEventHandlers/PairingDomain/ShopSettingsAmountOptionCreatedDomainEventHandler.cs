using MediatR;
using Pairing.Domain.Events;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebMVC.Applications.DomainEventHandlers.PairingDomain
{
    /// <summary>
    /// Update qr code entities: validate bar code's amount
    /// </summary>
    public class ShopSettingsAmountOptionCreatedDomainEventHandler
            : INotificationHandler<ShopSettingsAmountOptionCreatedDomainEvent>
    {
        private readonly IQrCodeRepository _qrCodeRepository;

        public ShopSettingsAmountOptionCreatedDomainEventHandler(IQrCodeRepository qrCodeRepository)
        {
            _qrCodeRepository = qrCodeRepository ?? throw new ArgumentNullException(nameof(qrCodeRepository));
        }

        public async Task Handle(ShopSettingsAmountOptionCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var qrCodes = await _qrCodeRepository.GetByShopIdAsync(domainEvent.ShopSettings.ShopId);

            if (qrCodes.Any())
            {
                foreach (var qrCode in qrCodes)
                {
                    qrCode.ShopOrderAmountOptionsUpdated(domainEvent.ShopSettings);
                    _qrCodeRepository.Update(qrCode);
                }
            }

            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
