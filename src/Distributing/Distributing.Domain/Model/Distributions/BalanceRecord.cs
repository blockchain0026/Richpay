using Distributing.Domain.Exceptions;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Model.Distributions
{
    public class BalanceRecord : ValueObject
    {
        public BalanceRecord(decimal balanceBefore, decimal? balanceAfter)
        {
            ValidateBalance(balanceBefore);

            if (balanceAfter != null)
            {
                ValidateBalance((decimal)balanceAfter);
            }

            BalanceBefore = balanceBefore;
            BalanceAfter = balanceAfter;
        }

        public decimal BalanceBefore { get; private set; }
        public decimal? BalanceAfter { get; private set; }

        private void ValidateBalance(decimal amount)
        {
            //Checking the amount's value doesn't has more than 3 points.
            if (decimal.Round(amount, 3) != amount)
            {
                throw new DistributingDomainException("The balance's value must not has more than 2 points" + ". At Order()");
            }

            //Checking the amount is larger than or equal to 0.
            if (amount < 0)
            {
                throw new DistributingDomainException("The balance amount must larger than or equal 0" + ". At Order()");
            }
        }
        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return BalanceBefore;
            yield return BalanceAfter;
        }
    }
}
