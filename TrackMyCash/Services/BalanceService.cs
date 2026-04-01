using Microsoft.EntityFrameworkCore;
using TrackMyCash.Data;
using System.Threading.Tasks;
using System.Linq;

namespace TrackMyCash.Services
{
    public class BalanceService
    {
        private readonly ApplicationDbContext _context;

        public BalanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetBalanceAsync(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return 0m;

            // Використовуємо client-side Sum для decimal у SQLite
            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId)
                .ToListAsync();

            decimal income = transactions
                .Where(t => t.Type == "Income")
                .Sum(t => t.Amount);

            decimal expense = transactions
                .Where(t => t.Type == "Expense")
                .Sum(t => t.Amount);

            return income - expense;
        }

        public async Task UpdateBalanceAsync(string? userId)
        {
            // Observer поки що порожній, бо баланс рахуємо динамічно
            await Task.CompletedTask;
        }
    }
}