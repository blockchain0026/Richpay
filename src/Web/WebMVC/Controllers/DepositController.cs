using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebMVC.Extensions;
using WebMVC.Infrastructure.Services;
using WebMVC.Models.Permissions;
using WebMVC.Models.Queries;
using WebMVC.Models.Roles;
using WebMVC.ViewModels.DepositViewModels;
using WebMVC.ViewModels.Json;

namespace WebMVC.Controllers
{
    [Authorize(Policy = Permissions.Additional.Reviewed)]
    public class DepositController : Controller
    {
        private readonly IDepositService _depositService;
        private readonly IPersonalService _personalService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DepositController(IDepositService depositService, IPersonalService personalService, UserManager<ApplicationUser> userManager)
        {
            _depositService = depositService ?? throw new ArgumentNullException(nameof(depositService));
            _personalService = personalService ?? throw new ArgumentNullException(nameof(personalService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }



        [Authorize(Policy = Permissions.DepositManagement.Deposits.View)]
        public IActionResult Index()
        {
            var vm = new IndexViewModel
            {
                UserId = User.GetUserId<string>(),
                UserBaseRole = User.IsInRole(Roles.Manager) ? Roles.Manager : 
                User.IsInRole(Roles.TraderAgent) ? Roles.TraderAgent : "",
                DepositBankAccounts = _depositService.GetDepositBankAccountSelectList()
            };

            return View(vm);
        }


        [Authorize(Policy = Permissions.DepositManagement.Deposits.View)]
        [HttpPost]
        public async Task<IActionResult> Search([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Deposit");
            }

            var contextUserId = User.GetUserId<string>();

            if (!DateTime.TryParse(query.DateFrom, out DateTime dateFrom))
            {
                //dateFrom = DateTime.Now.AddDays(-1);
                dateFrom = DateTime.Now;
            }
            if (!DateTime.TryParse(query.DateTo, out DateTime dateTo))
            {
                //dateTo = DateTime.Now;
                dateTo = DateTime.Now.AddDays(1);
            }

            var totalItems = await _depositService.GetDepositEntrysTotalCount(
                contextUserId,
                query.generalSearch ?? string.Empty,
                dateFrom,
                dateTo,
                query.userType ?? string.Empty,
                query.status ?? string.Empty
                );

            var depositEntries = await _depositService.GetDepositEntrys(
                pagination.page - 1,
                pagination.perpage,
                contextUserId,
                query.generalSearch ?? string.Empty,
                sort.field,
                dateFrom,
                dateTo,
                query.userType ?? string.Empty,
                query.status ?? string.Empty,
                sort.sort);

            var jsonResult = new KTDatatableResult<DepositEntry>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "DepositId"
                },
                data = depositEntries
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.DepositManagement.PendingReview.View)]
        [HttpGet]
        public IActionResult PendingReview()
        {
            return View();
        }


        [Authorize(Policy = Permissions.DepositManagement.PendingReview.View)]
        [HttpPost]
        public async Task<IActionResult> PendingReview([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "PendingReview");
            }

            var contextUserId = User.GetUserId<string>();

            if (!DateTime.TryParse(query.DateFrom, out DateTime dateFrom))
            {
                //dateFrom = DateTime.Now.AddDays(-1);
                dateFrom = DateTime.Now;
            }
            if (!DateTime.TryParse(query.DateTo, out DateTime dateTo))
            {
                //dateTo = DateTime.Now;
                dateTo = DateTime.Now.AddDays(1);
            }

            var totalItems = await _depositService.GetPendingDepositEntrysTotalCount(
                contextUserId,
                query.generalSearch ?? string.Empty,
                dateFrom,
                dateTo,
                query.userType ?? string.Empty,
                query.status ?? string.Empty
                );

            var depositEntries = await _depositService.GetPendingDepositEntrys(
                pagination.page - 1,
                pagination.perpage,
                contextUserId,
                query.generalSearch ?? string.Empty,
                sort.field,
                dateFrom,
                dateTo,
                query.userType ?? string.Empty,
                query.status ?? string.Empty,
                sort.sort);

