using Microsoft.EntityFrameworkCore;
using TrackMyCash.Data;
using System.Threading.Tasks;

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

            var income = await _context.Transactions
                .Where(t => t.UserId == userId && t.Type == "Income")
                .SumAsync(t => t.Amount);

            var expense = await _context.Transactions
                .Where(t => t.UserId == userId && t.Type == "Expense")
                .SumAsync(t => t.Amount);

            return income - expense;
        }

        public async Task UpdateBalanceAsync(string? userId)
        {
            await Task.CompletedTask;
        }
    }
}