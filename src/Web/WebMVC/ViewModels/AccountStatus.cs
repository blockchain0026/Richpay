using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels
{
    public class AccountStatus
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public bool? IsEnabled { get; set; }
    }
}
