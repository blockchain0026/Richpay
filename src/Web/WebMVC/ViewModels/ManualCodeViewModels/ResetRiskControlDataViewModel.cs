using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.ManualCodeViewModels
{
    public class ResetRiskControlDataViewModel
    {
        [Required]
        public int QrCodeId { get; set; }

        public bool ResetQuotaLeftToday { get; set; }
        public bool ResetCurrentConsecutiveFailures { get; set; }
        public bool ResetSuccessRateAndRelatedData { get; set; }


    }
}
