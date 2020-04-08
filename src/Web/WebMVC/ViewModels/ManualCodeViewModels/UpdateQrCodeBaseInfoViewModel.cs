using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.ManualCodeViewModels
{
    public class UpdateQrCodeBaseInfoViewModel
    {
        [Required]
        public int QrCodeId { get; set; }

        //Base Info
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string FullName { get; set; }

        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 0)]
        public string SpecifiedShopId { get; set; }
    }
}
