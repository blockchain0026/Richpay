using Pairing.Domain.Exceptions;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pairing.Domain.Model.QrCodes
{
    public class Quota : ValueObject
    {
        public decimal DailyAmountLimit { get; private set; }
        public decimal OrderAmountUpperLimit { get; private set; }
        public decimal OrderAmountLowerLimit { get; private set; }

        public Quota(decimal dailyAmountLimit,decimal orderAmountUpperLimit, decimal orderAmountLowerLimit)
        {
            //Checking the quota limit value is correct.
            if (dailyAmountLimit < 0 || orderAmountUpperLimit < 0 || orderAmountLowerLimit < 0
                || orderAmountLowerLimit > orderAmountUpperLimit)
            {
                throw new PairingDomainException("无效的额度限制参数" + ". At Quota()");
            }
            else if (decimal.Round(dailyAmountLimit, 0) != dailyAmountLimit
                || decimal.Round(orderAmountUpperLimit, 0) != orderAmountUpperLimit
                || decimal.Round(orderAmountLowerLimit, 0) != orderAmountLowerLimit)
            {
                throw new PairingDomainException("额度限制参数必须为整数" + ". At Quota()");
            }

            DailyAmountLimit = dailyAmountLimit;
            OrderAmountUpperLimit = orderAmountUpperLimit;
            OrderAmountLowerLimit = orderAmountLowerLimit;
        }

        public bool HasExceededTheDailyLimit(List<QrCodeOrder> orders, QrCodeOrder newOrder = null)
        {
            if (orders == null)
            {
                throw new PairingDomainException("The order list must be provided" + ". At Quota.HasExceededTheDailyLimit()");
            }

            //Calculate processing order and success order.
            var validOrders = orders.Where(w => w.IsFailed() != true);

            decimal totalAmount = 0;

            foreach (var order in validOrders)
            {
                totalAmount = totalAmount + order.GetAmount();
            }

            if (newOrder != null)
            {
                totalAmount = totalAmount + newOrder.GetAmount();
            }

            if (totalAmount > this.DailyAmountLimit)
            {
                return true;
            }

            return false;
        }


        public bool HasExceededTheOrderAmountLimit(QrCodeOrder order)
        {
            if (order == null)
            {
                throw new PairingDomainException("The order must be provided" + ". At Quota.HasExceededTheOrderAmountLimit()");
            }
            var amount = order.GetAmount();


            if (amount > this.OrderAmountUpperLimit || amount < this.OrderAmountLowerLimit)
            {
                return true;
            }

            return false;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return DailyAmountLimit;
            yield return OrderAmountUpperLimit;
            yield return OrderAmountLowerLimit;
        }
    }
}
