using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.RoleViewModels
{
    public class UpdateViewModel
    {
        [Required]
        public string RoleName { get; set; }

        [Required]
        public List<string> Permissions { get; set; }
    }
}
