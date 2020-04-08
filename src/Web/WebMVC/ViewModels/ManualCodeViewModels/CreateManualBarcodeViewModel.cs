using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.QrCodes;

namespace WebMVC.ViewModels.ManualCodeViewModels
{
    public class CreateManualBarcodeViewModel
    {
        //Base Info
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string UserId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string FullName { get; set; }

        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 0)]
        public string SpecifiedShopId { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string PaymentChannel { get; set; }

        

        public bool AutoPairingBySuccessRate { get; set; }
        public bool AutoPairingByQuotaLeft { get; set; }
        public bool AutoPairingByBusinessHours { get; set; }
        public bool AutoPairingByCurrentConsecutiveFailures { get; set; }
        public bool AutoPairngByAvailableBalance { get; set; }
        public int? SuccessRateThresholdInHundredth { get; set; }
        public int? SuccessRateMinOrders { get; set; }
        public decimal? QuotaLeftThreshold { get; set; }
        public int? CurrentConsecutiveFailuresThreshold { get; set; }
        public decimal? AvailableBalanceThreshold { get; set; }




        public int? DailyAmountLimit { get; set; }


        public int? OrderAmountUpperLimit { get; set; }


        public int? OrderAmountLowerLimit { get; set; }



        //Code Data
        [Required]
        [Url]
        public string QrCodeUrl { get; set; }

        public decimal? Amount { get; set; }
    }
}
