using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.Json
{
    public class KTPagination
    {
        [Required]
        public int page { get; set; }
        [Required]
        public int pages { get; set; }
        [Required]
        public int perpage { get; set; }
        [Required]
        public int total { get; set; }
        public string sort { get; set; }
        public string field { get; set; }
    }
}
