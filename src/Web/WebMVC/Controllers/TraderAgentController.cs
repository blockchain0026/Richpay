using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebMVC.Extensions;
using WebMVC.Infrastructure.Services;
using WebMVC.Models;
using WebMVC.Models.Permissions;
using WebMVC.Models.Queries;
using WebMVC.Models.Roles;
using WebMVC.ViewModels;
using WebMVC.ViewModels.Json;
using WebMVC.ViewModels.TraderAgentViewModels;

namespace WebMVC.Controllers
{
    [Authorize(Policy = Permissions.Additional.Reviewed)]
    public class TraderAgentController : Controller
    {
        private readonly ITraderAgentService _traderAgentService;
        private readonly ISystemConfigurationService _systemConfigurationService;
        private readonly IBalanceService _balanceService;

        public TraderAgentController(ITraderAgentService traderAgentService, ISystemConfigurationService systemConfigurationService, IBalanceService balanceService)
        {
            _traderAgentService = traderAgentService ?? throw new ArgumentNullException(nameof(traderAgentService));
            _systemConfigurationService = systemConfigurationService ?? throw new ArgumentNullException(nameof(systemConfigurationService));
            _balanceService = balanceService ?? throw new ArgumentNullException(nameof(balanceService));
        }

        [Authorize(Policy = Permissions.Organization.TraderAgents.View)]
        public IActionResult Index(int? page, string searchString = null)
        {
            return View();
        }



        [Authorize(Policy = Permissions.Organization.TraderAgents.Downlines.View)]
        public IActionResult Downlines(int? page, string searchString = null)
        {
            return View("DownlineIndex");
        }





        [Authorize(Policy = Permissions.Organization.TraderAgents.View)]
        [HttpGet]
        public async Task<IActionResult> Detail(string traderAgentId)
        {
            if (string.IsNullOrWhiteSpace(traderAgentId))
            {
                return BadRequest("Invalid trader agent Id.");
            }
            var contextUserId = User.GetUserId<string>();

            var traderAgent = await _traderAgentService.GetTraderAgent(
                traderAgentId,
                User.IsInRole(Roles.TraderAgent) && contextUserId != traderAgentId ?
                User.GetUserId<string>() : null
                );

            return Json(traderAgent);
        }



        [Authorize(Policy = Permissions.Organization.TraderAgents.Downlines.View)]
        [HttpGet]
        public async Task<IActionResult> DownlineDetail(string traderAgentId)
        {
            if (string.IsNullOrWhiteSpace(traderAgentId))
            {
                return BadRequest("Invalid trader agent Id.");
            }
            var contextUserId = User.GetUserId<string>();

            var traderAgent = await _traderAgentService.GetTraderAgent(
                traderAgentId,
                //If trader agent is searching his details, then let it search.
                //If trader agent is searching someone else's detail, then validate his right.
                User.IsInRole(Roles.TraderAgent) && contextUserId != traderAgentId ?
                User.GetUserId<string>() : null
                );

            return Json(traderAgent);
        }


        [Authorize(Policy = Permissions.Organization.TraderAgents.View)]
        [HttpGet]
        public async Task<IActionResult> Search(string generalSearch, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(generalSearch))
            {
                return BadRequest("Invalid query string.");
            }

            var totalItems = await _traderAgentService.GetTraderAgentsTotalCount(
                generalSearch);

            var traderAgents = await _traderAgentService.GetTraderAgents(
                page - 1,
                10,
                generalSearch);

