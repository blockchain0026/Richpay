using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.PersonalViewModels
{
    public class TwoFactorAuthViewModel
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public bool HasAuthenticator { get; set; }
        public bool Is2faEnabled { get; set; }
        public EnableTwoFactorAuthViewModel EnableTwoFactorAuthViewModel { get; set; }
    }
}
