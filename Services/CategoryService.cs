using TrackMyCash.Models;
using System.Collections.Generic;

namespace TrackMyCash.Services
{
    public class CategoryService
    {
        public List<Category> GetCategories()
        {
            // Повертаємо список для вибору у формі
            return new List<Category>
            {
                new Category { Id = 1, Name = "Зарплата", Type = "Income" },
                new Category { Id = 2, Name = "Стипендія", Type = "Income" },
                new Category { Id = 3, Name = "Їжа", Type = "Expense" },
                new Category { Id = 4, Name = "Розваги", Type = "Expense" },
                new Category { Id = 5, Name = "Транспорт", Type = "Expense" }
            };
        }
    }
}