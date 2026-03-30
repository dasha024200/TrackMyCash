using Microsoft.AspNetCore.Mvc;
using TrackMyCash.Services;  
using TrackMyCash.Factories;
using TrackMyCash.Models;

namespace TrackMyCash.Controllers
{
    public class TransactionController : Controller
    {
        private readonly TransactionService _transactionService;
        private readonly CategoryService _categoryService;

        public TransactionController(TransactionService transactionService, CategoryService categoryService)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
        }

        [HttpPost]
        public IActionResult Create(decimal Amount, int CategoryId, string Type)
        {
            TransactionFactory factory = Type == "Income" 
                ? new IncomeTransactionFactory() 
                : new ExpenseTransactionFactory();

            var transaction = factory.CreateTransaction();
            transaction.Amount = Amount;
            transaction.CategoryId = CategoryId;
            transaction.Date = System.DateTime.Now; // Важливо для помилки з Date

            _transactionService.AddTransaction(transaction);

            return RedirectToAction("Index", "Home");
        }
    }
}