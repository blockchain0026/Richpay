using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.ManualCodeViewModels
{
    public class UpdateQrCodeQuotaViewModel
    {
        [Required]
        public int QrCodeId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "The value must be greater than or equal to 0.")]
        public int DailyAmountLimit { get; set; }


        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "The value must be greater than or equal to 0.")]
        public int OrderAmountUpperLimit { get; set; }


        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "The value must be greater than or equal to 0.")]
        public int OrderAmountLowerLimit { get; set; }
    }
}
