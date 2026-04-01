using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TrackMyCash.Models;

namespace TrackMyCash.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            // Переконаємося, що база даних створена
            await context.Database.MigrateAsync();

            // Перевіряємо, чи вже є стандартні категорії
            if (await context.Categories.AnyAsync(c => c.IsDefault))
                return;

            var defaultCategories = new List<Category>
            {
                // Категорії доходу
                new Category { Name = "Зарплата", Type = "Income", IsDefault = true, UserId = null },
                new Category { Name = "Фрилансерські замовлення", Type = "Income", IsDefault = true, UserId = null },
                new Category { Name = "Бонус", Type = "Income", IsDefault = true, UserId = null },
                new Category { Name = "Подарунок", Type = "Income", IsDefault = true, UserId = null },
                new Category { Name = "Інвестиційні доходи", Type = "Income", IsDefault = true, UserId = null },

                // Категорії витрат
                new Category { Name = "Продукти", Type = "Expense", IsDefault = true, UserId = null },
                new Category { Name = "Транспорт", Type = "Expense", IsDefault = true, UserId = null },
                new Category { Name = "Комунальні послуги", Type = "Expense", IsDefault = true, UserId = null },
                new Category { Name = "Розваги", Type = "Expense", IsDefault = true, UserId = null },
                new Category { Name = "Здоров'я", Type = "Expense", IsDefault = true, UserId = null },
                new Category { Name = "Освіта", Type = "Expense", IsDefault = true, UserId = null },
                new Category { Name = "Технологія", Type = "Expense", IsDefault = true, UserId = null },
                new Category { Name = "Милість", Type = "Expense", IsDefault = true, UserId = null },
                new Category { Name = "Одяг", Type = "Expense", IsDefault = true, UserId = null },
                new Category { Name = "Проживання", Type = "Expense", IsDefault = true, UserId = null }
            };

            await context.Categories.AddRangeAsync(defaultCategories);
            await context.SaveChangesAsync();
        }
    }
}
