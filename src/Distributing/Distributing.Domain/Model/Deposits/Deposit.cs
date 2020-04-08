using Distributing.Domain.Events;
using Distributing.Domain.Exceptions;
using Distributing.Domain.Helpers;
using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Banks;
using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Distributions;
using Distributing.Domain.Model.Roles;
using Distributing.Domain.Model.Shared;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Model.Deposits
{
    public class Deposit : Entity, IAggregateRoot
    {
        //public string DepositId { get; private set; }

        public DepositStatus DepositStatus { get; private set; }
        private int _depositStatusId;
        public DepositType DepositType { get; private set; }
        private int _depositTypeId;

        public int BalanceId { get; private set; }

        public string CreateByUplineId { get; private set; }
        public string UserId { get; private set; }

        public BankAccount BankAccount { get; private set; }

        public decimal TotalAmount { get; private set; }

        public decimal CommissionRate { get; private set; }
        public decimal CommissionAmount { get; private set; }

        public decimal ActualAmount { get; private set; }
        public BalanceRecord BalanceRecord { get; private set; }
        public string Description { get; private set; }

        public Admin VerifiedBy { get; private set; }

        public DateTime DateCreated { get; private set; }
        public DateTime? DateFinished { get; private set; }

        public DepositStatus GetDepositStatus => DepositStatus.From(_depositStatusId);

        public DepositType GetDepositType => DepositType.From(_depositTypeId);

        protected Deposit()
        {
        }

        public Deposit(string userId, int balanceId, BankAccount bankAccount, decimal totalAmount, decimal commissionRate, decimal commissionAmount, decimal actualAmount, BalanceRecord balanceRecord, int depositTypeId, string description, DateTime dateCreated, string createByUplineId) : this()
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            BalanceId = balanceId;
            BankAccount = bankAccount;

            _depositTypeId = depositTypeId;

            TotalAmount = totalAmount;
            CommissionRate = commissionRate;
            CommissionAmount = commissionAmount;
            ActualAmount = actualAmount;
            BalanceRecord = balanceRecord ?? throw new ArgumentNullException(nameof(userId));
            Description = description;
            DateCreated = dateCreated;

            //DepositId = Guid.NewGuid().ToString();
            _depositStatusId = DepositStatus.Submitted.Id;

            CreateByUplineId = createByUplineId;

            this.AddDomainEvent(new DepositSubmittedDomainEvent(
                this,
                this._depositStatusId,
                balanceId,
                userId,
                bankAccount,
                totalAmount,
                commissionRate,
                commissionAmount,
                actualAmount,
                depositTypeId,
                balanceRecord,
                description,
                dateCreated
                ));
        }


        public static Deposit FromUser(Balance balance, DepositAccount depositAccount, decimal amount, string description, IDateTimeService dateTimeService, IOpenTimeService openTimeService, string createByUplineId)
        {
            //Checking the balance isn't null.
            var currentDateTime = dateTimeService.GetCurrentDateTime();

            if (!openTimeService.IsDepositOpenNow(
                currentDateTime != null ? currentDateTime : throw new DistributingDomainException("Couldn't retrieve current date time" + ". At Deposit.FromUser()")
                ))
            {
                throw new DistributingDomainException("目前非入金开放时间" + ". At Deposit.FromUser()");
            }

            if (balance == null || balance.IsTransient())
            {
                throw new DistributingDomainException("Invalid Param: " + nameof(balance) + ". At Deposit.FromUser()");
            }

            //Checking the deposit amount's value doesn't has more than 0 points.
            if (decimal.Round(amount, 0) != amount)
            {
                throw new DistributingDomainException("The deposit amount's value rates must not has more than 0 points" + ". At Deposit.FromUser()");
            }

            //Checking the deposit amount is larger than 0.
            if (amount <= 0)
            {
                throw new DistributingDomainException("The deposit amount must larger than 0" + ". At Deposit.FromUser()");
            }

            //Checking the deposit account isn't null.
            if (depositAccount == null || depositAccount.IsTransient())
            {
                throw new DistributingDomainException("The deposit account must be provided" + ". At Deposit.FromUser()");
            }

            //Checking the length of description is not too long.
            if (!string.IsNullOrEmpty(description) && description.Length > 500)
            {
                throw new DistributingDomainException("The length of description must less than 500 charactors" +
                    ". At " + nameof(Deposit) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking this is not a shop user's balance.
            if (balance.GetUserType.Id == UserType.Shop.Id || balance.GetUserType.Id == UserType.ShopAgent.Id)
            {
                throw new DistributingDomainException("Can not make deposit to shop user." +
                    ". At " + nameof(Deposit) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }
            var bankAccount = new BankAccount(
                depositAccount.BankAccount.BankName,
                depositAccount.BankAccount.AccountName,
                depositAccount.BankAccount.AccountNumber);

            var userId = !string.IsNullOrEmpty(balance.UserId) ? balance.UserId :
                throw new DistributingDomainException("The user id must be provided" + ". At Deposit.FromUser()");

            var totalAmount = amount;
            var commissionRate = balance.DepositCommissionRate;
            var commissionAmount = totalAmount * commissionRate;
            var actualAmount = totalAmount - commissionAmount;

            var balanceRecord = new BalanceRecord(balance.AmountAvailable, null);

            var deposit = new Deposit(
                userId,
                balance.Id,
                bankAccount,
                totalAmount,
                commissionRate,
                commissionAmount,
                actualAmount,
                balanceRecord,
                DepositType.ByUser.Id,
                description,
                currentDateTime,
                createByUplineId
                );

            return deposit;
        }


        public static Deposit FromAdmin(Balance balance, Admin admin, decimal amount, string description, IDateTimeService dateTimeService)
        {
            //Checking the balance isn't null.
            if (balance == null || balance.IsTransient())
            {
                throw new DistributingDomainException("Invalid Param: " + nameof(balance) + ". At Deposit.FromAdmin()");
            }

            //Checking the deposit amount's value doesn't has more than 3 points.
            //The admin can made deposit with at most 3 points because of no commission.
            if (decimal.Round(amount, 3) != amount)
            {
                throw new DistributingDomainException("The deposit amount's value rates must not has more than 3 points" + ". At Deposit.FromAdmin()");
            }

            //Checking the deposit amount is larger than 0.
            if (amount <= 0)
            {
                throw new DistributingDomainException("The deposit amount must larger than 0" + ". At Deposit.FromAdmin()");
            }

            //Checking the length of description is not too long.
            if (!string.IsNullOrEmpty(description) && description.Length > 500)
            {
                throw new DistributingDomainException("The length of description must less than 500 charactors" +
                    ". At " + nameof(Deposit) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }



            var userId = !string.IsNullOrEmpty(balance.UserId) ? balance.UserId :
                throw new DistributingDomainException("The user id must be provided" + ". At Deposit.FromAdmin()");

            var totalAmount = amount;
            var commissionRate = balance.DepositCommissionRate;
            var commissionAmount = totalAmount * commissionRate;
            var actualAmount = totalAmount - commissionAmount;

            var currentDateTime = dateTimeService.GetCurrentDateTime();

            var balanceRecord = new BalanceRecord(balance.AmountAvailable, null);

            var deposit = new Deposit(
                userId,
                balance.Id,
                null,
                totalAmount,
                commissionRate,
                commissionAmount,
                actualAmount,
                balanceRecord,
                DepositType.ByAdmin.Id,
                description,
                currentDateTime != null ? currentDateTime : throw new DistributingDomainException("Couldn't retrieve current date time" + ". At Deposit.FromAdmin()"),
                null
                );

            deposit.Verify(admin, balance, dateTimeService);

            return deposit;
        }

        public void Verify(Admin admin, Balance balance, IDateTimeService dateTimeService)
        {
            //Checking the status is in submitted.
            if (this._depositStatusId != DepositStatus.Submitted.Id)
            {
                throw new DistributingDomainException("Can not verify a deposit if it isn't at submitted status" + ". At Deposit.Verify()");
            }

            //Checking the admin is not null.
            if (admin == null || string.IsNullOrEmpty(admin.AdminId) || string.IsNullOrEmpty(admin.Name))
            {
                throw new DistributingDomainException("The auditor must be provided" + ". At Deposit.Verify()");
            }

            var currentDateTime = dateTimeService.GetCurrentDateTime();

            var balanceBefore = balance.AmountAvailable;
            var balanceAfter = balanceBefore + this.ActualAmount;

            var balanceRecord = new BalanceRecord(
                balanceBefore,
                balanceAfter
                );

            this.BalanceRecord = balanceRecord;
            this._depositStatusId = DepositStatus.Success.Id;
            this.VerifiedBy = admin;
            this.DateFinished = currentDateTime != null ? currentDateTime : throw new DistributingDomainException("Couldn't retrieve current date time" + ". At Withdrawal.ConfirmPaymentReceived()");

            this.AddDomainEvent(new DepositVerifiedDomainEvent(
                this,
                balance,
                this._depositStatusId,
                admin,
                currentDateTime
                ));
        }

        public void Cancel(IDateTimeService dateTimeService)
        {
            //Checking the status is in submitted or approved.
            if (this._depositStatusId != DepositStatus.Submitted.Id)
            {
                throw new DistributingDomainException("Can not cancel a deposit if it isn't at submitted status" + ". At Deposit.Cancel()");
            }
            var currentDateTime = dateTimeService.GetCurrentDateTime();

            var balanceBefore = this.BalanceRecord.BalanceBefore;
            var balanceAfter = balanceBefore;
            var balanceRecord = new BalanceRecord(
                balanceBefore,
                balanceAfter
                );

            this.BalanceRecord = balanceRecord;

            this.DateFinished = currentDateTime != null ? currentDateTime : throw new DistributingDomainException("Couldn't retrieve current date time" + ". At Deposit.Cancel()");

            this._depositStatusId = DepositStatus.Canceled.Id;

            this.AddDomainEvent(new DepositCanceledDomainEvent(
                this,
                this._depositStatusId,
                currentDateTime
                ));
        }
    }
}