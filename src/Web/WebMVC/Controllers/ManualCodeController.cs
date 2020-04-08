using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Util.Tools.QrCode.QrCoder;
using WebMVC.Extensions;
using WebMVC.Infrastructure.Services;
using WebMVC.Models.Permissions;
using WebMVC.Models.Queries;
using WebMVC.Models.Roles;
using WebMVC.ViewModels.Json;
using WebMVC.ViewModels.ManualCodeViewModels;

namespace WebMVC.Controllers
{
    [Authorize(Policy = Permissions.Additional.Reviewed)]
    public class ManualCodeController : Controller
    {
        private readonly IQrCodeService _qrCodeService;
        private readonly Util.Tools.QrCode.IQrCodeService _qrCodeGenerator;
        private readonly IPersonalService _personalService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISystemConfigurationService _systemConfigurationService;

        public ManualCodeController(IQrCodeService qrCodeService, Util.Tools.QrCode.IQrCodeService qrCodeGenerator, IPersonalService personalService, UserManager<ApplicationUser> userManager, ISystemConfigurationService systemConfigurationService)
        {
            _qrCodeService = qrCodeService ?? throw new ArgumentNullException(nameof(qrCodeService));
            _qrCodeGenerator = qrCodeGenerator ?? throw new ArgumentNullException(nameof(qrCodeGenerator));
            _personalService = personalService ?? throw new ArgumentNullException(nameof(personalService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _systemConfigurationService = systemConfigurationService ?? throw new ArgumentNullException(nameof(systemConfigurationService));
        }

        [Authorize(Policy = Permissions.QrCodeManagement.Manual.View)]
        public IActionResult Index()
        {
            string userBaseRole = string.Empty;

            if (User.IsInRole(Roles.Manager))
            {
                userBaseRole = Roles.Manager;
            }
            else if (User.IsInRole(Roles.TraderAgent))
            {
                userBaseRole = Roles.TraderAgent;
            }
            else if (User.IsInRole(Roles.Trader))
            {
                userBaseRole = Roles.Trader;
            }

            var getAlipayUserIdQrCode = _qrCodeGenerator.CreateQrCode("https://www.dedemao.com/alipay/authorize.php?scope=auth_base");

            var vm = new IndexViewModel
            {
                UserId = User.GetUserId<string>(),
                UserBaseRole = userBaseRole,
                GetAlipayUserIdQrCodeData= "data:image/png;base64," + Convert.ToBase64String(getAlipayUserIdQrCode)
            };

            return View(vm);
        }




        [Authorize(Policy = Permissions.QrCodeManagement.Manual.View)]
        [HttpPost]
        public async Task<IActionResult> Search([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "QrCode");
            }

            var contextUserId = User.GetUserId<string>();

            var totalItems = await _qrCodeService.GetQrCodeEntrysTotalCount(
                contextUserId,
                query.generalSearch ?? string.Empty,
                QrCodeTypeOption.Manual.Value,
                query.paymentChannel ?? null,
                query.paymentScheme ?? null,
                query.qrCodeStatus ?? null,
                query.pairingStatus ?? null
                );

            var qrCodeEntries = await _qrCodeService.GetQrCodeEntrys(
                pagination.page - 1,
                pagination.perpage,
                contextUserId,
                query.generalSearch ?? string.Empty,
                sort.field,
                QrCodeTypeOption.Manual.Value,
                query.paymentChannel ?? null,
                query.paymentScheme ?? null,
                query.qrCodeStatus ?? null,
                query.pairingStatus ?? null,
                sort.sort);

            var jsonResult = new KTDatatableResult<QrCodeEntry>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "QrCodeId"
                },
                data = qrCodeEntries
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.QrCodeManagement.Manual.PendingReview.View)]
        [HttpGet]
        public IActionResult PendingReview()
        {
            string userBaseRole = string.Empty;

            if (User.IsInRole(Roles.Manager))
            {
                userBaseRole = Roles.Manager;
            }
            else if (User.IsInRole(Roles.TraderAgent))
            {
                userBaseRole = Roles.TraderAgent;
            }
            else if (User.IsInRole(Roles.Trader))
            {
                userBaseRole = Roles.Trader;
            }

            var vm = new IndexViewModel
            {
                UserId = User.GetUserId<string>(),
                UserBaseRole = userBaseRole
            };

            return View(vm);
        }


