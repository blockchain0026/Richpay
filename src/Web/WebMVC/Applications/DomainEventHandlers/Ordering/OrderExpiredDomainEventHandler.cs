using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Distributions;
using Distributing.Domain.Model.Frozens;
using MediatR;
using Ordering.Domain.Events;
using Ordering.Domain.Model.Orders;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.DomainServices.DistributingDomain;
using WebMVC.Applications.Queries.Orders;
using WebMVC.Applications.Queries.QrCodes;
using WebMVC.Extensions;

namespace WebMVC.Applications.DomainEventHandlers.Ordering
{
    /// <summary>
    /// TODO: Return amount to trader if the order is paired.
    /// 
    /// Update view model: Update Order Entry.
    /// </summary>
    public class OrderExpiredDomainEventHandler
            : INotificationHandler<OrderExpiredDomainEvent>
    {
        private readonly IOrderQueries _orderQueries;
        private readonly IDistributionService _distributionService;
        private readonly IQrCodeQueries _qrCodeQueries;
        private readonly IQrCodeRepository _qrCodeRepository;
        private readonly IBalanceRepository _balanceRepository;

        public OrderExpiredDomainEventHandler(IOrderQueries orderQueries, IDistributionService distributionService, IQrCodeQueries qrCodeQueries, IQrCodeRepository qrCodeRepository, IBalanceRepository balanceRepository)
        {
            _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
            _distributionService = distributionService ?? throw new ArgumentNullException(nameof(distributionService));
            _qrCodeQueries = qrCodeQueries ?? throw new ArgumentNullException(nameof(qrCodeQueries));
            _qrCodeRepository = qrCodeRepository ?? throw new ArgumentNullException(nameof(qrCodeRepository));
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
        }

        public async Task Handle(OrderExpiredDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var orderEntity = domainEvent.Order;

            //If the order is for test, no need to do things.
            if (orderEntity.IsTestOrder)
            {
                return;
            }

            //Return amount to trader if the order is paired.
            if (orderEntity.GetOrderStatus.Id == OrderStatus.AwaitingPayment.Id)
            {
                //Distribute the dividends.
                RateType rateType = null;
                if (orderEntity.GetOrderPaymentChannel.Name == OrderPaymentChannel.Alipay.Name)
                {
                    rateType = RateType.Alipay;
                }
                else if (orderEntity.GetOrderPaymentChannel.Name == OrderPaymentChannel.Wechat.Name)
                {
                    rateType = RateType.Wechat;
                }
                else
                {
                    throw new Exception("Unrecognized rate type.");
                }

                await _distributionService.OrderCanceled(
                    new Distributing.Domain.Model.Distributions.Order(
                        orderEntity.TrackingNumber,
                        orderEntity.ShopInfo.ShopOrderId,
                        orderEntity.Amount,
                        0, //Set to 0  because failed to proccess payment.
                        orderEntity.ShopInfo.ShopId,
                        orderEntity.PayeeInfo.TraderId,
                        orderEntity.DateCreated
                        ),
                    new DateTimeService(),
                    rateType
                    );


                #region Unfreeze trader frozen balance
                var amountAvailable = await _balanceRepository.UpdateBalanceForDistributeByUserId(orderEntity.PayeeInfo.TraderId, orderEntity.Amount);
                #endregion

                //Update available balance of the trader's QR code.
                //Prevent qr code that have inefficient balance pair with order.
                var availableBalance = amountAvailable;
                //Update target QR code pairing data and status.
                var userQrCode = await _qrCodeRepository.GetByQrCodeIdForFinishingOrderAsync((int)orderEntity.PayeeInfo.QrCodeId);
                userQrCode.OrderFailed(
                    new Pairing.Domain.Model.QrCodes.Order(
                        orderEntity.TrackingNumber,
                        orderEntity.ShopInfo.ShopOrderId,
                        orderEntity.GetOrderPaymentChannel.Name,
                        orderEntity.GetOrderPaymentScheme.Name,
                        orderEntity.Amount,
                        orderEntity.ShopInfo.ShopId,
                        orderEntity.ShopInfo.RateRebateShop,
                        orderEntity.DateCreated,
                        DateTime.UtcNow
                        )
                    );

                userQrCode.BalanceUpdated(availableBalance);

                //Use direct modify approach.
                await _qrCodeRepository.UpdateUserQrCodesBalanceWhenPaired(
                    orderEntity.PayeeInfo.TraderId, availableBalance, userQrCode.Id);


                //Update Qr code entry.
                await _qrCodeQueries.UpdateQrCodeEntryForOrderCreated(
                    userQrCode.Id,
                    userQrCode.AvailableBalance,
                    userQrCode.GetPairingStatus.Name,
                    userQrCode.PairingStatusDescription,
                    userQrCode.DateLastTraded?.ToFullString()
                    );
            }
        }
    }
}
