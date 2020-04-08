using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Queries;
using WebMVC.ViewModels;

namespace WebMVC.Infrastructure.Services
{
    public interface IManagerService
    {
        Task<Manager> GetManager(string managerId);
        Task<List<Manager>> GetManagers(int pageIndex, int take, string searchString = null, string sortField = null, string sort = SortDirections.Descending);
        Task<int> GetManagersTotalCount(string searchString = null);
        Task CreateManager(string fullName, string nickname, string phoneNumber, string email,
            string username, string password, bool isEnabled, string roleName,
            string wechat = null, string qq = null);

        Task UpdateManagerStatus(List<AccountStatus> accountStatus);
        Task UpdateManager(Manager manager, string password = null);
        Task DeleteManager(List<string> managerIds);
        IEnumerable<SelectListItem> GetManagerStatus();
    }
}
