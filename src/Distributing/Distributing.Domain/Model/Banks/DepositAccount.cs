using Distributing.Domain.Exceptions;
using Distributing.Domain.Model.Shared;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Model.Banks
{
    public class DepositAccount : Entity, IAggregateRoot
    {
        public string Name { get; private set; }
        public BankAccount BankAccount { get; private set; }
        public DateTime DateCreated { get; private set; }



        protected DepositAccount()
        {
        }

        public DepositAccount(string name, BankAccount bankAccount, DateTime dateCreated) : this()
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name)); ;
            BankAccount = bankAccount ?? throw new ArgumentNullException(nameof(bankAccount));
            DateCreated = dateCreated;
        }

        public static DepositAccount From(string name, string bankName, string accountName, string accountNumber, IDateTimeService dateTimeService)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DistributingDomainException("The name must be provided" + ". At DepositAccount.From()");
            }
            var bankAccount = new BankAccount(bankName, accountName, accountNumber);

            var currentDateTime = dateTimeService.GetCurrentDateTime();

            var depositAccount = new DepositAccount(
                name,
                bankAccount,
                currentDateTime != null ? currentDateTime : throw new DistributingDomainException("Couldn't retrieve current date time" + ". At DepositAccount.From()")
                );

            return depositAccount;
        }
    }
}
