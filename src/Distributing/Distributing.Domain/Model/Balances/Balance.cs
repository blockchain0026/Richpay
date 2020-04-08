using Distributing.Domain.Events;
using Distributing.Domain.Exceptions;
using Distributing.Domain.Helpers;
using Distributing.Domain.Model.Banks;
using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Deposits;
using Distributing.Domain.Model.Distributions;
using Distributing.Domain.Model.Frozens;
using Distributing.Domain.Model.Roles;
using Distributing.Domain.Model.Shared;
using Distributing.Domain.Model.Transfers;
using Distributing.Domain.Model.Withdrawals;
using Distributing.Domain.SeedWork;
using Distributing.Domain.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributing.Domain.Model.Balances
{
    public class Balance : Entity, IAggregateRoot
    {
        //public string BalanceId { get; private set; }
        public string UserId { get; private set; }

        public UserType UserType { get; private set; }
        private int _userTypeId;

        public decimal AmountAvailable { get; private set; }
        //public decimal AmountFrozen { get; private set; }

        public WithdrawalLimit WithdrawalLimit { get; private set; }

        public decimal WithdrawalCommissionRate { get; private set; }
        public decimal DepositCommissionRate { get; private set; }

        private readonly List<BalanceWithdrawal> _balanceWithdrawals;
        public IReadOnlyCollection<BalanceWithdrawal> BalanceWithdrawals => _balanceWithdrawals;





        public UserType GetUserType => UserType.From(this._userTypeId);

        //To prevent concurrency conflics.
        public byte[] RowVersion { get; set; }


        protected Balance()
        {
            _balanceWithdrawals = new List<BalanceWithdrawal>();
        }

        //For efficient update purpose.
        public Balance(int id, string userId, decimal amountAvailable, byte[] rowVersion)
        {
            this.Id = id;
            this.UserId = userId;
            this.AmountAvailable = amountAvailable;
            this.RowVersion = rowVersion;
        }
        //For efficient update purpose.
        public Balance(int id, decimal amountAvailable, byte[] rowVersion)
        {
            this.Id = id;
            this.AmountAvailable = amountAvailable;
            this.RowVersion = rowVersion;
        }

        public Balance(string userId, int userTypeId, WithdrawalLimit withdrawalLimit, decimal withdrawalCommissionRate, decimal depositCommissionRate) : this()
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            _userTypeId = userTypeId;

            WithdrawalLimit = withdrawalLimit ?? throw new ArgumentNullException(nameof(withdrawalLimit));

            WithdrawalCommissionRate = withdrawalCommissionRate;
            DepositCommissionRate = depositCommissionRate;

            AmountAvailable = 0;

            this.AddDomainEvent(new BalanceCreatedDomainEvent(
                this,
                userId,
                userTypeId,
                withdrawalLimit,
                withdrawalCommissionRate,
                depositCommissionRate
                ));
        }

        public static Balance From(string userId, UserType userType,
            decimal dailyAmountLimit, int dailyFrequencyLimit, decimal eachAmountUpperLimit, decimal eachAmountLowerLimit,
            decimal withdrawalCommissionRate, decimal depositCommissionRate)
        {
            //Checking the user's Id isn't null or empty.
            if (string.IsNullOrEmpty(userId))
            {
                throw new DistributingDomainException("Invalid Param: " + nameof(userId) + ". At Balance.From()");
            }

            ValidateCommissionRate(withdrawalCommissionRate);
            ValidateCommissionRate(depositCommissionRate);

            var withdrawalLimit = new WithdrawalLimit(
                dailyAmountLimit,
                dailyFrequencyLimit,
                eachAmountUpperLimit,
                eachAmountLowerLimit
                );

            var balance = new Balance(
                userId,
                userType.Id,
                withdrawalLimit,
                withdrawalCommissionRate,
                depositCommissionRate
                );


            return balance;
        }


        public void UpdateWithdrawalLimit(decimal dailyAmountLimit, int dailyFrequencyLimit, decimal eachAmountUpperLimit, decimal eachAmountLowerLimit)
        {
            var withdrawalLimit = new WithdrawalLimit(
               dailyAmountLimit,
               dailyFrequencyLimit,
               eachAmountUpperLimit,
               eachAmountLowerLimit
               );

            this.WithdrawalLimit = withdrawalLimit;

            this.AddDomainEvent(new BalanceWithdrawalLimitUpdatedDomainEvent(
                this,
                withdrawalLimit
                ));
        }

        public void UpdateWithdrawalFeeRate(decimal rate)
        {
            ValidateCommissionRate(rate);

            this.WithdrawalCommissionRate = rate;

            this.AddDomainEvent(new BalanceWithdrawalFeeRateUpdatedDomainEvent(this, rate));
        }

        public void UpdateDepositFeeRate(decimal rate)
        {
            ValidateCommissionRate(rate);

            this.DepositCommissionRate = rate;

            this.AddDomainEvent(new BalanceDepositFeeRateUpdatedDomainEvent(this, rate));
        }

        public void Freeze(Admin byAdmin, decimal amount, string description, IDateTimeService dateTimeService)
        {
            //Checking the frozen amount's value doesn't has more than 3 points.
            /*if (decimal.Round(amount, 3) != amount)
            {
                throw new DistributingDomainException("The frozen amount's value rates must not has more than 2 points" + ". At Balance.Frozen()");
            }

            if (amount <= 0)
            {
                throw new DistributingDomainException("The frozen amount must larger than 0" + ". At Balance.Frozen()");
            }

            if (amount > this.AmountAvailable)
            {
                throw new DistributingDomainException("The frozen amount must less than or equal to available amount" + ". At Balance.Frozen()");
            }

            if (byAdmin == null || string.IsNullOrWhiteSpace(byAdmin.AdminId))
            {
                throw new DistributingDomainException("The admin must be provided" + ". At Balance." + DistributingDomainHelper.GetMethodString() + "()");
            }*/
            //These logics above are already in Frozen.FromAdmin().



            //This line must be above other lines that modify balance entity.
            var frozen = Frozen.FromAdmin(
                this,
                byAdmin,
                amount,
                description,
                dateTimeService);

            this.AmountAvailable -= amount;


            this.AddDomainEvent(new BalanceFrozeByAdminDomainEvent(
                this,
                amount,
                this.AmountAvailable,
                byAdmin,
                frozen
                ));
        }

        public void Unfrozen(Frozen frozen)
        {
            //Checking the frozen is not null.
            if (frozen == null)
            {
                throw new DistributingDomainException("The frozen must be provided" + ". At Balance.Unfrozen()");
            }

            //Checking the frozen is at right status.
            if (frozen.GetFrozenStatus.Id != FrozenStatus.Unfrozen.Id)
            {
                throw new DistributingDomainException("The frozen must be provided" + ". At Balance.Unfrozen()");
            }

            //Checking the forzen's balance Id is equal to balance's id.
            if (frozen.BalanceId != this.Id)
            {
                throw new DistributingDomainException("The balance id didn't matched" + ". At Balance.Unfrozen()");
            }

            if (frozen.FrozenType.Id == FrozenType.Withdrawal.Id)
            {
                if (frozen.WithdrawalId == null)
                {
                    throw new DistributingDomainException("Frozen's withdrawal id is missing." + ". At Balance.Unfrozen()");
                }
                this.RefundFrom((int)frozen.WithdrawalId, frozen.Amount);

            }
            else
            {
                var frozenAmount = frozen.Amount;
                this.AmountAvailable = this.AmountAvailable + frozenAmount;
            }

            this.AddDomainEvent(new BalanceUnfrozenDomainEvent(
                this,
                frozen.Amount,
                this.AmountAvailable
                ));
        }

        public void Withdraw(decimal amount, WithdrawalBank withdrawalBank, string accountName, string accountNumber, string description, IDateTimeService dateTimeService, IOpenTimeService openTimeService)
        {
            var withdrawal = Withdrawal.FromUser(this, withdrawalBank, accountName, accountNumber, amount, description, dateTimeService, openTimeService);
            //var balanceWithdrawal = new BalanceWithdrawal(withdrawal.Id, withdrawal.TotalAmount, withdrawal.DateCreated);

            if (!openTimeService.IsWithdrawalOpenNow(withdrawal.DateCreated))
            {
                throw new DistributingDomainException("Non-business hours for withdrawal" + ". At Balance.Withdraw()");
            }



            if (amount > this.AmountAvailable)
            {
                throw new DistributingDomainException("提现金额不能超过可用余额" + ". At Balance.Withdraw()");
            }

            //The amount must less than 0 point.
            if (decimal.Round(amount, 0) != amount)
            {
                throw new DistributingDomainException("The withdrawal amount's point must less than 0" + ". At Balance.Withdraw()");
            }


            var balanceBefore = this.AmountAvailable;


            this.AmountAvailable = this.AmountAvailable - amount;
            //this._balanceWithdrawals.Add(balanceWithdrawal);

            var balanceAfter = this.AmountAvailable;


            this.AddDomainEvent(new BalanceWithdrawalCreatedDomainEvent(
                this,
                amount,
                withdrawal.DateCreated,
                withdrawal,
                balanceBefore,
                balanceAfter
                ));
        }

        public void WithdrawByAdmin(Admin admin, decimal amount, string description, IDateTimeService dateTimeService)
        {
            var withdrawal = Withdrawal.FromAdmin(
                this,
                admin,
                amount,
                description,
                dateTimeService
                );

            if (amount > this.AmountAvailable)
            {
                throw new DistributingDomainException("The withdraw amount must less than or equal to balance's available amount" + ". At Balance.Withdraw()");
            }

            //The amount must less than 3 point.
            if (decimal.Round(amount, 3) != amount)
            {
                throw new DistributingDomainException("The withdrawal amount's point must less than 3" + ". At Balance.Withdraw()");
            }


            var balanceBefore = this.AmountAvailable;

            this.AmountAvailable = this.AmountAvailable - amount;

            var balanceAfter = this.AmountAvailable;


            this.AddDomainEvent(new BalanceWithdrawalCreatedDomainEvent(
                this,
                amount,
                withdrawal.DateCreated,
                withdrawal,
                balanceBefore,
                balanceAfter
                ));
        }

        public void WithdrawalCreated(Withdrawal withdrawal)
        {
            if (withdrawal == null)
            {
                throw new DistributingDomainException("The withdrawal must be provided" + ". At Balance.WithdrawalCreated()");
            }

            var balanceWithdrawal = new BalanceWithdrawal(withdrawal.Id, withdrawal.TotalAmount, withdrawal.DateCreated);

            //Checking if the withdrawals has exceeded the daily limit.
            if (this.WithdrawalLimit.HasExceededTheDailyLimit(this._balanceWithdrawals, balanceWithdrawal))
            {
                throw new DistributingDomainException("提现金额超出今日限制:" + this.WithdrawalLimit.DailyAmountLimit + "¥. At Balance.WithdrawalCreated()");
            }

            //Checking if the withdrawal times has exceeded the daily limit.
            if (this.WithdrawalLimit.HasExceededTheDailyFrquencyLimit(this._balanceWithdrawals, balanceWithdrawal))
            {
                throw new DistributingDomainException("提现次数超出今日限制:" + this.WithdrawalLimit.DailyFrequencyLimit + "笔. At Balance.WithdrawalCreated()");
            }

            //Checking if the new withdrawal's amount did't exceed the limit.
            if (this.WithdrawalLimit.HasExceededTheEachAmountLimit(balanceWithdrawal))
            {
                throw new DistributingDomainException("提现金额超出每笔限制:"
                    + this.WithdrawalLimit.EachAmountLowerLimit
                    + "¥ ~ " + this.WithdrawalLimit.EachAmountUpperLimit
                    + "¥. At Balance.WithdrawalCreated()");
            }

            this._balanceWithdrawals.Add(balanceWithdrawal);
        }

        public void WithdrawSuccess(Withdrawal withdrawal)
        {
            //Checking if withdrawal isn't null.
            if (withdrawal == null)
            {
                throw new DistributingDomainException("The withdrawal must be provided" + ". At Balance.WithdrawSuccess()");
            }

            var existingWithdrawal = this._balanceWithdrawals.Where(b => b.WithdrawalId == withdrawal.Id).SingleOrDefault();

            if (existingWithdrawal == null)
            {
                throw new DistributingDomainException("No matching withdrawal found on balance." + ". At Balance.WithdrawSuccess()");
            }

            existingWithdrawal.Success(withdrawal);
        }

        private void RefundFrom(int withdrawalId, decimal refundAmount)
        {
            var existingWithdrawal = this._balanceWithdrawals.Where(b => b.WithdrawalId == withdrawalId).SingleOrDefault();


            if (existingWithdrawal == null)
            {
                throw new DistributingDomainException("No matching withdrawal found on balance." + ". At Balance.RefundFrom()");
            }

            //Checking the withdrawal entitiy stored in balance is at right status.
            if (existingWithdrawal.IsSuccess() || existingWithdrawal.IsFailed())
            {
                throw new DistributingDomainException("The withdrawal is at wrong status." + ". At Balance.RefundFrom()");
            }

            existingWithdrawal.Failed();

            this.AmountAvailable = this.AmountAvailable + refundAmount;

            this.AddDomainEvent(new BalanceRefundedFromWithdrawalDomainEvent(
                this,
                withdrawalId,
                refundAmount,
                this.AmountAvailable
                ));
        }

        public void Deposit(Deposit deposit)
        {
            //Checking the deposit isn't null.
            if (deposit == null || deposit.IsTransient())
            {
                throw new DistributingDomainException("The withdrawal must be provided" + ". At Balance.Deposit()");
            }

            //Checking the deposit's balance id is matched.
            if (deposit.BalanceId != this.Id)
            {
                throw new DistributingDomainException("The deposit's balance id didn't match this balance" + ". At Balance.Deposit()");
            }

            if (deposit.GetDepositStatus.Id != DepositStatus.Success.Id)
            {
                throw new DistributingDomainException("Can not deposit if it isn't approved (isn't at success status)" + ". At Balance.Deposit()");
            }

            //Checking the deposit amount is positive.
            if (deposit.ActualAmount <= 0)
            {
                throw new DistributingDomainException("The deposit amount must be positive" + ". At Balance.Deposit()");
            }


            this.AmountAvailable = this.AmountAvailable + deposit.ActualAmount;
        }

        public void Distribute(Distribution distribution, int qrCodeId)
        {
            //Checking the distribution isn't null.
            if (distribution == null || distribution.IsTransient())
            {
                throw new DistributingDomainException("The distribution must be provided" + ". At Balance.Distribute()");
            }

            //Checking the distribution's balance id is matched.
            if (distribution.BalanceId != this.Id)
            {
                throw new DistributingDomainException("The distribution's balance id didn't match this balance" + ". At Balance.Distribute()");
            }

            //Checking the distribution amount is positive.
            if (distribution.DistributedAmount <= 0)
            {
                throw new DistributingDomainException("The distributed amount must be positive" + ". At Balance.Distribute()");
            }

            var distributedAmount = distribution.DistributedAmount;

            this.AmountAvailable = this.AmountAvailable + distributedAmount;

            this.AddDomainEvent(new BalanceDistributedDomainEvent(
                this,
                distribution,
                qrCodeId
                ));
        }

        /// <summary>
        /// Call by ResolveCompletedDistributionBackgroundTaskService.
        /// Add sum amount of distributions created recently to the user's balance.
        /// 
        /// Call by BalanceDomainService.
        /// </summary>
        /// <param name="distributedAmount"></param>
        public void Distribute(decimal distributedAmount)
        {
            this.AmountAvailable = this.AmountAvailable + distributedAmount;
        }

        public void TransferOut(Transfer transfer)
        {
            //Checking the transfer isn't null.
            if (transfer == null)
            {
                throw new DistributingDomainException("The transfer must be provided" + ". At Balance.TransferOut()");
            }

            //Checking the transfer's balance id is matched.
            if (transfer.FromBalanceId != this.Id)
            {
                throw new DistributingDomainException("The transfer's balance id didn't match this balance" + ". At Balance.TransferOut()");
            }


            if (transfer.Amount > this.AmountAvailable)
            {
                throw new DistributingDomainException("The transfer amount must less than or equal to balance's available amount" + ". At Balance.TransferOut()");
            }


            var transferAmount = transfer.Amount;

            this.AmountAvailable = this.AmountAvailable - transferAmount;
        }

        public void TransferIn(Transfer transfer)
        {
            //Checking the transfer isn't null.
            if (transfer == null)
            {
                throw new DistributingDomainException("The transfer must be provided" + ". At Balance.TransferOut()");
            }

            //Checking the transfer's balance id is matched.
            if (transfer.ToBalanceId != this.Id)
            {
                throw new DistributingDomainException("The transfer's balance id didn't match this balance" + ". At Balance.TransferOut()");
            }

            var transferAmount = transfer.Amount;

            this.AmountAvailable = this.AmountAvailable + transferAmount;
        }

        public void NewOrder(Order order, IDateTimeService dateTimeService)
        {
            if (order == null)
            {
                throw new DistributingDomainException("The order must be provided" + ". At Balance.NewOrder()");
            }

            //Validation logics are in side frozen.
            //This line must be above other lines that modify balance entity.
            /*var frozen = Frozen.FromOrder(this, order, dateTimeService);*/
            //Remove frozen logics(change balance directly)

            //Update available amount.
            //var frozenAmount = frozen.Amount;
            this.AmountAvailable = this.AmountAvailable - order.Amount;

            /*this.AddDomainEvent(new BalanceFrozeFromNewOrderDomainEvent(
                this,
                order.Amount,
                this.AmountAvailable,
                null
                ));*/
            /*this.AddDomainEvent(new BalanceFrozeFromNewOrderDomainEvent(
                this,
                frozenAmount,
                this.AmountAvailable,
                frozen
                ));*/
        }

        private static void ValidateCommissionRate(decimal commissionRate)
        {
            //Checking the withdrawal & deposit commission rates is larger or equal to 0 and less than 1.
            if (commissionRate < 0 || commissionRate >= 1)
            {
                throw new DistributingDomainException("The withdrawal & deposit commission rates must be larger or equal to 0 and less than 1" + ". At Balance.ValidateCommissionRate()");
            }

            //Checking the withdrawal & deposit commission rates doesn't has more than 3 points.
            if (decimal.Round(commissionRate, 3) != commissionRate)
            {
                throw new DistributingDomainException("The withdrawal & deposit commission rates must not has more than 3 points" + ". At Balance.ValidateCommissionRate()");
            }
        }
    }
}
