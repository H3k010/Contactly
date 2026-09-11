using System.Diagnostics;
using ContactsManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ContactsManager.Presentation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;

namespace ContactsManager.Presentation.Controllers;

[AllowAnonymous]
[Route("[action]")]
public class HomeController : Controller
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IAccountService _accountService;

    public HomeController(IAccountService accountService, ICurrentUserService currentUserService)
    {
        _accountService = accountService;
        _currentUserService = currentUserService;
    }

    [Route("/")]
    [Route("/[controller]")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Settings()
    {
        var viewModel = await BuildSettingsViewModel();
        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public IActionResult About()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Terms()
    {
        return View();
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        var statusCodeDetails = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

        if (exceptionDetails == null && statusCodeDetails == null)
        {
            return RedirectToAction("Index", "Home");
        }
        
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    
    [NonAction]
    private async Task<SettingsViewModel> BuildSettingsViewModel()
    {
        if (_currentUserService.UserId == null)
        {
            return new SettingsViewModel
            {
                IsAuthenticated = false,
                IsEmailConfirmed = false,
                ConfirmPassword = new ConfirmPasswordViewModel()
            };
        }

        var userId = _accountService.GetCurrentUserId();
        var user = await _accountService.GetUserByIdAsync(userId);

        return new SettingsViewModel
        {
            DisplayName = user.UserName,
            Email = user.Email,
            IsAuthenticated = user.IsAuthenticated,
            IsEmailConfirmed = user.EmailConfirmed,
            ConfirmPassword = new ConfirmPasswordViewModel()
        };
    }
}