            var jsonResult = new KTDatatableResult<TraderAgent>
            {
                meta = new KTPagination
                {
                    page = page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / 10)),
                    perpage = 10,
                    total = totalItems,
                    sort = "desc",
                    field = "TraderAgentId"
                },
                data = traderAgents
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.Organization.TraderAgents.View)]
        [HttpPost]
        public async Task<IActionResult> Search([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "TraderAgent");
            }

            var totalItems = await _traderAgentService.GetTraderAgentsTotalCount(
                query.generalSearch
                );

            var traderAgents = await _traderAgentService.GetTraderAgents(
                pagination.page - 1,
                pagination.perpage,
                query.generalSearch ?? string.Empty,
                sort.field,
                sort.sort);

            var jsonResult = new KTDatatableResult<TraderAgent>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "TraderAgentId"
                },
                data = traderAgents
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
            //If context user is trader agent and the top user Id is not provided,
            //set it to trader agent's id to search his downlines.
            if (User.IsInRole(Roles.TraderAgent))
            {
                if (string.IsNullOrEmpty(query.userId))
                {
                    userId = User.GetUserId<string>();
                }
            }

            var totalItems = await _traderAgentService.GetDownlinesTotalCount(
                userId,
                query.downlineSearch ?? string.Empty,
                searchByUplineId: User.IsInRole(Roles.TraderAgent) ? User.GetUserId<string>() : null
                );

            var traderAgents = await _traderAgentService.GetDownlines(
                pagination.page - 1,
                pagination.perpage,
                userId,
                searchByUplineId: User.IsInRole(Roles.TraderAgent) ? User.GetUserId<string>() : null,
                query.downlineSearch ?? string.Empty,
                sort.field,
                sort.sort);

            var jsonResult = new KTDatatableResult<TraderAgent>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "TraderAgentId"
                },
                data = traderAgents
            };

            return Json(jsonResult);
        }




        [Authorize(Policy = Permissions.Organization.TraderAgents.BankBook.View)]
        [HttpPost]
        public async Task<IActionResult> Bankbook([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (string.IsNullOrEmpty(query.userId))
            {
                return BadRequest("Invalid params.");
            }

            var totalItems = await _traderAgentService.GetBankbookRecordsTotalCount(
                query.userId,
                User.IsInRole(Roles.TraderAgent) ? User.GetUserId<string>() : null,
                query.generalSearch
                );

            var bankbookRecords = await _traderAgentService.GetBankbookRecords(
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



        [Authorize(Policy = Permissions.Organization.TraderAgents.FrozenRecord.View)]
        [HttpPost]
        public async Task<IActionResult> FrozenRecord([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (string.IsNullOrEmpty(query.userId))
            {
                return BadRequest("Invalid params.");
            }

            var totalItems = await _traderAgentService.GetAwaitingUnfrozeByAdminTotalCount(
                query.userId,
                User.IsInRole(Roles.TraderAgent) ? User.GetUserId<string>() : null,
                query.generalSearch
                );

            var frozenRecords = await _traderAgentService.GetAwaitingUnfrozeByAdmin(
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


        [Authorize(Policy = Permissions.Organization.TraderAgents.PendingReview.View)]
        [HttpGet]
        public IActionResult PendingReview()
        {
            return View();
        }


        [Authorize(Policy = Permissions.Organization.TraderAgents.PendingReview.View)]
        [HttpPost]
        public async Task<IActionResult> PendingReview([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "TraderAgent");
            }

            int totalItems = 0;
            List<TraderAgent> traderAgents = null;

            //If User is a trader agent, then he can only see his downlines.
            if (User.IsInRole(Roles.TraderAgent))
            {
                totalItems = await _traderAgentService.GetPendingReviewDownlinesTotalCount(
                    User.GetUserId<string>(),
                    query.generalSearch
                    );
                traderAgents = _traderAgentService.GetPendingReviewDownlines(
                    pagination.page - 1,
                    pagination.perpage,
                    User.GetUserId<string>(),
                    query.generalSearch ?? string.Empty,
                    sort.field,
                    sort.sort);
            }
            else
            {
                totalItems = await _traderAgentService.GetPendingReviewTraderAgentsTotalCount(
                    query.generalSearch
                    );
                traderAgents = _traderAgentService.GetPendingReviewTraderAgents(
                    pagination.page - 1,
                    pagination.perpage,
                    query.generalSearch ?? string.Empty,
                    sort.field,
                    sort.sort);
            }





            var jsonResult = new KTDatatableResult<TraderAgent>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "TraderAgentId"
                },
                data = traderAgents
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.Organization.TraderAgents.PendingReview.Review)]
        [HttpPost]
        public async Task<IActionResult> Review([FromBody]List<AccountReview> accounts)
        {
            if (!TryValidateModel(accounts) || !accounts.Any())
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                await _traderAgentService.ReviewTraderAgents(accounts);
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


        [Authorize(Policy = Permissions.Organization.TraderAgents.Create)]
        // GET: TraderAgent/Create
        public async Task<IActionResult> Create()
        {
            string uplineId = null;
            //If the context user is a trader agent, checking he has the right to create trader agents.
            if (User.IsInRole(Roles.TraderAgent))
            {
                if (!User.IsInRole(Roles.TraderAgentWithGrantRight))
                {
                    return Unauthorized("The user doesn't has the right to create trader agent.");
                }
                else
                {
                    //Will give the user's Id.
                    uplineId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                }
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


        // POST: TraderAgent/Create
        [Authorize(Policy = Permissions.Organization.TraderAgents.Create)]
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

                var traderAgent = new TraderAgent
                {
                    Username = vm.Username,
                    FullName = vm.FullName,
                    Nickname = !string.IsNullOrWhiteSpace(vm.Nickname) ? vm.Nickname : vm.FullName,
                    PhoneNumber = vm.PhoneNumber,
                    Email = vm.Email,
                    Wechat = vm.Wechat,
                    QQ = vm.QQ,
                    UplineUserId = vm.UplineId,
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
                    IsReviewed = false,
                    HasGrantRight = vm.HasGrantRight
                };

                await _traderAgentService.CreateTraderAgents(
                    traderAgent,
                    vm.Password
                   );

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                //Delete the trader agent and all data relate to him to handle eventual consistency and compensatory actions;
                try
                {
                    if (vm.Username != User.Identity.Name)
                    {
                        await _traderAgentService.DeleteTraderAgent(traderAgentUsername: vm.Username);
                    }
                }
                catch (Exception exDelete)
                {
                    return BadRequest("1." + ex.Message + " 2." + exDelete.Message);
                }
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Policy = Permissions.Organization.TraderAgents.Downlines.Create)]
        // GET: TraderAgent/Create
        public async Task<IActionResult> CreateDownline()
        {
            string uplineId = null;
            //If the context user is a trader agent, set the upline id to his id in view model.
            if (User.IsInRole(Roles.TraderAgent))
            {
                //Will give the user's Id.
                uplineId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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

            return View("Create", vm);
        }


        // POST: TraderAgent/CreateDownline
        [Authorize(Policy = Permissions.Organization.TraderAgents.Downlines.Create)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDownline([FromForm]CreateViewModel vm)
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

                uplineUserId = uplineTraderAgent.TraderAgentId;
                uplineUserName = uplineTraderAgent.Username;
                uplineFullName = uplineTraderAgent.FullName;


                var traderAgent = new TraderAgent
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
                    IsReviewed = false,
                    HasGrantRight = vm.HasGrantRight
                };

                await _traderAgentService.CreateTraderAgents(
                    traderAgent,
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
                        await _traderAgentService.DeleteTraderAgent(traderAgentUsername: vm.Username);
                    }
                }
                catch (Exception exDelete)
                {
                    return BadRequest("1." + ex.Message + " 2." + exDelete.Message);
                }
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Policy = Permissions.Organization.TraderAgents.Edit)]
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
                await _traderAgentService.UpdateTraderAgentStatus(accounts);
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


        [Authorize(Policy = Permissions.Organization.TraderAgents.Edit)]
        // GET: TraderAgent/Update/5
        public async Task<IActionResult> Update(string traderAgentId)
        {
            try
            {
                var traderAgent = await _traderAgentService.GetTraderAgent(traderAgentId);
                var maxRateAlipayInThousandth = 999;
                var maxRateWechatInThousandth = 999;

                if (!string.IsNullOrEmpty(traderAgent.UplineUserId))
                {
                    var uplineCommission = await _traderAgentService.GetTradingCommissionFromTraderAgentId(traderAgent.UplineUserId);
                    maxRateAlipayInThousandth = uplineCommission.RateAlipayInThousandth;
                    maxRateWechatInThousandth = uplineCommission.RateWechatInThousandth;
                }

                var vm = new UpdateViewModel
                {
                    TraderAgentId = traderAgent.TraderAgentId,
                    FullName = traderAgent.FullName,
                    Nickname = traderAgent.Nickname,
                    PhoneNumber = traderAgent.PhoneNumber,
                    Email = traderAgent.Email,
                    Wechat = traderAgent.Wechat,
                    QQ = traderAgent.QQ,
                    UplineId = traderAgent.UplineUserId,
                    IsEnabled = traderAgent.IsEnabled,
                    HasGrantRight = traderAgent.HasGrantRight,
                    DailyAmountLimit = traderAgent.Balance.WithdrawalLimit.DailyAmountLimit,
                    DailyFrequencyLimit = traderAgent.Balance.WithdrawalLimit.DailyFrequencyLimit,
                    EachAmountUpperLimit = traderAgent.Balance.WithdrawalLimit.EachAmountUpperLimit,
                    EachAmountLowerLimit = traderAgent.Balance.WithdrawalLimit.EachAmountLowerLimit,
                    WithdrawalCommissionRateInThousandth = traderAgent.Balance.WithdrawalCommissionRateInThousandth,
                    DepositCommissionRateInThousandth = traderAgent.Balance.DepositCommissionRateInThousandth,
                    RateAlipayInThousandth = traderAgent.TradingCommission.RateAlipayInThousandth,
                    RateWechatInThousandth = traderAgent.TradingCommission.RateWechatInThousandth,
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

        // POST: TraderAgent/Update/5
        [Authorize(Policy = Permissions.Organization.TraderAgents.Edit)]
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
                var traderAgent = new TraderAgent
                {
                    TraderAgentId = vm.TraderAgentId,
                    FullName = vm.FullName,
                    Nickname = !string.IsNullOrWhiteSpace(vm.Nickname) ? vm.Nickname : vm.FullName,
                    PhoneNumber = vm.PhoneNumber,
                    Email = vm.Email,
                    Wechat = vm.Wechat,
                    QQ = vm.QQ,

                    UplineUserId = vm.UplineId,

                    IsEnabled = vm.IsEnabled,
                    HasGrantRight = vm.HasGrantRight,

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

                await _traderAgentService.UpdateTraderAgent(
                    traderAgent,
                    vm.Password);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: TraderAgent/SubmitTransfer
        [Authorize(Policy = Permissions.Organization.TraderAgents.Transfer.Create)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitTransfer(string fromTraderAgentId, string toTraderId, decimal amount)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }

                await _traderAgentService.SubmitTransfer(fromTraderAgentId, toTraderId, User.GetUserId<string>(), amount);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: TraderAgent/ChangeBalance
        [Authorize(Policy = Permissions.Organization.TraderAgents.ChangeBalance.Create)]
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




        // POST: TraderAgent/Unfreeze
        [Authorize(Policy = Permissions.Organization.TraderAgents.ChangeBalance.Create)]
        [HttpPost]
        public async Task<IActionResult> Unfreeze([FromBody]int frozenId)
        {
            try
            {
                await _traderAgentService.Unfreeze(frozenId, User.GetUserId<string>());

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }





        // POST: TraderAgent/Delete/5
        [Authorize(Policy = Permissions.Organization.TraderAgents.Delete)]
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]List<string> traderAgentIds)
        {
            if (!traderAgentIds.Any())
            {
                return BadRequest("Invalid Request Params.");
            }

            try
            {
                // TODO: Add delete logic here
                await _traderAgentService.DeleteTraderAgents(traderAgentIds);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




    }
}