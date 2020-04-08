using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Shared;
using MediatR;
using Pairing.Domain.Events;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.QrCodes;
using WebMVC.Extensions;

namespace WebMVC.Applications.DomainEventHandlers.PairingDomain
{
    /// <summary>
    /// Freeze trader balance and update available balance of the trader's QR code.
    /// Update view model: update QR code entry.
    /// </summary>
    public class QrCodePairedWithOrderDomainEventHandler
            : INotificationHandler<QrCodePairedWithOrderDomainEvent>
    {
        private readonly IQrCodeQueries _qrCodeQueries;
        private readonly IQrCodeRepository _qrCodeRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly IDateTimeService _dateTimeService;

        public QrCodePairedWithOrderDomainEventHandler(IQrCodeQueries qrCodeQueries, IQrCodeRepository qrCodeRepository, IBalanceRepository balanceRepository, IDateTimeService dateTimeService)
        {
            _qrCodeQueries = qrCodeQueries ?? throw new ArgumentNullException(nameof(qrCodeQueries));
            _qrCodeRepository = qrCodeRepository ?? throw new ArgumentNullException(nameof(qrCodeRepository));
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
            _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
        }

        public async Task Handle(QrCodePairedWithOrderDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            //return;
            var qrCode = domainEvent.QrCode;
            var pairingOrder = domainEvent.Order;

            var traderId = qrCode.UserId;

            //Update balance entity.
            //Use improved function to update balance from new order.
            var amountAvailable = await _balanceRepository.UpdateBalanceForNewOrderByUserId(
                traderId,
                new Distributing.Domain.Model.Distributions.Order(
                    pairingOrder.TrackingNumber,
                    pairingOrder.ShopOrderId,
                    pairingOrder.Amount,
                    pairingOrder.RateRebate * pairingOrder.Amount,
                    pairingOrder.ShopId,
                    qrCode.UserId,
                    pairingOrder.DateCreated
                    ),
                this._dateTimeService
                );


            //_balanceRepository.Update(balance);


            //Update available balance of the trader's QR code.
            //Prevent qr code that have inefficient balance pair with order.
            var availableBalance = amountAvailable;

            qrCode.BalanceUpdated(availableBalance);

            //Use direct modify approach.
            /*await _qrCodeRepository.UpdateUserQrCodesBalanceWhenPaired(
                traderId, availableBalance, qrCode.Id);*/


            //Update Qr code entry.
            await _qrCodeQueries.UpdateQrCodeEntryForOrderCreated(
                qrCode.Id,
                qrCode.AvailableBalance,
                qrCode.GetPairingStatus.Name,
                qrCode.PairingStatusDescription,
                qrCode.DateLastTraded?.ToFullString()
                );

            //Let order service save the VM changes after all domain things done.
            //await _qrCodeQueries.SaveChangesAsync();
        }
    }
}
