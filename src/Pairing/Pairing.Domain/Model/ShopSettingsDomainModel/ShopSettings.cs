using Pairing.Domain.Events;
using Pairing.Domain.Exceptions;
using Pairing.Domain.Model.QrCodes;
using Pairing.Domain.Model.Shared;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pairing.Domain.Model.ShopSettingsDomainModel
{
    public class ShopSettings : Entity, IAggregateRoot
    {
        public string ShopId { get; private set; }

        public bool IsOpen { get; private set; }


        private readonly List<OrderAmountOption> _orderAmountOptions;
        public IReadOnlyCollection<OrderAmountOption> OrderAmountOptions => _orderAmountOptions;


        protected ShopSettings()
        {
            _orderAmountOptions = new List<OrderAmountOption>();
        }

        public ShopSettings(string shopId) : this()
        {
            ShopId = shopId ?? throw new PairingDomainException("The shop Id must be provided. At ShopSettings()");
            IsOpen = false;

            this.AddDomainEvent(new ShopSettingsCreatedDomainEvent(
                this,
                shopId
                ));
        }


        public void AddAmountOption(decimal amount, IDateTimeService dateTimeService)
        {
            //Check the shop is closed.
            if (this.IsOpen)
            {
                throw new PairingDomainException("接单时无法更改订单金额选项" + ". At ShopSettings.UpdateAlipayPreference()");
            }

            //Checking the amount is an integer and is greater than 0.
            if (amount <= 0 || decimal.Round(amount, 0) != amount)
            {
                throw new PairingDomainException("金额必须大于零且为整数" + ". At ShopSettings.AddAmountOption()");
            }

            //Checking the amount option is not duplicated.
            if (this._orderAmountOptions.Any(o => o.GetAmount() == amount))
            {
                throw new PairingDomainException("不可重复添加同样金额的选项" + ". At ShopSettings.AddAmountOption()");
            }
            var dateCreated = dateTimeService.GetCurrentDateTime();

            this._orderAmountOptions.Add(new OrderAmountOption(
                this.ShopId,
                amount,
                dateCreated
                ));

            this.AddDomainEvent(new ShopSettingsAmountOptionCreatedDomainEvent(
                this,
                amount,
                dateCreated
                ));
        }

        public void DeleteAmountOption(decimal amount)
        {
            //Checking the amount is an integer and is greater than 0.
            /*if (amount <= 0 || decimal.Round(amount, 0) != amount)
            {
                throw new PairingDomainException("金额必须大于零且为整数" + ". At ShopSettings.DeleteAmountOption()");
            }*/

            //Check the shop is closed.
            if (this.IsOpen)
            {
                throw new PairingDomainException("营业期间无法更改订单金额选项" + ". At ShopSettings.UpdateAlipayPreference()");
            }

            //Checking the amount option is exist.
            var existingAmountOption = this._orderAmountOptions.FirstOrDefault(o => o.GetAmount() == amount);

            if (existingAmountOption is null)
            {
                throw new PairingDomainException("找不到此金额选项" + ". At ShopSettings.DeleteAmountOption()");
            }

            this._orderAmountOptions.Remove(existingAmountOption);

            this.AddDomainEvent(new ShopSettingsAmountOptionDeletedDomainEvent(
                this,
                amount
                ));
        }

        public void Open()
        {
            this.IsOpen = true;

            this.AddDomainEvent(new ShopSettingsOpenedDomainEvent(this, this.IsOpen));
        }

        public void Close()
        {
            this.IsOpen = false;

            this.AddDomainEvent(new ShopSettingsClosedDomainEvent(this, this.IsOpen));
        }
    }
}