        [Authorize(Policy = Permissions.QrCodeManagement.Manual.PendingReview.View)]
        [HttpPost]
        public async Task<IActionResult> PendingReview([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "PendingReview");
            }

            var contextUserId = User.GetUserId<string>();

            var totalItems = await _qrCodeService.GetPendingQrCodeEntrysTotalCount(
                contextUserId,
                query.generalSearch ?? string.Empty,
                QrCodeTypeOption.Manual.Value,
                query.paymentChannel ?? null,
                query.paymentScheme ?? null
                );

            var qrCodeEntries = await _qrCodeService.GetPendingQrCodeEntrys(
                pagination.page - 1,
                pagination.perpage,
                contextUserId,
                query.generalSearch ?? string.Empty,
                sort.field,
                QrCodeTypeOption.Manual.Value,
                query.paymentChannel ?? null,
                query.paymentScheme ?? null,
                sort.sort);

            var jsonResult = new KTDatatableResult<QrCodeEntry>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)totalItems / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = totalItems,
                    sort = "desc",
                    field = "QrCodeId"
                },
                data = qrCodeEntries
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.QrCodeManagement.Manual.View)]
        [Authorize(Policy = Permissions.QrCodeManagement.Manual.CodeData.View)]
        // GET: ManualCode/Detail
        public async Task<IActionResult> Detail(int qrCodeId)
        {
            try
            {
                var qrCode = await _qrCodeService.GetQrCode(
                    User.GetUserId<string>(),
                    qrCodeId
                    );

                return Json(qrCode);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Policy = Permissions.QrCodeManagement.Manual.CodeData.View)]
        // GET: ManualCode/BarcodeInfoForManual
        public async Task<IActionResult> BarcodeInfo(int qrCodeId)
        {
            try
            {
                var barcodeInfo = await _qrCodeService.GetBarcodeInfoForManual(
                    User.GetUserId<string>(),
                    qrCodeId
                    );

                return Json(barcodeInfo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Policy = Permissions.QrCodeManagement.Manual.CodeData.View)]
        // GET: ManualCode/MerchantInfo
        public async Task<IActionResult> MerchantInfo(int qrCodeId)
        {
            try
            {
                var merchantInfo = await _qrCodeService.GetMerchantInfoAsync(
                    User.GetUserId<string>(),
                    qrCodeId
                    );

                return Json(merchantInfo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Policy = Permissions.QrCodeManagement.Manual.CodeData.View)]
        // GET: ManualCode/TransactionInfo
        public async Task<IActionResult> TransactionInfo(int qrCodeId)
        {
            try
            {
                var transactionInfo = await _qrCodeService.GetTransactionInfoAsync(
                    User.GetUserId<string>(),
                    qrCodeId
                    );

                return Json(transactionInfo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Policy = Permissions.QrCodeManagement.Manual.CodeData.View)]
        // GET: ManualCode/BankInfo
        public async Task<IActionResult> BankInfo(int qrCodeId)
        {
            try
            {
                var bankInfo = await _qrCodeService.GetBankInfoAsync(
                    User.GetUserId<string>(),
                    qrCodeId
                    );

                return Json(bankInfo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        [Authorize(Policy = Permissions.QrCodeManagement.Manual.SearchTrader)]
        [HttpGet]
        public async Task<IActionResult> SearchTrader(string generalSearch, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(generalSearch))
            {
                return BadRequest("Invalid query string.");
            }

            int totalItems = 0;
            IQueryable<Object> users = null;

            //If context user is a trader agent, search only his downliness.
            if (User.IsInRole(Roles.TraderAgent))
            {
                //Get trader agent's Id.
                var contextUserId = User.GetUserId<string>();

                totalItems = await this._userManager.Users
                    .Where(u => u.BaseRoleType == BaseRoleType.Trader
                    && u.UplineId == contextUserId
                    && (u.UserName.Contains(generalSearch)
                    || u.FullName.Contains(generalSearch)
                    || u.Nickname.Contains(generalSearch)
                    || u.Email.Contains(generalSearch)
                    ))
                    .CountAsync();
                users = _userManager.Users
                    .Where(u => u.BaseRoleType == BaseRoleType.Trader
                    && u.UplineId == contextUserId
                    && (u.UserName.Contains(generalSearch)
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
            }


            //If context user is a manager, search all traders.
            if (User.IsInRole(Roles.Manager))
            {
                totalItems = await this._userManager.Users
                    .Where(u => u.BaseRoleType == BaseRoleType.Trader
                    && (u.UserName.Contains(generalSearch)
                    || u.FullName.Contains(generalSearch)
                    || u.Nickname.Contains(generalSearch)
                    || u.Email.Contains(generalSearch)
                    ))
                    .CountAsync();
                users = _userManager.Users
                    .Where(u => u.BaseRoleType == BaseRoleType.Trader
                    && (u.UserName.Contains(generalSearch)
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
            }


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


        [Authorize(Policy = Permissions.QrCodeManagement.Manual.SearchShop)]
        [HttpGet]
        public async Task<IActionResult> SearchShop(string generalSearch, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(generalSearch))
            {
                return BadRequest("Invalid query string.");
            }

            int totalItems = 0;
            IQueryable<Object> users = null;



            //Only manager can search shop.
            if (User.IsInRole(Roles.Manager))
            {
                totalItems = await this._userManager.Users
                    .Where(u => u.BaseRoleType == BaseRoleType.Shop
                    && (u.UserName.Contains(generalSearch)
                    || u.FullName.Contains(generalSearch)
                    || u.Nickname.Contains(generalSearch)
                    || u.Email.Contains(generalSearch)
                    ))
                    .CountAsync();
                users = _userManager.Users
                    .Where(u => u.BaseRoleType == BaseRoleType.Shop
                    && (u.UserName.Contains(generalSearch)
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
            }


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





        // POST: ManualCode/CreateBarcode
        [Authorize(Policy = Permissions.QrCodeManagement.Manual.Create)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBarcode([FromForm]CreateManualBarcodeViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("无效参数。");
                }


                //Build Qr Code Settings.
                var qrCodeConf = _systemConfigurationService.GetQrCodeConfAsync();
                var template = qrCodeConf.RiskControlTemplate;

                QrCodeEntrySetting qrCodeEntrySetting = null;
                if (User.IsInRole(Roles.Manager))
                {
                    //If the context user is a manager, get data from VM.
                    qrCodeEntrySetting = new QrCodeEntrySetting
                    {
                        AutoPairingBySuccessRate = vm.AutoPairingBySuccessRate,
                        AutoPairingByQuotaLeft = vm.AutoPairingByQuotaLeft,
                        AutoPairingByBusinessHours = vm.AutoPairingByBusinessHours,
                        AutoPairingByCurrentConsecutiveFailures = vm.AutoPairingByCurrentConsecutiveFailures,
                        AutoPairngByAvailableBalance = vm.AutoPairngByAvailableBalance,
                        SuccessRateThresholdInHundredth = vm.SuccessRateThresholdInHundredth ?? template.SuccessRateThresholdInHundredth,
                        SuccessRateMinOrders = vm.SuccessRateMinOrders ?? template.SuccessRateMinOrders,
                        QuotaLeftThreshold = vm.QuotaLeftThreshold ?? template.QuotaLeftThreshold,
                        CurrentConsecutiveFailuresThreshold = vm.CurrentConsecutiveFailuresThreshold ?? template.CurrentConsecutiveFailuresThreshold,
                        AvailableBalanceThreshold = vm.AvailableBalanceThreshold ?? template.AvailableBalanceThreshold
                    };
                }
                else
                {
                    //If the context user is a trader agent or trader, get data from template.

                    qrCodeEntrySetting = new QrCodeEntrySetting
                    {
                        AutoPairingBySuccessRate = template.AutoPairingBySuccessRate,
                        AutoPairingByQuotaLeft = template.AutoPairingByQuotaLeft,
                        AutoPairingByBusinessHours = template.AutoPairingByBusinessHours,
                        AutoPairingByCurrentConsecutiveFailures = template.AutoPairingByCurrentConsecutiveFailures,
                        AutoPairngByAvailableBalance = template.AutoPairngByAvailableBalance,
                        SuccessRateThresholdInHundredth = template.SuccessRateThresholdInHundredth,
                        SuccessRateMinOrders = template.SuccessRateMinOrders,
                        QuotaLeftThreshold = template.QuotaLeftThreshold,
                        CurrentConsecutiveFailuresThreshold = template.CurrentConsecutiveFailuresThreshold,
                        AvailableBalanceThreshold = template.AvailableBalanceThreshold
                    };
                }

                await _qrCodeService.CreateBarcode(
                    User.GetUserId<string>(),
                    vm.UserId,
                    null,
                    QrCodeTypeOption.Manual.Value,
                    vm.PaymentChannel,
                    qrCodeEntrySetting,
                    vm.DailyAmountLimit ?? template.DailyAmountLimit,
                    vm.OrderAmountUpperLimit ?? template.OrderAmountUpperLimit,
                    vm.OrderAmountLowerLimit ?? template.OrderAmountLowerLimit,
                    vm.FullName,
                    vm.SpecifiedShopId,
                    vm.QrCodeUrl,
                    vm.Amount
                    );

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: ManualCode/CreateManualMerchantCode
        [Authorize(Policy = Permissions.QrCodeManagement.Manual.Create)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMerchantCode([FromForm]CreateManualMerchantCodeViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }

                //Build Qr Code Settings.
                var qrCodeConf = _systemConfigurationService.GetQrCodeConfAsync();
                var template = qrCodeConf.RiskControlTemplate;

                QrCodeEntrySetting qrCodeEntrySetting = null;
                if (User.IsInRole(Roles.Manager))
                {
                    //If the context user is a manager, get data from VM.
                    qrCodeEntrySetting = new QrCodeEntrySetting
                    {
                        AutoPairingBySuccessRate = vm.AutoPairingBySuccessRate,
                        AutoPairingByQuotaLeft = vm.AutoPairingByQuotaLeft,
                        AutoPairingByBusinessHours = vm.AutoPairingByBusinessHours,
                        AutoPairingByCurrentConsecutiveFailures = vm.AutoPairingByCurrentConsecutiveFailures,
                        AutoPairngByAvailableBalance = vm.AutoPairngByAvailableBalance,
                        SuccessRateThresholdInHundredth = vm.SuccessRateThresholdInHundredth ?? template.SuccessRateThresholdInHundredth,
                        SuccessRateMinOrders = vm.SuccessRateMinOrders ?? template.SuccessRateMinOrders,
                        QuotaLeftThreshold = vm.QuotaLeftThreshold ?? template.QuotaLeftThreshold,
                        CurrentConsecutiveFailuresThreshold = vm.CurrentConsecutiveFailuresThreshold ?? template.CurrentConsecutiveFailuresThreshold,
                        AvailableBalanceThreshold = vm.AvailableBalanceThreshold ?? template.AvailableBalanceThreshold
                    };
                }
                else
                {
                    //If the context user is a trader agent or trader, get data from template.
                    qrCodeEntrySetting = new QrCodeEntrySetting
                    {
                        AutoPairingBySuccessRate = template.AutoPairingBySuccessRate,
                        AutoPairingByQuotaLeft = template.AutoPairingByQuotaLeft,
                        AutoPairingByBusinessHours = template.AutoPairingByBusinessHours,
                        AutoPairingByCurrentConsecutiveFailures = template.AutoPairingByCurrentConsecutiveFailures,
                        AutoPairngByAvailableBalance = template.AutoPairngByAvailableBalance,
                        SuccessRateThresholdInHundredth = template.SuccessRateThresholdInHundredth,
                        SuccessRateMinOrders = template.SuccessRateMinOrders,
                        QuotaLeftThreshold = template.QuotaLeftThreshold,
                        CurrentConsecutiveFailuresThreshold = template.CurrentConsecutiveFailuresThreshold,
                        AvailableBalanceThreshold = template.AvailableBalanceThreshold
                    };
                }

                await _qrCodeService.CreateMerchantCode(
                    User.GetUserId<string>(),
                    vm.UserId,
                    null,
                    QrCodeTypeOption.Manual.Value,
                    vm.PaymentChannel,
                    qrCodeEntrySetting,
                    vm.DailyAmountLimit ?? template.DailyAmountLimit,
                    vm.OrderAmountUpperLimit ?? template.OrderAmountUpperLimit,
                    vm.OrderAmountLowerLimit ?? template.OrderAmountLowerLimit,
                    vm.FullName,
                    vm.SpecifiedShopId,
                    vm.AppId,
                    vm.PrivateKey,
                    vm.MerchantId,
                    vm.AlipayPublicKey,
                    vm.WechatApiCertificate
                    );

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: ManualCode/CreateManualTransactionCode
        [Authorize(Policy = Permissions.QrCodeManagement.Manual.Create)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTransactionCode([FromForm]CreateManualTransactionCodeViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }


                //Build Qr Code Settings.
                var qrCodeConf = _systemConfigurationService.GetQrCodeConfAsync();
                var template = qrCodeConf.RiskControlTemplate;

                QrCodeEntrySetting qrCodeEntrySetting = null;
                if (User.IsInRole(Roles.Manager))
                {
                    //If the context user is a manager, get data from VM.
                    qrCodeEntrySetting = new QrCodeEntrySetting
                    {
                        AutoPairingBySuccessRate = vm.AutoPairingBySuccessRate,
                        AutoPairingByQuotaLeft = vm.AutoPairingByQuotaLeft,
                        AutoPairingByBusinessHours = vm.AutoPairingByBusinessHours,
                        AutoPairingByCurrentConsecutiveFailures = vm.AutoPairingByCurrentConsecutiveFailures,
                        AutoPairngByAvailableBalance = vm.AutoPairngByAvailableBalance,
                        SuccessRateThresholdInHundredth = vm.SuccessRateThresholdInHundredth ?? template.SuccessRateThresholdInHundredth,
                        SuccessRateMinOrders = vm.SuccessRateMinOrders ?? template.SuccessRateMinOrders,
                        QuotaLeftThreshold = vm.QuotaLeftThreshold ?? template.QuotaLeftThreshold,
                        CurrentConsecutiveFailuresThreshold = vm.CurrentConsecutiveFailuresThreshold ?? template.CurrentConsecutiveFailuresThreshold,
                        AvailableBalanceThreshold = vm.AvailableBalanceThreshold ?? template.AvailableBalanceThreshold
                    };
                }
                else
                {
                    //If the context user is a trader agent or trader, get data from template.
                    qrCodeEntrySetting = new QrCodeEntrySetting
                    {
                        AutoPairingBySuccessRate = template.AutoPairingBySuccessRate,
                        AutoPairingByQuotaLeft = template.AutoPairingByQuotaLeft,
                        AutoPairingByBusinessHours = template.AutoPairingByBusinessHours,
                        AutoPairingByCurrentConsecutiveFailures = template.AutoPairingByCurrentConsecutiveFailures,
                        AutoPairngByAvailableBalance = template.AutoPairngByAvailableBalance,
                        SuccessRateThresholdInHundredth = template.SuccessRateThresholdInHundredth,
                        SuccessRateMinOrders = template.SuccessRateMinOrders,
                        QuotaLeftThreshold = template.QuotaLeftThreshold,
                        CurrentConsecutiveFailuresThreshold = template.CurrentConsecutiveFailuresThreshold,
                        AvailableBalanceThreshold = template.AvailableBalanceThreshold
                    };
                }

                await _qrCodeService.CreateTransactionCode(
                    User.GetUserId<string>(),
                    vm.UserId,
                    null,
                    QrCodeTypeOption.Manual.Value,
                    vm.PaymentScheme,
                    qrCodeEntrySetting,
                    vm.DailyAmountLimit ?? template.DailyAmountLimit,
                    vm.OrderAmountUpperLimit ?? template.OrderAmountUpperLimit,
                    vm.OrderAmountLowerLimit ?? template.OrderAmountLowerLimit,
                    vm.FullName,
                    vm.SpecifiedShopId,
                    vm.AlipayUserId
                    );

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: ManualCode/CreateManualBankCode
        [Authorize(Policy = Permissions.QrCodeManagement.Manual.Create)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBankCode([FromForm]CreateManualBankCodeViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }

                //Build Qr Code Settings.
                var qrCodeConf = _systemConfigurationService.GetQrCodeConfAsync();
                var template = qrCodeConf.RiskControlTemplate;

                QrCodeEntrySetting qrCodeEntrySetting = null;
                if (User.IsInRole(Roles.Manager))
                {
                    //If the context user is a manager, get data from VM.
                    qrCodeEntrySetting = new QrCodeEntrySetting
                    {
                        AutoPairingBySuccessRate = vm.AutoPairingBySuccessRate,
                        AutoPairingByQuotaLeft = vm.AutoPairingByQuotaLeft,
                        AutoPairingByBusinessHours = vm.AutoPairingByBusinessHours,
                        AutoPairingByCurrentConsecutiveFailures = vm.AutoPairingByCurrentConsecutiveFailures,
                        AutoPairngByAvailableBalance = vm.AutoPairngByAvailableBalance,
                        SuccessRateThresholdInHundredth = vm.SuccessRateThresholdInHundredth ?? template.SuccessRateThresholdInHundredth,
                        SuccessRateMinOrders = vm.SuccessRateMinOrders ?? template.SuccessRateMinOrders,
                        QuotaLeftThreshold = vm.QuotaLeftThreshold ?? template.QuotaLeftThreshold,
                        CurrentConsecutiveFailuresThreshold = vm.CurrentConsecutiveFailuresThreshold ?? template.CurrentConsecutiveFailuresThreshold,
                        AvailableBalanceThreshold = vm.AvailableBalanceThreshold ?? template.AvailableBalanceThreshold
                    };
                }
                else
                {
                    //If the context user is a trader agent or trader, get data from template.
                    qrCodeEntrySetting = new QrCodeEntrySetting
                    {
                        AutoPairingBySuccessRate = template.AutoPairingBySuccessRate,
                        AutoPairingByQuotaLeft = template.AutoPairingByQuotaLeft,
                        AutoPairingByBusinessHours = template.AutoPairingByBusinessHours,
                        AutoPairingByCurrentConsecutiveFailures = template.AutoPairingByCurrentConsecutiveFailures,
                        AutoPairngByAvailableBalance = template.AutoPairngByAvailableBalance,
                        SuccessRateThresholdInHundredth = template.SuccessRateThresholdInHundredth,
                        SuccessRateMinOrders = template.SuccessRateMinOrders,
                        QuotaLeftThreshold = template.QuotaLeftThreshold,
                        CurrentConsecutiveFailuresThreshold = template.CurrentConsecutiveFailuresThreshold,
                        AvailableBalanceThreshold = template.AvailableBalanceThreshold
                    };
                }

                await _qrCodeService.CreateBankCode(
                    User.GetUserId<string>(),
                    vm.UserId,
                    null,
                    QrCodeTypeOption.Manual.Value,
                    qrCodeEntrySetting,
                    vm.DailyAmountLimit ?? template.DailyAmountLimit,
                    vm.OrderAmountUpperLimit ?? template.OrderAmountUpperLimit,
                    vm.OrderAmountLowerLimit ?? template.OrderAmountLowerLimit,
                    vm.FullName,
                    vm.SpecifiedShopId,
                    vm.BankName,
                    vm.BankMark,
                    vm.AccountName,
                    vm.CardNumber
                    );

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: ManualCode/UpdateManualBarcode
        [Authorize(Policy = Permissions.QrCodeManagement.Manual.CodeData.Edit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBarcode([FromForm]UpdateManualBarcodeViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }


                await _qrCodeService.UpdateBarcodeDataForManual(
                    User.GetUserId<string>(),
                    vm.QrCodeId,
                    vm.QrCodeUrl,
                    vm.Amount
                    );

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: ManualCode/UpdateManualMerchantCode
        [Authorize(Policy = Permissions.QrCodeManagement.Manual.CodeData.Edit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMerchantCode([FromForm]UpdateManualMerchantCodeViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }


                await _qrCodeService.UpdateMerchantData(
                    User.GetUserId<string>(),
                    vm.QrCodeId,
                    vm.AppId,
                    vm.PrivateKey,
                    vm.MerchantId,
                    vm.AlipayPublicKey,
                    vm.WechatApiCertificate
                    );

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: ManualCode/UpdateTransactionCode
        [Authorize(Policy = Permissions.QrCodeManagement.Manual.CodeData.Edit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTransactionCode([FromForm]UpdateManualTransactionCodeViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }


                await _qrCodeService.UpdateTransactionData(
                    User.GetUserId<string>(),
                    vm.QrCodeId,
                    vm.AlipayUserId
                    );

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: ManualCode/UpdateBankCode
        [Authorize(Policy = Permissions.QrCodeManagement.Manual.CodeData.Edit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBankCode([FromForm]UpdateManualBankCodeViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }

                await _qrCodeService.UpdateBankData(
                    User.GetUserId<string>(),
                    vm.QrCodeId,
                    vm.BankName,
                    vm.BankMark,
                    vm.AccountName,
                    vm.CardNumber
                    );

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: ManualCode/UpdateQrCodeSettings
        [Authorize(Policy = Permissions.QrCodeManagement.Manual.Edit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQrCodeSettings([FromForm]UpdaetQrCodeSettingsViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }

                //Build Qr Code Settings.
                var qrCodeEntrySetting = new QrCodeEntrySetting
                {
                    AutoPairingBySuccessRate = vm.AutoPairingBySuccessRate,
                    AutoPairingByQuotaLeft = vm.AutoPairingByQuotaLeft,
                    AutoPairingByBusinessHours = vm.AutoPairingByBusinessHours,
                    AutoPairingByCurrentConsecutiveFailures = vm.AutoPairingByCurrentConsecutiveFailures,
                    AutoPairngByAvailableBalance = vm.AutoPairngByAvailableBalance,
                    SuccessRateThresholdInHundredth = vm.SuccessRateThresholdInHundredth,
                    SuccessRateMinOrders = vm.SuccessRateMinOrders,
                    QuotaLeftThreshold = vm.QuotaLeftThreshold,
                    CurrentConsecutiveFailuresThreshold = vm.CurrentConsecutiveFailuresThreshold,
                    AvailableBalanceThreshold = vm.AvailableBalanceThreshold
                };

                await _qrCodeService.UpdateQrCodeSettings(
                    User.GetUserId<string>(),
                    vm.QrCodeId,
                    qrCodeEntrySetting
                    );

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: ManualCode/UpdateQrCodeQuota
        [Authorize(Policy = Permissions.QrCodeManagement.Manual.Edit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQrCodeQuota([FromForm]UpdateQrCodeQuotaViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }

                await _qrCodeService.UpdateQuota(
                    User.GetUserId<string>(),
                    vm.QrCodeId,
                    vm.DailyAmountLimit,
                    vm.OrderAmountUpperLimit,
                    vm.OrderAmountLowerLimit
                    );

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: ManualCode/UpdateQrCodeBaseInfo
        [Authorize(Policy = Permissions.QrCodeManagement.Manual.Edit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQrCodeBaseInfo([FromForm]UpdateQrCodeBaseInfoViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }

                await _qrCodeService.UpdateBaseInfo(
                    User.GetUserId<string>(),
                    vm.QrCodeId,
                    vm.FullName,
                    vm.SpecifiedShopId
                    );

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: ManualCode/GenerateQrCode
        [Authorize(Policy = Permissions.QrCodeManagement.Manual.CodeData.View)]
        [HttpPost]
        public async Task<IActionResult> GenerateQrCode([FromBody]int qrCodeId, [FromBody]decimal? amount = null)
        {
            try
            {
                var qrCode = await _qrCodeService.GenerateQrCode(
                    User.GetUserId<string>(),
                    qrCodeId,
                    amount ?? null
                    );


                return Ok(new { QrCodeData = "data:image/png;base64," + Convert.ToBase64String(qrCode) });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }





        // POST: ManualCode/Approve
        [Authorize(Policy = Permissions.QrCodeManagement.Manual.PendingReview.Approve)]
        [HttpPost]
        public async Task<IActionResult> Approve([FromBody]List<int> ids)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("无效请求。");
                }

                var contextUserId = User.GetUserId<string>();

                await _qrCodeService.ApproveQrCodes(ids, contextUserId);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: ManualCode/Enable
        [Authorize(Policy = Permissions.QrCodeManagement.Manual.Enable)]
        [HttpPost]
        public async Task<IActionResult> Enable([FromBody]List<int> ids)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("无效请求。");
                }

                var contextUserId = User.GetUserId<string>();

                await _qrCodeService.EnableQrCodes(ids, contextUserId);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: ManualCode/Disable
        [Authorize(Policy = Permissions.QrCodeManagement.Manual.Disable)]
        [HttpPost]
        public async Task<IActionResult> Disable([FromBody]List<int> ids)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("无效请求。");
                }

                var contextUserId = User.GetUserId<string>();

                await _qrCodeService.DisableQrCodes(ids, contextUserId, null);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        // POST: ManualCode/StartPairing
        [Authorize(Policy = Permissions.QrCodeManagement.Manual.StartPairing)]
        [HttpPost]
        public async Task<IActionResult> StartPairing([FromBody]List<int> ids)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("无效请求。");
                }

                var contextUserId = User.GetUserId<string>();

                await _qrCodeService.StartPairingQrCodes(ids, contextUserId);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: ManualCode/StopPairing
        [Authorize(Policy = Permissions.QrCodeManagement.Manual.StopPairing)]
        [HttpPost]
        public async Task<IActionResult> StopPairing([FromBody]List<int> ids)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("无效请求。");
                }

                var contextUserId = User.GetUserId<string>();

                await _qrCodeService.StopPairingQrCodes(ids, contextUserId);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        // POST: ManualCode/ResetRiskControlData
        [Authorize(Policy = Permissions.QrCodeManagement.Manual.ResetRiskControlData)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetRiskControlData([FromForm]ResetRiskControlDataViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }

                await _qrCodeService.ResetRiskControlData(
                    User.GetUserId<string>(),
                    vm.QrCodeId,
                    vm.ResetQuotaLeftToday,
                    vm.ResetCurrentConsecutiveFailures,
                    vm.ResetSuccessRateAndRelatedData
                    );

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        // POST: ManualCode/Approve
        [Authorize(Policy = Permissions.QrCodeManagement.Manual.PendingReview.Reject)]
        [HttpPost]
        public async Task<IActionResult> Reject([FromBody]List<int> ids)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("无效请求。");
                }

                var contextUserId = User.GetUserId<string>();

                await _qrCodeService.RejectQrCodes(ids, contextUserId);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



    }
}