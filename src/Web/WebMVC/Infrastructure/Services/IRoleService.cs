using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Infrastructure.Services
{
    public interface IRoleService
    {
        Task CreateManagerRole(string roleName, List<string> permissions);
        Task DeleteManagerRole(string roleName);
        Task UpdateRolePermissions(string roleName, List<string> permissions);

        IEnumerable<SelectListItem> GetManagerRoles(string selectedItemValue = null);
        Task<List<IdentityRole>> GetManagersRoles(int pageIndex, int take, string searchString = null, string sortField = null, string sort = "desc");
        Task<int> GetManagersRolesTotalCount(string searchString = null);
        Task DeleteManagerRoles(List<string> roleIds);
        Task<List<string>> GetRolePermissions(string roleId);
    }
}
