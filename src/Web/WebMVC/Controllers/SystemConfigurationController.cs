using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Infrastructure.Services;
using WebMVC.Models.Permissions;
using WebMVC.ViewModels.SystemConfigurationViewModels;

namespace WebMVC.Controllers
{
    [Authorize]
    public class SystemConfigurationController : Controller
    {
        private readonly ISystemConfigurationService _systemConfigurationService;

        public SystemConfigurationController(ISystemConfigurationService systemConfigurationService)
        {
            _systemConfigurationService = systemConfigurationService ?? throw new ArgumentNullException(nameof(systemConfigurationService));
        }


        // GET: SystemConfiguration
        public ActionResult Index()
        {
            return View();
        }


        [Authorize(Policy = Permissions.SystemConfiguration.SiteBaseInfo.View)]
        [HttpGet]
        public IActionResult SiteBaseInfo()
        {
            var settings = _systemConfigurationService.GetSiteInfoAsync();

            TimeSpan businessHoursFrom = TimeSpan.Parse(settings.BusinessHoursFrom);
            TimeSpan businessHoursTo = TimeSpan.Parse(settings.BusinessHoursTo);

            var vm = new SiteBaseInfoViewModel()
            {
                SiteTitle = settings.SiteTitle,
                ContactAddress = settings.ContactAddress,
                ContactEmail = settings.ContactEmail,
                ConsumerHotline = settings.ConsumerHotline,
                ConsumerServiceQQ = settings.ConsumerServiceQQ,
                ConsumerServiceSkype = settings.ConsumerServiceSkype,
                BusinessHoursFroms = _systemConfigurationService.GetDateTimeSelectList(isHour: true, selectedItemValue: businessHoursFrom.Hours),
                BusinessMinutesFroms = _systemConfigurationService.GetDateTimeSelectList(isMinute: true, selectedItemValue: businessHoursFrom.Minutes),
                BusinessHoursTos = _systemConfigurationService.GetDateTimeSelectList(isHour: true, selectedItemValue: businessHoursTo.Hours),
                BusinessMinutesTos = _systemConfigurationService.GetDateTimeSelectList(isMinute: true, selectedItemValue: businessHoursTo.Minutes),
                BusinessHoursFrom = businessHoursFrom.ToString(@"hh"),
                BusinessMinutesFrom = businessHoursFrom.ToString(@"mm"),
                BusinessHoursTo = businessHoursTo.ToString(@"hh"),
                BusinessMinutesTo = businessHoursTo.ToString(@"mm"),
            };

            //ViewBag.BasketInoperativeMsg = errorMsg;

            return View(vm);
        }


