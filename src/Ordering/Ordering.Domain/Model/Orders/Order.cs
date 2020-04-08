using Ordering.Domain.Events;
using Ordering.Domain.Exceptions;
using Ordering.Domain.Model.Shared;
using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.Model.Orders
{
    public class Order : Entity, IAggregateRoot
    {
        public string TrackingNumber { get; private set; }

        public OrderType OrderType { get; private set; }
        private int _orderTypeId;

        public OrderStatus OrderStatus { get; private set; }
        private int _orderStatusId;

        public string OrderStatusDescription { get; private set; }

        public int? ExpirationTimeInSeconds { get; private set; }

        public bool IsTestOrder { get; private set; }
        public bool IsExpired { get; private set; }

        public ShopInfo ShopInfo { get; private set; }

        public OrderPaymentChannel OrderPaymentChannel { get; private set; }
        private int _orderPaymentChannelId;

        public OrderPaymentScheme OrderPaymentScheme { get; private set; }
        private int _orderPaymentSchemeId;

        public AlipayPagePreference AlipayPagePreference { get; private set; }

        public PayerInfo PayerInfo { get; private set; }
        public PayeeInfo PayeeInfo { get; private set; }

        public decimal Amount { get; private set; }
        public decimal? AmountPaid { get; private set; }
        public decimal? CommissionRealized { get; private set; }

        public DateTime DateCreated { get; private set; }
        public DateTime? DatePaired { get; private set; }
        public DateTime? DatePaymentRecieved { get; private set; }


        public OrderType GetOrderType => OrderType.From(this._orderTypeId);
        public OrderStatus GetOrderStatus => OrderStatus.From(this._orderStatusId);
        public OrderPaymentChannel GetOrderPaymentChannel => OrderPaymentChannel.From(this._orderPaymentChannelId);
        public OrderPaymentScheme GetOrderPaymentScheme => OrderPaymentScheme.From(this._orderPaymentSchemeId);


        protected Order()
        {
        }

        public Order(int orderTypeId, bool isTestOrder, int? expirationTimeInSeconds, ShopInfo shopInfo,
            int orderPaymentChannelId, int orderPaymentSchemeId,
            AlipayPagePreference alipayPagePreference, PayeeInfo payeeInfo, decimal amount, DateTime dateCreated)
        {
            TrackingNumber = Guid.NewGuid().ToString();
            this.IsTestOrder = isTestOrder;
            _orderTypeId = orderTypeId;
            _orderStatusId = OrderStatus.Submitted.Id;
            OrderStatusDescription = string.Empty;
            ExpirationTimeInSeconds = expirationTimeInSeconds;
            IsExpired = false;
            ShopInfo = shopInfo;
            _orderPaymentChannelId = orderPaymentChannelId;
            _orderPaymentSchemeId = orderPaymentSchemeId;
            AlipayPagePreference = alipayPagePreference;
            PayeeInfo = payeeInfo;
            Amount = amount;
            DateCreated = dateCreated;

            /*this.AddDomainEvent(new OrderCreatedDomainEvent(
                this,
                isTestOrder,
                orderTypeId,
                expirationTimeInSeconds,
                orderPaymentChannelId,
                orderPaymentSchemeId,
                alipayPagePreference,
                payeeInfo,
                amount,
                dateCreated
                ));*/
        }

        public static Order FromShopToPlatform(int expirationTimeInSeconds,
            string shopId, string shopOrderId, string shopReturnUrl, string shopOkReturnUrl,
            decimal rateRebateShop, decimal rateRebateFinal, decimal shopOrderAmount,
            OrderPaymentChannel orderPaymentChannel, OrderPaymentScheme orderPaymentScheme, IDateTimeService dateTimeService, AlipayPagePreference alipayPagePreference = null)
        {
            //Checking the expiration time's value is positive and less than 9999.
            if (expirationTimeInSeconds < 0 || expirationTimeInSeconds > 9999)
            {
                throw new OrderingDomainException("支付有效时间需介于0~9999秒 。 At Order.FromShopToPlatform()");
            }

            //Build shop info and validate.
            var shopInfo = new ShopInfo(
                shopId,
                shopOrderId,
                shopReturnUrl,
                shopOkReturnUrl,
                rateRebateShop,
                rateRebateFinal
                );

            //Checking the shop order amount is a positive integer and greater than 0.
            if (shopOrderAmount <= 0 || decimal.Round(shopOrderAmount, 0) != shopOrderAmount)
            {
                throw new OrderingDomainException("商户订单金额需为正整数。 At Order.FromShopToPlatform()");
            }

            //If the payment channel is alipay, checking the alipay page preference is provided.
            if (orderPaymentChannel.Id == OrderPaymentChannel.Alipay.Id)
            {
                if (alipayPagePreference is null)
                {
                    throw new OrderingDomainException("必须提供支付页面组态。 At Order.FromShopToPlatform()");
                }
            }

            var currentDateTime = dateTimeService.GetCurrentDateTime();

            var order = new Order(
                OrderType.ShopToPlatform.Id,
                false,
                expirationTimeInSeconds,
                shopInfo,
                orderPaymentChannel.Id,
                orderPaymentScheme.Id,
                alipayPagePreference,
                null,
                shopOrderAmount,
                currentDateTime
                );

            return order;
        }

        public static Order FromShopToFourthParty(
            string shopId, string shopOrderId, string shopReturnUrl, string shopOkReturnUrl,
            decimal rateRebateShop, decimal rateRebateFinal, decimal shopOrderAmount,
            OrderPaymentChannel orderPaymentChannel, OrderPaymentScheme orderPaymentScheme,
            string fourthPartyId, string fourthPartyName,
            IDateTimeService dateTimeService)
        {
            //Build shop info and validate.
            var shopInfo = new ShopInfo(
                shopId,
                shopOrderId,
                shopReturnUrl,
                shopOkReturnUrl,
                rateRebateShop,
                rateRebateFinal
                );

            //Checking the shop order amount is a positive integer and greater than 0.
            if (shopOrderAmount <= 0 || decimal.Round(shopOrderAmount, 0) != shopOrderAmount)
            {
                throw new OrderingDomainException("商户订单金额需为正整数。 At Order.FromShopToPlatform()");
            }

            //Build and validate fourth party info.
            var payeeInfo = PayeeInfo.ToFourthParty(fourthPartyId, fourthPartyName);

            var currentDateTime = dateTimeService.GetCurrentDateTime();

            var order = new Order(
                OrderType.ShopToPlatform.Id,
                false,
                null,
                shopInfo,
                orderPaymentChannel.Id,
                orderPaymentScheme.Id,
                null,
                payeeInfo,
                shopOrderAmount,
                currentDateTime
                );

            return order;
        }

        public static Order FromAdminToPlatform(int expirationTimeInSeconds, OrderPaymentChannel orderPaymentChannel, OrderPaymentScheme orderPaymentScheme, decimal orderAmount, IDateTimeService dateTimeService, AlipayPagePreference alipayPagePreference = null)
        {
            //Checking the expiration time's value is positive and less than 9999.
            if (expirationTimeInSeconds < 0 || expirationTimeInSeconds > 9999)
            {
                throw new OrderingDomainException("支付有效时间需介于0~9999秒 。 At Order.FromAdminToPlatform()");
            }

            //Checking the order amount is a positive integer and greater than 0.
            if (orderAmount <= 0 || decimal.Round(orderAmount, 0) != orderAmount)
            {
                throw new OrderingDomainException("订单金额需为正整数。 At Order.FromAdminToPlatform()");
            }

            //If the payment channel is alipay, checking the alipay page preference is provided.
            if (orderPaymentChannel.Id == OrderPaymentChannel.Alipay.Id)
            {
                if (alipayPagePreference is null)
                {
                    throw new OrderingDomainException("必须提供支付页面组态。 At Order.FromAdminToPlatform()");
                }
            }

            var currentDateTime = dateTimeService.GetCurrentDateTime();


            //Create Order
            var order = new Order(
                OrderType.ShopToPlatform.Id,
                true,
                expirationTimeInSeconds,
                null,
                orderPaymentChannel.Id,
                orderPaymentScheme.Id,
                alipayPagePreference,
                null,
                orderAmount,
                currentDateTime
                );

            //Pair Order
            //order.PairedByPlatform(traderId, qrCodeId, 0, dateTimeService);

            return order;
        }


        public void PairedByPlatform(string traderId, int qrCodeId, decimal toppestTradingRate, IDateTimeService dateTimeService)
        {
            //Checking the order is not expired yet.
            if (this.IsExpired)
            {
                throw new OrderingDomainException("无法配对过期订单。 At Order.PairedByPlatform()");
            }

            //Checking the order is at submitted status.
            if (this._orderStatusId != OrderStatus.Submitted.Id)
            {
                throw new OrderingDomainException("订单处于提交状态时才可进行配对。 At Order.PairedByPlatform()");
            }

            //Checking this is a platform order.
            if (this._orderTypeId != OrderType.ShopToPlatform.Id)
            {
                throw new OrderingDomainException("无法与平台二维码配对。 At Order.PairedByPlatform()");
            }
            var payeeInfo = PayeeInfo.ToPlatform(traderId, qrCodeId, toppestTradingRate);

            this.PayeeInfo = payeeInfo;
            this._orderStatusId = OrderStatus.AwaitingPayment.Id;
            this.DatePaired = dateTimeService.GetCurrentDateTime();

            //No need.
            /*this.AddDomainEvent(new OrderPairedByPlatformDomainEvent(
                this,
                payeeInfo));*/
        }

        public void PairedByFourthParty(string fourthPartyId, string fourthPartyOrderPaymentUrl, string fourthPartyOrderNumber, IDateTimeService dateTimeService)
        {
            //Checking the order is not expired yet.
            if (this.IsExpired)
            {
                throw new OrderingDomainException("无法配对过期订单。 At Order.PairedByFourthParty()");
            }

            if (this._orderStatusId != OrderStatus.Submitted.Id)
            {
                throw new OrderingDomainException("订单处于提交状态时才可进行配对。 At Order.PairedByFourthParty()");
            }
            if (this._orderTypeId != OrderType.ShopToFourthParty.Id)
            {
                throw new OrderingDomainException("无法与四方配对。 At Order.PairedByFourthParty()");
            }
            var payeeInfo = this.PayeeInfo.Paired(fourthPartyOrderPaymentUrl, fourthPartyOrderNumber);

            this.PayeeInfo = payeeInfo;
            this._orderStatusId = OrderStatus.AwaitingPayment.Id;
            this.DatePaired = dateTimeService.GetCurrentDateTime();

            this.AddDomainEvent(new OrderPairedByPlatformDomainEvent(this, payeeInfo));
        }


        public void UpdatePayerInfo(string ipAddress, string device, string location)
        {
            //Checking the order is not expired yet.
            if (this.IsExpired)
            {
                throw new OrderingDomainException("无法配对过期订单。 At Order.UpdatePayerInfo()");
            }

            if (this._orderStatusId != OrderStatus.AwaitingPayment.Id)
            {
                throw new OrderingDomainException("订单处于等待付款状态时才可更新付款人信息。 At Order.PairedByFourthParty()");
            }
            var payerInfo = new PayerInfo(ipAddress, device, location);

            this.PayerInfo = payerInfo;
        }

        public void ConfirmPaymentByTrader(string traderId, IDateTimeService dateTimeService)
        {
            //Checking the order is not a test order.
            if (this.IsTestOrder)
            {
                throw new OrderingDomainException("无法确认测试订单。");
            }

            //Checking the order is not expired yet.
            if (this.IsExpired)
            {
                throw new OrderingDomainException("无法确认过期订单。 At Order.ConfirmPaymentByTrader()");
            }

            //Checking the order is not expired yet.
            if (this._orderStatusId != OrderStatus.AwaitingPayment.Id)
            {
                throw new OrderingDomainException("订单处于等待付款状态时才可确认收款。 At Order.ConfirmPaymentByTrader()");
            }

            //Checking this is a platform order.
            if (this._orderTypeId != OrderType.ShopToPlatform.Id)
            {
                throw new OrderingDomainException("此为四方收款订单，交易员无法进行确认收款。 At Order.ConfirmPaymentByTrader()");
            }

            //Checking the trader id match payee info.
            if (this.PayeeInfo.TraderId != traderId)
            {
                throw new OrderingDomainException("交易员无法对他人订单进行确认收款。 At Order.ConfirmPaymentByTrader()");
            }

            this._orderStatusId = OrderStatus.Success.Id;
            this.AmountPaid = this.Amount;

            //The platform's income.
            this.CommissionRealized = this.ShopInfo.RateRebateFinal * this.Amount - this.PayeeInfo.ToppestTradingRate * this.Amount;

            this.DatePaymentRecieved = dateTimeService.GetCurrentDateTime();

            this.AddDomainEvent(new OrderPaymentConfirmedByTraderDomainEvent(
                this,
                traderId));
        }

        public void ConfirmPaymentByFourthParty(string fourthPartyId, IDateTimeService dateTimeService)
        {
            //Checking the order is not expired yet.
            if (this.IsExpired)
            {
                throw new OrderingDomainException("无法配对过期订单。 At Order.ConfirmPaymentByFourthParty()");
            }

            //Checking the order is not expired yet.
            if (this._orderStatusId != OrderStatus.AwaitingPayment.Id)
            {
                throw new OrderingDomainException("订单处于等待付款状态时才可确认收款。 At Order.ConfirmPaymentByFourthParty()");
            }

            //Checking this is a platform order.
            if (this._orderTypeId != OrderType.ShopToFourthParty.Id)
            {
                throw new OrderingDomainException("此为平台内订单，四方无法进行确认收款。 At Order.ConfirmPaymentByFourthParty()");
            }

            //Checking the trader id match payee info.
            if (this.PayeeInfo.FourthPartyId != fourthPartyId)
            {
                throw new OrderingDomainException("四方平台无法对其他四方订单进行确认收款。 At Order.ConfirmPaymentByFourthParty()");
            }

            this._orderStatusId = OrderStatus.Success.Id;
            this.AmountPaid = this.Amount;
            this.CommissionRealized = this.ShopInfo.RateRebateShop * this.AmountPaid;

            this.DatePaymentRecieved = dateTimeService.GetCurrentDateTime();

            this.AddDomainEvent(new OrderPaymentConfirmedByTraderDomainEvent(
                this,
                fourthPartyId));
        }

        public void Expired()
        {
            string description = string.Empty;

            if (this._orderStatusId == OrderStatus.Success.Id)
            {
                throw new OrderingDomainException("已成功订单无法标记为过期订单。 At Order.Expired()");
            }
            else if (this._orderStatusId == OrderStatus.Submitted.Id)
            {
                if (this._orderTypeId == OrderType.ShopToPlatform.Id)
                {
                    description = "没有可用二维码";
                }
                else if (this._orderTypeId == OrderType.ShopToFourthParty.Id)
                {
                    description = "通道异常";
                }
            }
            else if (this._orderStatusId == OrderStatus.AwaitingPayment.Id)
            {
                if (this._orderTypeId == OrderType.ShopToPlatform.Id)
                {
                    description = "支付期限没有收到款项或交易员没有确认收款";
                }
                else if (this._orderTypeId == OrderType.ShopToFourthParty.Id)
                {
                    description = "四方订单付款超时";
                }
            }
            this.IsExpired = true;
            this.OrderStatusDescription = description;

            this.AddDomainEvent(new OrderExpiredDomainEvent(this, this.OrderStatusDescription));
        }

        public void ForcedConfirmation(string byAdminId, string description)
        {
            if (this._orderStatusId != OrderStatus.AwaitingPayment.Id)
            {
                throw new OrderingDomainException("只有等待付款中的订单才可强制确认收款。 At Order.ForcedConfirmation()");
            }

            this._orderStatusId = OrderStatus.Success.Id;
            this.OrderStatusDescription = description;

            this.AddDomainEvent(new OrderForcedConfirmDomainEvent(this, this.OrderStatusDescription));
        }
    }
}
