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
using WebMVC.ViewModels.ShopAgentViewModels;

namespace WebMVC.Controllers
{
    [Authorize(Policy = Permissions.Additional.Reviewed)]
    public class ShopAgentController : Controller
    {
        private readonly IShopAgentService _shopAgentService;
        private readonly ISystemConfigurationService _systemConfigurationService;
        private readonly IBalanceService _balanceService;

        public ShopAgentController(IShopAgentService shopAgentService, ISystemConfigurationService systemConfigurationService, IBalanceService balanceService)
        {
            _shopAgentService = shopAgentService ?? throw new ArgumentNullException(nameof(shopAgentService));
            _systemConfigurationService = systemConfigurationService ?? throw new ArgumentNullException(nameof(systemConfigurationService));
            _balanceService = balanceService ?? throw new ArgumentNullException(nameof(balanceService));
        }

        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.View)]
        public IActionResult Index(int? page, string searchString = null)
        {
            return View();
        }



        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.Downlines.View)]
        public IActionResult Downlines(int? page, string searchString = null)
        {
            return View("DownlineIndex");
        }





        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.View)]
        [HttpGet]
        public async Task<IActionResult> Detail(string shopAgentId)
        {
            if (string.IsNullOrWhiteSpace(shopAgentId))
            {
                return BadRequest("Invalid shop agent Id.");
            }
            var contextUserId = User.GetUserId<string>();

            var shopAgent = await _shopAgentService.GetShopAgent(
                shopAgentId,
                User.IsInRole(Roles.ShopAgent) && contextUserId != shopAgentId ?
                User.GetUserId<string>() : null
                );

            return Json(shopAgent);
        }



        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.Downlines.View)]
        [HttpGet]
        public async Task<IActionResult> DownlineDetail(string shopAgentId)
        {
            if (string.IsNullOrWhiteSpace(shopAgentId))
            {
                return BadRequest("Invalid shop agent Id.");
            }
            var contextUserId = User.GetUserId<string>();

            var shopAgent = await _shopAgentService.GetShopAgent(
                shopAgentId,
                //If shop agent is searching his details, then let it search.
                //If shop agent is searching someone else's detail, then validate his right.
                User.IsInRole(Roles.ShopAgent) && contextUserId != shopAgentId ?
                User.GetUserId<string>() : null
                );

            return Json(shopAgent);
        }


        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.View)]
        [HttpGet]
        public async Task<IActionResult> Search(string generalSearch, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(generalSearch))
            {
                return BadRequest("Invalid query string.");
            }

            var totalItems = await _shopAgentService.GetShopAgentsTotalCount(
                generalSearch);

            var shopAgents = await _shopAgentService.GetShopAgents(
                page - 1,
                10,
                generalSearch);

            var jsonResult = new KTDatatableResult<ShopAgent>
            {
                meta = new KTPagination
                {
                    page = page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / 10)),
                    perpage = 10,
                    total = totalItems,
                    sort = "desc",
                    field = "ShopAgentId"
                },
                data = shopAgents
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.View)]
        [HttpPost]
        public async Task<IActionResult> Search([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "ShopAgent");
            }

            var totalItems = await _shopAgentService.GetShopAgentsTotalCount(
                query.generalSearch
                );

            var shopAgents = await _shopAgentService.GetShopAgents(
                pagination.page - 1,
                pagination.perpage,
                query.generalSearch ?? string.Empty,
                sort.field,
                sort.sort);

            var jsonResult = new KTDatatableResult<ShopAgent>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "ShopAgentId"
                },
                data = shopAgents
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.Downlines.View)]
        [HttpPost]
        public async Task<IActionResult> SearchDownlines([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "ShopAgent");
            }

            string userId = query.userId;
            //If context user is shop agent and the top user Id is not provided,
            //set it to shop agent's id to search his downlines.
            if (User.IsInRole(Roles.ShopAgent))
            {
                if (string.IsNullOrEmpty(query.userId))
                {
                    userId = User.GetUserId<string>();
                }
            }

            var totalItems = await _shopAgentService.GetDownlinesTotalCount(
                userId,
                query.downlineSearch ?? string.Empty,
                searchByUplineId: User.IsInRole(Roles.ShopAgent) ? User.GetUserId<string>() : null
                );

            var shopAgents = await _shopAgentService.GetDownlines(
                pagination.page - 1,
                pagination.perpage,
                userId,
                searchByUplineId: User.IsInRole(Roles.ShopAgent) ? User.GetUserId<string>() : null,
                query.downlineSearch ?? string.Empty,
                sort.field,
                sort.sort);

            var jsonResult = new KTDatatableResult<ShopAgent>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "ShopAgentId"
                },
                data = shopAgents
            };

            return Json(jsonResult);
        }




        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.BankBook.View)]
        [HttpPost]
        public async Task<IActionResult> Bankbook([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (string.IsNullOrEmpty(query.userId))
            {
                return BadRequest("Invalid params.");
            }

            var totalItems = await _shopAgentService.GetBankbookRecordsTotalCount(
                query.userId,
                User.IsInRole(Roles.ShopAgent) ? User.GetUserId<string>() : null,
                query.generalSearch
                );

            var bankbookRecords = await _shopAgentService.GetBankbookRecords(
                pagination.page - 1,
                pagination.perpage,
                query.userId,
                User.IsInRole(Roles.ShopAgent) ? User.GetUserId<string>() : null,
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



        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.FrozenRecord.View)]
        [HttpPost]
        public async Task<IActionResult> FrozenRecord([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (string.IsNullOrEmpty(query.userId))
            {
                return BadRequest("Invalid params.");
            }

            var totalItems = await _shopAgentService.GetAwaitingUnfrozeByAdminTotalCount(
                query.userId,
                User.IsInRole(Roles.ShopAgent) ? User.GetUserId<string>() : null,
                query.generalSearch
                );

            var frozenRecords = await _shopAgentService.GetAwaitingUnfrozeByAdmin(
                query.userId,
                pagination.page - 1,
                pagination.perpage,
                User.IsInRole(Roles.ShopAgent) ? User.GetUserId<string>() : null,
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


        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.PendingReview.View)]
        [HttpGet]
        public IActionResult PendingReview()
        {
            return View();
        }


        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.PendingReview.View)]
        [HttpPost]
        public async Task<IActionResult> PendingReview([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "ShopAgent");
            }

            int totalItems = 0;
            List<ShopAgent> shopAgents = null;

            //If User is a shop agent, then he can only see his downlines.
            if (User.IsInRole(Roles.ShopAgent))
            {
                totalItems = await _shopAgentService.GetPendingReviewDownlinesTotalCount(
                    User.GetUserId<string>(),
                    query.generalSearch
                    );
                shopAgents = _shopAgentService.GetPendingReviewDownlines(
                    pagination.page - 1,
                    pagination.perpage,
                    User.GetUserId<string>(),
                    query.generalSearch ?? string.Empty,
                    sort.field,
                    sort.sort);
            }
            else
            {
                totalItems = await _shopAgentService.GetPendingReviewShopAgentsTotalCount(
                    query.generalSearch
                    );
                shopAgents = _shopAgentService.GetPendingReviewShopAgents(
                    pagination.page - 1,
                    pagination.perpage,
                    query.generalSearch ?? string.Empty,
                    sort.field,
                    sort.sort);
            }





            var jsonResult = new KTDatatableResult<ShopAgent>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "ShopAgentId"
                },
                data = shopAgents
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.PendingReview.Review)]
        [HttpPost]
        public async Task<IActionResult> Review([FromBody]List<AccountReview> accounts)
        {
            if (!TryValidateModel(accounts) || !accounts.Any())
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                await _shopAgentService.ReviewShopAgents(accounts);
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


        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.Create)]
        // GET: ShopAgent/Create
        public async Task<IActionResult> Create()
        {
            string uplineId = null;
            //If the context user is a shop agent, checking he has the right to create shop agents.
            if (User.IsInRole(Roles.ShopAgent))
            {
                if (!User.IsInRole(Roles.ShopAgentWithGrantRight))
                {
                    return Unauthorized("The user doesn't has the right to create shop agent.");
                }
                else
                {
                    //Will give the user's Id.
                    uplineId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                }
            }

            var withdrawalLimit = _systemConfigurationService.GetWithdrawalAndDepositAsync();

            RebateCommission uplineRebateCommission = null;
            int minRateRebateAlipay = 0;
            int minRateRebateWechat = 0;

            if (!string.IsNullOrEmpty(uplineId))
            {
                uplineRebateCommission = await _shopAgentService.GetRebateCommissionFromShopAgentId(uplineId);
                minRateRebateAlipay = uplineRebateCommission.RateRebateAlipayInThousandth + 1;
                minRateRebateWechat = uplineRebateCommission.RateRebateWechatInThousandth + 1;
            }

            var vm = new CreateViewModel()
            {
                DailyAmountLimit = withdrawalLimit.WithdrawalTemplate.DailyAmountLimit,
                DailyFrequencyLimit = withdrawalLimit.WithdrawalTemplate.DailyFrequencyLimit,
                EachAmountUpperLimit = withdrawalLimit.WithdrawalTemplate.EachAmountUpperLimit,
                EachAmountLowerLimit = withdrawalLimit.WithdrawalTemplate.EachAmountLowerLimit,

                WithdrawalCommissionRateInThousandth = withdrawalLimit.WithdrawalTemplate.CommissionInThousandth,

                UplineId = uplineId,

                MinRateRebateAlipayInThousandth = minRateRebateAlipay,
                MinRateRebateWechatInThousandth = minRateRebateWechat
            };

            return View(vm);
        }


        // POST: ShopAgent/Create
        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.Create)]
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

                var shopAgent = new ShopAgent
                {
                    Username = vm.Username,
                    FullName = vm.FullName,
                    Nickname = !string.IsNullOrWhiteSpace(vm.Nickname) ? vm.Nickname : vm.FullName,
                    PhoneNumber = vm.PhoneNumber,
                    Email = vm.Email,
                    Wechat = vm.Wechat,
                    QQ = vm.QQ,
                    UplineUserId = vm.UplineId,
                    Balance = new ShopUserBalance
                    {
                        WithdrawalLimit = new WithdrawalLimit
                        {
                            DailyAmountLimit = vm.DailyAmountLimit,
                            DailyFrequencyLimit = vm.DailyFrequencyLimit,
                            EachAmountUpperLimit = vm.EachAmountUpperLimit,
                            EachAmountLowerLimit = vm.EachAmountLowerLimit
                        },
                        WithdrawalCommissionRateInThousandth = vm.WithdrawalCommissionRateInThousandth
                    },
                    RebateCommission = new RebateCommission
                    {
                        RateRebateAlipayInThousandth = vm.RateRebateAlipayInThousandth,
                        RateRebateWechatInThousandth = vm.RateRebateWechatInThousandth
                    },
                    IsEnabled = false,
                    IsReviewed = false,
                    HasGrantRight = vm.HasGrantRight
                };

                await _shopAgentService.CreateShopAgents(
                    shopAgent,
                    vm.Password
                   );

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                //Delete the shop agent and all data relate to him to handle eventual consistency and compensatory actions;
                try
                {
                    if (vm.Username != User.Identity.Name)
                    {
                        await _shopAgentService.DeleteShopAgent(shopAgentUsername: vm.Username);
                    }
                }
                catch (Exception exDelete)
                {
                    return BadRequest("1." + ex.Message + " 2." + exDelete.Message);
                }
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.Downlines.Create)]
        // GET: ShopAgent/Create
        public async Task<IActionResult> CreateDownline()
        {
            string uplineId = null;
            //If the context user is a shop agent, set the upline id to his id in view model.
            if (User.IsInRole(Roles.ShopAgent))
            {
                //Will give the user's Id.
                uplineId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            var withdrawalLimit = _systemConfigurationService.GetWithdrawalAndDepositAsync();

            RebateCommission uplineRebateCommission = null;
            int minRateRebateAlipay = 0;
            int minRateRebateWechat = 0;

            if (!string.IsNullOrEmpty(uplineId))
            {
                uplineRebateCommission = await _shopAgentService.GetRebateCommissionFromShopAgentId(uplineId);
                minRateRebateAlipay = uplineRebateCommission.RateRebateAlipayInThousandth + 1;
                minRateRebateWechat = uplineRebateCommission.RateRebateWechatInThousandth + 1;
            }

            var vm = new CreateViewModel()
            {
                DailyAmountLimit = withdrawalLimit.WithdrawalTemplate.DailyAmountLimit,
                DailyFrequencyLimit = withdrawalLimit.WithdrawalTemplate.DailyFrequencyLimit,
                EachAmountUpperLimit = withdrawalLimit.WithdrawalTemplate.EachAmountUpperLimit,
                EachAmountLowerLimit = withdrawalLimit.WithdrawalTemplate.EachAmountLowerLimit,

                WithdrawalCommissionRateInThousandth = withdrawalLimit.WithdrawalTemplate.CommissionInThousandth,

                UplineId = uplineId,

                MinRateRebateAlipayInThousandth = minRateRebateAlipay,
                MinRateRebateWechatInThousandth = minRateRebateWechat
            };

            return View("Create", vm);
        }


        // POST: ShopAgent/CreateDownline
        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.Downlines.Create)]
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

                var uplineShopAgent = await _shopAgentService.GetShopAgent(vm.UplineId);

                uplineUserId = uplineShopAgent.ShopAgentId;
                uplineUserName = uplineShopAgent.Username;
                uplineFullName = uplineShopAgent.FullName;


                var shopAgent = new ShopAgent
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
                    Balance = new ShopUserBalance
                    {
                        WithdrawalLimit = new WithdrawalLimit
                        {
                            DailyAmountLimit = vm.DailyAmountLimit,
                            DailyFrequencyLimit = vm.DailyFrequencyLimit,
                            EachAmountUpperLimit = vm.EachAmountUpperLimit,
                            EachAmountLowerLimit = vm.EachAmountLowerLimit
                        },
                        WithdrawalCommissionRateInThousandth = vm.WithdrawalCommissionRateInThousandth
                    },
                    RebateCommission = new RebateCommission
                    {
                        RateRebateAlipayInThousandth = vm.RateRebateAlipayInThousandth,
                        RateRebateWechatInThousandth = vm.RateRebateWechatInThousandth
                    },
                    IsEnabled = false,
                    IsReviewed = false,
                    HasGrantRight = vm.HasGrantRight
                };

                await _shopAgentService.CreateShopAgents(
                    shopAgent,
                    vm.Password,
                    User.IsInRole(Roles.ShopAgent) ? User.GetUserId<string>() : null
                   );

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                //Delete the shop agent to create and all data relate to him to handle eventual consistency and compensatory actions;
                try
                {
                    if (vm.Username != User.Identity.Name)
                    {
                        await _shopAgentService.DeleteShopAgent(shopAgentUsername: vm.Username);
                    }
                }
                catch (Exception exDelete)
                {
                    return BadRequest("1." + ex.Message + " 2." + exDelete.Message);
                }
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.Edit)]
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
                await _shopAgentService.UpdateShopAgentStatus(accounts);
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


        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.Edit)]
        // GET: ShopAgent/Update/5
        public async Task<IActionResult> Update(string shopAgentId)
        {
            try
            {
                var shopAgent = await _shopAgentService.GetShopAgent(shopAgentId);
                var minRateRebateAlipayInThousandth = 0;
                var minRateRabateWechatInThousandth = 0;

                if (!string.IsNullOrEmpty(shopAgent.UplineUserId))
                {
                    var uplineCommission = await _shopAgentService.GetRebateCommissionFromShopAgentId(shopAgent.UplineUserId);
                    minRateRebateAlipayInThousandth = uplineCommission.RateRebateAlipayInThousandth + 1;
                    minRateRabateWechatInThousandth = uplineCommission.RateRebateWechatInThousandth + 1;
                }

                var vm = new UpdateViewModel
                {
                    ShopAgentId = shopAgent.ShopAgentId,
                    FullName = shopAgent.FullName,
                    Nickname = shopAgent.Nickname,
                    PhoneNumber = shopAgent.PhoneNumber,
                    Email = shopAgent.Email,
                    Wechat = shopAgent.Wechat,
                    QQ = shopAgent.QQ,
                    UplineId = shopAgent.UplineUserId,
                    IsEnabled = shopAgent.IsEnabled,
                    HasGrantRight = shopAgent.HasGrantRight,
                    DailyAmountLimit = shopAgent.Balance.WithdrawalLimit.DailyAmountLimit,
                    DailyFrequencyLimit = shopAgent.Balance.WithdrawalLimit.DailyFrequencyLimit,
                    EachAmountUpperLimit = shopAgent.Balance.WithdrawalLimit.EachAmountUpperLimit,
                    EachAmountLowerLimit = shopAgent.Balance.WithdrawalLimit.EachAmountLowerLimit,
                    WithdrawalCommissionRateInThousandth = shopAgent.Balance.WithdrawalCommissionRateInThousandth,
                    RateRebateAlipayInThousandth = shopAgent.RebateCommission.RateRebateAlipayInThousandth,
                    RateRebateWechatInThousandth = shopAgent.RebateCommission.RateRebateWechatInThousandth,
                    MinRateRebateAlipayInThousandth = minRateRebateAlipayInThousandth,
                    MinRateRebateWechatInThousandth = minRateRabateWechatInThousandth
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // POST: ShopAgent/Update
        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.Edit)]
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
                var shopAgent = new ShopAgent
                {
                    ShopAgentId = vm.ShopAgentId,
                    FullName = vm.FullName,
                    Nickname = !string.IsNullOrWhiteSpace(vm.Nickname) ? vm.Nickname : vm.FullName,
                    PhoneNumber = vm.PhoneNumber,
                    Email = vm.Email,
                    Wechat = vm.Wechat,
                    QQ = vm.QQ,

                    UplineUserId = vm.UplineId,

                    IsEnabled = vm.IsEnabled,
                    HasGrantRight = vm.HasGrantRight,

                    Balance = new ShopUserBalance
                    {
                        WithdrawalLimit = new WithdrawalLimit
                        {
                            DailyAmountLimit = vm.DailyAmountLimit,
                            DailyFrequencyLimit = vm.DailyFrequencyLimit,
                            EachAmountUpperLimit = vm.EachAmountUpperLimit,
                            EachAmountLowerLimit = vm.EachAmountLowerLimit
                        },
                        WithdrawalCommissionRateInThousandth = vm.WithdrawalCommissionRateInThousandth
                    },
                    RebateCommission = new RebateCommission
                    {
                        RateRebateAlipayInThousandth = vm.RateRebateAlipayInThousandth,
                        RateRebateWechatInThousandth = vm.RateRebateWechatInThousandth
                    }
                };

                await _shopAgentService.UpdateShopAgent(
                    shopAgent,
                    vm.Password);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        // POST: ShopAgent/ChangeBalance
        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.ChangeBalance.Create)]
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




        // POST: ShopAgent/Unfreeze
        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.ChangeBalance.Create)]
        [HttpPost]
        public async Task<IActionResult> Unfreeze([FromBody]int frozenId)
        {
            try
            {
                await _shopAgentService.Unfreeze(frozenId, User.GetUserId<string>());

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: ShopAgent/Delete/5
        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.Delete)]
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]List<string> shopAgentIds)
        {
            if (!shopAgentIds.Any())
            {
                return BadRequest("Invalid Request Params.");
            }

            try
            {
                // TODO: Add delete logic here
                await _shopAgentService.DeleteShopAgents(shopAgentIds);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}