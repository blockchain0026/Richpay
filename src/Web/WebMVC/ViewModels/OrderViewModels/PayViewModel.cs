using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.OrderViewModels
{
    public class PayViewModel
    {
        public string H5PaymentCommand20000123QR { get; set; }
        public string H5PaymentCommand20000123S { get; set; }
        public string H5PaymentCommand09999988S { get; set; }
        public string H5PaymentCommand56 { get; set; }
        public string H5EnvelopCommand { get; set; }

        public string QrCodeImageData { get; set; }
    }
}
