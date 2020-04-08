using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.SystemConfigurationViewModels
{
    public class PaymentChannelViewModel
    {
        public IEnumerable<SelectListItem> OpenFromInHours { get; set; }
        public IEnumerable<SelectListItem> OpenFromInMinutes { get; set; }
        public IEnumerable<SelectListItem> OpenToInHours { get; set; }
        public IEnumerable<SelectListItem> OpenToInMinutes { get; set; }

        [Required]
        public string OpenFromInHour { get; set; }

        [Required]
        public string OpenFromInMinute { get; set; }

        [Required]
        public string OpenToInHour { get; set; }

        [Required]
        public string OpenToInMinute { get; set; }

        [Required]
        public bool AutoToggle { get; set; }
    }
}
