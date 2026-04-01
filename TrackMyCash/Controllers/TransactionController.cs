using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrackMyCash.Services;
using TrackMyCash.Models.ViewModels;
using System.Threading.Tasks;

namespace TrackMyCash.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly TransactionService _transactionService;
        private readonly CategoryService _categoryService;
        private readonly PdfExportService _pdfExportService;

        public TransactionController(
            TransactionService transactionService, 
            CategoryService categoryService,
            PdfExportService pdfExportService)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
            _pdfExportService = pdfExportService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var transactions = await _transactionService.GetTransactionsAsync(userId);
            return View(transactions);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var categories = await _categoryService.GetCategoriesAsync(userId);

            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.Name} ({c.Type})"
            });

            return View(new TransactionViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransactionViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var result = await _transactionService.CreateTransactionAsync(model, userId);

            if (result.Success)
                return RedirectToAction("Index");

            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var transaction = await _transactionService.GetTransactionByIdAsync(id, userId);

            if (transaction == null)
                return NotFound();

            var categories = await _categoryService.GetCategoriesAsync(userId);
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.Name} ({c.Type})"
            });

            var model = new TransactionViewModel
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Type = transaction.Type,
                CategoryId = transaction.CategoryId,
                Comment = transaction.Comment,
                DateCreated = transaction.DateCreated
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TransactionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var result = await _transactionService.UpdateTransactionAsync(id, model, userId);

            if (result.Success)
                return RedirectToAction("Index");

            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var result = await _transactionService.DeleteTransactionAsync(id, userId);

            if (result.Success)
                TempData["Success"] = "Транзакцію успішно видалено!";
            else
                TempData["Error"] = result.Message;

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ExportPDF()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userName = User.Identity?.Name ?? "Unknown";
            var transactions = await _transactionService.GetTransactionsAsync(userId);

            var pdfData = _pdfExportService.ExportTransactionsReceiptPdf(transactions, userName);
            return File(pdfData, "application/pdf", $"TrackMyCash_Receipt_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
        }
    }
}