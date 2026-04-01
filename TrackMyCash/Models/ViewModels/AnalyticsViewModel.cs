using TrackMyCash.Models;

namespace TrackMyCash.Models.ViewModels
{
    public class AnalyticsViewModel
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }

        public string SelectedPeriod { get; set; } = "Month"; // Month, Year, Week, Date

        // Дані для діаграм (JSON для Chart.js)
        public string IncomeDataJson { get; set; } = "[]";
        public string ExpenseDataJson { get; set; } = "[]";
        public string LabelsJson { get; set; } = "[]";

        public List<Transaction> RecentTransactions { get; set; } = new();
    }
}