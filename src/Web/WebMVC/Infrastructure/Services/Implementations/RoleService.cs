using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebMVC.Infrastructure.Services;
using WebMVC.Models;
using WebMVC.Models.Permissions;
using WebMVC.Models.Roles;

namespace WebMVC.Infrastructure.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        public IEnumerable<SelectListItem> GetManagerRoles(string selectedItemValue = null)
        {
            var managerRoleNames = new List<SelectListItem>();

            var roles = _roleManager.Roles;

            managerRoleNames.Add(new SelectListItem()
            {
                Value = string.Empty,
                Text = "选择角色...",
                Selected = selectedItemValue != null ? false : true
            });

            bool selected = selectedItemValue != null ? false : true;
            foreach (var role in roles)
            {
                if (Roles.IsManagerRole(role.Name))
                {
                    managerRoleNames.Add(new SelectListItem()
                    {
                        Value = role.Name,
                        Text = role.Name,
                        Selected = role.Name == selectedItemValue ? true : false
                    });

                    if (role.Name == selectedItemValue)
                    {
                        selected = true;
                    }
                }
            }
            /*if (!selected)
            {
                throw new ArgumentOutOfRangeException("No role found in select list.");
            }*/
            return managerRoleNames;
        }

        public async Task<List<IdentityRole>> GetManagersRoles(int pageIndex, int take, string searchString = null, string sortField = null, string sort = SortDirections.Descending)
        {
            var roles = _roleManager.Roles.Where(r =>
            r.Name != Roles.Manager
            && r.Name != Roles.Trader
            && r.Name != Roles.TraderAgent
            && r.Name != Roles.Shop
            && r.Name != Roles.ShopAgent
            && r.Name != Roles.TraderAgentWithGrantRight
            && r.Name != Roles.ShopAgentWithGrantRight
            && r.Name != Roles.UserReviewed
            );
            IQueryable<IdentityRole> searchResult;

            List<IdentityRole> itemsOnPage = null;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = roles.Where(m =>
                             m.Name.Contains(searchString));
            }
            else
            {
                searchResult = roles;
            }

            itemsOnPage = await GetSortedResult(searchResult, pageIndex, take, sortField, sort);

            return itemsOnPage;
        }

        public async Task<int> GetManagersRolesTotalCount(string searchString = null)
        {
            var roles = _roleManager.Roles.Where(r =>
            r.Name != Roles.Manager
            && r.Name != Roles.Trader
            && r.Name != Roles.TraderAgent
            && r.Name != Roles.Shop
            && r.Name != Roles.ShopAgent
            && r.Name != Roles.TraderAgentWithGrantRight
            && r.Name != Roles.ShopAgentWithGrantRight
            && r.Name != Roles.UserReviewed
            );

            if (!string.IsNullOrEmpty(searchString))
            {
                var result = await _roleManager.Roles.Where(m =>
                 m.Name.Contains(searchString)
                ).CountAsync();

                return result;
            }

            var count = await roles.CountAsync();

            return count;
        }


        public async Task<List<string>> GetRolePermissions(string roleId)
        {
            var result = new List<string>();

            //checking the role is not duplicated.
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role is null)
            {
                throw new InvalidOperationException("找无角色");
            }


            //Get all permissions from role.
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            foreach (var roleClaim in roleClaims)
            {
                result.Add(roleClaim.Value);
            }


            return result;
        }


        public async Task CreateManagerRole(string roleName, List<string> permissions)
        {
            //checking the role is not duplicated.
            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (roleExist)
            {
                throw new InvalidOperationException("请勿重复建立角色");
            }

            //Checking the permissions are valid.
            this.ValidatePermissions(permissions);

            //Create role.
            var createRoleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (!createRoleResult.Succeeded)
            {
                throw new InvalidOperationException("角色建立失败");
            }

            //Add permissions to role.
            var role = await _roleManager.FindByNameAsync(roleName);
            var roleClaims = await _roleManager.GetClaimsAsync(role);

            foreach (var permissionValue in permissions)
            {
                if (!roleClaims.Any(c => c.Value == permissionValue))
                {
                    await _roleManager.AddClaimAsync(
                        role,
                        new Claim(CustomClaimTypes.Permission, permissionValue));
                }
            }
        }

        public async Task DeleteManagerRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role is null)
            {
                throw new InvalidOperationException("找无角色");
            }

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.ToList().ToString());
            }
        }

        public async Task DeleteManagerRoles(List<string> roleIds)
        {
            var roles = new List<IdentityRole>();

            foreach (var roleId in roleIds)
            {
                var role = await _roleManager.FindByIdAsync(roleId);

                if (role == null)
                {
                    throw new KeyNotFoundException("No role found by given Id.");
                }

                roles.Add(role);
            }

            foreach (var role in roles)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.ToList().ToString());
                }
            }
        }

        public async Task UpdateRolePermissions(string roleName, List<string> permissions)
        {
            //checking the role is not duplicated.
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                throw new InvalidOperationException("找无角色");
            }

            //Checking the permissions are valid.
            this.ValidatePermissions(permissions);

            //Remove all permissions from role.
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            foreach (var roleClaim in roleClaims)
            {
                await _roleManager.RemoveClaimAsync(role, roleClaim);
            }

            //Add permissions to role.
            foreach (var permissionValue in permissions)
            {
                if (!roleClaims.Any(c => c.Value == permissionValue))
                {
                    await _roleManager.AddClaimAsync(
                        role,
                        new Claim(CustomClaimTypes.Permission, permissionValue));
                }
            }
        }


        private void ValidatePermissions(List<string> permissions)
        {
            //Check the permissions are exist.
            var existingPermissions = Permissions.GetPermissions();

            foreach (var permission in permissions)
            {
                if (!existingPermissions.Any(p => p == permission))
                {
                    throw new InvalidOperationException($"查无权限: {permission}");
                }
            }
        }

        private async Task<List<IdentityRole>> GetSortedResult(IQueryable<IdentityRole> roles, int? pageIndex, int? take, string sortField, string direction)
        {
            var result = new List<IdentityRole>();

            if (pageIndex != null && take != null)
            {
                var skip = (int)take * (int)pageIndex;
                if (!string.IsNullOrEmpty(sortField))
                {
                    if (sortField == "Name")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await roles
                               .OrderBy(f => f.Name)
                               .Skip(skip)
                               .Take((int)take)
                               .ToListAsync();
                        }
                        else
                        {
                            result = await roles
                               .OrderByDescending(f => f.Name)
                               .Skip(skip)
                               .Take((int)take)
                               .ToListAsync();
                        }
                    }
                    else
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await roles
                               .OrderBy(f => f.Name)
                               .Skip(skip)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await roles
                               .OrderByDescending(f => f.Name)
                               .Skip(skip)
                               .Take((int)take).ToListAsync();
                        }
                    }
                }
                else
                {
                    result = await roles
                       .OrderByDescending(f => f.Name)
                       .Skip(skip)
                       .Take((int)take)
                       .ToListAsync();
                }
            }
            else
            {
                result = await roles.ToListAsync();
            }

            return result;
        }
    }
}
