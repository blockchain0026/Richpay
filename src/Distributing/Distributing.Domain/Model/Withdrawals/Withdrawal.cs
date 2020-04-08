using Distributing.Domain.Events;
using Distributing.Domain.Exceptions;
using Distributing.Domain.Helpers;
using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Banks;
using Distributing.Domain.Model.Distributions;
using Distributing.Domain.Model.Roles;
using Distributing.Domain.Model.Shared;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Model.Withdrawals
{
    public class Withdrawal : Entity, IAggregateRoot
    {
        //public string WithdrawalId { get; private set; }

        public WithdrawalStatus WithdrawalStatus { get; private set; }
        private int _withdrawalStatusId;
        public WithdrawalType WithdrawalType { get; private set; }
        private int _withdrawalTypeId;

        public int BalanceId { get; private set; }
        public string UserId { get; private set; }

        public BankAccount BankAccount { get; private set; }

        public decimal TotalAmount { get; private set; }

        public decimal CommissionRate { get; private set; }
        public decimal CommissionAmount { get; private set; }

        public decimal ActualAmount { get; private set; }
        //public BalanceRecord BalanceRecord { get; private set; }


        public Admin ApprovedBy { get; private set; }
        public Admin CancellationApprovedBy { get; private set; }

        public string Description { get; private set; }

        public DateTime DateCreated { get; private set; }
        public DateTime? DateFinished { get; private set; }

        public WithdrawalType GetWithdrawalType => WithdrawalType.From(_withdrawalTypeId);
        public WithdrawalStatus GetWithdrawalStatus => WithdrawalStatus.From(_withdrawalStatusId);

        protected Withdrawal()
        {
        }

        public Withdrawal(string userId, int balanceId, int withdrawalTypeId, BankAccount bankAccount, decimal totalAmount, decimal commissionRate, decimal commissionAmount, decimal actualAmount, string description, DateTime dateCreated) : this()
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            BalanceId = balanceId;

            _withdrawalTypeId = withdrawalTypeId;

            BankAccount = bankAccount;

            TotalAmount = totalAmount;
            CommissionRate = commissionRate;
            CommissionAmount = commissionAmount;
            ActualAmount = actualAmount;
            DateCreated = dateCreated;
            Description = description;
            //WithdrawalId = Guid.NewGuid().ToString();
            _withdrawalStatusId = WithdrawalStatus.Submitted.Id;

            this.AddDomainEvent(new WithdrawalSubmittedDomainEvent(
                this,
                this._withdrawalStatusId,
                this._withdrawalTypeId,
                balanceId,
                userId,
                bankAccount,
                totalAmount,
                commissionRate,
                commissionAmount,
                actualAmount,
                description,
                dateCreated
                ));
        }

        public static Withdrawal FromUser(Balance balance, WithdrawalBank withdrawalBank, string accountName, string accountNumber, decimal amount, string description, IDateTimeService dateTimeService, IOpenTimeService openTimeService)
        {
            //Checking the balance isn't null.
            var currentDateTime = dateTimeService.GetCurrentDateTime();

            if (!openTimeService.IsWithdrawalOpenNow(
                currentDateTime != null ? currentDateTime : throw new DistributingDomainException("Couldn't retrieve current date time" + ". At Withdrawal.FromUser()")
                ))
            {
                throw new DistributingDomainException("目前非提现开放时间" + ". At Withdrawal.FromUser()");
            }

            if (balance == null || balance.IsTransient())
            {
                throw new DistributingDomainException("Invalid Param: " + nameof(balance) + ". At Withdrawal.FromUser()");
            }

            //Checking the withdraw amount's value doesn't has more than 0 points.
            if (decimal.Round(amount, 0) != amount)
            {
                throw new DistributingDomainException("The withdraw amount's value must not has more than 0 points" + ". At Withdrawal.FromUser()");
            }

            //Checking the withdraw amount is larger than 0.
            if (amount <= 0)
            {
                throw new DistributingDomainException("The withdraw amount must larger than 0" + ". At Withdrawal.FromUser()");
            }

            //Checking the withdraw amount is less than or equal to balance's available amount.
            if (amount > balance.AmountAvailable)
            {
                throw new DistributingDomainException("提现金额不可超过可用金额: " + balance.AmountAvailable + "¥ . At Withdrawal.FromUser()");
            }

            //Checking the withdrawal bank isn't null.
            if (withdrawalBank == null || withdrawalBank.IsTransient())
            {
                throw new DistributingDomainException("The withdrawal bank must be provided" + ". At Withdrawal.FromUser()");
            }

            //Checking the length of description is not too long.
            if (!string.IsNullOrEmpty(description) && description.Length > 500)
            {
                throw new DistributingDomainException("The length of description must less than 500 charactors" +
                    ". At " + nameof(Withdrawal) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            var bankAccount = new BankAccount(withdrawalBank.BankName, accountName, accountNumber);

            var userId = !string.IsNullOrEmpty(balance.UserId) ? balance.UserId :
                throw new DistributingDomainException("The user id must be provided" + ". At Withdrawal.FromUser()");

            var totalAmount = amount;
            var commissionRate = balance.WithdrawalCommissionRate;
            var commissionAmount = totalAmount * commissionRate;
            var actualAmount = totalAmount - commissionAmount;

            var withdrawal = new Withdrawal(
                userId,
                balance.Id,
                WithdrawalType.ByUser.Id,
                bankAccount,
                totalAmount,
                commissionRate,
                commissionAmount,
                actualAmount,
                description,
                currentDateTime
                );

            return withdrawal;
        }

        public static Withdrawal FromAdmin(Balance balance, Admin admin, decimal amount, string description, IDateTimeService dateTimeService)
        {
            //Checking the balance isn't null.
            if (balance == null || balance.IsTransient())
            {
                throw new DistributingDomainException("Invalid Param: " + nameof(balance) + ". At Withdrawal.FromAdmin()");
            }

            //Checking the withdraw amount's value doesn't has more than 2 points.
            //The admin can made withdrawal with at most 3 points because of no commission.
            if (decimal.Round(amount, 3) != amount)
            {
                throw new DistributingDomainException("The withdraw amount's value must not has more than 3 points" + ". At Withdrawal.FromAdmin()");
            }

            //Checking the withdraw amount is larger than 0.
            if (amount <= 0)
            {
                throw new DistributingDomainException("The withdraw amount must larger than 0" + ". At Withdrawal.FromAdmin()");
            }

            //Checking the withdraw amount is less than or equal to balance's available amount.
            if (amount > balance.AmountAvailable)
            {
                throw new DistributingDomainException("减款金额不可超过可用金额: " + balance.AmountAvailable + "¥ . At Withdrawal.FromAdmin()");
            }

            //Checking the length of description is not too long.
            if (!string.IsNullOrEmpty(description) && description.Length > 500)
            {
                throw new DistributingDomainException("The length of description must less than 500 charactors" +
                    ". At " + nameof(Balance) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            var userId = !string.IsNullOrEmpty(balance.UserId) ? balance.UserId :
                throw new DistributingDomainException("The user id must be provided" + ". At Withdrawal.FromAdmin()");

            var totalAmount = amount;
            var commissionRate = 0;  //No need to calculate commission.
            var commissionAmount = totalAmount * commissionRate;
            var actualAmount = totalAmount - commissionAmount;

            var currentDateTime = dateTimeService.GetCurrentDateTime();

            var withdrawal = new Withdrawal(
                userId,
                balance.Id,
                WithdrawalType.ByAdmin.Id,
                null,
                totalAmount,
                commissionRate,
                commissionAmount,
                actualAmount,
                description,
                currentDateTime != null ? currentDateTime : throw new DistributingDomainException("Couldn't retrieve current date time" + ". At Withdrawal.FromAdmin()")
                );

            withdrawal.Approve(admin);
            withdrawal.ForceSuccess(admin, dateTimeService);

            return withdrawal;
        }

        public void Approve(Admin admin)
        {
            //Checking the status is in submitted.
            if (this._withdrawalStatusId != WithdrawalStatus.Submitted.Id)
            {
                throw new DistributingDomainException("只有等待打款中的提现申请才能通知收款。目前的提现状态为 " + GetWithdrawalStatus.Name + " . At Withdrawal.Approve()");
            }

            //Checking the admin is not null.
            if (admin == null || string.IsNullOrEmpty(admin.AdminId) || string.IsNullOrEmpty(admin.Name))
            {
                throw new DistributingDomainException("The auditor must be provided" + ". At Withdrawal.Approve()");
            }

            this._withdrawalStatusId = WithdrawalStatus.Approved.Id;
            this.ApprovedBy = admin;

            this.AddDomainEvent(new WithdrawalApprovedDomainEvent(
                this,
                this._withdrawalStatusId,
                admin));
        }

        public void ConfirmPaymentReceived(string userId, IDateTimeService dateTimeService)
        {
            //Checking the status is in approved.
            if (this._withdrawalStatusId != WithdrawalStatus.Approved.Id)
            {
                throw new DistributingDomainException("等待确认中的提现才可确认收款。目前提现状态为 " + GetWithdrawalStatus.Name + " . At Withdrawal.ConfirmPaymentReceived()");
            }

            //Checking the payment is confirm by applicant.
            if (string.IsNullOrEmpty(userId) || userId != this.UserId)
            {
                throw new DistributingDomainException("The payment can only be confirmed by applicant" + ". At Withdrawal.ConfirmPaymentReceived()");
            }

            var currentDateTime = dateTimeService.GetCurrentDateTime();

            this._withdrawalStatusId = WithdrawalStatus.Success.Id;
            this.DateFinished = currentDateTime != null ? currentDateTime : throw new DistributingDomainException("Couldn't retrieve current date time" + ". At Withdrawal.ConfirmPaymentReceived()");

            this.AddDomainEvent(new WithdrawalSuccessDomainEvent(
                this,
                this._withdrawalStatusId,
                currentDateTime
                ));
        }

        public void ForceSuccess(Admin admin, IDateTimeService dateTimeService)
        {
            //Checking the status is in approved.
            if (this._withdrawalStatusId != WithdrawalStatus.Approved.Id && this._withdrawalStatusId != WithdrawalStatus.AwaitingCancellation.Id)
            {
                throw new DistributingDomainException("等待确认或等待取消中的提现才可强制成功。目前提现状态为 " + GetWithdrawalStatus.Name + " . At Withdrawal.ForceSuccess()");
            }

            //Checking the admin is not null.
            if (admin == null || string.IsNullOrEmpty(admin.AdminId) || string.IsNullOrEmpty(admin.Name))
            {
                throw new DistributingDomainException("The auditor must be provided" + ". At Withdrawal.ForceSuccess()");
            }

            var currentDateTime = dateTimeService.GetCurrentDateTime();

            this._withdrawalStatusId = WithdrawalStatus.Success.Id;
            this.DateFinished = currentDateTime != null ? currentDateTime : throw new DistributingDomainException("Couldn't retrieve current date time" + ". At Withdrawal.ConfirmPaymentReceived()");
            this.Description += $" Force Success By:{admin.Name}. ID:{admin.AdminId}";
            this.AddDomainEvent(new WithdrawalForcedSuccessDomainEvent(
                this,
                this._withdrawalStatusId,
                admin,
                currentDateTime
                ));
        }

        public void Cancel(string userId, IDateTimeService dateTimeService)
        {
            //Checking the status is in submitted or approved.
            if (this._withdrawalStatusId != WithdrawalStatus.Approved.Id && this._withdrawalStatusId != WithdrawalStatus.Submitted.Id)
            {
                throw new DistributingDomainException("等待打款或等待确认收款中的提现才可申请取消。 提现目前状态为 " + GetWithdrawalStatus.Name + " . At Withdrawal.Cancel()");
            }

            //Checking the payment is confirm by applicant.
            if (string.IsNullOrEmpty(userId) || userId != this.UserId)
            {
                throw new DistributingDomainException("The payment can only be canceled by applicant" + ". At Withdrawal.Cancel()");
            }


            this._withdrawalStatusId = WithdrawalStatus.AwaitingCancellation.Id;

            this.AddDomainEvent(new WithdrawalAwaitingCancellationDomainEvent(
                this,
                this._withdrawalStatusId
                ));
        }

        public void ApproveCancellation(Admin admin, IDateTimeService dateTimeService)
        {
            //Checking the status is at awaiting cancellation.
            if (this._withdrawalStatusId != WithdrawalStatus.AwaitingCancellation.Id)
            {
                throw new DistributingDomainException("等待取消中的提现才可确认取消。 目前提现状态为 " + GetWithdrawalStatus.Name + " . At Withdrawal.ApproveCancellation()");
            }

            //Checking the admin is not null.
            if (admin == null || string.IsNullOrEmpty(admin.AdminId) || string.IsNullOrEmpty(admin.Name))
            {
                throw new DistributingDomainException("The auditor must be provided" + ". At Withdrawal.ApproveCancellation()");
            }
            var currentDateTime = dateTimeService.GetCurrentDateTime();

            this._withdrawalStatusId = WithdrawalStatus.Canceled.Id;
            this.CancellationApprovedBy = admin;

            this.DateFinished = currentDateTime != null ? currentDateTime : throw new DistributingDomainException("Couldn't retrieve current date time" + ". At Withdrawal.ApproveCancellation()");

            this.AddDomainEvent(new WithdrawalCanceledDomainEvent(
                this,
                _withdrawalStatusId,
                admin,
                currentDateTime
                ));
        }
    }
}
