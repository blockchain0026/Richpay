using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.PersonalViewModels
{
    public class UpdateViewModel
    {
        //Base Info
        [Required]
        [StringLength(256, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string UserId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string FullName { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 0)]
        public string Nickname { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 0)]
        public string SiteAddress { get; set; }

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




        //Readonly Info
        public string UplineId { get; set; }


        public bool IsEnabled { get; set; }
        public bool HasGrantRight { get; set; }


        public int DailyAmountLimit { get; set; }
        public int DailyFrequencyLimit { get; set; }
        public int EachAmountUpperLimit { get; set; }
        public int EachAmountLowerLimit { get; set; }


        public int WithdrawalCommissionRateInThousandth { get; set; }
        public int DepositCommissionRateInThousandth { get; set; }


        public int RateAlipayInThousandth { get; set; }
        public int RateWechatInThousandth { get; set; }


        public int RateRebateAlipayInThousandth { get; set; }
        public int RateRebateWechatInThousandth { get; set; }

    }
}
