using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TrackMyCash.Services;
using TrackMyCash.Models.ViewModels;
using System.Threading.Tasks;

namespace TrackMyCash.Controllers
{
    [Authorize]
    public class AnalyticsController : Controller
    {
        private readonly TransactionService _transactionService;
        private readonly ChartContext _chartContext;

        public AnalyticsController(TransactionService transactionService, ChartContext chartContext)
        {
            _transactionService = transactionService;
            _chartContext = chartContext;
        }

        public async Task<IActionResult> Index(string period = "Month")
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var transactions = await _transactionService.GetTransactionsAsync(userId);

            _chartContext.SetStrategy(period);

            var chartData = _chartContext.BuildChart(userId, transactions);

            var model = new AnalyticsViewModel
            {
                TotalIncome = transactions.Where(t => t.Type == "Income").Sum(t => t.Amount),
                TotalExpense = transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount),
                Balance = chartData.Balance,
                SelectedPeriod = period,
                IncomeDataJson = chartData.IncomeDataJson,
                ExpenseDataJson = chartData.ExpenseDataJson,
                LabelsJson = chartData.LabelsJson,
                RecentTransactions = transactions.Take(10).ToList()
            };

            return View(model);
        }
    }
}