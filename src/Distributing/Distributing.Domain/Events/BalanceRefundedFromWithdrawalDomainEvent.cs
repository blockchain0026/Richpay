using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Withdrawals;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Events
{
    public class BalanceRefundedFromWithdrawalDomainEvent:INotification
    {
        public BalanceRefundedFromWithdrawalDomainEvent(Balance balance, int withdrawalId, decimal refundAmount, decimal amountAvailable)
        {
            Balance = balance;
            WithdrawalId = withdrawalId;
            RefundAmount = refundAmount;
            AmountAvailable = amountAvailable;
        }

        public Balance Balance { get; }
        public int WithdrawalId { get; set; }
        public decimal RefundAmount { get; }
        public decimal AmountAvailable { get; }
    }
}
