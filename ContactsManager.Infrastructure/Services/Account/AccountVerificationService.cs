using ContactsManager.Application.Interfaces;
using ContactsManager.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;

namespace ContactsManager.Infrastructure.Services.Account;

public class AccountVerificationService : IAccountVerificationService
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly LinkGenerator _linkGenerator;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public AccountVerificationService(IHttpContextAccessor contextAccessor,
        LinkGenerator linkGenerator, UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService)
    {
        _contextAccessor = contextAccessor;
        _userManager = userManager;
        _linkGenerator = linkGenerator;
        _currentUserService = currentUserService;
    }

    public async Task<string> GenerateVerificationLinkAsync(Guid userId, string controller, string action, string scheme)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }
        
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var callBackUrl = _linkGenerator.GetUriByAction(
            _contextAccessor.HttpContext!,
            action,
            controller,
            new { userId = user.Id, token },
            scheme: scheme);

        if (callBackUrl == null)
        {
            throw new InvalidOperationException($"Cannot generate verification link for {controller}/{action}");
        }

        return callBackUrl;
    }

    public async Task<string> GenerateConfirmEmailVerificationLinkAsync(string controller, string action, string scheme)
    {
        var user = await _userManager.FindByIdAsync(GetUserId().ToString());
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var callBackUrl = _linkGenerator.GetUriByAction(
            _contextAccessor.HttpContext!,
            action,
            controller,
            new { userId = user.Id, token },
            scheme: scheme);

        if (callBackUrl == null)
        {
            throw new InvalidOperationException($"Cannot generate verification link for {controller}/{action}");
        }

        return callBackUrl;
    }
    
    public async Task<string> GenerateChangeEmailVerificationLinkAsync(string newEmail, string controller, string action, string scheme)
    {
        var userId = GetUserId();

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);

        var callBackUrl = _linkGenerator.GetUriByAction(
            _contextAccessor.HttpContext!,
            action,
            controller,
            new { userId = user.Id, newEmail, token },
            scheme: scheme);

        if (callBackUrl == null)
        {
            throw new InvalidOperationException($"Cannot generate verification link for {controller}/{action}");
        }

        return callBackUrl;
    }

    public async Task<string> GeneratePasswordResetVeificiationLinkAsync(Guid id, string controller, string action, string scheme)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var callBackUrl = _linkGenerator.GetUriByAction(
            _contextAccessor.HttpContext!,
            action,
            controller,
            new { user.Email, token },
            scheme: scheme);

        if (callBackUrl == null)
        {
            throw new InvalidOperationException($"Cannot generate password verification link for {controller}/{action}");
        }

        return callBackUrl;
    }

    private Guid GetUserId()
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            throw new UnauthorizedAccessException("Current user is not authenticated.");
        }

        return userId.Value;
    }
}