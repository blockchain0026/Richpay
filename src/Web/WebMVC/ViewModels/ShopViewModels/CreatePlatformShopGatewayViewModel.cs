using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.ShopViewModels
{
    public class CreatePlatformShopGatewayViewModel
    {
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string ShopId { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string ShopGatewayType { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string PaymentChannel { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string PaymentScheme { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string GatewayNumber { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string Name { get; set; }



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
