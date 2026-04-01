using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrackMyCash.Services;
using TrackMyCash.Models.ViewModels;
using System.Threading.Tasks;
using System.Linq;

namespace TrackMyCash.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly TransactionService _transactionService;
        private readonly BalanceService _balanceService;
        private readonly CategoryService _categoryService;

        public HomeController(
            TransactionService transactionService, 
            BalanceService balanceService, 
            CategoryService categoryService)
        {
            _transactionService = transactionService;
            _balanceService = balanceService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var balance = await _balanceService.GetBalanceAsync(userId);
            var recentTransactions = await _transactionService.GetRecentTransactionsAsync(userId, 10);
            var categories = await _categoryService.GetCategoriesAsync(userId);

            var model = new HomeViewModel
            {
                Balance = balance,
                RecentTransactions = recentTransactions
            };

            // Заповнення випадаючого списку категорій
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.Name} ({c.Type})"
            }).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuickTransaction(TransactionViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Index));

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var result = await _transactionService.CreateTransactionAsync(model, userId);

            if (result.Success)
                TempData["Success"] = "Транзакцію успішно додано!";
            else
                TempData["Error"] = result.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}