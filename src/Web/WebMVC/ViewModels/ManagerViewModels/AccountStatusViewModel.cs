using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.ManagerViewModels
{
    public class AccountStatusViewModel
    {
        public List<string> Ids { get; set; }
        public bool? IsEnabled { get; set; }
    }
}
