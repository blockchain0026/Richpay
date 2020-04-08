using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Models.Queries
{
    public class ShopAgent
    {
        public string ShopAgentId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Nickname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public string Wechat { get; set; }
        public string QQ { get; set; }

        public string UplineUserId { get; set; }
        public string UplineUserName { get; set; }
        public string UplineFullName { get; set; }

        public ShopUserBalance Balance { get; set; }
        public RebateCommission RebateCommission { get; set; }

        public bool IsEnabled { get; set; }
        public bool IsReviewed { get; set; }
        public bool HasGrantRight { get; set; }

        public string LastLoginIP { get; set; }
        public string DateLastLoggedIn { get; set; }
        public string DateCreated { get; set; }
    }
}
