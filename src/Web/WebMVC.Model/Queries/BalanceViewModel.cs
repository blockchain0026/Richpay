using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Models.Queries
{
    public class Balance
    {
        public string UserId { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "The value must be greater than or equal to 0.")]
        public decimal AmountAvailable { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "The value must be greater than or equal to 0.")]
        public decimal AmountFrozen { get; set; }

        public WithdrawalLimit WithdrawalLimit { get; set; }

        [Required]
        [Range(0, 999, ErrorMessage = "Please enter valid commission")]
        public int WithdrawalCommissionRateInThousandth { get; set; }

        [Required]
        [Range(0, 999, ErrorMessage = "Please enter valid commission")]
        public int DepositCommissionRateInThousandth { get; set; }
    }

    public class ShopUserBalance
    {
        public decimal AmountAvailable { get; set; }

        public decimal AmountFrozen { get; set; }

        public WithdrawalLimit WithdrawalLimit { get; set; }

        public int WithdrawalCommissionRateInThousandth { get; set; }
    }

    public class WithdrawalLimit
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "The value must be greater than or equal to 0.")]
        public int DailyAmountLimit { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "The value must be greater than or equal to 0.")]
        public int DailyFrequencyLimit { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "The value must be greater than or equal to 0.")]
        public int EachAmountUpperLimit { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "The value must be greater than or equal to 0.")]
        public int EachAmountLowerLimit { get; set; }
    }
}
