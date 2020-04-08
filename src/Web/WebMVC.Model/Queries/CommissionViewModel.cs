using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Models.Queries
{

    public class TradingCommission
    {
        [Required]
        [Range(0, 999, ErrorMessage = "Please enter valid rates")]
        public int RateAlipayInThousandth { get; set; }

        [Required]
        [Range(0, 999, ErrorMessage = "Please enter valid rates")]
        public int RateWechatInThousandth { get; set; }
    }
    public class RebateCommission
    {

        [Required]
        [Range(0, 999, ErrorMessage = "Please enter valid rates")]
        public int RateRebateAlipayInThousandth { get; set; }

        [Required]
        [Range(0, 999, ErrorMessage = "Please enter valid rates")]
        public int RateRebateWechatInThousandth { get; set; }
    }

}
