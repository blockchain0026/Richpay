using Distributing.Domain.Exceptions;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributing.Domain.Model.Balances
{
    public class WithdrawalLimit : ValueObject
    {
        public decimal DailyAmountLimit { get; private set; }
        public int DailyFrequencyLimit { get; private set; }

        public decimal EachAmountUpperLimit { get; private set; }
        public decimal EachAmountLowerLimit { get; private set; }

        public WithdrawalLimit(decimal dailyAmountLimit, int dailyFrequencyLimit, decimal eachAmountUpperLimit, decimal eachAmountLowerLimit)
        {
            //Checking the withdrawal limit value is correct.
            if (dailyAmountLimit < 0 || dailyFrequencyLimit < 0 || eachAmountUpperLimit < 0 || eachAmountLowerLimit < 0
                || eachAmountLowerLimit > eachAmountUpperLimit)
            {
                throw new DistributingDomainException("The withdrawal limit value isn't correct" + ". At Balance.From()");
            }
            else if (decimal.Round(dailyAmountLimit, 0) != dailyAmountLimit
                || decimal.Round(eachAmountUpperLimit, 0) != eachAmountUpperLimit
                || decimal.Round(eachAmountLowerLimit, 0) != eachAmountLowerLimit)
            {
                throw new DistributingDomainException("The withdrawal limit value must be an integer" + ". At Balance.From()");
            }

            DailyAmountLimit = dailyAmountLimit;
            DailyFrequencyLimit = dailyFrequencyLimit;
            EachAmountUpperLimit = eachAmountUpperLimit;
            EachAmountLowerLimit = eachAmountLowerLimit;
        }

        public bool HasExceededTheDailyLimit(List<BalanceWithdrawal> withdrawals, BalanceWithdrawal newWithdrawal = null)
        {
            if (withdrawals == null)
            {
                throw new DistributingDomainException("The withdrawal list must be provided" + ". At WithdrawalLimit.HasExceededTheDailyLimit()");
            }

            var validWithdrawals = withdrawals.Where(w => w.IsFailed() != true);

            decimal totalAmount = 0;

            foreach (var withdrawal in validWithdrawals)
            {
                totalAmount = totalAmount + withdrawal.GetAmount();
            }

            if (newWithdrawal != null)
            {
                totalAmount = totalAmount + newWithdrawal.GetAmount();
            }

            if (totalAmount > this.DailyAmountLimit)
            {
                return true;
            }

            return false;
        }

        public bool HasExceededTheDailyFrquencyLimit(List<BalanceWithdrawal> withdrawals, BalanceWithdrawal newWithdrawal = null)
        {
            if (withdrawals == null)
            {
                throw new DistributingDomainException("The withdrawal list must be provided" + ". At WithdrawalLimit.HasExceededTheDailyFrquencyLimit()");
            }



            int totalNumbers = 0;

            totalNumbers = totalNumbers + withdrawals.Count();

            if (newWithdrawal != null)
            {
                totalNumbers = totalNumbers + 1;
            }

            if (totalNumbers > this.DailyFrequencyLimit)
            {
                return true;
            }

            return false;
        }

        public bool HasExceededTheEachAmountLimit(BalanceWithdrawal withdrawal)
        {
            if (withdrawal == null)
            {
                throw new DistributingDomainException("The withdrawal must be provided" + ". At WithdrawalLimit.HasExceededTheEachAmountLimit()");
            }
            var amount = withdrawal.GetAmount();


            if (amount > this.EachAmountUpperLimit || amount < this.EachAmountLowerLimit)
            {
                return true;
            }

            return false;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return DailyAmountLimit;
            yield return DailyFrequencyLimit;
            yield return EachAmountUpperLimit;
            yield return EachAmountLowerLimit;
        }
    }
}
