using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.SystemConfigurationViewModels
{
    public class SiteBaseInfoViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string SiteTitle { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string ContactAddress { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string ContactEmail { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string ConsumerHotline { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string ConsumerServiceQQ { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string ConsumerServiceSkype { get; set; }

        public IEnumerable<SelectListItem> BusinessHoursFroms { get; set; }
        public IEnumerable<SelectListItem> BusinessMinutesFroms { get; set; }
        public IEnumerable<SelectListItem> BusinessHoursTos { get; set; }
        public IEnumerable<SelectListItem> BusinessMinutesTos { get; set; }

        [Required]
        [Range(0, 23, ErrorMessage = "请输入正确的开始营业时间")]
        public string BusinessHoursFrom { get; set; }

        [Required]
        [Range(0, 59, ErrorMessage = "请输入正确的开始营业时间")]
        public string BusinessMinutesFrom { get; set; }

        [Required]
        [Range(0, 23, ErrorMessage = "请输入正确的结束营业时间")]
        public string BusinessHoursTo { get; set; }

        [Required]
        [Range(0, 59, ErrorMessage = "请输入正确的结束营业时间")]
        public string BusinessMinutesTo { get; set; }
    }
}
