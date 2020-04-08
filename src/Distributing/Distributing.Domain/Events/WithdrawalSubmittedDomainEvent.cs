using Distributing.Domain.Model.Banks;
using Distributing.Domain.Model.Withdrawals;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class WithdrawalSubmittedDomainEvent : INotification
    {
        public WithdrawalSubmittedDomainEvent(Withdrawal withdrawal, int withdrawalStatusId, int withdrawalTypeId, int balanceId, string userId, BankAccount bankAccount, decimal totalAmount, decimal commissionRate, decimal commissionAmount, decimal actualAmount, string description, DateTime dateCreated)
        {
            Withdrawal = withdrawal;
            WithdrawalStatusId = withdrawalStatusId;
            WithdrawalTypeId = withdrawalTypeId;
            BalanceId = balanceId;
            UserId = userId;
            BankAccount = bankAccount;
            TotalAmount = totalAmount;
            CommissionRate = commissionRate;
            CommissionAmount = commissionAmount;
            ActualAmount = actualAmount;
            Description = description;
            DateCreated = dateCreated;
        }

        public Withdrawal Withdrawal { get; }
        public int WithdrawalStatusId { get; }
        public int WithdrawalTypeId { get; }
        public int BalanceId { get; }
        public string UserId { get; }
        public BankAccount BankAccount { get; }
        public decimal TotalAmount { get; private set; }
        public decimal CommissionRate { get; private set; }
        public decimal CommissionAmount { get; private set; }
        public decimal ActualAmount { get; private set; }
        public string Description { get; private set; }
        public DateTime DateCreated { get; }
    }
}
