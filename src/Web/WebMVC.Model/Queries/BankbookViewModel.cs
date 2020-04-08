using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Models.Queries
{
    public class BankbookRecord
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public int BalanceId { get; set; }

        public string DateOccurred { get; set; }

        public string Type { get; set; }


        public decimal BalanceBefore { get; set; }

        public decimal AmountChanged { get; set; }

        public decimal BalanceAfter { get; set; }


        public string TrackingId { get; set; }

        public string Description { get; set; }
    }

    public static class BankbookRecordType
    {
        public const string Withdrawal = "提现";
        public const string CancelWithdrawal = "取消提现";
        public const string Deposit = "入金";
        public const string ProcessOrder = "接收订单";
        public const string CancelOrder = "取消订单";
        public const string WithdrawalByAdmin = "管理员减款";
        public const string DepositByAdmin = "管理员加款";
        public const string FreezeByAdmin = "管理员冻结";
        public const string UnfreezeByAdmin = "管理员取消冻结";
        public const string TransferIn = "调点转入";
        public const string TransferOut = "调点转出";
    }
}
