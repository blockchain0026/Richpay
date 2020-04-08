using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.SystemConfigurationViewModels
{
    public class UserNotificationViewModel
    {
        [Required]
        public bool OrderTimeout { get; set; }

        [Required]
        public bool SuccessDeposit { get; set; }
    }
}
