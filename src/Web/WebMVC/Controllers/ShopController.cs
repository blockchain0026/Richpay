using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Polly;
using WebMVC.Extensions;
using WebMVC.Infrastructure.Services;
using WebMVC.Models.Permissions;
using WebMVC.Models.Queries;
using WebMVC.Models.Roles;
using WebMVC.ViewModels;
using WebMVC.ViewModels.Json;
using WebMVC.ViewModels.ShopViewModels;

namespace WebMVC.Controllers
{
    [Authorize(Policy = Permissions.Additional.Reviewed)]
    public class ShopController : Controller
    {
        private readonly IShopService _shopService;
        private readonly IApiService _apiService;
        private readonly IShopAgentService _shopAgentService;
        private readonly IBalanceService _balanceService;
        private readonly ISystemConfigurationService _systemConfigurationService;

        public ShopController(IShopService shopService, IApiService apiService, IShopAgentService shopAgentService, IBalanceService balanceService, ISystemConfigurationService systemConfigurationService)
        {
            _shopService = shopService ?? throw new ArgumentNullException(nameof(shopService));
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _shopAgentService = shopAgentService ?? throw new ArgumentNullException(nameof(shopAgentService));
            _balanceService = balanceService ?? throw new ArgumentNullException(nameof(balanceService));
            _systemConfigurationService = systemConfigurationService ?? throw new ArgumentNullException(nameof(systemConfigurationService));
        }


