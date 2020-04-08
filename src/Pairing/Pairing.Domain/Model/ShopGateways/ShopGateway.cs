using Pairing.Domain.Events;
using Pairing.Domain.Exceptions;
using Pairing.Domain.Model.FourthPartyGateways;
using Pairing.Domain.Model.QrCodes;
using Pairing.Domain.Model.Shared;
using Pairing.Domain.Model.ShopSettingsDomainModel;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Model.ShopGateways
{
    public class ShopGateway : Entity, IAggregateRoot
    {
        public string ShopId { get; private set; }

        public ShopGatewayType ShopGatewayType { get; private set; }
        private int _shopGatewayTypeId;

        public PaymentChannel PaymentChannel { get; private set; }
        private int _paymentChannelId;
        public PaymentScheme PaymentScheme { get; private set; }
        private int _paymentSchemeId;

        public AlipayPreference AlipayPreference { get; private set; }

        public int? FourthPartyGatewayId { get; private set; }

        public DateTime DateCreated { get; private set; }

        public ShopGatewayType GetShopGatewayType => ShopGatewayType.From(this._shopGatewayTypeId);
        public PaymentChannel GetPaymentChannel => PaymentChannel.From(this._paymentChannelId);
        public PaymentScheme GetPaymentScheme => PaymentScheme.From(this._paymentSchemeId);

        protected ShopGateway()
        {
        }

        public ShopGateway(string shopId, int shopGatewayTypeId, int paymentChannelId, int paymentSchemeId, DateTime dateCreated, int? fourthPartyGatewayId, AlipayPreference alipayPreference) : this()
        {
            ShopId = shopId ?? throw new ArgumentNullException(nameof(shopId));
            _shopGatewayTypeId = shopGatewayTypeId;
            _paymentChannelId = paymentChannelId;
            _paymentSchemeId = paymentSchemeId;

            DateCreated = dateCreated;
            FourthPartyGatewayId = fourthPartyGatewayId;
            AlipayPreference = alipayPreference;

            this.AddDomainEvent(new ShopGatewayCreatedDomainEvent(
                this,
                shopGatewayTypeId,
                paymentChannelId,
                paymentSchemeId,
                fourthPartyGatewayId,
                alipayPreference,
                dateCreated
                ));
        }


        public static ShopGateway FromPlatForm(string shopId, PaymentChannel paymentChannel, PaymentScheme paymentScheme,
            IDateTimeService dateTimeService,
            int secondsBeforePayment, bool isAmountUnchangeable, bool isAccountUnchangeable, bool isH5RedirectByScanEnabled, bool isH5RedirectByClickEnabled, bool isH5RedirectByPickingPhotoEnabled)
        {
            //Checking the shop Id, gateway number and name are provided.
            if (string.IsNullOrWhiteSpace(shopId))
            {
                throw new PairingDomainException("The shop Id must be provided" + ". At ShopGateway.FromPlatForm()");
            }
            /*if (string.IsNullOrWhiteSpace(gatewayNumber))
            {
                throw new PairingDomainException("无效的通道编号" + ". At ShopGateway.FromPlatForm()");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new PairingDomainException("无效的通道命名" + ". At ShopGateway.FromPlatForm()");
            }*/



            if (!PaymentSchemeValidator.IsPaymentSchemeSupportedBy(paymentChannel, paymentScheme))
            {
                throw new PairingDomainException($"目前支付类型 {paymentChannel.Name} 不支援 {paymentScheme.Name} 通道" + ". At ShopGateway.FromPlatForm()");
            }

            AlipayPreference alipayPreference = null;
            if (paymentChannel.Id == PaymentChannel.Alipay.Id)
            {
                alipayPreference = new AlipayPreference(
                    secondsBeforePayment,
                    isAmountUnchangeable,
                    isAccountUnchangeable,
                    isH5RedirectByScanEnabled,
                    isH5RedirectByClickEnabled,
                    isH5RedirectByPickingPhotoEnabled
                    );
            }


            var shopGateway = new ShopGateway(
                shopId,
                ShopGatewayType.ToPlatform.Id,
                paymentChannel.Id,
                paymentScheme.Id,
                dateTimeService.GetCurrentDateTime(),
                null,
                alipayPreference
                );

            return shopGateway;
        }

        public static ShopGateway FromFourthParty(string shopId,
            FourthPartyGateway fourthPartyGateway, IDateTimeService dateTimeService)
        {
            //Checking the shop Id, gateway number and name are provided.
            if (string.IsNullOrWhiteSpace(shopId))
            {
                throw new PairingDomainException("The shop Id must be provided" + ". At ShopGateway.FromPlatForm()");
            }

            //Checking the fourthPartyGateway is provided.
            if (fourthPartyGateway is null)
            {
                throw new PairingDomainException("The fourth party gateway must be provided" + ". At ShopGateway.FromPlatForm()");
            }

            var paymentChannel = fourthPartyGateway.PaymentChannel;
            var paymentScheme = fourthPartyGateway.PaymentScheme;
            if (!PaymentSchemeValidator.IsPaymentSchemeSupportedBy(paymentChannel, paymentScheme))
            {
                throw new PairingDomainException($"目前支付类型 {paymentChannel.Name} 不支援 {paymentScheme.Name} 通道" + ". At ShopGateway.FromPlatForm()");
            }


            var shopGateway = new ShopGateway(
                shopId,
                ShopGatewayType.ToFourthParty.Id,
                paymentChannel.Id,
                paymentScheme.Id,
                dateTimeService.GetCurrentDateTime(),
                fourthPartyGateway.Id,
                null
                );

            return shopGateway;
        }


        public void UpdateAlipayPreference(int secondsBeforePayment, bool isAmountUnchangeable, bool isAccountUnchangeable,
            bool isH5RedirectByScanEnabled, bool isH5RedirectByClickEnabled, bool isH5RedirectByPickingPhotoEnabled)
        {
            //Check this gateway is to platform and payment channel is alipay.
            if (this._shopGatewayTypeId != ShopGatewayType.ToPlatform.Id || this._paymentChannelId != PaymentChannel.Alipay.Id)
            {
                throw new PairingDomainException("此通道没有支付宝偏好选项" + ". At ShopGateway.FromPlatForm()");
            }

            var alipayPreference = new AlipayPreference(
                secondsBeforePayment,
                isAmountUnchangeable,
                isAccountUnchangeable,
                isH5RedirectByScanEnabled,
                isH5RedirectByClickEnabled,
                isH5RedirectByPickingPhotoEnabled
                );

            this.AlipayPreference = alipayPreference;

            this.AddDomainEvent(new ShopGatewayAlipayPreferenceUpdatedDomainEvent(
                this,
                alipayPreference
                ));
        }


        public void ChangeToPlatform(
            int secondsBeforePayment, bool isAmountUnchangeable, bool isAccountUnchangeable, bool isH5RedirectByScanEnabled, bool isH5RedirectByClickEnabled, bool isH5RedirectByPickingPhotoEnabled)
        {
            AlipayPreference alipayPreference = null;
            if (this._paymentChannelId == PaymentChannel.Alipay.Id)
            {
                alipayPreference = new AlipayPreference(
                    secondsBeforePayment,
                    isAmountUnchangeable,
                    isAccountUnchangeable,
                    isH5RedirectByScanEnabled,
                    isH5RedirectByClickEnabled,
                    isH5RedirectByPickingPhotoEnabled
                    );
            }

            this._shopGatewayTypeId = ShopGatewayType.ToPlatform.Id;
            this.AlipayPreference = alipayPreference;

            this.AddDomainEvent(new ShopGatewayTypeChangedToPlatformDomainEvent(
                this,
                this._shopGatewayTypeId,
                alipayPreference
                ));
        }

        public void ChangeToFourthParty(FourthPartyGateway fourthPartyGateway)
        {
            //Checking the fourthPartyGateway is provided.
            if (fourthPartyGateway is null)
            {
                throw new PairingDomainException("The fourth party gateway must be provided" + ". At ShopGateway.ChangeToFourthParty()");
            }

            var paymentChannel = fourthPartyGateway.PaymentChannel;
            var paymentScheme = fourthPartyGateway.PaymentScheme;
            if (!PaymentSchemeValidator.IsPaymentSchemeSupportedBy(paymentChannel, paymentScheme))
            {
                throw new PairingDomainException($"目前支付类型 {paymentChannel.Name} 不支援 {paymentScheme.Name} 通道" + ". At ShopGateway.ChangeToFourthParty()");
            }

            this._shopGatewayTypeId = ShopGatewayType.ToFourthParty.Id;
            this.FourthPartyGatewayId = fourthPartyGateway.Id;

            this.AddDomainEvent(new ShopGatewayTypeChangedToFourthPartyDomainEvent(
                this,
                this._shopGatewayTypeId,
                fourthPartyGateway
                ));
        }
    }
}
