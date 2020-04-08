using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using WebMVC.Models.Roles;
using Microsoft.AspNetCore.Http;
using WebMVC.Extensions;
using Microsoft.Extensions.Primitives;
using WebMVC.Models.Queries;

namespace WebMVC.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginModel(SignInManager<ApplicationUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        //[BindProperty] //Use Custom bind to avoid validate unnecessary properties.
        public LoginViewModel LoginInput { get; set; }

        //[BindProperty] //Use Custom bind to avoid validate unnecessary properties.
        public RegisterViewModel RegisterInput { get; set; }

        //[BindProperty] //Use Custom bind to avoid validate unnecessary properties.
        public LoginWithTwoFactorAuthViewModel TwoFactorAuthInput { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }


        #region Model
        public class LoginViewModel
        {
            [Required]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }

            [Required]
            [HiddenInput]
            public string LoginRole { get; set; }
        }
        public class RegisterViewModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            public string ConfirmPassword { get; set; }
        }
        public class LoginWithTwoFactorAuthViewModel
        {
            [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "认证码")]
            public string TwoFactorCode { get; set; }

            [Display(Name = "记住这台机器。")]
            public bool RememberMachine { get; set; }
        }

        public class AjaxPostResponce
        {
            public AjaxPostResponce(string result = null, string description = null, string returnurl = null)
            {
                this.result = result;
                this.description = description;
                this.returnurl = returnurl;
            }

            public string result { get; set; }

            public string description { get; set; }
            public string returnurl { get; set; }
        }
        #endregion


        public async Task<IActionResult> OnGetAsync(string returnUrl = null, string panel = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            if (_signInManager.IsSignedIn(User))
            {
                return LocalRedirect("~/");
            }

            this.LoginInput = new LoginViewModel();

            //Specific Login Panel
            if (panel != null)
            {
                if (Roles.IsPanelRole(panel))
                {
                    ViewData["Panel"] = panel;
                    LoginInput.LoginRole = panel;
                }
                else
                {
                    //Default panel is admin panel.
                    ViewData["Panel"] = Roles.Manager;
                    LoginInput.LoginRole = Roles.Manager;
                }
            }
            else
            {
                //Default panel is admin panel.
                ViewData["Panel"] = Roles.Manager;
                LoginInput.LoginRole = Roles.Manager;
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
            ViewData["ReturnUrl"] = this.ReturnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostLoginAsync([Bind("Email, Password, RememberMe, LoginRole")] LoginViewModel LoginInput, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                //Validate Login Role
                var roleToLogin = LoginInput.LoginRole;
                if (!Roles.IsPanelRole(roleToLogin))
                {
                    ModelState.AddModelError(string.Empty, "无效入口");
                    var ajaxResult = new AjaxPostResponce(
                        "failed",
                        "登录失败。无效入口。",
                        this.ReturnUrl);
                    return new JsonResult(ajaxResult);
                }

                var user = await _userManager.FindByNameAsync(LoginInput.Email);
                if (user != null)
                {
                    var userRoleNames = await _userManager.GetRolesAsync(user);
                    if (!userRoleNames.Any(r => r.Equals(roleToLogin, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        var ajaxResult = new AjaxPostResponce(
                            "failed",
                            "登录失败。请在正确的入口登录。",
                            this.ReturnUrl);
                        return new JsonResult(ajaxResult);
                    }

                    //Validating account status.
                    if (!user.IsEnabled)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        var ajaxResult = new AjaxPostResponce(
                            "failed",
                            "登录失败。您的账户尚未启用。",
                            this.ReturnUrl);
                        return new JsonResult(ajaxResult);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    var ajaxResult = new AjaxPostResponce(
                        "failed",
                        "登录失败。查无用户。",
                        this.ReturnUrl);
                    return new JsonResult(ajaxResult);
                }





                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(LoginInput.Email, LoginInput.Password, LoginInput.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    //return LocalRedirect(returnUrl);

                    //Update Login Record
                    user.DateLastLoggedIn = DateTime.UtcNow.ToFullString();

                    var logginIp = string.Empty;
                    //first try to get IP address from the forwarded header
                    if (_httpContextAccessor.HttpContext.Request.Headers != null)
                    {
                        //the X-Forwarded-For (XFF) HTTP header field is a de facto standard for identifying the originating IP address of a client
                        //connecting to a web server through an HTTP proxy or load balancer

                        var forwardedHeader = _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"];
                        if (!StringValues.IsNullOrEmpty(forwardedHeader))
                        {
                            logginIp = forwardedHeader.FirstOrDefault();
                        }
                    }

                    //if this header not exists try get connection remote IP address
                    if (string.IsNullOrEmpty(logginIp) && _httpContextAccessor.HttpContext.Connection.RemoteIpAddress != null)
                    {
                        logginIp = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    }

                    //user.LastLoginIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                    user.LastLoginIP = logginIp;

                    await _userManager.UpdateAsync(user);

                    var ajaxResult = new AjaxPostResponce(
                        "success",
                        "User logged in.",
                        returnUrl);
                    //return RedirectToPage("/Home");
                    return new JsonResult(ajaxResult);
                }
                if (result.RequiresTwoFactor)
                {
                    var ajaxResult = new AjaxPostResponce(
                        "auth_required",
                        "Require authentication.",
                        returnUrl);
                    //return RedirectToPage("/Home");
                    return new JsonResult(ajaxResult);
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("您的账户已锁住。");
                    var ajaxResult = new AjaxPostResponce(
                                   "failed",
                                   "您的账户已锁住。",
                                   this.ReturnUrl);
                    //return RedirectToPage("./Lockout");
                    return new JsonResult(ajaxResult);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    var ajaxResult = new AjaxPostResponce(
                        "failed",
                        "登录失败。",
                        this.ReturnUrl);
                    return new JsonResult(ajaxResult);
                }
            }

            // If we got this far, something failed, redisplay form


            return new JsonResult(new AjaxPostResponce(
                "failed",
                "登录失败。无效参数",
                this.ReturnUrl));
        }

        public async Task<IActionResult> OnPostRegisterAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = RegisterInput.Email, Email = RegisterInput.Email };
                var result = await _userManager.CreateAsync(user, RegisterInput.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(RegisterInput.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = RegisterInput.Email });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        public async Task<IActionResult> OnPostLoginWithTwoFactorAuthAsync([Bind("TwoFactorCode, RememberMachine")] LoginWithTwoFactorAuthViewModel TwoFactorAuthInput, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            var authenticatorCode = TwoFactorAuthInput.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, true, TwoFactorAuthInput.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);

                var ajaxResult = new AjaxPostResponce(
                    "success",
                    "User logged in.",
                    returnUrl);
                //return LocalRedirect(returnUrl);
                return new JsonResult(ajaxResult);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");

                var ajaxResult = new AjaxPostResponce(
                    "failed",
                    "您的账户已被锁住。请恰客服后续流程。",
                    this.ReturnUrl);
                //return RedirectToPage("./Lockout");
                return new JsonResult(ajaxResult);
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);

                var ajaxResult = new AjaxPostResponce(
                    "failed",
                    "验证失败，请仔细确认验证码后再次尝试。",
                    this.ReturnUrl);
                return new JsonResult(ajaxResult);
            }
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            this.ClearFieldErrors(key => key.Contains("RegisterInput"));
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(LoginInput.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                LoginInput.Email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            return Page();
        }

        private void ClearFieldErrors(Func<string, bool> predicate)
        {
            foreach (var field in ModelState)
            {
                if (field.Value.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                {
                    if (predicate(field.Key))
                    {
                        field.Value.ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
                    }
                }
            }
        }
    }
}
