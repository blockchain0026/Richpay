using Distributing.Domain.Events;
using Distributing.Domain.Exceptions;
using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Shared;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Model.Distributions
{
    public class Distribution : Entity, IAggregateRoot
    {
        //public string DistributionId { get; private set; }

        public DistributionType DistributionType { get; private set; }
        private int _distributionTypeId;

        public string UserId { get; private set; }
        public int CommissionId { get; private set; }

        public Order Order { get; private set; }
        public int BalanceId { get; private set; }

        public decimal DistributedAmount { get; private set; }
        public DateTime? DateDistributed { get; private set; }

        public bool IsFinished { get; private set; }

        public DistributionType GetDistributionType => DistributionType.From(this._distributionTypeId);

        protected Distribution()
        {
        }

        public Distribution(int distributionId)
        {
            this.Id = distributionId;
        }

        public Distribution(int distributionTypeId, int commissionId, string userId, Order order, int balanceId, decimal distributedAmount) : this()
        {
            _distributionTypeId = distributionTypeId;
            CommissionId = commissionId;
            UserId = userId;
            Order = order;
            BalanceId = balanceId;
            DistributedAmount = distributedAmount;
            //DateDistributed = dateDistributed;
            IsFinished = true;
            //this.DistributionId = Guid.NewGuid().ToString();
        }

        public static Distribution FromCommission(Order order, string userId, int balanceId, int commissionId, string downlineUserId)
        {
            //Checking the order isn't null.
            if (order == null)
            {
                throw new DistributingDomainException("The order must be provided" + ". At Distribution.FromCommission()");
            }

            //Checking the balance isn't null.
            /*if (balance == null || balance.IsTransient())
            {
                throw new DistributingDomainException("The balance must be provided" + ". At Distribution.FromCommission()");
            }*/

            //Checking the user type is correct.
            /*if (balance.GetUserType.Id != UserType.ShopAgent.Id && balance.GetUserType.Id != UserType.Trader.Id && balance.GetUserType.Id != UserType.TraderAgent.Id)
            {
                throw new DistributingDomainException("The user type is incorrect" + ". At Distribution.FromCommission()");
            }*/

            //Checking the commission isn't null.
            /*if (commission == null || commission.IsTransient())
            {
                throw new DistributingDomainException("The commission must be provided" + ". At Distribution.FromCommission()");
            }*/

            //Checking the commission and the balance are belong to the same user.
            /*if (string.IsNullOrEmpty(commission.UserId) || commission.UserId != balance.UserId)
            {
                throw new DistributingDomainException("The commission and the balance must belong to the same user" + ". At Distribution.FromCommission()");
            }*/

            //Checking the commission is enabled for distributing.
            /*if (!commission.IsEnabled)
            {
                throw new DistributingDomainException("The commission must be enabled for distributing." + ". At Distribution.FromCommission()");
            }*/

            //Checking the distribution date is provided.
            /*if (dateDistributed == null)
            {
                throw new DistributingDomainException("The distribution date must be provided" + ". At Distribution.FromCommission()");
            }*/

            var distributedAmount = order.CommissionAmount;

            //Checking the distributed amount is larger than 0.
            if (distributedAmount <= 0)
            {
                throw new DistributingDomainException("The distributed amount must be larger than 0" + ". At Distribution.FromCommission()");
            }

            //Checking the distributed amount doesn't has more than 3 points.
            if (decimal.Round(distributedAmount, 3) != distributedAmount)
            {
                throw new DistributingDomainException("The distributed amount must not has more than 3 points" + ". At Distribution.FromCommission()");
            }

            var distribution = new Distribution(
                DistributionType.Commission.Id,
                commissionId,
                userId,
                order,
                balanceId,
                distributedAmount
                );

            distribution.AddDomainEvent(new DistributionCreatedDomainEvent(
                distribution,
                DistributionType.Commission.Id,
                commissionId,
                order,
                balanceId,
                distributedAmount,
                downlineUserId
                ));

            return distribution;
        }

        public static Distribution FromLiquidation(Order order, string userId, int balanceId, int commissionId, string downlineUserId)
        {
            //Checking the order isn't null.
            if (order == null)
            {
                throw new DistributingDomainException("The order must be provided" + ". At Distribution.FromCommission()");
            }

            //Checking the balance isn't null.
            /*if (balance == null || balance.IsTransient())
            {
                throw new DistributingDomainException("The balance must be provided" + ". At Distribution.FromCommission()");
            }*/

            //Checking the user type is correct.
            /*if (balance.GetUserType.Id != UserType.Shop.Id)
            {
                throw new DistributingDomainException("The user type is incorrect" + ". At Distribution.FromCommission()");
            }*/

            //Checking the commission isn't null.
            /*if (commission == null || commission.IsTransient())
            {
                throw new DistributingDomainException("The commission must be provided" + ". At Distribution.FromCommission()");
            }*/

            //Checking the commission and the balance are belong to the same user.
            /*if (string.IsNullOrEmpty(commission.UserId) || commission.UserId != balance.UserId)
            {
                throw new DistributingDomainException("The commission and the balance must belong to the same user" + ". At Distribution.FromCommission()");
            }*/

            //Checking the commission is enabled for distributing.
            /*if (!commission.IsEnabled)
            {
                throw new DistributingDomainException("The commission must be enabled for distributing." + ". At Distribution.FromCommission()");
            }*/

            //Checking the distribution date is provided.
            /*if (dateDistributed == null)
            {
                throw new DistributingDomainException("The distribution date must be provided" + ". At Distribution.FromCommission()");
            }*/

            var distributedAmount = order.CommissionAmount;

            //Checking the distributed amount is larger than 0.
            if (distributedAmount <= 0)
            {
                throw new DistributingDomainException("The distributed amount must be larger than 0" + ". At Distribution.FromCommission()");
            }

            //Checking the distributed amount doesn't has more than 3 points.
            if (decimal.Round(distributedAmount, 3) != distributedAmount)
            {
                throw new DistributingDomainException("The distributed amount must not has more than 3 points" + ". At Distribution.FromCommission()");
            }

            var distribution = new Distribution(
                DistributionType.Liquidation.Id,
                commissionId,
                userId,
                order,
                balanceId,
                distributedAmount
                );

            /*distribution.AddDomainEvent(new DistributionCreatedDomainEvent(
                distribution,
                DistributionType.Liquidation.Id,
                commissionId,
                order,
                balanceId,
                distributedAmount,
                downlineUserId
                ));*/

            return distribution;
        }



        /// <summary>
        /// Deprecated
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="dateTimeService"></param>
        public void OrderCompleted(bool isSuccess, IDateTimeService dateTimeService)
        {
            if (isSuccess)
            {
                this.DistributedAmount = this.Order.CommissionAmount;
                this.DateDistributed = dateTimeService.GetCurrentDateTime();
            }
            else
            {
                this.DistributedAmount = 0;
            }

            this.IsFinished = true;
            /*this.AddDomainEvent(new DistributionFinishedDomainEvent(
                this,
                this.DistributedAmount,
                isSuccess
                ));*/
        }
    }
}
