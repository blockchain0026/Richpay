using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels
{
    public class AccountReview
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public bool IsReviewed { get; set; }
    }
}
