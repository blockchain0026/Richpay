using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.QrCodes;

namespace WebMVC.ViewModels.ManualCodeViewModels
{
    public class UpdaetQrCodeSettingsViewModel
    {
        [Required]
        public int QrCodeId { get; set; }


        [Required]
        public bool AutoPairingBySuccessRate { get; set; }
        [Required]
        public bool AutoPairingByQuotaLeft { get; set; }
        [Required]
        public bool AutoPairingByBusinessHours { get; set; }
        [Required]
        public bool AutoPairingByCurrentConsecutiveFailures { get; set; }
        [Required]
        public bool AutoPairngByAvailableBalance { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "The value must be greater than or equal to 0.")]
        public int SuccessRateThresholdInHundredth { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "The value must be greater than or equal to 0.")]
        public int SuccessRateMinOrders { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The value must be greater than or equal to 0.")]
        public decimal QuotaLeftThreshold { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The value must be greater than 0.")]
        public int CurrentConsecutiveFailuresThreshold { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The value must be greater than 0.")]
        public decimal AvailableBalanceThreshold { get; set; }

    }
}
