using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Queries;
using WebMVC.Models.Roles;

namespace WebMVC.Extensions
{
    public static class ApplicationUserExtensions
    {
        public async static Task AddToRole(this ApplicationUser user, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, string roleName)
        {
            var existingRole = await roleManager.Roles.Where(x => x.Name == roleName).SingleOrDefaultAsync();
            if (existingRole != null)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                if (!userRoles.Any(r => r == roleName))
                {
                    var result = await userManager.AddToRoleAsync(user, roleName);
                    if (!result.Succeeded)
                    {
                        throw new Exception("Operation failed when adding user to role.");
                    }
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("No role found with name:" + roleName);
            }

        }
        public async static Task RemoveFromRole(this ApplicationUser user, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, string roleName)
        {
            var existingRole = await roleManager.Roles.Where(x => x.Name == roleName).SingleOrDefaultAsync();
            if (existingRole != null)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                if (userRoles.Any(r => r == roleName))
                {
                    var result = await userManager.RemoveFromRoleAsync(user, roleName);
                    if (!result.Succeeded)
                    {
                        throw new Exception("Operation failed when remove user from role.");
                    }
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("No role found with name:" + roleName);
            }

        }
        public async static Task<List<ApplicationUser>> SortByFieldAsync<TKey>(this IQueryable<ApplicationUser> users, int pageIndex, int take, string direction, Expression<Func<ApplicationUser, TKey>> keySelector)
        {
            List<ApplicationUser> result = null;

            if (direction == SortDirections.Ascending)
            {
                result = await users
                    .OrderBy(keySelector)
                    .Skip(take * pageIndex)
                    .Take(take)
                    .ToListAsync();
            }
            else
            {
                result = await users
                    .OrderByDescending(keySelector)
                    .Skip(take * pageIndex)
                    .Take(take)
                    .ToListAsync();
            }

            return result;
        }


        public async static Task<List<Manager>> MapToManagersAsync(this List<ApplicationUser> users, UserManager<ApplicationUser> userManager)
        {

            var managers = new List<Manager>();
            foreach (var user in users)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                string managerRole = string.Empty;
                foreach (var userRole in userRoles)
                {
                    if (Roles.IsManagerRole(userRole))
                    {
                        managerRole = userRole;
                    }
                }
                var manager = new Manager
                {
                    ManagerId = user.Id,
                    Username = user.UserName,
                    FullName = user.FullName,
                    Nickname = user.Nickname,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Wechat = user.Wechat,
                    QQ = user.QQ,
                    IsEnabled = user.IsEnabled,

                    RoleName = managerRole ?? throw new Exception("No manager role found on user."),

                    LastLoginIP = user.LastLoginIP,
                    DateLastLoggedIn = user.DateLastLoggedIn,
                    DateCreated = user.DateCreated.ToFullString()
                };
                managers.Add(manager);
            }

            return managers;
        }

        public async static Task<Manager> MapToManagerAsync(this ApplicationUser user, UserManager<ApplicationUser> userManager)
        {
            var userRoles = await userManager.GetRolesAsync(user);
            string managerRole = string.Empty;
            foreach (var userRole in userRoles)
            {
                if (Roles.IsManagerRole(userRole))
                {
                    managerRole = userRole;
                }
            }
            var manager = new Manager
            {
                ManagerId = user.Id,
                Username = user.UserName,
                FullName = user.FullName,
                Nickname = user.Nickname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Wechat = user.Wechat,
                QQ = user.QQ,
                IsEnabled = user.IsEnabled,

                RoleName = managerRole ?? throw new Exception("No manager role found on user."),

                LastLoginIP = user.LastLoginIP,
                DateLastLoggedIn = user.DateLastLoggedIn,
                DateCreated = user.DateCreated.ToFullString()
            };

            return manager;
        }

        public static TraderAgent MapToTraderAgent(this ApplicationUser user, TradingCommission tradingCommission, Balance balance, bool hasGrantRight, ApplicationUser upline = null)
        {
            var traderAgent = new TraderAgent
            {
                TraderAgentId = user.Id,
                Username = user.UserName,
                FullName = user.FullName,
                Nickname = user.Nickname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Wechat = user.Wechat,
                QQ = user.QQ,

                UplineUserId = upline != null ? upline.Id : string.Empty,
                UplineUserName = upline != null ? upline.UserName : string.Empty,
                UplineFullName = upline != null ? upline.FullName : string.Empty,

                Balance = balance,
                TradingCommission = tradingCommission,

                IsEnabled = user.IsEnabled,
                IsReviewed = user.IsReviewed,
                HasGrantRight = hasGrantRight,

                LastLoginIP = user.LastLoginIP,
                DateLastLoggedIn = user.DateLastLoggedIn,
                DateCreated = user.DateCreated.ToFullString()
            };

            return traderAgent;
        }
        public static Trader MapToTrader(this ApplicationUser user, TradingCommission tradingCommission, Balance balance, ApplicationUser upline = null)
        {
            var trader = new Trader
            {
                TraderId = user.Id,
                Username = user.UserName,
                FullName = user.FullName,
                Nickname = user.Nickname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Wechat = user.Wechat,
                QQ = user.QQ,

                UplineUserId = upline != null ? upline.Id : string.Empty,
                UplineUserName = upline != null ? upline.UserName : string.Empty,
                UplineFullName = upline != null ? upline.FullName : string.Empty,

                Balance = balance,
                TradingCommission = tradingCommission,

                IsEnabled = user.IsEnabled,
                IsReviewed = user.IsReviewed,

                LastLoginIP = user.LastLoginIP,
                DateLastLoggedIn = user.DateLastLoggedIn,
                DateCreated = user.DateCreated.ToFullString()
            };

            return trader;
        }
    }
}
