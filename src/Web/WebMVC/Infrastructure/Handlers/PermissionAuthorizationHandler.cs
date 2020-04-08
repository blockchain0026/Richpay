using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Permissions;
using WebMVC.Models.Queries;

namespace WebMVC.Infrastructure.Handlers
{
    internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        UserManager<ApplicationUser> _userManager;
        RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public PermissionAuthorizationHandler(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            /*if (context.User == null)
            {
                return;
            }*/
            if (!_signInManager.IsSignedIn(context.User))
            {
                Console.WriteLine("The context user Isn't signed in.");
                return;
            }
            Console.WriteLine("The context user Is signed in.");

            // Get all the roles the user belongs to and check if any of the roles has the permission required
            // for the authorization to succeed.
            var user = await _userManager.GetUserAsync(context.User);
            var userRoleNames = await _userManager.GetRolesAsync(user);
            var userRoles = new List<IdentityRole>();
            foreach (var userRoleName in userRoleNames)
            {
                var userRole = _roleManager.Roles.Where(x => x.Name == userRoleName).SingleOrDefault();
                if (userRole != null)
                {
                    userRoles.Add(userRole);
                }
            }

            foreach (var role in userRoles)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                var permissions = roleClaims.Where(x => x.Type == CustomClaimTypes.Permission &&
                                                        x.Value == requirement.Permission &&
                                                        x.Issuer == "LOCAL AUTHORITY")
                                            .Select(x => x.Value);
                Console.WriteLine("Required Permission:" + requirement.Permission);
                if (permissions.Any())
                {
                    context.Succeed(requirement);
                    return;
                }
                Console.WriteLine("No required permmission found:" + requirement.Permission);
            }
        }
    }
}
