using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Models.Queries
{
    public class DepositBankAccount
    {
        public int BankAccountId { get; set; }
        public string Name { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string DateCreated { get; set; }
    }
    public class DepositEntry
    {
        public int DepositId { get; set; }
        public string DepositStatus { get; set; }
        public int BalanceId { get; set; }
        public string CreateByUplineId { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string UserType { get; set; }

        public DepositBankAccount DepositBankAccount { get; set; }

        public int TotalAmount { get; set; }

        public int CommissionRateInThousandth { get; set; }
        public decimal CommissionAmount { get; set; }

        public decimal ActualAmount { get; set; }

        public string Description { get; set; }

        public string VerifiedByAdminId { get; set; }
        public string VerifiedByAdminName { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime? DateFinished { get; set; }

    }
}
