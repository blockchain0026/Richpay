using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.ManualCodeViewModels
{
    public class UpdateManualMerchantCodeViewModel
    {
        [Required]
        public int QrCodeId { get; set; }

        //Code Data
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string AppId { get; set; }

        public string AlipayPublicKey { get; set; }

        public string WechatApiCertificate { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string PrivateKey { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string MerchantId { get; set; }
    }
}
