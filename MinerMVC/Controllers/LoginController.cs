using System.Collections.Concurrent;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MinerMVC.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private const int MaxFailures = 5;
        private static readonly TimeSpan LockoutPeriod = TimeSpan.FromMinutes(5);
        private static readonly ConcurrentDictionary<string, LoginAttempt> Attempts = new();

        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginController> _logger;

        public LoginController(IConfiguration configuration, ILogger<LoginController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToLocal(returnUrl);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string? username, string? password, string? returnUrl, bool rememberMe)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var key = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            if (IsLockedOut(key))
            {
                ViewBag.ErrorMessage = "登入失敗次數過多，請稍後再試。";
                return View();
            }

            if (!CredentialsMatch(username, password))
            {
                RegisterFailure(key);
                ViewBag.ErrorMessage = "帳號或密碼不正確。";
                return View();
            }

            Attempts.TryRemove(key, out _);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, username!.Trim())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var properties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(14) : null
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                properties);

            return RedirectToLocal(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        public IActionResult CreateAccount()
        {
            return View();
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "CustomExcel");
        }

        private bool CredentialsMatch(string? username, string? password)
        {
            var expectedUser = _configuration["Auth:Username"];
            var expectedPassword = _configuration["Auth:Password"];
            if (string.IsNullOrEmpty(expectedUser) || string.IsNullOrEmpty(expectedPassword))
            {
                _logger.LogWarning("Login rejected because Auth:Username or Auth:Password is not configured.");
                return false;
            }

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            return FixedTimeEquals(username.Trim(), expectedUser) && FixedTimeEquals(password, expectedPassword);
        }

        private static bool FixedTimeEquals(string provided, string expected)
        {
            var providedHash = SHA256.HashData(Encoding.UTF8.GetBytes(provided));
            var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(expected));
            return CryptographicOperations.FixedTimeEquals(providedHash, expectedHash);
        }

        private static bool IsLockedOut(string key)
        {
            if (!Attempts.TryGetValue(key, out var attempt))
                return false;

            if (attempt.LockedUntil > DateTimeOffset.UtcNow)
                return true;

            if (attempt.LockedUntil != DateTimeOffset.MinValue)
                Attempts.TryRemove(key, out _);

            return false;
        }

        private static void RegisterFailure(string key)
        {
            Attempts.AddOrUpdate(
                key,
                _ => new LoginAttempt(1, DateTimeOffset.MinValue),
                (_, existing) =>
                {
                    var failures = existing.LockedUntil > DateTimeOffset.UtcNow ? existing.Failures : existing.Failures + 1;
                    var lockedUntil = failures >= MaxFailures
                        ? DateTimeOffset.UtcNow.Add(LockoutPeriod)
                        : DateTimeOffset.MinValue;
                    return new LoginAttempt(failures >= MaxFailures ? 0 : failures, lockedUntil);
                });
        }

        private sealed record LoginAttempt(int Failures, DateTimeOffset LockedUntil);
    }
}
