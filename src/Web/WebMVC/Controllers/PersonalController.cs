using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Extensions;
using WebMVC.Models.Permissions;
using WebMVC.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebMVC.Models.Roles;
using WebMVC.ViewModels.PersonalViewModels;
using WebMVC.Applications.Queries.ShopAgents;
using System.Text;
using System.Text.Encodings.Web;
using Util.Tools.QrCode.QrCoder;
using WebMVC.Models.Queries;

namespace WebMVC.Controllers
{
    public class PersonalController : Controller
    {
        private readonly IPersonalService _personalService;
        private readonly ITraderService _traderService;
        private readonly ITraderAgentService _traderAgentService;
        private readonly IShopService _shopService;
        private readonly IShopAgentService _shopAgentService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UrlEncoder _urlEncoder;

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public PersonalController(IPersonalService personalService,
            ITraderService traderService,
            ITraderAgentService traderAgentService,
            IShopService shopService,
            IShopAgentService shopAgentService,
            UserManager<ApplicationUser> userManager,
            UrlEncoder urlEncoder)
        {
            _personalService = personalService ?? throw new ArgumentNullException(nameof(personalService));
            _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
            _traderAgentService = traderAgentService ?? throw new ArgumentNullException(nameof(traderAgentService));
            _shopService = shopService ?? throw new ArgumentNullException(nameof(shopService));
            _shopAgentService = shopAgentService ?? throw new ArgumentNullException(nameof(shopAgentService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _urlEncoder = urlEncoder ?? throw new ArgumentNullException(nameof(urlEncoder));
        }

        [Authorize(Policy = Permissions.Personal.Edit)]
        [HttpGet]
        public async Task<IActionResult> Update()
        {
            try
            {
                var contextUserId = User.GetUserId<string>();

                var userBaseRoleType = await _userManager.Users
                    .Where(u => u.Id == contextUserId)
                    .Select(u => u.BaseRoleType)
                    .FirstOrDefaultAsync();

                UpdateViewModel vm = null;

                if (userBaseRoleType == BaseRoleType.Trader)
                {
                    var trader = await _personalService.GetTrader(
                        contextUserId
                        );

                    vm = new UpdateViewModel
                    {
                        UserId = trader.TraderId,
                        FullName = trader.FullName,
                        UserName = trader.Username,
                        Nickname = trader.Nickname,
                        PhoneNumber = trader.PhoneNumber,
                        Email = trader.Email,
                        Wechat = trader.Wechat,
                        QQ = trader.QQ,
                        UplineId = trader.UplineUserId,
                        IsEnabled = trader.IsEnabled,
                        HasGrantRight = false,
                        DailyAmountLimit = trader.Balance.WithdrawalLimit.DailyAmountLimit,
                        DailyFrequencyLimit = trader.Balance.WithdrawalLimit.DailyFrequencyLimit,
                        EachAmountUpperLimit = trader.Balance.WithdrawalLimit.EachAmountUpperLimit,
                        EachAmountLowerLimit = trader.Balance.WithdrawalLimit.EachAmountLowerLimit,
                        WithdrawalCommissionRateInThousandth = trader.Balance.WithdrawalCommissionRateInThousandth,
                        DepositCommissionRateInThousandth = trader.Balance.DepositCommissionRateInThousandth,
                        RateAlipayInThousandth = trader.TradingCommission.RateAlipayInThousandth,
                        RateWechatInThousandth = trader.TradingCommission.RateWechatInThousandth
                    };
                }
                else if (userBaseRoleType == BaseRoleType.TraderAgent)
                {
                    var traderAgent = await _personalService.GetTraderAgent(
                        contextUserId
                        );

                    vm = new UpdateViewModel
                    {
                        UserId = traderAgent.TraderAgentId,
                        FullName = traderAgent.FullName,
                        UserName = traderAgent.Username,
                        Nickname = traderAgent.Nickname,
                        PhoneNumber = traderAgent.PhoneNumber,
                        Email = traderAgent.Email,
                        Wechat = traderAgent.Wechat,
                        QQ = traderAgent.QQ,
                        UplineId = traderAgent.UplineUserId,
                        IsEnabled = traderAgent.IsEnabled,
                        HasGrantRight = false,
                        DailyAmountLimit = traderAgent.Balance.WithdrawalLimit.DailyAmountLimit,
                        DailyFrequencyLimit = traderAgent.Balance.WithdrawalLimit.DailyFrequencyLimit,
                        EachAmountUpperLimit = traderAgent.Balance.WithdrawalLimit.EachAmountUpperLimit,
                        EachAmountLowerLimit = traderAgent.Balance.WithdrawalLimit.EachAmountLowerLimit,
                        WithdrawalCommissionRateInThousandth = traderAgent.Balance.WithdrawalCommissionRateInThousandth,
                        DepositCommissionRateInThousandth = traderAgent.Balance.DepositCommissionRateInThousandth,
                        RateAlipayInThousandth = traderAgent.TradingCommission.RateAlipayInThousandth,
                        RateWechatInThousandth = traderAgent.TradingCommission.RateWechatInThousandth
                    };
                }
                else if (userBaseRoleType == BaseRoleType.Shop)
                {
                    var shop = await _personalService.GetShop(
                        contextUserId
                        );

                    vm = new UpdateViewModel
                    {
                        UserId = shop.ShopId,
                        UserName = shop.Username,
                        FullName = shop.FullName,
                        SiteAddress = shop.SiteAddress,
                        PhoneNumber = shop.PhoneNumber,
                        Email = shop.Email,
                        Wechat = shop.Wechat,
                        QQ = shop.QQ,
                        UplineId = shop.UplineUserId,
                        IsEnabled = shop.IsEnabled,
                        HasGrantRight = false,
                        DailyAmountLimit = shop.Balance.WithdrawalLimit.DailyAmountLimit,
                        DailyFrequencyLimit = shop.Balance.WithdrawalLimit.DailyFrequencyLimit,
                        EachAmountUpperLimit = shop.Balance.WithdrawalLimit.EachAmountUpperLimit,
                        EachAmountLowerLimit = shop.Balance.WithdrawalLimit.EachAmountLowerLimit,
                        WithdrawalCommissionRateInThousandth = shop.Balance.WithdrawalCommissionRateInThousandth,
                        RateRebateAlipayInThousandth = shop.RebateCommission.RateRebateAlipayInThousandth,
                        RateRebateWechatInThousandth = shop.RebateCommission.RateRebateWechatInThousandth
                    };
                }
                else if (userBaseRoleType == BaseRoleType.ShopAgent)
                {
                    var shopAgent = await _personalService.GetShopAgent(
                        contextUserId
                        );

                    vm = new UpdateViewModel
                    {
                        UserId = shopAgent.ShopAgentId,
                        UserName = shopAgent.Username,
                        FullName = shopAgent.FullName,
                        Nickname = shopAgent.Nickname,
                        PhoneNumber = shopAgent.PhoneNumber,
                        Email = shopAgent.Email,
                        Wechat = shopAgent.Wechat,
                        QQ = shopAgent.QQ,
                        UplineId = shopAgent.UplineUserId,
                        IsEnabled = shopAgent.IsEnabled,
                        HasGrantRight = false,
                        DailyAmountLimit = shopAgent.Balance.WithdrawalLimit.DailyAmountLimit,
                        DailyFrequencyLimit = shopAgent.Balance.WithdrawalLimit.DailyFrequencyLimit,
                        EachAmountUpperLimit = shopAgent.Balance.WithdrawalLimit.EachAmountUpperLimit,
                        EachAmountLowerLimit = shopAgent.Balance.WithdrawalLimit.EachAmountLowerLimit,
                        WithdrawalCommissionRateInThousandth = shopAgent.Balance.WithdrawalCommissionRateInThousandth,
                        RateRebateAlipayInThousandth = shopAgent.RebateCommission.RateRebateAlipayInThousandth,
                        RateRebateWechatInThousandth = shopAgent.RebateCommission.RateRebateWechatInThousandth
                    };
                }
                /*else if (userBaseRoleType == BaseRoleType.Manager)
                {
                }*/
                else
                {
                    return BadRequest("无法辨识用户身份");
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        // POST: Personal/Update
        [Authorize(Policy = Permissions.Personal.Edit)]
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
                var contextUserId = User.GetUserId<string>();

                //Check the user is updating his info.
                if (vm.UserId != contextUserId)
                {
                    return BadRequest("无效操作");
                }

                var userBaseRoleType = await _userManager.Users
                    .Where(u => u.Id == contextUserId)
                    .Select(u => u.BaseRoleType)
                    .FirstOrDefaultAsync();

                await _personalService.UpdatePersonalInfo(
                    contextUserId,
                    vm.FullName,
                    vm.Nickname,
                    vm.SiteAddress,
                    vm.PhoneNumber,
                    vm.Email,
                    vm.Wechat,
                    vm.QQ
                    );

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Policy = Permissions.Personal.TwoFactorAuth.View)]
        [HttpGet]
        public async Task<IActionResult> TwoFactorAuth()
        {
            try
            {
                var contextUserId = User.GetUserId<string>();


                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var hasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null;
                var is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user);

                TwoFactorAuthViewModel vm = new TwoFactorAuthViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    HasAuthenticator = hasAuthenticator,
                    Is2faEnabled = is2faEnabled
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        [Authorize(Policy = Permissions.Personal.TwoFactorAuth.Enable)]
        [HttpGet]
        public async Task<IActionResult> EnableTwoFactorAuth()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return BadRequest($"找无用户: '{_userManager.GetUserId(User)}'");
                }

                var unformattedKey = await this.GetUnformattedKey(user);

                var sharedKey = LoadSharedKey(unformattedKey);
                var authenticatorUri = await LoadQrCodeUriAsync(user, unformattedKey);

                var qrCode = new QrCoderService().CreateQrCode(authenticatorUri);

                return Ok(new { success = true, sharedKey, authQrCode = "data:image/png;base64," + Convert.ToBase64String(qrCode) });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Personal/EnableTwoFactorAuth
        [Authorize(Policy = Permissions.Personal.TwoFactorAuth.Enable)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableTwoFactorAuth(EnableTwoFactorAuthViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "无效的验证码" });
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                if (!ModelState.IsValid)
                {
                    var unformattedKey = await this.GetUnformattedKey(user);

                    var sharedKey = LoadSharedKey(unformattedKey);
                    var authenticatorUri = await LoadQrCodeUriAsync(user, unformattedKey);
                    return BadRequest(new { sharedKey, authenticatorUri });
                }

                // Strip spaces and hypens
                var verificationCode = vm.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

                var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                    user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

                if (!is2faTokenValid)
                {
                    ModelState.AddModelError("Input.Code", "无效的验证码");

                    var unformattedKey = await this.GetUnformattedKey(user);

                    var sharedKey = LoadSharedKey(unformattedKey);
                    var authenticatorUri = await LoadQrCodeUriAsync(user, unformattedKey);

                    return BadRequest(new { message = "无效的验证码", sharedKey, authenticatorUri });
                }

                await _userManager.SetTwoFactorEnabledAsync(user, true);
                var userId = await _userManager.GetUserIdAsync(user);

                //var statusMessage = "Your authenticator app has been verified.";

                if (await _userManager.CountRecoveryCodesAsync(user) == 0)
                {
                    var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                    //RecoveryCodes = recoveryCodes.ToArray();

                    //No need recovery codes.
                    return Ok(new { success = true, recoveryCodes = recoveryCodes.ToArray() });
                    //return RedirectToPage("./ShowRecoveryCodes");
                }
                else
                {
                    return Ok(new { success = true });
                    //return RedirectToPage("./TwoFactorAuthentication");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Personal/DisableTwoFactorAuth
        [Authorize(Policy = Permissions.Personal.TwoFactorAuth.Enable)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableTwoFactorAuth()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
                if (!disable2faResult.Succeeded)
                {
                    throw new InvalidOperationException($"Unexpected error occurred disabling 2FA for user with ID '{_userManager.GetUserId(User)}'.");
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        private async Task<string> GetUnformattedKey(ApplicationUser user)
        {
            // Load the authenticator key & QR code URI to display on the form
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            return unformattedKey;
        }

        private string LoadSharedKey(string unformattedKey)
        {
            var sharedKey = FormatKey(unformattedKey);
            return sharedKey;
        }

        private async Task<string> LoadQrCodeUriAsync(ApplicationUser user, string unformattedKey)
        {
            var email = await _userManager.GetEmailAsync(user);
            var authenticatorUri = GenerateQrCodeUri(email, unformattedKey);
            return authenticatorUri;
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("闪付"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }
    }
}