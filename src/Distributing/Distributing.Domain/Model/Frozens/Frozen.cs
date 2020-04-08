using Distributing.Domain.Events;
using Distributing.Domain.Exceptions;
using Distributing.Domain.Helpers;
using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Distributions;
using Distributing.Domain.Model.Roles;
using Distributing.Domain.Model.Shared;
using Distributing.Domain.Model.Withdrawals;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Model.Frozens
{
    public class Frozen : Entity, IAggregateRoot
    {
        public FrozenStatus FrozenStatus { get; private set; }
        private int _frozenStatusId;

        public FrozenType FrozenType { get; private set; }
        private int _frozenTypeId;

        public int BalanceId { get; private set; }
        public string UserId { get; private set; }
        public BalanceRecord BalanceFrozenRecord { get; private set; }
        public BalanceRecord BalanceUnfrozenRecord { get; private set; }

        public decimal Amount { get; private set; }
        public string OrderTrackingNumber { get; private set; }
        public int? WithdrawalId { get; private set; }
        public Admin ByAdmin { get; private set; }
        public string Description { get; private set; }

        public DateTime DateFroze { get; private set; }
        public DateTime? DateUnfroze { get; private set; }
        public FrozenStatus GetFrozenStatus => FrozenStatus.From(this._frozenStatusId);
        protected Frozen()
        {
        }

        public Frozen(int balanceId, string userId, int frozenTypeId, decimal amount, BalanceRecord balanceFrozenRecord, string orderTrackingNumber, int? withdrawalId, Admin byAdmin, DateTime dateFroze, string description) : this()
        {
            BalanceId = balanceId;
            UserId = userId;
            BalanceFrozenRecord = balanceFrozenRecord ?? throw new ArgumentNullException(nameof(withdrawalId));

            _frozenTypeId = frozenTypeId;
            _frozenStatusId = FrozenStatus.Frozen.Id;
            Amount = amount;

            if (frozenTypeId == FrozenType.Order.Id)
            {
                OrderTrackingNumber = orderTrackingNumber ?? throw new ArgumentNullException(nameof(orderTrackingNumber));
            }
            else if (frozenTypeId == FrozenType.Withdrawal.Id)
            {
                WithdrawalId = withdrawalId ?? throw new ArgumentNullException(nameof(withdrawalId));
            }
            else if (frozenTypeId == FrozenType.ByAdmin.Id)
            {
                ByAdmin = byAdmin ?? throw new ArgumentNullException(nameof(byAdmin));
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(frozenTypeId));
            }

            DateFroze = dateFroze;
            Description = description;

            this.AddDomainEvent(new FrozenCreatedDomainEvent(
                this,
                balanceId,
                frozenTypeId,
                amount,
                orderTrackingNumber,
                withdrawalId,
                byAdmin,
                dateFroze,
                description
                ));
        }

        public static Frozen FromOrder(Balance balance, Order order, IDateTimeService dateTimeService)
        {
            var amount = order.Amount;

            //Checking the balance isn't null.
            /*if (balance == null)
            {
                throw new DistributingDomainException("The balance must be provided" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the balance is a trader balacne.
            if (balance.GetUserType.Id != UserType.Trader.Id)
            {
                throw new DistributingDomainException("Order can only freeze balance that belongs to traders" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the balance user Id is equal to order's trader Id
            if (balance.UserId != order.TraderId)
            {
                throw new DistributingDomainException("The balance's user id must equal to order's trader Id." +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }*/
            //Remove logics above to improve performance.

            //Checking the balance's available amount is enough.
            if (amount > balance.AmountAvailable)
            {
                throw new DistributingDomainException("The frozen amount must less than balance's available amount" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }



            var currentDateTime = dateTimeService.GetCurrentDateTime();
            if (currentDateTime == null)
            {
                throw new DistributingDomainException("Couldn't retrieve current date time" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }
            var balanceBefore = balance.AmountAvailable;
            var balanceAfter = balanceBefore - amount;

            var balanceRecord = new BalanceRecord(balance.AmountAvailable, balanceAfter);


            var frozen = new Frozen(
                balance.Id,
                balance.UserId,
                FrozenType.Order.Id,
                amount,
                balanceRecord,
                order.TrackingNumber,
                null,
                null,
                currentDateTime,
                null
                );

            return frozen;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="withdrawal"></param>
        /// <param name="balanceBefore">
        /// Because the balance entity is been modified before calling this method,
        /// so we need this param to create BalanceRecord property correctly.
        /// </param>
        /// <param name="balanceAfter">
        /// Same as the description in comment for param 'balanceBefore' .
        /// </param>
        /// <param name="dateTimeService"></param>
        /// <returns></returns>
        public static Frozen FromWithdrawal(Withdrawal withdrawal, decimal balanceBefore, decimal balanceAfter, IDateTimeService dateTimeService)
        {
            //Checking the balance before and after param is positive.
            if (balanceBefore < 0 || balanceAfter < 0)
            {
                throw new DistributingDomainException("Invalid param: balanceBefore, balanceAfter. Must be positive." +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the balance before and after param has less than or equal to 3 point.
            if (decimal.Round(balanceBefore, 3) != balanceBefore
                || decimal.Round(balanceAfter, 3) != balanceAfter)
            {
                throw new DistributingDomainException("Invalid param: balanceBefore, balanceAfter. Must be less than or equal to 3 point." +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the withdrawal isn't null.
            if (withdrawal == null)
            {
                throw new DistributingDomainException("The withdrawal must be provided" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            var amount = withdrawal.TotalAmount;


            var currentDateTime = dateTimeService.GetCurrentDateTime();
            if (currentDateTime == null)
            {
                throw new DistributingDomainException("Couldn't retrieve current date time" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }


            var balanceRecord = new BalanceRecord(balanceBefore, balanceAfter);

            var frozen = new Frozen(
                withdrawal.BalanceId,
                withdrawal.UserId,
                FrozenType.Withdrawal.Id,
                amount,
                balanceRecord,
                null,
                withdrawal.Id,
                null,
                currentDateTime,
                null
                );

            return frozen;
        }

        public static Frozen FromAdmin(Balance balance, Admin admin, decimal amount, string description, IDateTimeService dateTimeService)
        {
            //Checking the balance isn't null.
            if (balance == null)
            {
                throw new DistributingDomainException("The balance must be provided" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the admin isn't null.
            if (admin == null || string.IsNullOrWhiteSpace(admin.AdminId))
            {
                throw new DistributingDomainException("The admin must be provided" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the frozen amount is positive.
            if (amount < 0)
            {
                throw new DistributingDomainException("The frozen amount must be positive" +
                   ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the balance's available amount is enough.
            if (amount > balance.AmountAvailable)
            {
                throw new DistributingDomainException("The frozen amount must less than balance's available amount" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Chekcing the frozen amount's points is less than 3.
            if (decimal.Round(amount, 3) != amount)
            {
                throw new DistributingDomainException("The point of frozen amount must less than 3" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the length of description is not too long.
            if (!string.IsNullOrEmpty(description) && description.Length > 500)
            {
                throw new DistributingDomainException("The length of description must less than 500 charactors" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }


            var currentDateTime = dateTimeService.GetCurrentDateTime();
            if (currentDateTime == null)
            {
                throw new DistributingDomainException("Couldn't retrieve current date time" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            var balanceBefore = balance.AmountAvailable;
            var balanceAfter = balanceBefore - amount;

            var balanceRecord = new BalanceRecord(balance.AmountAvailable, balanceAfter);

            var frozen = new Frozen(
                balance.Id,
                balance.UserId,
                FrozenType.ByAdmin.Id,
                amount,
                balanceRecord,
                null,
                null,
                admin,
                currentDateTime,
                description
                );

            return frozen;
        }

        public void OrderCanceled(Balance balance, Order order, IDateTimeService dateTimeService)
        {
            //Checking the frozen type is correct.
            if (this._frozenTypeId != FrozenType.Order.Id)
            {
                throw new DistributingDomainException("Order cannot unfreeze a frozen amount that isn't froze by order" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the amount hasn't been unfroze.
            if (this._frozenStatusId != FrozenStatus.Frozen.Id)
            {
                throw new DistributingDomainException("Can not unfreeze a frozen that isn't at frozen status" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the order's tracking number is equal to frozen's order tracking number.
            if (order.TrackingNumber != this.OrderTrackingNumber)
            {
                throw new DistributingDomainException("The order's tracking number provided must equal to the frozen's order tracking number" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the amount to unfroze is correct.
            if (order.Amount != this.Amount)
            {
                throw new DistributingDomainException("The order's amount provided must equal to the frozen amount" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            var currentDateTime = dateTimeService.GetCurrentDateTime();
            if (currentDateTime == null)
            {
                throw new DistributingDomainException("Couldn't retrieve current date time" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }


            var balanceBefore = balance.AmountAvailable;
            var balanceAfter = balanceBefore + this.Amount;

            this.BalanceUnfrozenRecord = new BalanceRecord(balance.AmountAvailable, balanceAfter);

            this._frozenStatusId = FrozenStatus.Unfrozen.Id;
            this.DateUnfroze = currentDateTime;

            this.AddDomainEvent(new UnfrozenDomainEvent(
                this,
                balance,
                this.BalanceUnfrozenRecord,
                this._frozenStatusId,
                currentDateTime));
        }

        public void WithdrawalCanceled(Balance balance, Withdrawal withdrawal, IDateTimeService dateTimeService)
        {
            //Checking the balance isn't null.
            if (balance == null)
            {
                throw new DistributingDomainException("The balance must be provided" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the frozen type is correct.
            if (this._frozenTypeId != FrozenType.Withdrawal.Id)
            {
                throw new DistributingDomainException("Withdrawal cannot unfreeze a frozen amount that isn't froze by withdrawal" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the amount hasn't been unfroze.
            if (this._frozenStatusId != FrozenStatus.Frozen.Id)
            {
                throw new DistributingDomainException("Can not unfreeze a frozen that isn't at frozen status" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the withdrawal id is equal to frozen's withdrawal id.
            if (withdrawal.Id != this.WithdrawalId)
            {
                throw new DistributingDomainException("The wihtdrawal's Id must be equal to the frozen's withdrawal Id" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the withdrawal is canceled.
            if (withdrawal.GetWithdrawalStatus.Id != WithdrawalStatus.Canceled.Id)
            {
                throw new DistributingDomainException("Can not refund from a withdrawal if it isn't at canceled status" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the amount to unfroze is correct.
            if (withdrawal.TotalAmount != this.Amount)
            {
                throw new DistributingDomainException("The withdrawal's total amount provided must be equal to the frozen amount" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            var currentDateTime = dateTimeService.GetCurrentDateTime();
            if (currentDateTime == null)
            {
                throw new DistributingDomainException("Couldn't retrieve current date time" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }
            var balanceBefore = balance.AmountAvailable;
            var balanceAfter = balanceBefore + this.Amount;

            this.BalanceUnfrozenRecord = new BalanceRecord(balance.AmountAvailable, balanceAfter);

            this._frozenStatusId = FrozenStatus.Unfrozen.Id;
            this.DateUnfroze = currentDateTime;

            this.AddDomainEvent(new UnfrozenDomainEvent(
                       this,
                       balance,
                       this.BalanceUnfrozenRecord,
                       this._frozenStatusId,
                       currentDateTime));
        }

        public void Unfreeze(Balance balance, Admin admin, string description, IDateTimeService dateTimeService)
        {
            //Checking the balance isn't null.
            if (balance == null)
            {
                throw new DistributingDomainException("The balance must be provided" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the admin isn't null.
            if (admin == null || string.IsNullOrWhiteSpace(admin.AdminId))
            {
                throw new DistributingDomainException("The admin must be provided" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the frozen type is correct.
            if (this._frozenTypeId != FrozenType.ByAdmin.Id)
            {
                throw new DistributingDomainException("Withdrawal cannot unfreeze a frozen amount that isn't froze by withdrawal" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the amount hasn't been unfroze.
            if (this._frozenStatusId != FrozenStatus.Frozen.Id)
            {
                throw new DistributingDomainException("Can not unfreeze a frozen that isn't at frozen status" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the length of description is not too long.
            if (!string.IsNullOrEmpty(description) && description.Length > 500)
            {
                throw new DistributingDomainException("The length of description must less than 500 charactors" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            var currentDateTime = dateTimeService.GetCurrentDateTime();
            if (currentDateTime == null)
            {
                throw new DistributingDomainException("Couldn't retrieve current date time" +
                    ". At " + nameof(Frozen) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }


            var balanceBefore = balance.AmountAvailable;
            var balanceAfter = balanceBefore + this.Amount;

            this.BalanceUnfrozenRecord = new BalanceRecord(balance.AmountAvailable, balanceAfter);


            if (!string.IsNullOrEmpty(description))
            {
                this.Description = description + " ----- Unfroze By Admin:" + admin.Name + " ___Admin Id:" + admin.AdminId;
            }

            this._frozenStatusId = FrozenStatus.Unfrozen.Id;
            this.DateUnfroze = currentDateTime;

            this.AddDomainEvent(new UnfrozenDomainEvent(
                       this,
                       balance,
                       this.BalanceUnfrozenRecord,
                       this._frozenStatusId,
                       currentDateTime));
        }

    }
}
