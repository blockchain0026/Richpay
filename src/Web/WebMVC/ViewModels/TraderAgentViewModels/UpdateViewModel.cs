using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.TraderAgentViewModels
{
    public class UpdateViewModel
    {
        [Required]
        public string TraderAgentId { get; set; }

        //Personal Info
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string FullName { get; set; }

        public string Nickname { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 0)]
        public string Wechat { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 0)]
        public string QQ { get; set; }

        public string UplineId { get; set; }


        public string Password { get; set; }


        public bool IsEnabled { get; set; }

        public bool HasGrantRight { get; set; }


        //Finance
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




        [Required]
        [Range(0, 999, ErrorMessage = "Please enter valid commission")]
        public int WithdrawalCommissionRateInThousandth { get; set; }

        [Required]
        [Range(0, 999, ErrorMessage = "Please enter valid commission")]
        public int DepositCommissionRateInThousandth { get; set; }

        [Required]
        [Range(0, 999, ErrorMessage = "Please enter valid rates")]
        public int RateAlipayInThousandth { get; set; }

        [Required]
        [Range(0, 999, ErrorMessage = "Please enter valid rates")]
        public int RateWechatInThousandth { get; set; }


        [HiddenInput]
        public int MaxRateAlipayInThousandth { get; set; }

        [HiddenInput]
        public int MaxRateWechatInThousandth { get; set; }
    }
}
