using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.SystemConfigurationViewModels
{
    public class WithdrawalAndDepositViewModel
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "最高单笔额度须为正整数")]
        public int EachAmountUpperLimit { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "最低单笔额度须为正整数")]
        public int EachAmountLowerLimit { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "单日限额须为正整数")]
        public int DailyAmountLimit { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "单日次数须为正整数")]
        public int DailyFrequencyLimit { get; set; }

        [Required]
        [Range(0, 1000, ErrorMessage = "提现手续费需介于 千分之1 至 千分之999 之间")]
        public int CommissionInThousandth { get; set; }


        public IEnumerable<SelectListItem> WithdrawalOpenFromInHours { get; set; }
        public IEnumerable<SelectListItem> WithdrawalOpenFromInMinutes { get; set; }
        public IEnumerable<SelectListItem> WithdrawalOpenToInHours { get; set; }
        public IEnumerable<SelectListItem> WithdrawalOpenToInMinutes { get; set; }

        public IEnumerable<SelectListItem> DepositOpenFromInHours { get; set; }
        public IEnumerable<SelectListItem> DepositOpenFromInMinutes { get; set; }
        public IEnumerable<SelectListItem> DepositOpenToInHours { get; set; }
        public IEnumerable<SelectListItem> DepositOpenToInMinutes { get; set; }


        [Required]
        public string WithdrawalOpenFromInHour { get; set; }

        [Required]
        public string WithdrawalOpenFromInMinute { get; set; }

        [Required]
        public string WithdrawalOpenToInHour { get; set; }

        [Required]
        public string WithdrawalOpenToInMinute { get; set; }

        [Required]
        public string DepositOpenFromInHour { get; set; }

        [Required]
        public string DepositOpenFromInMinute { get; set; }

        [Required]
        public string DepositOpenToInHour { get; set; }

        [Required]
        public string DepositOpenToInMinute { get; set; }
    }
}
