using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.QrCodes;

namespace WebMVC.ViewModels.ShopViewModels
{
    public class CreateViewModel
    {
        //Personal Info
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string FullName { get; set; }

        [Required]
        public string SiteAddress { get; set; }

        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 0)]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 0)]
        public string Wechat { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 0)]
        public string QQ { get; set; }


        //Account info
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 5)]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        public string UplineId { get; set; }


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
        [Range(0, 999, ErrorMessage = "Please enter valid rates")]
        public int RateRebateAlipayInThousandth { get; set; }

        [Required]
        [Range(0, 999, ErrorMessage = "Please enter valid rates")]
        public int RateRebateWechatInThousandth { get; set; }

        [HiddenInput]
        public int MinRateRebateAlipayInThousandth { get; set; }

        [HiddenInput]
        public int MinRateRebateWechatInThousandth { get; set; }
    }
}
