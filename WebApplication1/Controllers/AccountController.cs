using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// Controller for login, register, logout
    /// </summary>
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // If already logged in, redirect to dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToDashboard();
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _authService.Authenticate(model.Email, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            if (user.Status == UserStatus.Pending)
            {
                ModelState.AddModelError("", "Your account is pending approval");
                return View(model);
            }

            if (user.Status == UserStatus.Locked)
            {
                ModelState.AddModelError("", "Your account has been locked");
                return View(model);
            }

            // Create claims
            var claims = new List<Claim>
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim("Role", user.Role.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Redirect by role
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToDashboardByRole(user.Role);
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToDashboard();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = _authService.Register(model.Email, model.Password, model.FullName);
            if (!success)
            {
                ModelState.AddModelError("", "Email already exists in the system");
                return View(model);
            }

            TempData["SuccessMessage"] = "Registration successful! Your account is pending approval.";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _authService.GetUserByEmail(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email does not exist in the system");
                return View(model);
            }

            // Mock reset password - reset to default password
            _authService.ResetPassword(model.Email, "123456");

            TempData["SuccessMessage"] = "Password has been reset to: 123456. Please login and change your password.";
            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectToDashboard()
        {
            var roleClaim = User.FindFirst("Role");
            if (roleClaim == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var role = Enum.Parse<UserRole>(roleClaim.Value);
            return RedirectToDashboardByRole(role);
        }

        private IActionResult RedirectToDashboardByRole(UserRole role)
        {
            return role switch
            {
                UserRole.Admin => Redirect("/Admin/Dashboard"),
                UserRole.Lecturer => Redirect("/Lecturer/Dashboard"),
                UserRole.Student => Redirect("/Student/Dashboard"),
                _ => RedirectToAction("Index", "Home")
            };
        }
    }
}
