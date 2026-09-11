using ContactsManager.Application.DTO;
using ContactsManager.Application.Interfaces;
using ContactsManager.Presentation.Extentions;
using ContactsManager.Presentation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;

namespace ContactsManager.Presentation.Controllers;

[Route("[controller]/[action]")]
public class AccountController : Controller
{
    private readonly IStringLocalizer<AccountController> _localizer;
    private readonly IHtmlLocalizer<AccountController> _htmlLocalizer;
    private readonly IAccountService _accountService;
    private readonly IAccountVerificationService _verificationService;
    private readonly IStepUpAuthentication _stepUpAuthentication;
    private readonly IEmailSenderService _emailSenderService;

    public AccountController(IStringLocalizer<AccountController> localizer,
        IHtmlLocalizer<AccountController> htmlLocalizer, IAccountService accountService,
        IAccountVerificationService accountVerificationService, IStepUpAuthentication stepUpAuthentication,
        IEmailSenderService emailSenderService)
    {
        _localizer = localizer;
        _htmlLocalizer = htmlLocalizer;
        _accountService = accountService;
        _verificationService = accountVerificationService;
        _stepUpAuthentication = stepUpAuthentication;
        _emailSenderService = emailSenderService;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (_accountService.IsUserAuthenticated())
        {
            return RedirectToAction("Index", "Contacts");
        }

        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        if (_accountService.IsUserAuthenticated())
        {
            return RedirectToAction("Index", "Contacts");
        }

        if (!ModelState.IsValid)
        {
            return View(registerDto);
        }

        var result = await _accountService.RegisterAsync(registerDto);

        if (result.IsEmailDuplicate)
        {
            TempData["EmailSent"] = true;
            TempData["RegisterMessage"] = _localizer["EmailConfirmationSent"].Value;
            return View(registerDto);
        }

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(registerDto);
        }

        var callBackUrl =
            await _verificationService.GenerateVerificationLinkAsync(
                result.UserId, "Account", "ConfirmEmail", Request.Scheme);

        if (string.IsNullOrEmpty(callBackUrl))
        {
            ModelState.AddModelError(string.Empty, _localizer["RegistrationFailed"]);
            return View(registerDto);
        }

        var subject = _localizer["EmailSubject"];
        var message = _htmlLocalizer["EmailBody", callBackUrl].ToHtmlString();

        await _emailSenderService.SendEmailAsync(registerDto.Email, subject, message);

