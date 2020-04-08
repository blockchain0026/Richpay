using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Extensions;
using WebMVC.Infrastructure.Services;
using WebMVC.Models.Permissions;
using WebMVC.Models.Queries;
using WebMVC.Models.Roles;
using WebMVC.ViewModels;
using WebMVC.ViewModels.Json;
using WebMVC.ViewModels.TraderViewModels;

namespace WebMVC.Controllers
{
    [Authorize(Policy = Permissions.Additional.Reviewed)]
    public class TraderController : Controller
    {
        private readonly ITraderService _traderService;
        private readonly ITraderAgentService _traderAgentService;
        private readonly IBalanceService _balanceService;
        private readonly ISystemConfigurationService _systemConfigurationService;

        public TraderController(ITraderService traderService, ITraderAgentService traderAgentService, IBalanceService balanceService, ISystemConfigurationService systemConfigurationService)
        {
            _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
            _traderAgentService = traderAgentService ?? throw new ArgumentNullException(nameof(traderAgentService));
            _balanceService = balanceService ?? throw new ArgumentNullException(nameof(balanceService));
            _systemConfigurationService = systemConfigurationService ?? throw new ArgumentNullException(nameof(systemConfigurationService));
        }

        [Authorize(Policy = Permissions.Organization.Traders.View)]
        public IActionResult Index(int? page, string searchString = null)
        {
            return View();
        }



        [Authorize(Policy = Permissions.Organization.TraderAgents.Downlines.View)]
        public IActionResult Downlines(int? page, string searchString = null)
        {
            return View("DownlineIndex");
        }


        [Authorize(Policy = Permissions.Organization.Traders.View)]
        [HttpGet]
        public async Task<IActionResult> Detail(string traderId)
        {
            if (string.IsNullOrWhiteSpace(traderId))
            {
                return BadRequest("Invalid trader Id.");
            }
            var contextUserId = User.GetUserId<string>();

            var trader = await _traderService.GetTrader(
                traderId,
                User.IsInRole(Roles.TraderAgent) && contextUserId != traderId ?
                User.GetUserId<string>() : null
                );

            return Json(trader);
        }



        [Authorize(Policy = Permissions.Organization.TraderAgents.Downlines.View)]
        [HttpGet]
        public async Task<IActionResult> DownlineDetail(string traderId)
        {
            if (string.IsNullOrWhiteSpace(traderId))
            {
                return BadRequest("Invalid trader Id.");
            }
            var contextUserId = User.GetUserId<string>();

            var trader = await _traderService.GetTrader(
                traderId,
                //If trader agent is searching his details, then get it.
                //If trader agent is searching someone else's detail, then validate his right.
                User.IsInRole(Roles.TraderAgent) && contextUserId != traderId ?
                User.GetUserId<string>() : null
                );

            return Json(trader);
        }


        [Authorize(Policy = Permissions.Organization.Traders.View)]
        [HttpGet]
        public async Task<IActionResult> Search(string generalSearch, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(generalSearch))
            {
                return BadRequest("Invalid query string.");
            }

            var totalItems = await _traderService.GetTradersTotalCount(
                generalSearch);

            var traders = await _traderService.GetTraders(
                page - 1,
                10,
                generalSearch);

