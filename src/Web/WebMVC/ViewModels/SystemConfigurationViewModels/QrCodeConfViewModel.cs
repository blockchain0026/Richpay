using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.SystemConfigurationViewModels
{
    public class QrCodeConfViewModel
    {
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
        [Range(1, 100, ErrorMessage = "Please enter valid integer Number")]
        public int SuccessRateThresholdInHundredth { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int SuccessRateMinOrders { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public decimal QuotaLeftThreshold { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int CurrentConsecutiveFailuresThreshold { get; set; }
         
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public decimal AvailableBalanceThreshold { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int DailyAmountLimit { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int OrderAmountUpperLimit { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int OrderAmountLowerLimit { get; set; }

    }
}
