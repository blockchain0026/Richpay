using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Models.Queries
{
    public class ShopGatewayEntry
    {
        public int ShopGatewayId { get; set; }
        public string ShopId { get; set; }
        public string ShopGatewayType { get; set; }
        public string PaymentChannel { get; set; }
        public string PaymentScheme { get; set; }
        public int? FourthPartyGatewayId { get; set; }
        public AlipayPreferenceInfo AlipayPreferenceInfo { get; set; }
        public string DateCreated { get; set; }
    }

    public class AlipayPreferenceInfo
    {
        public int SecondsBeforePayment { get; set; }
        public bool IsAmountUnchangeable { get; set; }
        public bool IsAccountUnchangeable { get; set; }
        public bool IsH5RedirectByScanEnabled { get; set; }
        public bool IsH5RedirectByClickEnabled { get; set; }
        public bool IsH5RedirectByPickingPhotoEnabled { get; set; }
    }
}
