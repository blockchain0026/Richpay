using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;
using WebMVC.ViewModels.Pagination;

namespace WebMVC.ViewModels.ManagerViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<Manager> Managers { get; set; }
        public IEnumerable<SelectListItem> ManagerStatus { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}