            var jsonResult = new KTDatatableResult<DepositEntry>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "DepositId"
                },
                data = depositEntries
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.DepositManagement.DepositBankAccounts.View)]
        [HttpGet]
        public IActionResult DepositBankAccount()
        {
            return View();
        }


        [Authorize(Policy = Permissions.DepositManagement.DepositBankAccounts.View)]
        [HttpPost]
        public async Task<IActionResult> DepositBankAccount([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("DepositBankAccount", "Deposit");
            }

            var totalItems = await _depositService.GetDepositBankAccountsTotalCount(
                query.generalSearch ?? string.Empty);

            var depositBankAccount = _depositService.GetDepositBankAccounts(
                pagination.page - 1,
                pagination.perpage,
                query.generalSearch ?? string.Empty,
                sort.field,
                sort.sort
                );

            var jsonResult = new KTDatatableResult<DepositBankAccount>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "DepositBankId"
                },
                data = depositBankAccount
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.DepositManagement.DepositBankAccounts.View)]
        [HttpGet]
        public async Task<IActionResult> DepositBankAccountDetail(int depositBankAccountId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request.");
            }

            try
            {
                var depositBankAccount = await _depositService.GetDepositBankAccount(depositBankAccountId);

                return Json(new
                {
                    BankName = depositBankAccount.BankName,
                    AccountName = depositBankAccount.AccountName,
                    AccountNumber = depositBankAccount.AccountNumber
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [Authorize(Policy = Permissions.DepositManagement.Deposits.Create)]
        // GET: Deposit/AvailableBalance
        public async Task<IActionResult> AvailableBalance(string userId)
        {
            try
            {
                var availableBalance = await _personalService.GetAvailableBalance(
                    userId,
                    User.GetUserId<string>());

                return Json(new { AvailableBalance = availableBalance });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Policy = Permissions.DepositManagement.Deposits.SearchUser)]
        [HttpGet]
        public async Task<IActionResult> SearchUser(string generalSearch, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(generalSearch))
            {
                return BadRequest("Invalid query string.");
            }

            int totalItems = 0;
            IQueryable<Object> users = null;

            totalItems = await this._userManager.Users
                .Where(u => (u.BaseRoleType == BaseRoleType.Trader || u.BaseRoleType == BaseRoleType.TraderAgent)
                && (u.UserName.Contains(generalSearch) || u.FullName.Contains(generalSearch)
                || u.Nickname.Contains(generalSearch) || u.Email.Contains(generalSearch)))
                .CountAsync();
            users = _userManager.Users
                .Where(u => (u.BaseRoleType == BaseRoleType.Trader || u.BaseRoleType == BaseRoleType.TraderAgent)
                && (u.UserName.Contains(generalSearch) || u.FullName.Contains(generalSearch)
                || u.Nickname.Contains(generalSearch) || u.Email.Contains(generalSearch)))
                .Select(u => new
                {
                    UserId = u.Id,
                    Username = u.UserName,
                    FullName = u.FullName,
                    IsEnabled = u.IsEnabled,
                    IsReviewed = u.IsReviewed
                })
                .Skip(10 * (page - 1))
                .Take(10);

            var jsonResult = new KTDatatableResult<object>
            {
                meta = new KTPagination
                {
                    page = page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / 10)),
                    perpage = 10,
                    total = totalItems,
                    sort = "desc",
                    field = "UserId"
                },
                data = users.ToList()
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.DepositManagement.Deposits.SearchTraderUser)]
        [HttpGet]
        public async Task<IActionResult> SearchTraderUser(string generalSearch, int page = 1)
        {
            var contextTraderAgentId = User.GetUserId<string>();
            int totalItems = 0;
            IQueryable<Object> users = null;


            totalItems = await this._userManager.Users
                .Where(u => (u.BaseRoleType == BaseRoleType.Trader && u.UplineId == contextTraderAgentId)
                && (u.UserName.Contains(generalSearch) || u.FullName.Contains(generalSearch)
                || u.Nickname.Contains(generalSearch) || u.Email.Contains(generalSearch)))
                .CountAsync();
            users = _userManager.Users
                .Where(u => (u.BaseRoleType == BaseRoleType.Trader && u.UplineId == contextTraderAgentId)
                && (u.UserName.Contains(generalSearch) || u.FullName.Contains(generalSearch)
                || u.Nickname.Contains(generalSearch) || u.Email.Contains(generalSearch)))
                .Select(u => new
                {
                    UserId = u.Id,
                    Username = u.UserName,
                    FullName = u.FullName,
                    IsEnabled = u.IsEnabled,
                    IsReviewed = u.IsReviewed
                })
                .Skip(10 * (page - 1))
                .Take(10);

            var jsonResult = new KTDatatableResult<object>
            {
                meta = new KTPagination
                {
                    page = page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / 10)),
                    perpage = 10,
                    total = totalItems,
                    sort = "desc",
                    field = "UserId"
                },
                data = users.ToList()
            };

            return Json(jsonResult);
        }


        // POST: Deposit/CreateDepositBankAccount
        [Authorize(Policy = Permissions.DepositManagement.DepositBankAccounts.Create)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDepositBankAccount(string name, string bankName, string accountName, string accountNumber)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }

                await _depositService.CreateDepositBankAccount(name, bankName, accountName, accountNumber);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Deposit/CreateDepositBankAccount
        [Authorize(Policy = Permissions.DepositManagement.DepositBankAccounts.Delete)]
        [HttpPost]
        public async Task<IActionResult> DeleteDepositBankAccounts([FromBody]List<int> depositBankAccountIds)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }

                await _depositService.DeleteDepositBankAccounts(depositBankAccountIds);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Deposit/Create
        [Authorize(Policy = Permissions.DepositManagement.Deposits.Create)]
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

                await _depositService.CreateDeposit(
                    vm.UserId,
                    vm.DepositAmount,
                    vm.Description,
                    User.GetUserId<string>(),
                    vm.DepositBankAccountId
                    );

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Deposit/Verify
        [Authorize(Policy = Permissions.DepositManagement.PendingReview.Verify)]
        [HttpPost]
        public async Task<IActionResult> Verify([FromBody]List<int> ids)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }

                var contextUserId = User.GetUserId<string>();

                await _depositService.VerifyDeposits(ids, contextUserId);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Deposit/Cancel
        [Authorize(Policy = Permissions.DepositManagement.PendingReview.Cancel)]
        [HttpPost]
        public async Task<IActionResult> Cancel([FromBody]int depositId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }

                var contextUserId = User.GetUserId<string>();

                await _depositService.CancelDeposit(depositId, contextUserId);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}