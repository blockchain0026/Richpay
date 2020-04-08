using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Models.Queries
{
    public class TraderAgent
    {
        //public int Id { get; private set; }
        public string TraderAgentId { get; set; } //Required For View.
        public string Username { get; set; } //Required For View.
        public string Password { get; set; }
        public string FullName { get; set; } //Required For View.
        public string Nickname { get; set; } //Required For View.
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public string Wechat { get; set; }
        public string QQ { get; set; }

        public string UplineUserId { get; set; } //Required For View.
        public string UplineUserName { get; set; } //Required For View.
        public string UplineFullName { get; set; } //Required For View.


        //public decimal AmountAvailable { get; set; } //Required For View.
        //public decimal AmountFrozen { get; set; } //Required For View.


        public Balance Balance { get; set; }
        public TradingCommission TradingCommission { get; set; } //Required For View.

        public bool IsEnabled { get; set; } //Required For View.
        public bool IsReviewed { get; set; }
        public bool HasGrantRight { get; set; } //Required For View.

        public string LastLoginIP { get; set; } //Required For View.
        public string DateLastLoggedIn { get; set; } //Required For View.
        public string DateCreated { get; set; } //Required For View.
    }
}
