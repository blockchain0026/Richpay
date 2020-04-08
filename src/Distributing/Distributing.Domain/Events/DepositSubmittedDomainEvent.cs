using Distributing.Domain.Model.Banks;
using Distributing.Domain.Model.Deposits;
using Distributing.Domain.Model.Distributions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class DepositSubmittedDomainEvent : INotification
    {
        public DepositSubmittedDomainEvent(Deposit deposit, int depositStatusId, int balanceId, string userId, BankAccount bankAccount, decimal totalAmount, decimal commissionRate, decimal commissionAmount, decimal actualAmount, int depositTypeId, BalanceRecord balanceRecord, string description, DateTime dateCreated)
        {
            Deposit = deposit;
            DepositStatusId = depositStatusId;
            BalanceId = balanceId;
            UserId = userId;
            BankAccount = bankAccount;
            TotalAmount = totalAmount;
            CommissionRate = commissionRate;
            CommissionAmount = commissionAmount;
            ActualAmount = actualAmount;
            DepositTypeId = depositTypeId;
            BalanceRecord = balanceRecord;
            Description = description;
            DateCreated = dateCreated;
        }

        public Deposit Deposit { get; }
        public int DepositStatusId { get; }
        public int BalanceId { get; }
        public string UserId { get; }
        public BankAccount BankAccount { get; }

        public decimal TotalAmount { get; }
        public decimal CommissionRate { get; }
        public decimal CommissionAmount { get; }
        public decimal ActualAmount { get; }
        public int DepositTypeId { get; }
        public BalanceRecord BalanceRecord { get; private set; }
        public string Description { get; private set; }
        public DateTime DateCreated { get; }
    }

}
