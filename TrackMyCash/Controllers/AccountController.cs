using Microsoft.AspNetCore.Mvc;
using TrackMyCash.Models.ViewModels;
using TrackMyCash.Services;
using System.Threading.Tasks;

namespace TrackMyCash.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;

        public AccountController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _authService.RegisterAsync(model.Email, model.Password);
            if (result.Success)
            {
                TempData["Success"] = "Реєстрація пройшла успішно!";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _authService.LoginAsync(model.Email, model.Password, model.RememberMe);
            if (result.Success)
                return RedirectToLocal(returnUrl);

            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Login");
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            return Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Index", "Home");
        }
    }
}