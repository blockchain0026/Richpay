using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.ShopViewModels
{
    public class UpdateShopGatewayAlipayPreference
    {
        [Required]
        public int ShopGatewayId { get; set; }

        [Required]
        [Range(0, 9999, ErrorMessage = "Please enter valid integer.")]
        public int SecondsBeforePayment { get; set; }

        [Required]
        public bool IsAmountUnchangeable { get; set; }

        [Required]
        public bool IsAccountUnchangeable { get; set; }

        [Required]
        public bool IsH5RedirectByScanEnabled { get; set; }

        [Required]
        public bool IsH5RedirectByClickEnabled { get; set; }

        [Required]
        public bool IsH5RedirectByPickingPhotoEnabled { get; set; }
    }
}
