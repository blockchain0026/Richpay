using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.SystemConfigurationViewModels
{
    public class PaymentViewModel
    {
        [Required]
        [Range(1, 600, ErrorMessage = "支付超时时间需介于 1 秒至 600 秒之间")]
        public int PaymentTimeoutInSecond { get; set; }

        [Required]
        [Range(1, 60000, ErrorMessage = "抢卖倒计时需介于 1 秒至 60000 秒之间")]
        public int SellRequestCountdownInSecond { get; set; }


        [Required]
        [Range(0, 9999, ErrorMessage = "Please enter valid integer Number")]
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
