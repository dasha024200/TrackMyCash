using TrackMyCash.Models;
using System.Collections.Generic;

namespace TrackMyCash.ViewModels
{
    public class HomeViewModel
    {
        // Вказуємо конкретні класи, щоб не було помилки CS0029
        public List<Transaction> RecentTransactions { get; set; } = new List<Transaction>();
        public List<Category> Categories { get; set; } = new List<Category>();
        
        public decimal TotalBalance { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
    }
}