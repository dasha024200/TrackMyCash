using Microsoft.AspNetCore.Mvc;
using TrackMyCash.Services;
using TrackMyCash.ViewModels;

namespace TrackMyCash.Controllers
{
    public class HomeController : Controller
    {
        private readonly TransactionService _transactionService;
        private readonly CategoryService _categoryService;

        public HomeController(TransactionService transactionService, CategoryService categoryService)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            var transactions = _transactionService.GetAllTransactions();
            
            // Рахуємо суми для карток
            decimal income = transactions.Where(t => t.Type == "Income").Sum(t => t.Amount);
            decimal expense = transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount);

            var viewModel = new HomeViewModel
            {
                RecentTransactions = transactions,
                Categories = _categoryService.GetCategories(),
                TotalIncome = income,
                TotalExpense = expense,
                TotalBalance = income - expense // Виводимо чистий баланс
            };

            return View(viewModel);
        }
    }
}