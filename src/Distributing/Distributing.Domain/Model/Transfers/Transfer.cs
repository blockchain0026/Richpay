using Distributing.Domain.Events;
using Distributing.Domain.Exceptions;
using Distributing.Domain.Helpers;
using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Shared;
using Distributing.Domain.SeedWork;
using Distributing.Domain.Validations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Model.Transfers
{
    public class Transfer : Entity, IAggregateRoot
    {
        public string UserId { get; private set; }

        public int FromBalanceId { get; private set; }
        public int ToBalanceId { get; private set; }

        public decimal Amount { get; private set; }
        public InitiatedBy InitiatedBy { get; private set; }
        private int _initiatedById;

        public DateTime DateTransferred { get; private set; }


        protected Transfer()
        {
        }

        public Transfer(string userId, int fromBalanceId, int toBalanceId, decimal amount, int initiatedById, DateTime dateTransferred)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            FromBalanceId = fromBalanceId;
            ToBalanceId = toBalanceId;
            Amount = amount;
            _initiatedById = initiatedById;
            DateTransferred = dateTransferred;

            this.AddDomainEvent(new TransferCreatedDomainEvent(
                this,
                userId,
                FromBalanceId,
                ToBalanceId,
                amount,
                initiatedById,
                dateTransferred
                ));
        }

        public static Transfer FromTraderAgent(string userId, Balance fromBalance, Balance toBalance, decimal amount, IDateTimeService dateTimeService)
        {
            //Checking the balances isn't null.
            if (fromBalance == null || toBalance == null)
            {
                throw new DistributingDomainException("The order must be provided" +
                    ". At " + nameof(Transfer) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the payer and payee isn't the same.
            if (fromBalance.Id == toBalance.Id)
            {
                throw new DistributingDomainException("Can not transfer money to self" +
                    ". At " + nameof(Transfer) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the fromBalance's user is equal to user id provided.
            if (string.IsNullOrEmpty(userId) || fromBalance.UserId != userId)
            {
                throw new DistributingDomainException("The user id provided must equal to fromBalance's user id" +
                    ". At " + nameof(Transfer) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the destination balance is a trader balance.
            if (toBalance.UserType.Id != UserType.Trader.Id)
            {
                throw new DistributingDomainException("Trader agent can only transfer balance to a trader" +
                    ". At " + nameof(Transfer) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the trasfer amount is larger than 0.
            if (amount <= 0)
            {
                throw new DistributingDomainException("The transfer amount must be larger than 0" +
                    ". At " + nameof(Transfer) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the transfer amount doesn't has more than 3 points.
            DistributingValidation.ValidateDecimalPoint(amount, 3);

            //Checking the original balance has enough avalialbe balance.
            if (amount > fromBalance.AmountAvailable)
            {
                throw new DistributingDomainException("The transfer amount must less than original balance's available amount" +
                    ". At " + nameof(Transfer) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }


            var currentDateTime = dateTimeService.GetCurrentDateTime();
            if (currentDateTime == null)
            {
                throw new DistributingDomainException("Couldn't retrieve current date time" +
                    ". At " + nameof(Transfer) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            var transfer = new Transfer(
                userId,
                fromBalance.Id,
                toBalance.Id,
                amount,
                InitiatedBy.TraderAgent.Id,
                currentDateTime
                );

            return transfer;
        }

        public static Transfer FromAdmin(string userId, Balance fromBalance, Balance toBalance, decimal amount, IDateTimeService dateTimeService)
        {
            //Checking the balances isn't null.
            if (fromBalance == null || toBalance == null)
            {
                throw new DistributingDomainException("The order must be provided" +
                    ". At " + nameof(Transfer) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the payer and payee isn't the same.
            if (fromBalance.Id == toBalance.Id)
            {
                throw new DistributingDomainException("Can not transfer money to self" +
                    ". At " + nameof(Transfer) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the trasfer amount is larger than 0.
            if (amount <= 0)
            {
                throw new DistributingDomainException("The transfer amount must be larger than 0" +
                    ". At " + nameof(Transfer) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            //Checking the transfer amount doesn't has more than 3 points.
            DistributingValidation.ValidateDecimalPoint(amount, 3);

            //Checking the original balance has enough avalialbe balance.
            if (amount > fromBalance.AmountAvailable)
            {
                throw new DistributingDomainException("The transfer amount must less than original balance's available amount" +
                    ". At " + nameof(Transfer) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            var currentDateTime = dateTimeService.GetCurrentDateTime();
            if (currentDateTime == null)
            {
                throw new DistributingDomainException("Couldn't retrieve current date time" +
                    ". At " + nameof(Transfer) + "." + DistributingDomainHelper.GetMethodString() + "()");
            }

            var transfer = new Transfer(
                userId,
                fromBalance.Id,
                toBalance.Id,
                amount,
                InitiatedBy.Admin.Id,
                currentDateTime
                );

            return transfer;
        }


    }
}