        TempData["EmailSent"] = true;
        TempData["RegisterMessage"] = _localizer["EmailConfirmationSent"].Value;
        return View(registerDto);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        if (_accountService.IsUserAuthenticated())
        {
            return RedirectToAction("Index", "Contacts");
        }

        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto loginDto, string? returnUrl)
    {
        if (_accountService.IsUserAuthenticated())
        {
            return RedirectToAction("Index", "Contacts");
        }

        if (!ModelState.IsValid)
        {
            return View(loginDto);
        }

        var result = await _accountService.LoginAsync(loginDto);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(loginDto);
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction("Index", "Contacts");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _accountService.LogoutAsync();
        return RedirectToAction("Login");
    }

    [HttpGet]
    [ActionName("Profile-Settings")]
    public async Task<IActionResult> ProfileSettings()
    {
        var viewModel = await BuildProfileSettingsViewModel();

        return View("ProfileSettings", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeName(ChangeNameDto model)
    {
        if (!ModelState.IsValid)
        {
            var viewModel = await BuildProfileSettingsViewModel();
            viewModel.Name.NewName = model.NewName;
            return View("ProfileSettings", viewModel);
        }

        var result = await _accountService.ChangeUserNameAsync(model.NewName);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(nameof(model.NewName), error);
            }

            var viewModel = await BuildProfileSettingsViewModel();
            viewModel.Name.NewName = model.NewName;
            return View("ProfileSettings", viewModel);
        }

        return RedirectToAction("Settings", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> ChangeEmail(ChangeEmailDto model)
    {
        ProfileSettingsViewModel viewModel;
        if (!ModelState.IsValid)
        {
            viewModel = await BuildProfileSettingsViewModel();
            viewModel.Email.Email = model.Email;
            return View("ProfileSettings", viewModel);
        }

        if (!_stepUpAuthentication.IsAuthenticated(nameof(ChangeEmail)))
        {
            return Unauthorized();
        }

        string callBackUrl;
        var userId = _accountService.GetCurrentUserId();
        var user = await _accountService.GetUserByIdAsync(userId);

        if (user.Email == model.Email && user.EmailConfirmed)
        {
            ViewBag.EmailSent = false;
            ViewBag.Message = _localizer["EmailAlreadyVerified"];

            viewModel = await BuildProfileSettingsViewModel();
            viewModel.Email.Email = model.Email;
            return View("ProfileSettings", viewModel);
        }

        if (user.Email == model.Email && !user.EmailConfirmed)
        {
            callBackUrl =
                await _verificationService.GenerateConfirmEmailVerificationLinkAsync("Account", "ConfirmEmail",
                    Request.Scheme);
        }
        else
        {
            callBackUrl =
                await _verificationService.GenerateChangeEmailVerificationLinkAsync(model.Email, "Account",
                    "ConfirmEmail", Request.Scheme);
        }

        var subject = _localizer["EmailSubject"];
        var message = _htmlLocalizer["EmailBody", callBackUrl].ToHtmlString();

        await _emailSenderService.SendEmailAsync(model.Email, subject, message);

        ViewBag.EmailSent = true;
        ViewBag.Message = _localizer["EmailVerificationLinkSent"];

        _stepUpAuthentication.Clear(nameof(ChangeEmail));

        viewModel = await BuildProfileSettingsViewModel();
        viewModel.Email.Email = model.Email;
        return View("ProfileSettings", viewModel);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(Guid userId, string? newEmail, string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Settings", "Home");
        }

        var user = await _accountService.GetUserByIdAsync(userId);
        
        var result = new AuthenticationResultDto();
        if (newEmail == null && !user.EmailConfirmed)
        {
            result = await _accountService.ConfirmEmailAsync(userId, token);
        }
        else
        {
            if (newEmail != null)
            {
                result = await _accountService.ChangeEmailAsync(userId, newEmail, token);
            }
        }

        if (!result.Succeeded)
        {
            TempData["EmailConfirmed"] = false;
            TempData["ConfirmMessageFail"] = _localizer["EmailConfirmationFailed"].Value;
            return RedirectToAction("Confirm-Email-Verification", "Account");
        }

        TempData["EmailConfirmed"] = true;
        TempData["ConfirmMessageSuccess"] = _localizer["EmailConfirmationSuccess"].Value;
        return RedirectToAction("Confirm-Email-Verification", "Account");
    }

    [HttpGet]
    [AllowAnonymous]
    [ActionName("Confirm-Email-Verification")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult ConfirmEmailVerification() => View("ConfirmEmailVerification");

    [HttpPost]
    public async Task<IActionResult> ChangePhone(ChangePhoneDto model)
    {
        if (!ModelState.IsValid)
        {
            var viewModel = await BuildProfileSettingsViewModel();
            viewModel.Phone.PhoneNumber = model.PhoneNumber;
            return View("ProfileSettings", viewModel);
        }

        var result = await _accountService.ChangePhoneNumberAsync(model.PhoneNumber);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(nameof(model.PhoneNumber), error);
            }

            var viewModel = await BuildProfileSettingsViewModel();
            viewModel.Phone.PhoneNumber = model.PhoneNumber;
            return View("ProfileSettings", viewModel);
        }

        return RedirectToAction("Settings", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> ChangePreferences(ChangePreferencesDto model)
    {
        if (!ModelState.IsValid)
        {
            var viewModel = await BuildProfileSettingsViewModel();
            viewModel.UserPreferences.UserPreferences = model.UserPreferences;
            return View("ProfileSettings", viewModel);
        }

        var result = await _accountService.ChangeUserPreferencesAsync(model.UserPreferences);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(nameof(model.UserPreferences), error);
            }

            var viewModel = await BuildProfileSettingsViewModel();
            viewModel.UserPreferences.UserPreferences = model.UserPreferences;
            return View("ProfileSettings", viewModel);
        }

        return RedirectToAction("Settings", "Home");
    }

    [HttpGet]
    [ActionName("Change-Password")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult ChangePassword()
    {
        if (!_stepUpAuthentication.IsAuthenticated("Change-Password"))
        {
            return RedirectToAction("Settings", "Home");
        }

        return View("ChangePassword");
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!_stepUpAuthentication.IsAuthenticated("Change-Password"))
        {
            return Unauthorized();
        }

        var result = await _accountService.ChangePasswordAsync(model.CurrentPassword, model.NewPassword);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(nameof(model.NewPassword), error);
            }

            return View(model);
        }

        _stepUpAuthentication.Clear("Change-Password");
        TempData["PasswordChanged"] = true;

        return RedirectToAction("Settings", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmPassword(ConfirmPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                message = _localizer["IncorrectPassword"].Value
            });
        }

        var allowedActions = new[] { "Change-Password", "ChangeEmail", "DeleteAccount" };

        if (!allowedActions.Contains(model.Action))
        {
            return BadRequest();
        }

        var result = await _accountService.ConfirmPasswordAsync(model.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                success = false,
                message = _localizer["InvalidPassword"].Value
            });
        }

        _stepUpAuthentication.Authenticate(model.Action);

        return Json(new
        {
            success = true,
            action = model.Action
        });
    }

    [HttpGet]
    [AllowAnonymous]
    [ActionName("Forgot-Password")]
    public IActionResult ForgotPassword()
    {
        if (_accountService.IsUserAuthenticated())
        {
            return RedirectToAction("Index", "Contacts");
        }

        return View("ForgotPassword");
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgetPasswordDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _accountService.GetUserByEmailAsync(model.Email);
        if (user != null && await _accountService.IsUserEmailConfirmedAsync(user))
        {
            var callBackUrl = await _verificationService
                .GeneratePasswordResetVeificiationLinkAsync(user.Id, "Account", "Reset-Password", Request.Scheme);

            var subject = _localizer["ResetPasswordSubject"];
            var message = _htmlLocalizer["ResetPasswordBody", callBackUrl].ToHtmlString();
            await _emailSenderService.SendEmailAsync(user.Email, subject, message);
            TempData["EmailSent"] = true;
        }

        return RedirectToAction("Forgot-Password-Confirmation");
    }

    [HttpGet]
    [AllowAnonymous]
    [ActionName("Forgot-Password-Confirmation")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult ForgotPasswordConfirmation()
    {
        if (_accountService.IsUserAuthenticated())
        {
            return RedirectToAction("Index", "Contacts");
        }

        return View("ForgotPasswordConfirmation");
    }


    [HttpGet]
    [AllowAnonymous]
    [ActionName("Reset-Password")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult ResetPassword(string? email, string? token)
    {
        if (_accountService.IsUserAuthenticated())
        {
            return RedirectToAction("Index", "Contacts");
        }

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Home");
        }

        var viewModel = new ResetPasswordDto { Email = email, Token = token };

        return View("ResetPassword", viewModel);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _accountService.GetUserByEmailAsync(model.Email);
        if (user == null)
        {
            TempData["PasswordReset"] = true;
            return RedirectToAction("Reset-Password-Confirmation");
        }

        var result = await _accountService.ResetPasswordAsync(user, model.Token, model.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }

            return View(model);
        }

        TempData["PasswordReset"] = true;
        return RedirectToAction("Reset-Password-Confirmation");
    }

    [HttpGet]
    [AllowAnonymous]
    [ActionName("Reset-Password-Confirmation")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult ResetPasswordConfirmation()
    {
        if (_accountService.IsUserAuthenticated())
        {
            return RedirectToAction("Index", "Contacts");
        }

        return View("ResetPasswordConfirmation");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAccount()
    {
        if (!_stepUpAuthentication.IsAuthenticated(nameof(DeleteAccount)))
        {
            return Unauthorized();
        }

        var result = await _accountService.DeleteAccount();

        if (!result.Succeeded)
        {
            TempData["DeleteFailed"] = true;
            return RedirectToAction("Settings", "Home");
        }

        _stepUpAuthentication.Clear(nameof(DeleteAccount));

        return RedirectToAction("Index", "Home");
    }


    // non-actions

    [NonAction]
    private async Task<ProfileSettingsViewModel> BuildProfileSettingsViewModel()
    {
        var userId = _accountService.GetCurrentUserId();
        var user = await _accountService.GetUserByIdAsync(userId);

        return new ProfileSettingsViewModel
        {
            Name = new ChangeNameDto { NewName = user.UserName },
            Email = new ChangeEmailDto { Email = user.Email },
            Phone = new ChangePhoneDto { PhoneNumber = user.PhoneNumber },
            UserPreferences = new ChangePreferencesDto { UserPreferences = user.UserPreferences },
            ConfirmPassword = new ConfirmPasswordViewModel()
        };
    }
}