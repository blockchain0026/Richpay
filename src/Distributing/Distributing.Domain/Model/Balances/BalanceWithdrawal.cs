using Distributing.Domain.Exceptions;
using Distributing.Domain.Model.Withdrawals;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Model.Balances
{
    public class BalanceWithdrawal
     : Entity
    {
        // DDD Patterns comment
        // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (Instead of properties and property collections)
        private bool _isSuccess;
        private bool _isFailed;
        private decimal _amount;
        private DateTime _dateWithdraw;

        public int WithdrawalId { get; private set; }

        protected BalanceWithdrawal() { }

        public BalanceWithdrawal(int withdrawalId, decimal amount, DateTime dateWithdraw)
        {
            /*if (string.IsNullOrEmpty(withdrawalId))
            {
                throw new DistributingDomainException("The withdrawal id must be provided" + ". At BalanceWithdrawal()");
            }*/
            if (withdrawalId == default(int))
            {
                throw new DistributingDomainException("The withdrawal id must be provided" + ". At BalanceWithdrawal()");
            }

            if (dateWithdraw == null)
            {
                throw new DistributingDomainException("The withdrawal date must be provided" + ". At BalanceWithdrawal()");
            }

            if (amount <= 0)
            {
                throw new DistributingDomainException("The withdrawal amount must be larger than 0." + ". At BalanceWithdrawal()");
            }

            _dateWithdraw = dateWithdraw;
            WithdrawalId = withdrawalId;
            _amount = amount;

            _isSuccess = false;
            _isFailed = false;
        }

        public bool IsSuccess() => _isSuccess;
        public bool IsFailed() => _isFailed;

        public decimal GetAmount() => _amount;

        public bool IsWithdrawDateBefore(DateTime dateTime)
        {
            if (dateTime == null)
            {
                throw new DistributingDomainException("The date time must be provided" + ". At BalanceWithdrawal.IsWithdrawDateBefore()");
            }
            var result = DateTime.Compare(dateTime, this._dateWithdraw);

            //if dateTime is later than _dateWithdraw, the result is 1.
            if (result == 1)
            {
                return true;
            }

            return false;
        }

        public void Success(Withdrawal withdrawal)
        {
            //Checking if the withdrawal id is matched.
            if (withdrawal.Id != this.WithdrawalId)
            {
                throw new DistributingDomainException("The withdrawal id isn't match." + ". At BalanceWithdrawal.Success()");
            }

            //Checking if the withdrawal status is at success.
            if (withdrawal.WithdrawalStatus.Id != WithdrawalStatus.Success.Id)
            {
                throw new DistributingDomainException("The withdrawal status must be at success ." + ". At BalanceWithdrawal.Success()");
            }

            this._isSuccess = true;
            this._isFailed = false;
        }

        public void Failed()
        {
            //Checking if the withdrawal id is matched.
            /*if (withdrawal.Id != this.WithdrawalId)
            {
                throw new DistributingDomainException("The withdrawal id isn't match." + ". At BalanceWithdrawal.Failed()");
            }*/

            //Checking if the withdrawal status is at success.
            /*if (withdrawal.WithdrawalStatus.Id != WithdrawalStatus.Canceled.Id)
            {
                throw new DistributingDomainException("The withdrawal status must be at canceled ." + ". At BalanceWithdrawal.Failed()");
            }*/
            this._isFailed = true;
            this._isSuccess = false;
        }
    }
}
