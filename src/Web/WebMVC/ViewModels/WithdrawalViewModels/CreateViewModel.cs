using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.WithdrawalViewModels
{
    public class CreateViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "The value must be greater than or equal to 0.")]
        public int WithdrawalAmount { get; set; }

        [Required]
        public int WithdrawalBankOptionId { get; set; }

        [Required]
        public string AccountName { get; set; }

        [Required]
        public string AccountNumber { get; set; }

        public string Description { get; set; }

    }
}
