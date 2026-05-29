using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManager.Application.DTOs.Auth;
using PropertyManager.WEB.ApiClients.Contracts;
using PropertyManager.WEB.Infrastructure;
using PropertyManager.WEB.ViewModels.Account;

namespace PropertyManager.WEB.Controllers;

public class AccountController : Controller
{
    private readonly IAuthApiClient _authApiClient;

    public AccountController(IAuthApiClient authApiClient)
    {
        _authApiClient = authApiClient;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToLocal(returnUrl);

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var auth = await _authApiClient.LoginAsync(new LoginDto
        {
            Email = model.Email,
            Password = model.Password
        });

        if (auth is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        await SignInAsync(auth, model.RememberMe);
        return RedirectToLocal(model.ReturnUrl);
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Properties");

        return View(new RegisterViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var (auth, errors) = await _authApiClient.RegisterAsync(new RegisterDto
        {
            FullName = model.FullName,
            Email = model.Email,
            Password = model.Password,
            ConfirmPassword = model.ConfirmPassword
        });

        if (auth is null)
        {
            foreach (var error in errors)
                ModelState.AddModelError(string.Empty, error);

            return View(model);
        }

        await SignInAsync(auth, isPersistent: false);
        return RedirectToAction("Index", "Properties");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private async Task SignInAsync(AuthResponseDto auth, bool isPersistent)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, auth.UserId),
            new(ClaimTypes.Email, auth.Email),
            new(ClaimTypes.Name, auth.Email),
            new(AuthClaimTypes.AccessToken, auth.Token)
        };

        if (!string.IsNullOrWhiteSpace(auth.FullName))
            claims.Add(new Claim("full_name", auth.FullName));

        foreach (var role in auth.Roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
        await HttpContext.SignInAsync(
            IdentityConstants.ApplicationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = isPersistent });
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Properties");
    }
}
