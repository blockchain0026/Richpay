using Pairing.Domain.Exceptions;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Model.ShopSettingsDomainModel
{
    public class OrderAmountOption
     : Entity
    {
        // DDD Patterns comment
        // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (Instead of properties and property collections)
        private decimal _amount;
        private DateTime _dateCreated;

        public string ShopId { get; private set; }

        protected OrderAmountOption() { }

        public OrderAmountOption(string shopId, decimal amount, DateTime dateCreated)
        {
            //Checking the shop id is provided.
            if (string.IsNullOrWhiteSpace(shopId))
            {
                throw new PairingDomainException("The withdrawal id must be provided" + ". At OrderAmountOption()");
            }


            //Checking the amount is larger than 0.
            if (amount <= 0)
            {
                throw new PairingDomainException("金额必须大于0" + ". At OrderAmountOption()");
            }

            //Checking the amount is an integer.
            if (decimal.Round(amount, 0) != amount)
            {
                throw new PairingDomainException("金额必须为整数(不可包含小数点)" + ". At OrderAmountOption()");
            }


            ShopId = shopId;
            _amount = amount;
            _dateCreated = dateCreated;
        }

        public decimal GetAmount() => _amount;
        public DateTime GetDateCreated() => _dateCreated;
    }
}