        [Authorize(Policy = Permissions.ShopManagement.Shops.View)]
        public IActionResult Index(int? page, string searchString = null)
        {
            return View();
        }



        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.Downlines.View)]
        public IActionResult Downlines(int? page, string searchString = null)
        {
            return View("DownlineIndex");
        }

        [Authorize(Policy = Permissions.ShopManagement.Shops.View)]
        [HttpGet]
        public async Task<IActionResult> Detail(string shopId)
        {
            if (string.IsNullOrWhiteSpace(shopId))
            {
                return BadRequest("Invalid shop Id.");
            }
            var contextUserId = User.GetUserId<string>();

            var shop = await _shopService.GetShop(
                shopId,
                User.IsInRole(Roles.ShopAgent) && contextUserId != shopId ?
                User.GetUserId<string>() : null
                );

            return Json(shop);
        }



        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.Downlines.View)]
        [HttpGet]
        public async Task<IActionResult> DownlineDetail(string shopId)
        {
            if (string.IsNullOrWhiteSpace(shopId))
            {
                return BadRequest("Invalid shop Id.");
            }
            var contextUserId = User.GetUserId<string>();

            var shop = await _shopService.GetShop(
                shopId,
                //If shop agent is searching his details, then get it.
                //If shop agent is searching someone else's detail, then validate his right.
                User.IsInRole(Roles.ShopAgent) && contextUserId != shopId ?
                User.GetUserId<string>() : null
                );

            return Json(shop);
        }


        [Authorize(Policy = Permissions.ShopManagement.Shops.View)]
        [HttpGet]
        public async Task<IActionResult> Search(string generalSearch, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(generalSearch))
            {
                return BadRequest("Invalid query string.");
            }

            var totalItems = await _shopService.GetShopsTotalCount(
                generalSearch);

            var shops = await _shopService.GetShops(
                page - 1,
                10,
                generalSearch);

            var jsonResult = new KTDatatableResult<Shop>
            {
                meta = new KTPagination
                {
                    page = page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / 10)),
                    perpage = 10,
                    total = totalItems,
                    sort = "desc",
                    field = "ShopId"
                },
                data = shops
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.ShopManagement.Shops.View)]
        [HttpPost]
        public async Task<IActionResult> Search([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Shop");
            }

            var totalItems = await _shopService.GetShopsTotalCount(
                query.generalSearch
                );

            var shops = await _shopService.GetShops(
                pagination.page - 1,
                pagination.perpage,
                query.generalSearch ?? string.Empty,
                sort.field,
                sort.sort);

            var jsonResult = new KTDatatableResult<Shop>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "ShopId"
                },
                data = shops
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.ShopManagement.ShopAgents.Downlines.View)]
        [HttpPost]
        public async Task<IActionResult> SearchDownlines([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Shop");
            }

            string userId = query.userId;
            //If context user is shop agent ,
            //set it to shop agent's id to search his downlines.
            if (User.IsInRole(Roles.ShopAgent))
            {
                if (string.IsNullOrEmpty(query.userId))
                {
                    userId = User.GetUserId<string>();
                }
                else
                {
                    return BadRequest("Shop agent can only search his downline shops.");
                }
            }

            var totalItems = await _shopService.GetDownlinesTotalCount(
                userId,
                query.downlineShopSearch ?? string.Empty
                );

            var shops = await _shopService.GetDownlines(
                pagination.page - 1,
                pagination.perpage,
                userId,
                query.downlineShopSearch ?? string.Empty,
                sort.field,
                sort.sort);

            var jsonResult = new KTDatatableResult<Shop>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "ShopId"
                },
                data = shops
            };

            return Json(jsonResult);
        }




        [Authorize(Policy = Permissions.ShopManagement.Shops.BankBook.View)]
        [HttpPost]
        public async Task<IActionResult> Bankbook([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (string.IsNullOrEmpty(query.userId))
            {
                return BadRequest("Invalid params.");
            }

            var totalItems = await _shopService.GetBankbookRecordsTotalCount(
                query.userId,
                User.IsInRole(Roles.ShopAgent) ? User.GetUserId<string>() : null,
                query.generalSearch
                );

            var bankbookRecords = await _shopService.GetBankbookRecords(
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



        [Authorize(Policy = Permissions.ShopManagement.Shops.FrozenRecord.View)]
        [HttpPost]
        public async Task<IActionResult> FrozenRecord([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (string.IsNullOrEmpty(query.userId))
            {
                return BadRequest("Invalid params.");
            }

            var totalItems = await _shopService.GetAwaitingUnfrozeByAdminTotalCount(
                query.userId,
                User.IsInRole(Roles.ShopAgent) ? User.GetUserId<string>() : null,
                query.generalSearch
                );

            var frozenRecords = await _shopService.GetAwaitingUnfrozeByAdmin(
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



        [Authorize(Policy = Permissions.ShopManagement.Shops.ShopGateway.View)]
        [HttpPost]
        public async Task<IActionResult> ShopGateway([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (string.IsNullOrEmpty(query.userId))
            {
                return BadRequest("无效参数。");
            }

            var totalItems = await _shopService.GetShopGatewaysTotalCount(
                User.GetUserId<string>(),
                query.userId,
                query.generalSearch,
                !string.IsNullOrWhiteSpace(query.shopGatewayType) ? query.shopGatewayType : null,
                !string.IsNullOrWhiteSpace(query.paymentChannel) ? query.paymentChannel : null,
                !string.IsNullOrWhiteSpace(query.paymentScheme) ? query.paymentScheme : null
                );

            var shopGateways = await _shopService.GetShopGateways(
                pagination.page - 1,
                pagination.perpage,
                User.GetUserId<string>(),
                query.userId,
                query.generalSearch,
                sort.field,
                !string.IsNullOrWhiteSpace(query.shopGatewayType) ? query.shopGatewayType : null,
                !string.IsNullOrWhiteSpace(query.paymentChannel) ? query.paymentChannel : null,
                !string.IsNullOrWhiteSpace(query.paymentScheme) ? query.paymentScheme : null,
                sort.sort);

            var jsonResult = new KTDatatableResult<ShopGatewayEntry>
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
                data = shopGateways
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.ShopManagement.Shops.AmountOption.View)]
        [HttpPost]
        public async Task<IActionResult> AmountOption([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (string.IsNullOrEmpty(query.userId))
            {
                return BadRequest("无效参数。");
            }


            var amountOptions = await _shopService.GetShopOrderAmountOpitions(
                User.GetUserId<string>(),
                query.userId
                );

            var totalItems = amountOptions.Count;

            var jsonResult = new KTDatatableResult<decimal>
            {
                meta = new KTPagination
                {
                    page = 1,
                    pages = 1,
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "Id"
                },
                data = amountOptions
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.ShopManagement.Shops.PendingReview.View)]
        [HttpGet]
        public IActionResult PendingReview()
        {
            return View();
        }


        [Authorize(Policy = Permissions.ShopManagement.Shops.PendingReview.View)]
        [HttpPost]
        public async Task<IActionResult> PendingReview([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Manager");
            }

            int totalItems;
            List<Shop> shops;

            //If User is a shop agent, then he can only see his downlines.
            if (User.IsInRole(Roles.ShopAgent))
            {
                totalItems = await _shopService.GetPendingReviewDownlinesTotalCount(
                    User.GetUserId<string>(),
                    query.generalSearch
                    );
                shops = _shopService.GetPendingReviewDownlines(
                    pagination.page - 1,
                    pagination.perpage,
                    User.GetUserId<string>(),
                    query.generalSearch ?? string.Empty,
                    sort.field,
                    sort.sort);
            }
            else
            {
                totalItems = await _shopService.GetPendingReviewShopsTotalCount(
                    query.generalSearch
                    );
                shops = _shopService.GetPendingReviewShops(
                    pagination.page - 1,
                    pagination.perpage,
                    query.generalSearch ?? string.Empty,
                    sort.field,
                    sort.sort);
            }


            var jsonResult = new KTDatatableResult<Shop>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "ShopId"
                },
                data = shops
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.ShopManagement.Shops.PendingReview.Review)]
        [HttpPost]
        public async Task<IActionResult> Review([FromBody]List<AccountReview> accounts)
        {
            if (!TryValidateModel(accounts) || !accounts.Any())
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                await _shopService.ReviewShops(accounts);
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


        [Authorize(Policy = Permissions.ShopManagement.Shops.Create)]
        // GET: Shop/Create
        public async Task<IActionResult> Create()
        {
            string uplineId = null;
            //If the context user is a shop agent, set the upline Id to his Id.
            if (User.IsInRole(Roles.ShopAgent))
            {
                uplineId = User.GetUserId<string>();
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


        // POST: Shop/Create
        [Authorize(Policy = Permissions.ShopManagement.Shops.Create)]
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


                if (User.IsInRole(Roles.ShopAgent))
                {
                    var uplineShopAgent = await _shopAgentService.GetShopAgent(vm.UplineId);

                    if (uplineShopAgent.ShopAgentId != User.GetUserId<string>())
                    {
                        throw new InvalidOperationException("Invalid upline Id.");
                    }
                }

                uplineUserId = vm.UplineId;


                var shop = new Shop
                {
                    Username = vm.Username,
                    FullName = vm.FullName,
                    SiteAddress = vm.SiteAddress,
                    PhoneNumber = vm.PhoneNumber,
                    Email = vm.Email,
                    Wechat = vm.Wechat,
                    QQ = vm.QQ,
                    UplineUserId = uplineUserId,
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
                    IsReviewed = false
                };

                await _shopService.CreateShop(
                    shop,
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
                        await _shopService.DeleteShop(shopUsername: vm.Username);
                    }
                }
                catch (Exception exDelete)
                {
                    return BadRequest("1." + ex.Message + " 2." + exDelete.Message);
                }
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Policy = Permissions.ShopManagement.Shops.Edit)]
        [HttpPost]
        public async Task<IActionResult> UpdateAccountStatus([FromBody]List<AccountStatus> accounts)
        {
            if (!TryValidateModel(accounts) || !accounts.Any())
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                await _shopService.UpdateShopStatus(accounts, User.IsInRole(Roles.ShopAgent) ? User.GetUserId<string>() : null);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


            return Ok(new { success = true });
        }


        [Authorize(Policy = Permissions.ShopManagement.Shops.Edit)]
        // GET: Shop/Update/5
        public async Task<IActionResult> Update(string shopId)
        {
            try
            {
                var shop = await _shopService.GetShop(shopId);
                var ipWhitelists = _shopService.GetIpWhitelistByShopId(shopId);

                if (User.IsInRole(Roles.ShopAgent))
                {
                    if (shop.UplineUserId != User.GetUserId<string>())
                    {
                        throw new InvalidOperationException("Unauthorize request.");
                    }
                }


                var minRateRebateAlipayInThousandth = 0;
                var minRateRabateWechatInThousandth = 0;

                if (!string.IsNullOrEmpty(shop.UplineUserId))
                {
                    var uplineCommission = await _shopAgentService.GetRebateCommissionFromShopAgentId(shop.UplineUserId);
                    minRateRebateAlipayInThousandth = uplineCommission.RateRebateAlipayInThousandth + 1;
                    minRateRabateWechatInThousandth = uplineCommission.RateRebateWechatInThousandth + 1;
                }
                if (!ipWhitelists.Any())
                {
                    ipWhitelists.Add(string.Empty);  //For Test.
                }

                var vm = new UpdateViewModel
                {
                    ShopId = shop.ShopId,
                    FullName = shop.FullName,
                    SiteAddress = shop.SiteAddress,
                    PhoneNumber = shop.PhoneNumber,
                    Email = shop.Email,
                    Wechat = shop.Wechat,
                    QQ = shop.QQ,
                    UplineId = shop.UplineUserId,
                    IsEnabled = shop.IsEnabled,
                    DailyAmountLimit = shop.Balance.WithdrawalLimit.DailyAmountLimit,
                    DailyFrequencyLimit = shop.Balance.WithdrawalLimit.DailyFrequencyLimit,
                    EachAmountUpperLimit = shop.Balance.WithdrawalLimit.EachAmountUpperLimit,
                    EachAmountLowerLimit = shop.Balance.WithdrawalLimit.EachAmountLowerLimit,
                    WithdrawalCommissionRateInThousandth = shop.Balance.WithdrawalCommissionRateInThousandth,
                    RateRebateAlipayInThousandth = shop.RebateCommission.RateRebateAlipayInThousandth,
                    RateRebateWechatInThousandth = shop.RebateCommission.RateRebateWechatInThousandth,
                    MinRateRebateAlipayInThousandth = minRateRebateAlipayInThousandth,
                    MinRateRebateWechatInThousandth = minRateRabateWechatInThousandth,

                    IpWhitelists = ipWhitelists
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        // POST: Shop/Update/5
        [Authorize(Policy = Permissions.ShopManagement.Shops.Edit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request: " + ModelState.Values.ToString());
            }

            try
            {
                var shop = new Shop
                {
                    ShopId = vm.ShopId,
                    FullName = vm.FullName,
                    SiteAddress = vm.SiteAddress,
                    PhoneNumber = vm.PhoneNumber,
                    Email = vm.Email,
                    Wechat = vm.Wechat,
                    QQ = vm.QQ,

                    UplineUserId = vm.UplineId,

                    IsEnabled = vm.IsEnabled,

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

                var contextUserId = User.GetUserId<string>();


                //Update shop.
                await _shopService.UpdateShop(
                    shop,
                    vm.Password,
                    User.IsInRole(Roles.ShopAgent) ? contextUserId : null
                    );

                //Clear empty ips.
                if (vm.IpWhitelists != null)
                {
                    vm.IpWhitelists.RemoveAll(i => string.IsNullOrWhiteSpace(i));
                }


                //Update ip whitelist.
                await _apiService.AddIpToWhitelist(
                    vm.ShopId,
                    vm.IpWhitelists,
                    contextUserId
                    );


                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Shop/ChangeBalance
        [Authorize(Policy = Permissions.ShopManagement.Shops.ChangeBalance.Create)]
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


        // POST: Shop/Unfreeze
        [Authorize(Policy = Permissions.ShopManagement.Shops.ChangeBalance.Create)]
        [HttpPost]
        public async Task<IActionResult> Unfreeze([FromBody]int frozenId)
        {
            try
            {
                await _shopService.Unfreeze(frozenId, User.GetUserId<string>());

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Shop/Delete/5
        [Authorize(Policy = Permissions.ShopManagement.Shops.Delete)]
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]List<string> shopIds)
        {
            if (!shopIds.Any())
            {
                return BadRequest("Invalid Request Params.");
            }

            try
            {
                // TODO: Add delete logic here
                await _shopService.DeleteShops(
                    shopIds,
                    User.IsInRole(Roles.ShopAgent) ? User.GetUserId<string>() : null
                    );
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        // POST: Shop/CreatePlatformShopGateway
        [Authorize(Policy = Permissions.ShopManagement.Shops.ShopGateway.Create)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePlatformShopGateway([FromForm] CreatePlatformShopGatewayViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("无效请求。");
                }

                await _shopService.CreatePlatformShopGateway(
                    vm.ShopId,
                    vm.PaymentChannel,
                    vm.PaymentScheme,
                    vm.GatewayNumber,
                    vm.Name,
                    vm.SecondsBeforePayment,
                    vm.IsAmountUnchangeable,
                    vm.IsAccountUnchangeable,
                    vm.IsH5RedirectByScanEnabled,
                    vm.IsH5RedirectByClickEnabled,
                    vm.IsH5RedirectByPickingPhotoEnabled
                    );

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Shop/UpdateShopGatewayAlipayPreference
        [Authorize(Policy = Permissions.ShopManagement.Shops.ShopGateway.Edit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateShopGatewayAlipayPreference([FromForm] UpdateShopGatewayAlipayPreference vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("无效请求。");
                }

                await _shopService.UpdateShopGatewayAlipayPreference(
                    vm.ShopGatewayId,
                    vm.SecondsBeforePayment,
                    vm.IsAmountUnchangeable,
                    vm.IsAccountUnchangeable,
                    vm.IsH5RedirectByScanEnabled,
                    vm.IsH5RedirectByClickEnabled,
                    vm.IsH5RedirectByPickingPhotoEnabled
                    );

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Shop/DeleteShopGateway/5
        [Authorize(Policy = Permissions.ShopManagement.Shops.ShopGateway.Delete)]
        [HttpPost]
        public IActionResult DeleteShopGateway([FromBody]int shopGatewayId)
        {
            try
            {
                // TODO: Add delete logic here
                _shopService.DeleteShopGateway(shopGatewayId);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        // POST: Shop/AddAmountOption
        [Authorize(Policy = Permissions.ShopManagement.Shops.AmountOption.Create)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAmountOption(string shopId, decimal amount)
        {
            try
            {
                if (string.IsNullOrEmpty(shopId))
                {
                    return BadRequest("Invalid Request.");
                }

                await _shopService.AddAmountOption(shopId, amount);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: Shop/DeleteAmountOption
        [Authorize(Policy = Permissions.ShopManagement.Shops.AmountOption.Delete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAmountOption(string shopId, decimal amount)
        {
            try
            {
                if (string.IsNullOrEmpty(shopId))
                {
                    return BadRequest("Invalid Request.");
                }

                await _shopService.DeleteAmountOption(shopId, amount);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        // POST: Shop/GenerateApiKey
        [Authorize(Policy = Permissions.ShopManagement.Shops.ApiKey.Create)]
        [HttpPost]
        public async Task<IActionResult> GenerateApiKey([FromBody]string shopId)
        {
            try
            {
                if (string.IsNullOrEmpty(shopId))
                {
                    return BadRequest("Invalid Request.");
                }

                var apiKey = await _apiService.GenerateShopApiKey(
                    shopId,
                    User.GetUserId<string>()
                    );

                return Ok(new { success = true, apiKey = apiKey });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}