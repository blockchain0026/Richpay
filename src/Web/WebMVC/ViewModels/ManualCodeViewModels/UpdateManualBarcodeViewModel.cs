using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.ManualCodeViewModels
{
    public class UpdateManualBarcodeViewModel
    {
        [Required]
        public int QrCodeId { get; set; }

        //Code Data
        [Required]
        [Url]
        public string QrCodeUrl { get; set; }

        public decimal? Amount { get; set; }
    }
}
