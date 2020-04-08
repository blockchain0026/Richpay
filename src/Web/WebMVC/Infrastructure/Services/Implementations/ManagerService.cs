using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebMVC.Extensions;
using WebMVC.Infrastructure.Services;
using WebMVC.Models;
using WebMVC.Models.Queries;
using WebMVC.Models.Roles;
using WebMVC.ViewModels;

namespace WebMVC.Infrastructure.Services.Implementations
{
    public class ManagerService : IManagerService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ManagerService(UserManager<ApplicationUser> userManager,
         RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public async Task<Manager> GetManager(string managerId)
        {
            var user = await _userManager.FindByIdAsync(managerId);
            if (user == null || user.BaseRoleType != BaseRoleType.Manager)
            {
                throw new Exception("No manager found.");
            }

            return await user.MapToManagerAsync(_userManager);
        }

        public async Task<List<Manager>> GetManagers(int pageIndex, int take, string searchString = null, string sortField = null, string sort = SortDirections.Descending)
        {
            var result = new List<Manager>();

            var managers = _userManager.Users.Where(u => u.BaseRoleType == BaseRoleType.Manager);

            IQueryable<ApplicationUser> searchResult;

            List<ApplicationUser> itemsOnPage = null;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = managers.Where(m =>
                             m.Email.Contains(searchString)
                             || m.UserName.Contains(searchString)
                             || m.FullName.Contains(searchString)
                             || m.Nickname.Contains(searchString));
            }
            else
            {
                searchResult = managers;
            }


            //var totalItems = await queryableManagerList.LongCountAsync();
            itemsOnPage = await GetSortedResult(searchResult, pageIndex, take, sortField, sort);


            var populated = await itemsOnPage.MapToManagersAsync(_userManager);
            result.AddRange(populated);

            return result;
        }

        public async Task<int> GetManagersTotalCount(string searchString = null)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                var result = await _userManager.Users.Where(m =>
                 m.BaseRoleType == BaseRoleType.Manager
                 && (m.Email.Contains(searchString)
                 || m.UserName.Contains(searchString)
                 || m.FullName.Contains(searchString)
                 || m.Nickname.Contains(searchString))
                ).CountAsync();

