using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.SystemConfigurationViewModels
{
    public class SystemNotificationSoundViewModel
    {
        [Required]
        public bool Withdraw { get; set; }

        [Required]
        public bool Deposit { get; set; }

        [Required]
        public bool Member { get; set; }

        [Required]
        public bool QrCode { get; set; }

        [Required]
        public bool NewOrder { get; set; }

        [Required]
        public bool OrderTimeout { get; set; }
    }
}
