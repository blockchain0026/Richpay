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
using WebMVC.ViewModels.Json;
using WebMVC.ViewModels.WithdrawalViewModels;

namespace WebMVC.Controllers
{
    [Authorize(Policy = Permissions.Additional.Reviewed)]
    public class WithdrawalController : Controller
    {
        private readonly IWithdrawalService _withdrawalService;
        private readonly IPersonalService _personalService;
        private readonly UserManager<ApplicationUser> _userManager;

        public WithdrawalController(IWithdrawalService withdrawalService, IPersonalService personalService, UserManager<ApplicationUser> userManager)
        {
            _withdrawalService = withdrawalService ?? throw new ArgumentNullException(nameof(withdrawalService));
            _personalService = personalService ?? throw new ArgumentNullException(nameof(personalService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }



        [Authorize(Policy = Permissions.WithdrawalManagement.Withdrawals.View)]
        public IActionResult Index()
        {
            var vm = new IndexViewModel
            {
                UserId = User.GetUserId<string>(),
                UserBaseRole = User.IsInRole(Roles.Manager) ? "Manager" : "",
                WithdrawalBankOptions = _withdrawalService.GetWithdrawalBankOptionSelectList()
            };

            return View(vm);
        }


        [Authorize(Policy = Permissions.WithdrawalManagement.Withdrawals.View)]
        [HttpPost]
        public async Task<IActionResult> Search([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Withdrawal");
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

            var totalItems = await _withdrawalService.GetWithdrawalEntrysTotalCount(
                contextUserId,
                query.generalSearch ?? string.Empty,
                dateFrom,
                dateTo,
                query.userType ?? string.Empty
                );

            var withdrawalEntries = await _withdrawalService.GetWithdrawalEntrys(
                pagination.page - 1,
                pagination.perpage,
                contextUserId,
                query.generalSearch ?? string.Empty,
                sort.field,
                dateFrom,
                dateTo,
                query.userType ?? string.Empty,
                sort.sort);

            var jsonResult = new KTDatatableResult<WithdrawalEntry>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "WithdrawalId"
                },
                data = withdrawalEntries
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.WithdrawalManagement.PendingReview.View)]
        [HttpGet]
        public IActionResult PendingReview()
        {
            return View();
        }


        [Authorize(Policy = Permissions.WithdrawalManagement.PendingReview.View)]
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

            var totalItems = await _withdrawalService.GetPendingWithdrawalEntrysTotalCount(
                contextUserId,
                query.generalSearch ?? string.Empty,
                dateFrom,
                dateTo,
                query.userType ?? string.Empty,
                query.status ?? string.Empty
                );

            var withdrawalEntries = await _withdrawalService.GetPendingWithdrawalEntrys(
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

            var jsonResult = new KTDatatableResult<WithdrawalEntry>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "WithdrawalId"
                },
                data = withdrawalEntries
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.WithdrawalManagement.BankOptions.View)]
        [HttpGet]
        public IActionResult BankOption()
        {
            return View();
        }


        [Authorize(Policy = Permissions.WithdrawalManagement.BankOptions.View)]
        [HttpPost]
        public async Task<IActionResult> BankOption([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "PendingReview");
            }

            var totalItems = await _withdrawalService.GetWithdrawalBankOptionsTotalCount(
                query.generalSearch ?? string.Empty);

            var withdrawalBankOption = _withdrawalService.GetWithdrawalBankOptions(
                pagination.page - 1,
                pagination.perpage,
                query.generalSearch ?? string.Empty,
                sort.field,
                sort.sort
                );

            var jsonResult = new KTDatatableResult<WithdrawalBankOption>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "WithdrawalBankId"
                },
                data = withdrawalBankOption
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.WithdrawalManagement.Withdrawals.Create)]
        // GET: Withdrawal/AvailableBalance
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


        [Authorize(Policy = Permissions.WithdrawalManagement.Withdrawals.SearchUser)]
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
                .Where(u => u.BaseRoleType != BaseRoleType.Manager && (u.UserName.Contains(generalSearch)
                || u.FullName.Contains(generalSearch)
                || u.Nickname.Contains(generalSearch)
                || u.Email.Contains(generalSearch)
                ))
                .CountAsync();
            users = _userManager.Users
                .Where(u => u.BaseRoleType != BaseRoleType.Manager && (u.UserName.Contains(generalSearch)
                || u.FullName.Contains(generalSearch)
                || u.Nickname.Contains(generalSearch)
                || u.Email.Contains(generalSearch)))
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


        // POST: Withdrawal/CreateBankOption
        [Authorize(Policy = Permissions.WithdrawalManagement.BankOptions.Create)]
        [HttpPost]
        public async Task<IActionResult> CreateBankOption(string bankName)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }

                await _withdrawalService.CreateWithdrawalBankOption(bankName);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Withdrawal/CreateBankOption
        [Authorize(Policy = Permissions.WithdrawalManagement.BankOptions.Delete)]
        [HttpPost]
        public async Task<IActionResult> DeleteBankOptions([FromBody]List<int> bankOptionIds)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }

                await _withdrawalService.DeleteWithdrawalBankOptions(bankOptionIds);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        // POST: Withdrawal/Create
        [Authorize(Policy = Permissions.WithdrawalManagement.Withdrawals.Create)]
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


                await _withdrawalService.CreateWithdrawal(
                    vm.UserId,
                    vm.WithdrawalAmount,
                    vm.WithdrawalBankOptionId,
                    vm.AccountName,
                    vm.AccountNumber,
                    vm.Description,
                    User.GetUserId<string>()
                    );

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Withdrawal/Approve
        [Authorize(Policy = Permissions.WithdrawalManagement.PendingReview.Approve)]
        [HttpPost]
        public async Task<IActionResult> Approve([FromBody]List<int> ids)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }

                var contextUserId = User.GetUserId<string>();

                await _withdrawalService.ApproveWithdrawals(ids, contextUserId);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Withdrawal/ComfirmPayment
        [Authorize(Policy = Permissions.WithdrawalManagement.PendingReview.ConfirmPayment)]
        [HttpPost]
        public async Task<IActionResult> ComfirmPayment([FromBody]int withdrawalId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }

                var contextUserId = User.GetUserId<string>();

                await _withdrawalService.ConfirmWithdrawalPaymentReceived(withdrawalId, contextUserId);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Withdrawal/ForceSuccess
        [Authorize(Policy = Permissions.WithdrawalManagement.PendingReview.ForceSuccess)]
        [HttpPost]
        public async Task<IActionResult> ForceSuccess([FromBody]List<int> ids)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }

                var contextUserId = User.GetUserId<string>();

                await _withdrawalService.ForceWithdrawalsSuccess(ids, contextUserId);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Withdrawal/ComfirmPayment
        [Authorize(Policy = Permissions.WithdrawalManagement.PendingReview.Cancel)]
        [HttpPost]
        public async Task<IActionResult> Cancel([FromBody]int withdrawalId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }

                var contextUserId = User.GetUserId<string>();

                await _withdrawalService.CancelWithdrawal(withdrawalId, contextUserId);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Withdrawal/ApproveCancellation
        [Authorize(Policy = Permissions.WithdrawalManagement.PendingReview.ApproveCancellation)]
        [HttpPost]
        public async Task<IActionResult> ApproveCancellation([FromBody]List<int> ids)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }

                var contextUserId = User.GetUserId<string>();

                await _withdrawalService.ApproveCancellations(ids, contextUserId);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}