        [Authorize(Policy = Permissions.SystemConfiguration.SiteBaseInfo.Edit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SiteBaseInfo([FromForm]SiteBaseInfoViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("无效参数。");
            }
            try
            {

                var baseInfoSettings = new SiteBaseInfo
                {
                    SiteTitle = vm.SiteTitle,
                    ContactAddress = vm.ContactAddress,
                    ContactEmail = vm.ContactEmail,
                    ConsumerHotline = vm.ConsumerHotline,
                    ConsumerServiceQQ = vm.ConsumerServiceQQ,
                    ConsumerServiceSkype = vm.ConsumerServiceSkype,
                    BusinessHoursFrom = vm.BusinessHoursFrom + ":" + vm.BusinessMinutesFrom,
                    BusinessHoursTo = vm.BusinessHoursTo + ":" + vm.BusinessMinutesTo
                };

                _systemConfigurationService.UpdateSiteBaseInfo(baseInfoSettings);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }


        [Authorize(Policy = Permissions.SystemConfiguration.Payment.View)]
        [HttpGet]
        public IActionResult Payment()
        {
            var settings = _systemConfigurationService.GetPaymentAsync();


            var vm = new PaymentViewModel()
            {
                PaymentTimeoutInSecond = settings.PaymentTimeoutInSecond,
                SellRequestCountdownInSecond = settings.SellRequestCountdownInSecond,
                SecondsBeforePayment = settings.AlipayPaymentTemplate.SecondsBeforePayment,
                IsAmountUnchangeable = settings.AlipayPaymentTemplate.IsAmountUnchangeable,
                IsAccountUnchangeable = settings.AlipayPaymentTemplate.IsAccountUnchangeable,
                IsH5RedirectByScanEnabled = settings.AlipayPaymentTemplate.IsH5RedirectByScanEnabled,
                IsH5RedirectByClickEnabled = settings.AlipayPaymentTemplate.IsH5RedirectByClickEnabled,
                IsH5RedirectByPickingPhotoEnabled = settings.AlipayPaymentTemplate.IsH5RedirectByPickingPhotoEnabled
            };

            //ViewBag.BasketInoperativeMsg = errorMsg;

            return View(vm);
        }


        [Authorize(Policy = Permissions.SystemConfiguration.Payment.Edit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Payment([FromForm]PaymentViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("无效参数");
            }
            try
            {
                var settings = new Payment
                {
                    PaymentTimeoutInSecond = vm.PaymentTimeoutInSecond,
                    SellRequestCountdownInSecond = vm.SellRequestCountdownInSecond,
                    AlipayPaymentTemplate = new AlipayPaymentTemplate
                    {
                        SecondsBeforePayment = vm.SecondsBeforePayment,
                        IsAmountUnchangeable = vm.IsAmountUnchangeable,
                        IsAccountUnchangeable = vm.IsAccountUnchangeable,
                        IsH5RedirectByScanEnabled = vm.IsH5RedirectByScanEnabled,
                        IsH5RedirectByClickEnabled = vm.IsH5RedirectByClickEnabled,
                        IsH5RedirectByPickingPhotoEnabled = vm.IsH5RedirectByPickingPhotoEnabled,
                    }
                };

                _systemConfigurationService.UpdatePayment(settings);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Policy = Permissions.SystemConfiguration.SystemNotificationSound.View)]
        [HttpGet]
        public IActionResult SystemNotificationSound()
        {
            var settings = _systemConfigurationService.GetSystemNotificationSoundAsync();


            var vm = new SystemNotificationSoundViewModel()
            {
                Withdraw = settings.Withdraw,
                Deposit = settings.Deposit,
                Member = settings.Member,
                QrCode = settings.QrCode,
                NewOrder = settings.NewOrder,
                OrderTimeout = settings.OrderTimeout
            };

            //ViewBag.BasketInoperativeMsg = errorMsg;

            return View(vm);
        }


        [Authorize(Policy = Permissions.SystemConfiguration.SystemNotificationSound.Edit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SystemNotificationSound([FromForm]SystemNotificationSoundViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request Params.");
            }
            try
            {

                var settings = new SystemNotificationSound
                {
                    Withdraw = vm.Withdraw,
                    Deposit = vm.Deposit,
                    Member = vm.Member,
                    QrCode = vm.QrCode,
                    NewOrder = vm.NewOrder,
                    OrderTimeout = vm.OrderTimeout
                };

                _systemConfigurationService.UpdateSystemNotificationSound(settings);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Policy = Permissions.SystemConfiguration.UserNotification.View)]
        [HttpGet]
        public IActionResult UserNotification()
        {
            var settings = _systemConfigurationService.GetUserNotificationAsync();


            var vm = new UserNotificationViewModel()
            {
                OrderTimeout = settings.OrderTimeout,
                SuccessDeposit = settings.SuccessDeposit
            };

            //ViewBag.BasketInoperativeMsg = errorMsg;

            return View(vm);
        }


        [Authorize(Policy = Permissions.SystemConfiguration.UserNotification.Edit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UserNotification([FromForm]UserNotificationViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request Params.");
            }
            try
            {

                var settings = new UserNotification
                {
                    OrderTimeout = vm.OrderTimeout,
                    SuccessDeposit = vm.SuccessDeposit
                };

                _systemConfigurationService.UpdateUserNotification(settings);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Policy = Permissions.SystemConfiguration.WithdrawalAndDeposit.View)]
        [HttpGet]
        public IActionResult WithdrawalAndDeposit()
        {
            var settings = _systemConfigurationService.GetWithdrawalAndDepositAsync();

            TimeSpan withdrawalOpenFrom = TimeSpan.Parse(settings.WithdrawalOpenFrom);
            TimeSpan withdrawalOpenTo = TimeSpan.Parse(settings.WithdrawalOpenTo);
            TimeSpan depositOpenFrom = TimeSpan.Parse(settings.DepositOpenFrom);
            TimeSpan depositOpenTo = TimeSpan.Parse(settings.DepositOpenTo);

            var vm = new WithdrawalAndDepositViewModel()
            {
                EachAmountUpperLimit = settings.WithdrawalTemplate.EachAmountUpperLimit,
                EachAmountLowerLimit = settings.WithdrawalTemplate.EachAmountLowerLimit,
                DailyAmountLimit = settings.WithdrawalTemplate.DailyAmountLimit,
                DailyFrequencyLimit = settings.WithdrawalTemplate.DailyFrequencyLimit,
                CommissionInThousandth = settings.WithdrawalTemplate.CommissionInThousandth,

                WithdrawalOpenFromInHours = _systemConfigurationService.GetDateTimeSelectList(isHour: true, selectedItemValue: withdrawalOpenFrom.Hours),
                WithdrawalOpenFromInMinutes = _systemConfigurationService.GetDateTimeSelectList(isMinute: true, selectedItemValue: withdrawalOpenFrom.Minutes),
                WithdrawalOpenToInHours = _systemConfigurationService.GetDateTimeSelectList(isHour: true, selectedItemValue: withdrawalOpenTo.Hours),
                WithdrawalOpenToInMinutes = _systemConfigurationService.GetDateTimeSelectList(isMinute: true, selectedItemValue: withdrawalOpenTo.Minutes),
                DepositOpenFromInHours = _systemConfigurationService.GetDateTimeSelectList(isHour: true, selectedItemValue: depositOpenFrom.Hours),
                DepositOpenFromInMinutes = _systemConfigurationService.GetDateTimeSelectList(isMinute: true, selectedItemValue: depositOpenFrom.Minutes),
                DepositOpenToInHours = _systemConfigurationService.GetDateTimeSelectList(isHour: true, selectedItemValue: depositOpenTo.Hours),
                DepositOpenToInMinutes = _systemConfigurationService.GetDateTimeSelectList(isMinute: true, selectedItemValue: depositOpenTo.Minutes),

                WithdrawalOpenFromInHour = withdrawalOpenFrom.ToString(@"hh"),
                WithdrawalOpenFromInMinute = withdrawalOpenFrom.ToString(@"mm"),
                WithdrawalOpenToInHour = withdrawalOpenTo.ToString(@"hh"),
                WithdrawalOpenToInMinute = withdrawalOpenTo.ToString(@"mm"),
                DepositOpenFromInHour = depositOpenFrom.ToString(@"hh"),
                DepositOpenFromInMinute = depositOpenFrom.ToString(@"mm"),
                DepositOpenToInHour = depositOpenTo.ToString(@"hh"),
                DepositOpenToInMinute = depositOpenTo.ToString(@"mm")
            };

            //ViewBag.BasketInoperativeMsg = errorMsg;

            return View(vm);
        }


        [Authorize(Policy = Permissions.SystemConfiguration.WithdrawalAndDeposit.Edit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult WithdrawalAndDeposit([FromForm]WithdrawalAndDepositViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("无效参数。");
            }
            try
            {
                var withdrawalTemplate = new WithdrawalTemplate
                {
                    EachAmountUpperLimit = vm.EachAmountUpperLimit,
                    EachAmountLowerLimit = vm.EachAmountLowerLimit,
                    DailyAmountLimit = vm.DailyAmountLimit,
                    DailyFrequencyLimit = vm.DailyFrequencyLimit,
                    CommissionInThousandth = vm.CommissionInThousandth
                };

                var settings = new WithdrawalAndDeposit
                {
                    WithdrawalTemplate = withdrawalTemplate,
                    WithdrawalOpenFrom = vm.WithdrawalOpenFromInHour + ":" + vm.WithdrawalOpenFromInMinute,
                    WithdrawalOpenTo = vm.WithdrawalOpenToInHour + ":" + vm.WithdrawalOpenToInMinute,
                    DepositOpenFrom = vm.DepositOpenFromInHour + ":" + vm.DepositOpenFromInMinute,
                    DepositOpenTo = vm.DepositOpenToInHour + ":" + vm.DepositOpenToInMinute,
                };

                _systemConfigurationService.UpdateWithdrawalAndDeposit(settings);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Policy = Permissions.SystemConfiguration.PaymentChannel.View)]
        [HttpGet]
        public IActionResult PaymentChannel()
        {
            var settings = _systemConfigurationService.GetPaymentChannelAsync();

            TimeSpan openFrom = TimeSpan.Parse(settings.OpenFrom);
            TimeSpan openTo = TimeSpan.Parse(settings.OpenTo);

            var vm = new PaymentChannelViewModel()
            {
                OpenFromInHours = _systemConfigurationService.GetDateTimeSelectList(isHour: true, selectedItemValue: openFrom.Hours),
                OpenFromInMinutes = _systemConfigurationService.GetDateTimeSelectList(isMinute: true, selectedItemValue: openFrom.Minutes),
                OpenToInHours = _systemConfigurationService.GetDateTimeSelectList(isHour: true, selectedItemValue: openTo.Hours),
                OpenToInMinutes = _systemConfigurationService.GetDateTimeSelectList(isMinute: true, selectedItemValue: openTo.Minutes),

                OpenFromInHour = openFrom.ToString(@"hh"),
                OpenFromInMinute = openFrom.ToString(@"mm"),
                OpenToInHour = openTo.ToString(@"hh"),
                OpenToInMinute = openTo.ToString(@"mm"),

                AutoToggle = settings.AutoToggle
            };

            //ViewBag.BasketInoperativeMsg = errorMsg;

            return View(vm);
        }


        [Authorize(Policy = Permissions.SystemConfiguration.PaymentChannel.Edit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PaymentChannel([FromForm]PaymentChannelViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request Params.");
            }
            try
            {
                var settings = new PaymentChannel
                {
                    OpenFrom = vm.OpenFromInHour + ":" + vm.OpenFromInMinute,
                    OpenTo = vm.OpenToInHour + ":" + vm.OpenToInMinute,
                    AutoToggle = vm.AutoToggle,
                };

                _systemConfigurationService.UpdatePaymentChannel(settings);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [Authorize(Policy = Permissions.SystemConfiguration.QrCodeConf.View)]
        [HttpGet]
        public IActionResult QrCodeConf()
        {
            var settings = _systemConfigurationService.GetQrCodeConfAsync();


            var vm = new QrCodeConfViewModel()
            {
                AutoPairingBySuccessRate = settings.RiskControlTemplate.AutoPairingBySuccessRate,
                AutoPairingByQuotaLeft = settings.RiskControlTemplate.AutoPairingByQuotaLeft,
                AutoPairingByBusinessHours = settings.RiskControlTemplate.AutoPairingByBusinessHours,
                AutoPairingByCurrentConsecutiveFailures = settings.RiskControlTemplate.AutoPairingByCurrentConsecutiveFailures,
                AutoPairngByAvailableBalance = settings.RiskControlTemplate.AutoPairngByAvailableBalance,
                SuccessRateThresholdInHundredth = settings.RiskControlTemplate.SuccessRateThresholdInHundredth,
                SuccessRateMinOrders = settings.RiskControlTemplate.SuccessRateMinOrders,
                QuotaLeftThreshold = settings.RiskControlTemplate.QuotaLeftThreshold,
                CurrentConsecutiveFailuresThreshold = settings.RiskControlTemplate.CurrentConsecutiveFailuresThreshold,
                AvailableBalanceThreshold = settings.RiskControlTemplate.AvailableBalanceThreshold
            };

            //ViewBag.BasketInoperativeMsg = errorMsg;

            return View(vm);
        }


        [Authorize(Policy = Permissions.SystemConfiguration.QrCodeConf.Edit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult QrCodeConf([FromForm]QrCodeConfViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("无效参数");
            }
            try
            {
                var settings = new QrCodeConf
                {
                    RiskControlTemplate = new RiskControlTemplate
                    {
                        AutoPairingBySuccessRate = vm.AutoPairingBySuccessRate,
                        AutoPairingByQuotaLeft = vm.AutoPairingByQuotaLeft,
                        AutoPairingByBusinessHours = vm.AutoPairingByBusinessHours,
                        AutoPairingByCurrentConsecutiveFailures = vm.AutoPairingByCurrentConsecutiveFailures,
                        AutoPairngByAvailableBalance = vm.AutoPairngByAvailableBalance,
                        SuccessRateThresholdInHundredth = vm.SuccessRateThresholdInHundredth,
                        SuccessRateMinOrders = vm.SuccessRateMinOrders,
                        QuotaLeftThreshold = (int)vm.QuotaLeftThreshold,
                        CurrentConsecutiveFailuresThreshold = vm.CurrentConsecutiveFailuresThreshold,
                        AvailableBalanceThreshold = (int)vm.AvailableBalanceThreshold,
                        DailyAmountLimit = vm.DailyAmountLimit,
                        OrderAmountUpperLimit = vm.OrderAmountUpperLimit,
                        OrderAmountLowerLimit = vm.OrderAmountLowerLimit
                    }
                };

                _systemConfigurationService.UpdateQrCodeConf(settings);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}