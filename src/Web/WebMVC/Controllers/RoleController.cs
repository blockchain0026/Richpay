using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Infrastructure.Services;
using WebMVC.Models.Permissions;
using WebMVC.ViewModels.Json;
using WebMVC.ViewModels.RoleViewModels;

namespace WebMVC.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
        }

        // GET: Role
        [Authorize(Policy = Permissions.Administration.Roles.View)]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = Permissions.Administration.Roles.View)]
        [HttpGet]
        public async Task<IActionResult> Search(string generalSearch, int page = 1)
        {
            var itemsPage = 10;
            var totalItems = await _roleService.GetManagersRolesTotalCount(generalSearch);
            var roles = await _roleService.GetManagersRoles(
                page - 1,
                itemsPage,
                generalSearch);

            var jsonResult = new KTDatatableResult<IdentityRole>
            {
                meta = new KTPagination
                {
                    page = page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / 10)),
                    perpage = 10,
                    total = totalItems,
                    sort = "desc",
                    field = "RoleId"
                },
                data = roles
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.Administration.Roles.View)]
        [HttpPost]
        public async Task<IActionResult> Search([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("无效参数");
            }

            var totalItems = await _roleService.GetManagersRolesTotalCount(
                query.generalSearch
                );
            var roles = await _roleService.GetManagersRoles(
                pagination.page - 1,
                pagination.perpage,
                query.generalSearch ?? null,
                sort.field,
                sort.sort);

            var jsonResult = new KTDatatableResult<IdentityRole>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "RoleId"
                },
                data = roles
            };

            return Json(jsonResult);
        }


        // GET: Role
        [Authorize(Policy = Permissions.Administration.Roles.View)]
        public async Task<IActionResult> GetRolePermissions(string id)
        {
            var rolePermissions = await _roleService.GetRolePermissions(id);
            return Json(rolePermissions);
        }

        // POST: Role/Create
        [Authorize(Policy = Permissions.Administration.Roles.Create)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm]CreateViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }

                await _roleService.CreateManagerRole(vm.RoleName, vm.Permissions);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: Role/Update
        [Authorize(Policy = Permissions.Administration.Roles.Edit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromForm]UpdateViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }

                await _roleService.UpdateRolePermissions(vm.RoleName, vm.Permissions);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Role/Delete
        [Authorize(Policy = Permissions.Administration.Roles.Delete)]
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]List<string> roleIds)
        {
            if (!roleIds.Any())
            {
                return BadRequest("Invalid Request Params.");
            }

            try
            {
                await _roleService.DeleteManagerRoles(roleIds);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}