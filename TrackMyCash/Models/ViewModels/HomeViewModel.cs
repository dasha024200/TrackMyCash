using TrackMyCash.Models;

namespace TrackMyCash.Models.ViewModels
{
    public class HomeViewModel
    {
        public decimal Balance { get; set; } = 0;
        public List<Transaction> RecentTransactions { get; set; } = new List<Transaction>();

        // Для форми швидкої транзакції
        public string Type { get; set; } = "Income";
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public string? Comment { get; set; }
    }
}