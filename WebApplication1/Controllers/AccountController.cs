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

            if (user.Role == UserRole.Admin)
            {
                ModelState.AddModelError("", "Password reset is not available for admin accounts.");
                return View(model);
            }

            // Simple flow: send user to reset form (no email/token)
            return RedirectToAction(nameof(ResetPassword), new { email = model.Email });
        }

        [HttpGet]
        public IActionResult ResetPassword(string? email)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                var u = _authService.GetUserByEmail(email);
                if (u != null && u.Role == UserRole.Admin)
                {
                    TempData["ErrorMessage"] = "Password reset is not available for admin accounts.";
                    return RedirectToAction(nameof(Login));
                }
            }

            var model = new ResetPasswordViewModel
            {
                Email = email ?? string.Empty
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _authService.GetUserByEmail(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "Email does not exist in the system");
                return View(model);
            }

            if (user.Role == UserRole.Admin)
            {
                ModelState.AddModelError("", "Password reset is not available for admin accounts.");
                return View(model);
            }

            var ok = _authService.ResetPassword(model.Email, model.NewPassword);
            if (!ok)
            {
                ModelState.AddModelError("", "Unable to reset password");
                return View(model);
            }

            TempData["SuccessMessage"] = "Password updated successfully. Please login.";
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
