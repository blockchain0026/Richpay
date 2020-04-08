using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Distributions;
using Distributing.Domain.Model.Shared;
using MediatR;
using Ordering.Domain.Events;
using Ordering.Domain.Model.Orders;
using Pairing.Domain.Exceptions;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.Orders;
using WebMVC.Applications.Queries.QrCodes;
using WebMVC.Applications.Queries.RunningAccounts;
using WebMVC.Extensions;

namespace WebMVC.Applications.DomainEventHandlers.Ordering
{
    /// <summary>
    /// Update Qr Code and Distribute the commission.
    /// 
    /// Update view model: Update Order Entry, Qr Code Entry and Running Account Records.
    /// </summary>
    public class OrderPaymentConfirmedByTraderDomainEventHandler
            : INotificationHandler<OrderPaymentConfirmedByTraderDomainEvent>
    {
        private readonly IQrCodeRepository _qrCodeRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IDistributionRepository _distributionRepository;
        private readonly IDistributionService _distributionService;


        private readonly IOrderQueries _orderQueries;
        private readonly IQrCodeQueries _qrCodeQueries;
        private readonly IRunningAccountQueries _runningAccountQueries;


        private readonly IDateTimeService _distributingDateTimeService;

        public OrderPaymentConfirmedByTraderDomainEventHandler(IQrCodeRepository qrCodeRepository, IOrderRepository orderRepository, IDistributionRepository distributionRepository, IDistributionService distributionService, IOrderQueries orderQueries, IQrCodeQueries qrCodeQueries, IRunningAccountQueries runningAccountQueries, IDateTimeService distributingDateTimeService)
        {
            _qrCodeRepository = qrCodeRepository ?? throw new ArgumentNullException(nameof(qrCodeRepository));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _distributionRepository = distributionRepository ?? throw new ArgumentNullException(nameof(distributionRepository));
            _distributionService = distributionService ?? throw new ArgumentNullException(nameof(distributionService));
            _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
            _qrCodeQueries = qrCodeQueries ?? throw new ArgumentNullException(nameof(qrCodeQueries));
            _runningAccountQueries = runningAccountQueries ?? throw new ArgumentNullException(nameof(runningAccountQueries));
            _distributingDateTimeService = distributingDateTimeService ?? throw new ArgumentNullException(nameof(distributingDateTimeService));
        }

        public async Task Handle(OrderPaymentConfirmedByTraderDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var orderEntity = domainEvent.Order;

            //If this is a test order, only need to update VM.
            if (orderEntity.IsTestOrder)
            {
                _orderQueries.UpdateOrderEntryToSuccess(
                    orderEntity.Id,
                    orderEntity.GetOrderStatus.Name,
                    (decimal)orderEntity.AmountPaid,
                    0M,
                    0M,
                    0M,
                    0M,
                    0M,
                    orderEntity.DatePaymentRecieved?.ToFullString()
                    );
                return;
            }

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

            var distributedAmounts = await _distributionService.DistributeFrom(
                 new Distributing.Domain.Model.Distributions.Order(
                     orderEntity.TrackingNumber,
                     orderEntity.ShopInfo.ShopOrderId,
                     orderEntity.Amount,
                     (decimal)orderEntity.CommissionRealized,
                     orderEntity.ShopInfo.ShopId,
                     orderEntity.PayeeInfo.TraderId,
                     orderEntity.DateCreated
                     ),
                 this._distributingDateTimeService,
                 (int)orderEntity.PayeeInfo.QrCodeId,
                 rateType
                 );

            await _distributionRepository.UnitOfWork.SaveEntitiesAsync();

            #region Update QR codes.
            //Update target QR code pairing data and status.
            var userQrCode = await _qrCodeRepository.GetByQrCodeIdForFinishingOrderAsync((int)orderEntity.PayeeInfo.QrCodeId);
            userQrCode.OrderSuccess(
                orderEntity.TrackingNumber,
                orderEntity.Amount,
                orderEntity.DateCreated
                );
            //_qrCodeRepository.Update(userQrCode);

            var pairingData = userQrCode.PairingData;

            await _qrCodeQueries.UpdateQrCodeEntryForOrderCompleted(
                userQrCode.Id,
                userQrCode.GetPairingStatus.Name,
                userQrCode.PairingStatusDescription,
                userQrCode.AvailableBalance,
                pairingData.TotalSuccess,
                pairingData.TotalFailures,
                pairingData.HighestConsecutiveSuccess,
                pairingData.HighestConsecutiveFailures,
                pairingData.CurrentConsecutiveSuccess,
                pairingData.CurrentConsecutiveFailures,
                (int)(userQrCode.PairingData.SuccessRate * 100),
                userQrCode.QuotaLeftToday
                );
            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
            #endregion

            //Update order entry
            //Use improved update algorism to get better performance.
            decimal shopCommissionRealized = distributedAmounts[0];
            decimal traderCommissionRealized = distributedAmounts[1];

            //decimal shopCommissionRealized = 0;
            //decimal traderCommissionRealized = 0;


            _orderQueries.UpdateOrderEntryToSuccess(
                orderEntity.Id,
                orderEntity.GetOrderStatus.Name,
                (decimal)orderEntity.AmountPaid,
                orderEntity.Amount * (orderEntity.ShopInfo.RateRebateShop - orderEntity.ShopInfo.RateRebateFinal),
                orderEntity.Amount * (decimal)orderEntity.PayeeInfo.ToppestTradingRate,
                (decimal)orderEntity.CommissionRealized,
                traderCommissionRealized,
                shopCommissionRealized,
                orderEntity.DatePaymentRecieved?.ToFullString()
                );
        }
    }
}
