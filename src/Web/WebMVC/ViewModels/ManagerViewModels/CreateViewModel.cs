using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.ManagerViewModels
{
    public class CreateViewModel
    {
        
        public string FullName { get; set; }
        public string Nickname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Wechat { get; set; }
        public string QQ { get; set; }


        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsEnabled { get; set; }
        public IEnumerable<SelectListItem> RoleNames { get; set; }
        public string RoleName { get; set; }
    }
}
