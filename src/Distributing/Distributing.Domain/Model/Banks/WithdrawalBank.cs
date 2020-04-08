using Distributing.Domain.Exceptions;
using Distributing.Domain.Model.Shared;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Model.Banks
{
    public class WithdrawalBank : Entity, IAggregateRoot
    {
        //public string WithdrawalBankId { get; private set; }
        public string BankName { get; private set; }

        public DateTime DateCreated { get; private set; }



        protected WithdrawalBank()
        {
        }

        public WithdrawalBank(string bankName, DateTime dateCreated) : this()
        {
            BankName = bankName ?? throw new ArgumentNullException(nameof(bankName));
            DateCreated = dateCreated;

            //WithdrawalBankId = Guid.NewGuid().ToString();
        }

        public static WithdrawalBank From(string bankName, IDateTimeService dateTimeService)
        {
            //Checking the validity of bank's name.
            if (!BankAccount.IsBankNameValid(bankName))
            {
                throw new DistributingDomainException("The bank's name must be provided and The length of bank's name must less than or equal to 10 chinese words" + ". At WithdrawalBank.From()");
            }

            var currentDateTime = dateTimeService.GetCurrentDateTime();
            var withdrawalBank = new WithdrawalBank(
                bankName,
                currentDateTime != null ? currentDateTime : throw new DistributingDomainException("Couldn't retrieve current date time" + ". At WithdrawalBank.From()")
                );

            return withdrawalBank;
        }

    }
}
