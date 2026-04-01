using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TrackMyCash.Services;
using TrackMyCash.Models.ViewModels;
using System.Threading.Tasks;

namespace TrackMyCash.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var categories = await _categoryService.GetCategoriesAsync(userId);
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create() => View(new CategoryViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var result = await _categoryService.CreateCategoryAsync(model, userId);

            if (result.Success)
                return RedirectToAction("Index");

            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var category = await _categoryService.GetCategoryByIdAsync(id, userId);

            if (category == null || category.IsDefault)
                return NotFound();

            var model = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Type = category.Type
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var result = await _categoryService.UpdateCategoryAsync(model, userId);

            if (result.Success)
                return RedirectToAction("Index");

            ModelState.AddModelError("", result.Message);
            return View(model);
        }
    }
}