                return result;
            }

            var count = await _userManager.Users.Where(u => u.BaseRoleType == BaseRoleType.Manager).CountAsync();

            return count;
        }

        public async Task CreateManager(string fullName, string nickname, string phoneNumber, string email, string username, string password, bool isEnabled, string roleName, string wechat = null, string qq = null)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                throw new ArgumentOutOfRangeException("Role not found.");
            }
            else if (!Roles.IsManagerRole(roleName))
            {
                throw new ArgumentOutOfRangeException("Invalid manager role.");
            }

            //Create user.
            var user = new ApplicationUser
            {
                FullName = fullName,
                Nickname = nickname,
                PhoneNumber = phoneNumber,
                Email = email,
                UserName = username,
                IsEnabled = isEnabled,
                IsReviewed = true,

                Wechat = wechat ?? string.Empty,
                QQ = qq ?? string.Empty,

                //Must set to manager.
                BaseRoleType = BaseRoleType.Manager,

                DateCreated = DateTime.UtcNow,

                //Disable lockout feature.
                LockoutEnabled = false
            };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                //Add to role.
                var manager = await _userManager.FindByNameAsync(username);
                var managerRole = await _roleManager.Roles.Where(x => x.Name == roleName).FirstOrDefaultAsync();
                if (managerRole != null)
                {
                    await _userManager.AddToRoleAsync(manager, managerRole.Name);
                }

                //Add to base manager role.
                var baseManagerRole = await _roleManager.Roles.Where(x => x.Name == Roles.Manager).FirstOrDefaultAsync();
                if (baseManagerRole == null)
                {
                    throw new Exception("No base manager role found.");
                }
                await _userManager.AddToRoleAsync(manager, baseManagerRole.Name);

                //Add to reviewed user role.
                await _userManager.AddToRoleAsync(manager, Roles.UserReviewed);
            }
            else
            {
                throw new Exception(result.Errors.ToString());
            }
        }

        public async Task UpdateManager(Manager manager, string password = null)
        {
            if (manager == null)
            {
                throw new ArgumentOutOfRangeException("Invalid Argument");
            }

            var user = await _userManager.FindByIdAsync(manager.ManagerId);

            if (user == null || user.BaseRoleType != BaseRoleType.Manager)
            {
                throw new ArgumentOutOfRangeException("No manager found.");
            }



            //Update manager's info.
            user.FullName = manager.FullName;
            user.Nickname = manager.Nickname;
            user.Email = manager.Email;
            user.PhoneNumber = manager.PhoneNumber;
            user.Wechat = manager.Wechat;
            user.QQ = manager.QQ;
            user.IsEnabled = manager.IsEnabled;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                throw new Exception(updateResult.Errors.ToString());
            }


            //Update manager's role.
            if (!await _userManager.IsInRoleAsync(user, manager.RoleName))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                //Check if role exists.
                if (!await _roleManager.RoleExistsAsync(manager.RoleName))
                {
                    throw new ArgumentOutOfRangeException("No manager role found by given role name.");
                }

                //Check if the role is a manager role.
                if (!Roles.IsManagerRole(manager.RoleName))
                {
                    throw new InvalidOperationException("No manager role found by given role name.");

                }

                //Remove old role from user.
                Roles.GetManagerRole(userRoles.ToList(), out string oldRole);
                if (!string.IsNullOrEmpty(oldRole))
                {
                    var removeResult = await _userManager.RemoveFromRoleAsync(user, oldRole);

                    if (!removeResult.Succeeded)
                    {
                        throw new Exception(removeResult.Errors.ToString());
                    }
                }

                //Add new role to user.
                var addResult = await _userManager.AddToRoleAsync(user, manager.RoleName);
                if (!addResult.Succeeded)
                {
                    throw new Exception(addResult.Errors.ToString());
                }
            }

            //Forced change manager's password.
            if (!string.IsNullOrEmpty(password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var changeResult = await _userManager.ResetPasswordAsync(user, token, password);

                if (!changeResult.Succeeded)
                {
                    throw new Exception(changeResult.Errors.ToString());
                }
            }
        }

        public async Task UpdateManagerStatus(List<AccountStatus> accounts)
        {
            //The code needs to update since it doesn't achieve atomic.
            foreach (var account in accounts)
            {
                var user = await _userManager.FindByIdAsync(account.UserId);
                if (user.BaseRoleType != BaseRoleType.Manager)
                {
                    throw new Exception("The user status to update is not a manager.");
                }
                user.IsEnabled = account.IsEnabled ??
                    throw new ArgumentException("Invalid Argument: IsEnabled (bool?)");

                await _userManager.UpdateAsync(user);
            }
        }


        public async Task DeleteManager(List<string> managerIds)
        {
            foreach (var managerId in managerIds)
            {
                var manager = await _userManager.FindByIdAsync(managerId);

                if (manager.BaseRoleType != BaseRoleType.Manager)
                {
                    throw new ArgumentOutOfRangeException("The user to delete is not a manager.");
                }

                var result = await _userManager.DeleteAsync(manager);

                if (result.Succeeded)
                {
                    continue;
                }
                else
                {
                    throw new Exception(result.Errors.ToString());
                }
            }
        }

        public IEnumerable<SelectListItem> GetManagerStatus()
        {
            var items = new List<SelectListItem>();

            items.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });

            items.Add(new SelectListItem()
            {
                Value = "Enabled",
                Text = "Enabled"
            });
            items.Add(new SelectListItem()
            {
                Value = "Disabled",
                Text = "Disabled"
            });

            return items;
        }



        private List<IdentityRole> GetAllManagerRoles()
        {
            /*var managerRoles = _roleManager.Roles.Where(r => r.Name != Roles.Shop
            && r.Name != Roles.ShopAgent
            && r.Name != Roles.Trader
            && r.Name != Roles.TraderAgent);

            return managerRoles.ToList();*/
            throw new NotImplementedException();
        }

        private async Task<List<ApplicationUser>> GetSortedResult(IQueryable<ApplicationUser> users, int pageIndex, int take, string sortField, string direction)
        {
            List<ApplicationUser> result = null;
            if (!string.IsNullOrEmpty(sortField))
            {
                if (sortField == "UserName")
                {
                    result = await users.SortByFieldAsync(pageIndex, take, direction, m => m.UserName);
                }
                else if (sortField == "NickName")
                {
                    result = await users.SortByFieldAsync(pageIndex, take, direction, m => m.Nickname);
                }
                else if (sortField == "LastLogin")
                {
                    result = await users.SortByFieldAsync(pageIndex, take, direction, m => m.DateLastLoggedIn);


                }
                else if (sortField == "Status")
                {
                    result = await users.SortByFieldAsync(pageIndex, take, direction, m => m.IsEnabled);
                }
                else if (sortField == "DateCreated")
                {
                    result = await users.SortByFieldAsync(pageIndex, take, direction, m => m.DateCreated);
                }
                else
                {
                    result = await users.SortByFieldAsync(pageIndex, take, direction, m => m.DateCreated);
                }
            }
            else
            {
                result = await users.SortByFieldAsync(pageIndex, take, SortDirections.Descending, m => m.DateCreated);
            }

            return result;
        }



    }
}
