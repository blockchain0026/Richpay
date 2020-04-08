using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Models.Queries
{
    public class WithdrawalBankOption
    {
        public int WithdrawalBankId { get; set; }
        public string BankName { get; set; }
        public string DateCreated { get; set; }
    }

    public class WithdrawalEntry
    {
        public int WithdrawalId { get; set; }
        public string WithdrawalStatus { get; set; }

        public int BalanceId { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string UserType { get; set; }

        public WithdrawalBankOption WithdrawalBankOption { get; set; }

        public string AccountName { get; set; }
        public string AccountNumber { get; set; }

        public int TotalAmount { get; set; }

        public int CommissionRateInThousandth { get; set; }
        public decimal CommissionAmount { get; set; }

        public decimal ActualAmount { get; set; }


        public string ApprovedByAdminId { get; set; }
        public string ApprovedByAdminName { get; set; }
        public string CancellationApprovedByAdminId { get; set; }
        public string CancellationApprovedByAdminName { get; set; }

        public string Description { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime? DateFinished { get; set; }
    }

}
