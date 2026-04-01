using TrackMyCash.Models;

namespace TrackMyCash.Models.ViewModels
{
    public class HomeViewModel
    {
        public decimal Balance { get; set; }
        public List<Transaction> RecentTransactions { get; set; } = new();
        public string? Type { get; set; }
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public string? Comment { get; set; }
    }
}