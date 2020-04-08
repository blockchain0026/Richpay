using Microsoft.AspNetCore.Identity;
using System;
using WebMVC.Models.Roles;

namespace WebMVC.Models.Queries
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Nickname { get; set; }
        public string Wechat { get; set; }
        public string QQ { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsReviewed { get; set; }
        public BaseRoleType BaseRoleType { get; set; }
        public string UplineId { get; set; }
        public string LastLoginIP { get; set; }
        public string DateLastLoggedIn { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
