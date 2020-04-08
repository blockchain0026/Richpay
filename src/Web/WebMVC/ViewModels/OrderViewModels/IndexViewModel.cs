using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.OrderViewModels
{
    public class IndexViewModel
    {
        public string QrCodeImageData { get; set; }
        public string H5PaymentUrl { get; set; }
        public string H5PaymentPageUrl { get; set; }

        public string UserId { get; set; }
        public string UserBaseRole { get; set; }
    }
}
