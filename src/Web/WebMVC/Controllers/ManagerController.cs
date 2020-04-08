using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebMVC.Data;
using WebMVC.Extensions;
using WebMVC.Infrastructure.Services;
using WebMVC.Models.Permissions;
using WebMVC.Models.Queries;
using WebMVC.ViewModels;
using WebMVC.ViewModels.Json;
using WebMVC.ViewModels.ManagerViewModels;
using WebMVC.ViewModels.Pagination;

namespace WebMVC.Controllers
{
    [Authorize(Policy = Permissions.Additional.Reviewed)]
    public class ManagerController : Controller
    {
        private readonly IManagerService _managerService;
        private readonly IRoleService _roleService;

        public ManagerController(IManagerService managerService, IRoleService roleService)
        {
            this._managerService = managerService;
            this._roleService = roleService;
        }

        [Authorize(Policy = Permissions.Administration.Managers.View)]
        public async Task<IActionResult> Index(int? page, string searchString = null)
        {
            var itemsPage = 10;
            var totalItems = await _managerService.GetManagersTotalCount(searchString);
            var managers = await _managerService.GetManagers(page ?? 0, itemsPage, searchString);



            var vm = new IndexViewModel()
            {
                Managers = managers,
                ManagerStatus =  _managerService.GetManagerStatus(),
                PaginationInfo = new PaginationInfo()
                {
                    ActualPage = page ?? 0,
                    ItemsPerPage = managers.Count,
                    TotalItems = totalItems,
                    TotalPages = (int)Math.Ceiling(((decimal)totalItems / itemsPage))
                }
            };

            vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
            vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";

            //ViewBag.BasketInoperativeMsg = errorMsg;

            return View(vm);
        }


        [Authorize(Policy = Permissions.Administration.Managers.View)]
        [HttpGet]
        public async Task<IActionResult> Search(int? page, int? itemsPerPage)
        {
            var totalItems = await _managerService.GetManagersTotalCount(string.Empty);

            var managers = await _managerService.GetManagers(
                page != null ? page.Value - 1 : 0,
                itemsPerPage ?? 10,
                string.Empty);

            var jsonResult = new KTDatatableResult<Manager>
            {
                meta = new KTPagination
                {
                    page = page != null ? page.Value : 1,
                    pages = page != null ? (int)Math.Ceiling(((decimal)totalItems / itemsPerPage ?? 10)) : 1,
                    perpage = itemsPerPage ?? 10,
                    total = totalItems,
                    sort = "asc",
                    field = "ManagerId"
                },
                data = managers
            };


            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.Administration.Managers.View)]
        [HttpPost]
        public async Task<IActionResult> Search([FromForm] KTPagination pagination, [FromForm] KTQuery query,[FromForm] KTSort sort)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Manager");
            }

            var totalItems = await _managerService.GetManagersTotalCount(query.generalSearch);

            var managers = await _managerService.GetManagers(
                pagination.page - 1,
                pagination.perpage,
                query.generalSearch ?? string.Empty,
                sort.field,
                sort.sort);

            var jsonResult = new KTDatatableResult<Manager>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "ManagerId"
                },
                data = managers
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.Administration.Managers.Edit)]
        [HttpPost]
        public async Task<IActionResult> UpdateAccountStatus([FromBody]List<AccountStatus> accounts)
        {
            if (!TryValidateModel(accounts) || !accounts.Any())
            {
                return BadRequest("Invalid request data.");
            }

            //Prevent self edit.
            foreach (var account in accounts)
            {
                var currentLoggedInUserId = User.GetUserId<string>();

                if (account.UserId == currentLoggedInUserId)
                {
                    return BadRequest("You can not edit your account status by yourself.");
                }
            }

            try
            {
                await _managerService.UpdateManagerStatus(accounts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


            /*return Json(new AppJsonResponse
            {
                HttpStatusCode = (int)HttpStatusCode.OK,
                Result = HttpStatusCode.OK.ToString(),
                ErrorDescription = string.Empty
            });*/
            return Ok(new { success = true });

        }


        // GET: Manager/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        [Authorize(Policy = Permissions.Administration.Managers.Create)]
        // GET: Manager/Create
        public IActionResult Create()
        {
            var vm = new CreateViewModel()
            {
                RoleNames =  _roleService.GetManagerRoles()
            };
            return View(vm);
        }

        // POST: Manager/Create
        [Authorize(Policy = Permissions.Administration.Managers.Create)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm]CreateViewModel vm)
        {
            try
            {
                // TODO: Add insert logic here
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }

                await _managerService.CreateManager(
                    vm.FullName,
                    vm.Nickname ?? vm.FullName,
                    vm.PhoneNumber,
                    vm.Email,
                    vm.Username,
                    vm.Password,
                    vm.IsEnabled,
                    vm.RoleName,
                    vm.Wechat,
                    vm.QQ);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = Permissions.Administration.Managers.Edit)]
        // GET: Manager/Update/5
        public async Task<IActionResult> Update(string managerId)
        {
            try
            {
                var manager = await _managerService.GetManager(managerId);

                var managerRolesList =  _roleService.GetManagerRoles(manager.RoleName);

                var vm = new UpdateViewModel()
                {
                    ManagerId = manager.ManagerId,
                    Username=manager.Username,
                    FullName = manager.FullName,
                    Nickname = manager.Nickname,
                    PhoneNumber = manager.PhoneNumber,
                    Email = manager.Email,
                    Wechat = manager.Wechat,
                    QQ = manager.QQ,
                    IsEnabled = manager.IsEnabled,
                    RoleNames = managerRolesList,
                    RoleName = manager.RoleName
                };
                return View(vm);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // POST: Manager/Update/5
        [Authorize(Policy = Permissions.Administration.Managers.Edit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request.");
            }

            try
            {
                // TODO: Add update logic here
                var manager = new Manager
                {
                    ManagerId = vm.ManagerId,
                    Username = vm.Username,
                    FullName = vm.FullName,
                    Nickname = vm.Nickname,
                    PhoneNumber = vm.PhoneNumber,
                    Email = vm.Email,
                    Wechat = vm.Wechat,
                    QQ = vm.QQ,
                    IsEnabled = vm.IsEnabled,
                    RoleName = vm.RoleName
                };

                await _managerService.UpdateManager(manager,
                    string.IsNullOrEmpty(vm.Password) ? null : vm.Password);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Manager/Delete/5
        [Authorize(Policy = Permissions.Administration.Managers.Delete)]
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]List<string> managerIds)
        {
            if (!managerIds.Any())
            {
                return BadRequest("Invalid Request Params.");
            }

            try
            {
                // TODO: Add delete logic here
                await _managerService.DeleteManager(managerIds);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}