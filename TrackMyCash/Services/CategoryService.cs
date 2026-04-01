using Microsoft.EntityFrameworkCore;
using TrackMyCash.Data;
using TrackMyCash.Models;
using TrackMyCash.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrackMyCash.Services
{
    public class CategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetCategoriesAsync(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<Category>();

            return await _context.Categories
                .Where(c => c.UserId == userId || c.IsDefault)
                .OrderBy(c => c.Type)
                .ThenBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> CreateCategoryAsync(CategoryViewModel model, string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return (false, "Користувач не автентифікований");

            var category = new Category
            {
                Name = model.Name,
                Type = model.Type,
                IsDefault = false,
                UserId = userId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return (true, "Категорію успішно створено");
        }

        public async Task<Category?> GetCategoryByIdAsync(int id, string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && (c.UserId == userId || c.IsDefault));
        }

        public async Task<(bool Success, string Message)> UpdateCategoryAsync(CategoryViewModel model, string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return (false, "Користувач не автентифікований");

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == model.Id && c.UserId == userId);

            if (category == null)
                return (false, "Категорію не знайдено");

            category.Name = model.Name;
            category.Type = model.Type;

            await _context.SaveChangesAsync();
            return (true, "Категорію успішно оновлено");
        }

        public async Task<(bool Success, string Message)> DeleteCategoryAsync(int id, string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return (false, "Користувач не автентифікований");

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (category == null)
                return (false, "Категорію не знайдено");

            if (category.IsDefault)
                return (false, "Неможливо видалити стандартну категорію");

            // Перевірка чи використовується категорія
            var hasTransactions = await _context.Transactions
                .AnyAsync(t => t.CategoryId == id);

            if (hasTransactions)
                return (false, "Неможливо видалити категорію з транзакціями");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return (true, "Категорію успішно видалено");
        }
    }
}