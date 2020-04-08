using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.WithdrawalViewModels
{
    public class IndexViewModel
    {
        public string UserId { get; set; }
        public string UserBaseRole { get; set; }
        public IEnumerable<SelectListItem> WithdrawalBankOptions { get; set; }

    }
}
