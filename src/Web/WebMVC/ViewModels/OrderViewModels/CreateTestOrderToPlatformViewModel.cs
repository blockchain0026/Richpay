using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;

namespace WebMVC.ViewModels.OrderViewModels
{
    public class CreateTestOrderToPlatformViewModel
    {
        public int OrderAmount { get; set; }
        public int QrCodeId { get; set; }
    }
}