            var jsonResult = new KTDatatableResult<Trader>
            {
                meta = new KTPagination
                {
                    page = page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / 10)),
                    perpage = 10,
                    total = totalItems,
                    sort = "desc",
                    field = "TraderId"
                },
                data = traders
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.Organization.Traders.View)]
        [HttpPost]
        public async Task<IActionResult> Search([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Trader");
            }

            var totalItems = await _traderService.GetTradersTotalCount(
                query.generalSearch
                );

            var traders = await _traderService.GetTraders(
                pagination.page - 1,
                pagination.perpage,
                query.generalSearch ?? string.Empty,
                sort.field,
                sort.sort);

            var jsonResult = new KTDatatableResult<Trader>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "TraderId"
                },
                data = traders
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.Organization.TraderAgents.Downlines.View)]
        [HttpPost]
        public async Task<IActionResult> SearchDownlines([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Trader");
            }

            string userId = query.userId;
            //If context user is trader agent ,
            //set it to trader agent's id to search his downlines.
            if (User.IsInRole(Roles.TraderAgent))
            {
                if (string.IsNullOrEmpty(query.userId))
                {
                    userId = User.GetUserId<string>();
                }
                else
                {
                    return BadRequest("Trader agent can only search his downline traders.");
                }
            }

            var totalItems = await _traderService.GetDownlinesTotalCount(
                userId,
                query.downlineTraderSearch ?? string.Empty
                );

            var traders = await _traderService.GetDownlines(
                pagination.page - 1,
                pagination.perpage,
                userId,
                query.downlineTraderSearch ?? string.Empty,
                sort.field,
                sort.sort);

            var jsonResult = new KTDatatableResult<Trader>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "TraderId"
                },
                data = traders
            };

            return Json(jsonResult);
        }




        [Authorize(Policy = Permissions.Organization.Traders.BankBook.View)]
        [HttpPost]
        public async Task<IActionResult> Bankbook([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (string.IsNullOrEmpty(query.userId))
            {
                return BadRequest("Invalid params.");
            }

            var totalItems = await _traderService.GetBankbookRecordsTotalCount(
                query.userId,
                User.IsInRole(Roles.TraderAgent) ? User.GetUserId<string>() : null,
                query.generalSearch
                );

            var bankbookRecords = await _traderService.GetBankbookRecords(
                pagination.page - 1,
                pagination.perpage,
                query.userId,
                User.IsInRole(Roles.TraderAgent) ? User.GetUserId<string>() : null,
                query.generalSearch,
                sort.field,
                sort.sort);

            var jsonResult = new KTDatatableResult<BankbookRecord>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "Id"
                },
                data = bankbookRecords
            };

            return Json(jsonResult);
        }



        [Authorize(Policy = Permissions.Organization.Traders.FrozenRecord.View)]
        [HttpPost]
        public async Task<IActionResult> FrozenRecord([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (string.IsNullOrEmpty(query.userId))
            {
                return BadRequest("Invalid params.");
            }

            var totalItems = await _traderService.GetAwaitingUnfrozeByAdminTotalCount(
                query.userId,
                User.IsInRole(Roles.TraderAgent) ? User.GetUserId<string>() : null,
                query.generalSearch
                );

            var frozenRecords = await _traderService.GetAwaitingUnfrozeByAdmin(
                query.userId,
                pagination.page - 1,
                pagination.perpage,
                User.IsInRole(Roles.TraderAgent) ? User.GetUserId<string>() : null,
                query.generalSearch,
                sort.field,
                sort.sort);

            var jsonResult = new KTDatatableResult<FrozenRecord>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "Id"
                },
                data = frozenRecords
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.Organization.Traders.PendingReview.View)]
        [HttpGet]
        public IActionResult PendingReview()
        {
            return View();
        }


        [Authorize(Policy = Permissions.Organization.Traders.PendingReview.View)]
        [HttpPost]
        public async Task<IActionResult> PendingReview([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Manager");
            }

            int totalItems;
            List<Trader> traders;

            //If User is a trader agent, then he can only see his downlines.
            if (User.IsInRole(Roles.TraderAgent))
            {
                totalItems = await _traderService.GetPendingReviewDownlinesTotalCount(
                    User.GetUserId<string>(),
                    query.generalSearch
                    );
                traders = _traderService.GetPendingReviewDownlines(
                    pagination.page - 1,
                    pagination.perpage,
                    User.GetUserId<string>(),
                    query.generalSearch ?? string.Empty,
                    sort.field,
                    sort.sort);
            }
            else
            {
                totalItems = await _traderService.GetPendingReviewTradersTotalCount(
                    query.generalSearch
                    );
                traders = _traderService.GetPendingReviewTraders(
                    pagination.page - 1,
                    pagination.perpage,
                    query.generalSearch ?? string.Empty,
                    sort.field,
                    sort.sort);
            }


            var jsonResult = new KTDatatableResult<Trader>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "TraderId"
                },
                data = traders
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.Organization.Traders.PendingReview.Review)]
        [HttpPost]
        public async Task<IActionResult> Review([FromBody]List<AccountReview> accounts)
        {
            if (!TryValidateModel(accounts) || !accounts.Any())
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                await _traderService.ReviewTraders(accounts);
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


        [Authorize(Policy = Permissions.Organization.Traders.Create)]
        // GET: Trader/Create
        public async Task<IActionResult> Create()
        {
            string uplineId = null;
            //If the context user is a trader agent, set the upline Id to his Id.
            if (User.IsInRole(Roles.TraderAgent))
            {
                uplineId = User.GetUserId<string>();
            }


            var withdrawalLimit = _systemConfigurationService.GetWithdrawalAndDepositAsync();

            TradingCommission uplineTradingCommission = null;
            int maxRateAlipay = 999;
            int maxRateWechat = 999;

            if (!string.IsNullOrEmpty(uplineId))
            {
                uplineTradingCommission = await _traderAgentService.GetTradingCommissionFromTraderAgentId(uplineId);
                maxRateAlipay = uplineTradingCommission.RateAlipayInThousandth;
                maxRateWechat = uplineTradingCommission.RateWechatInThousandth;

            }

            var vm = new CreateViewModel()
            {
                DailyAmountLimit = withdrawalLimit.WithdrawalTemplate.DailyAmountLimit,
                DailyFrequencyLimit = withdrawalLimit.WithdrawalTemplate.DailyFrequencyLimit,
                EachAmountUpperLimit = withdrawalLimit.WithdrawalTemplate.EachAmountUpperLimit,
                EachAmountLowerLimit = withdrawalLimit.WithdrawalTemplate.EachAmountLowerLimit,

                WithdrawalCommissionRateInThousandth = withdrawalLimit.WithdrawalTemplate.CommissionInThousandth,
                DepositCommissionRateInThousandth = 0,

                UplineId = uplineId,

                MaxRateAlipayInThousandth = maxRateAlipay,
                MaxRateWechatInThousandth = maxRateWechat
            };

            return View(vm);
        }


        // POST: Trader/Create
        [Authorize(Policy = Permissions.Organization.Traders.Create)]
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

                string uplineUserId = null;
                string uplineUserName = null;
                string uplineFullName = null;

                var uplineTraderAgent = await _traderAgentService.GetTraderAgent(vm.UplineId);

                if (User.IsInRole(Roles.TraderAgent))
                {
                    if (uplineTraderAgent.TraderAgentId != User.GetUserId<string>())
                    {
                        throw new InvalidOperationException("Invalid upline Id.");
                    }
                }

                uplineUserId = uplineTraderAgent.TraderAgentId;
                uplineUserName = uplineTraderAgent.Username;
                uplineFullName = uplineTraderAgent.FullName;


                var trader = new Trader
                {
                    Username = vm.Username,
                    FullName = vm.FullName,
                    Nickname = !string.IsNullOrWhiteSpace(vm.Nickname) ? vm.Nickname : vm.FullName,
                    PhoneNumber = vm.PhoneNumber,
                    Email = vm.Email,
                    Wechat = vm.Wechat,
                    QQ = vm.QQ,
                    UplineUserId = uplineUserId,
                    UplineUserName = uplineUserName,
                    UplineFullName = uplineFullName,
                    Balance = new Balance
                    {
                        WithdrawalLimit = new WithdrawalLimit
                        {
                            DailyAmountLimit = vm.DailyAmountLimit,
                            DailyFrequencyLimit = vm.DailyFrequencyLimit,
                            EachAmountUpperLimit = vm.EachAmountUpperLimit,
                            EachAmountLowerLimit = vm.EachAmountLowerLimit
                        },
                        WithdrawalCommissionRateInThousandth = vm.WithdrawalCommissionRateInThousandth,
                        DepositCommissionRateInThousandth = vm.DepositCommissionRateInThousandth,
                    },
                    TradingCommission = new TradingCommission
                    {
                        RateAlipayInThousandth = vm.RateAlipayInThousandth,
                        RateWechatInThousandth = vm.RateWechatInThousandth
                    },
                    IsEnabled = false,
                    IsReviewed = false
                };

                await _traderService.CreateTrader(
                    trader,
                    vm.Password,
                    User.IsInRole(Roles.TraderAgent) ? User.GetUserId<string>() : null
                   );

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                //Delete the trader agent to create and all data relate to him to handle eventual consistency and compensatory actions;
                try
                {
                    if (vm.Username != User.Identity.Name)
                    {
                        await _traderService.DeleteTrader(traderUsername: vm.Username);
                    }
                }
                catch (Exception exDelete)
                {
                    return BadRequest("1." + ex.Message + " 2." + exDelete.Message);
                }
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Policy = Permissions.Organization.Traders.Edit)]
        [HttpPost]
        public async Task<IActionResult> UpdateAccountStatus([FromBody]List<AccountStatus> accounts)
        {
            if (!TryValidateModel(accounts) || !accounts.Any())
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                await _traderService.UpdateTraderStatus(accounts, User.IsInRole(Roles.TraderAgent) ? User.GetUserId<string>() : null);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


            return Ok(new { success = true });
        }


        [Authorize(Policy = Permissions.Organization.Traders.Edit)]
        // GET: Trader/Update/5
        public async Task<IActionResult> Update(string traderId)
        {
            try
            {
                var trader = await _traderService.GetTrader(traderId);

                if (User.IsInRole(Roles.TraderAgent))
                {
                    if (trader.UplineUserId != User.GetUserId<string>())
                    {
                        throw new InvalidOperationException("Unauthorize request.");
                    }
                }


                var maxRateAlipayInThousandth = 999;
                var maxRateWechatInThousandth = 999;


                var uplineCommission = await _traderAgentService.GetTradingCommissionFromTraderAgentId(trader.UplineUserId);
                maxRateAlipayInThousandth = uplineCommission.RateAlipayInThousandth;
                maxRateWechatInThousandth = uplineCommission.RateWechatInThousandth;


                var vm = new UpdateViewModel
                {
                    TraderId = trader.TraderId,
                    FullName = trader.FullName,
                    Nickname = trader.Nickname,
                    PhoneNumber = trader.PhoneNumber,
                    Email = trader.Email,
                    Wechat = trader.Wechat,
                    QQ = trader.QQ,
                    UplineId = trader.UplineUserId,
                    IsEnabled = trader.IsEnabled,
                    DailyAmountLimit = trader.Balance.WithdrawalLimit.DailyAmountLimit,
                    DailyFrequencyLimit = trader.Balance.WithdrawalLimit.DailyFrequencyLimit,
                    EachAmountUpperLimit = trader.Balance.WithdrawalLimit.EachAmountUpperLimit,
                    EachAmountLowerLimit = trader.Balance.WithdrawalLimit.EachAmountLowerLimit,
                    WithdrawalCommissionRateInThousandth = trader.Balance.WithdrawalCommissionRateInThousandth,
                    DepositCommissionRateInThousandth = trader.Balance.DepositCommissionRateInThousandth,
                    RateAlipayInThousandth = trader.TradingCommission.RateAlipayInThousandth,
                    RateWechatInThousandth = trader.TradingCommission.RateWechatInThousandth,
                    MaxRateAlipayInThousandth = maxRateAlipayInThousandth,
                    MaxRateWechatInThousandth = maxRateWechatInThousandth
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // POST: Trader/Update/5
        [Authorize(Policy = Permissions.Organization.Traders.Edit)]
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
                var trader = new Trader
                {
                    TraderId = vm.TraderId,
                    FullName = vm.FullName,
                    Nickname = !string.IsNullOrWhiteSpace(vm.Nickname) ? vm.Nickname : vm.FullName,
                    PhoneNumber = vm.PhoneNumber,
                    Email = vm.Email,
                    Wechat = vm.Wechat,
                    QQ = vm.QQ,

                    UplineUserId = vm.UplineId,

                    IsEnabled = vm.IsEnabled,

                    Balance = new Balance
                    {
                        WithdrawalLimit = new WithdrawalLimit
                        {
                            DailyAmountLimit = vm.DailyAmountLimit,
                            DailyFrequencyLimit = vm.DailyFrequencyLimit,
                            EachAmountUpperLimit = vm.EachAmountUpperLimit,
                            EachAmountLowerLimit = vm.EachAmountLowerLimit
                        },
                        WithdrawalCommissionRateInThousandth = vm.WithdrawalCommissionRateInThousandth,
                        DepositCommissionRateInThousandth = vm.DepositCommissionRateInThousandth,
                    },
                    TradingCommission = new TradingCommission
                    {
                        RateAlipayInThousandth = vm.RateAlipayInThousandth,
                        RateWechatInThousandth = vm.RateWechatInThousandth
                    }
                };

                await _traderService.UpdateTrader(
                    trader,
                    vm.Password,
                    User.IsInRole(Roles.TraderAgent) ? User.GetUserId<string>() : null
                    );

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Trader/ChangeBalance
        [Authorize(Policy = Permissions.Organization.Traders.ChangeBalance.Create)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeBalance(string type, string userId, string description, decimal amount)
        {
            try
            {
                if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid Request.");
                }

                await _balanceService.ChangeBalance(type, userId, amount, description, User.GetUserId<string>());

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Trader/Unfreeze
        [Authorize(Policy = Permissions.Organization.Traders.ChangeBalance.Create)]
        [HttpPost]
        public async Task<IActionResult> Unfreeze([FromBody]int frozenId)
        {
            try
            {
                await _traderService.Unfreeze(frozenId, User.GetUserId<string>());

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Trader/Delete/5
        [Authorize(Policy = Permissions.Organization.Traders.Delete)]
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]List<string> traderIds)
        {
            if (!traderIds.Any())
            {
                return BadRequest("Invalid Request Params.");
            }

            try
            {
                // TODO: Add delete logic here
                await _traderService.DeleteTraders(
                    traderIds,
                    User.IsInRole(Roles.TraderAgent) ? User.GetUserId<string>() : null
                    );
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}