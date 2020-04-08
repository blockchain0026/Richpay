using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Models.Queries
{
    public class Manager
    {
        public string ManagerId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Wechat { get; set; }
        public string QQ { get; set; }
        public bool IsEnabled { get; set; }
        public string RoleName { get; set; }
        public string LastLoginIP { get; set; }

        public string DateLastLoggedIn { get; set; }
        public string DateCreated { get; set; }

    }
}
