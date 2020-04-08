using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.PayViewModels
{
    public class IndexViewModel
    {
        public int TimeLeft { get; set; }

        public int Amount { get; set; }

        public string PaymentCommand { get; set; }

        public string OrderTrackingNumber { get; set; }

        public string QrCodeImageData { get; set; }
    }
}
