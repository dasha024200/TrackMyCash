using Microsoft.EntityFrameworkCore;
using TrackMyCash.Data;
using TrackMyCash.Models;
using TrackMyCash.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrackMyCash.Services
{
    public class TransactionService
    {
        private readonly ApplicationDbContext _context;
        private readonly BalanceService _balanceService;

        public TransactionService(ApplicationDbContext context, BalanceService balanceService)
        {
            _context = context;
            _balanceService = balanceService;
        }

        public async Task<(bool Success, string Message)> CreateTransactionAsync(TransactionViewModel model, string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return (false, "Користувач не автентифікований");

            var transaction = new Transaction
            {
                Amount = model.Amount,
                Type = model.Type,
                Comment = model.Comment,
                DateCreated = model.DateCreated,
                UserId = userId,
                CategoryId = model.CategoryId
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            await _balanceService.UpdateBalanceAsync(userId);

            return (true, "Транзакцію успішно додано");
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int id, string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            return await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }

        public async Task<(bool Success, string Message)> UpdateTransactionAsync(int id, TransactionViewModel model, string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return (false, "Користувач не автентифікований");

            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (transaction == null)
                return (false, "Транзакцію не знайдено");

            transaction.Amount = model.Amount;
            transaction.Type = model.Type;
            transaction.Comment = model.Comment;
            transaction.DateCreated = model.DateCreated;
            transaction.CategoryId = model.CategoryId;

            await _context.SaveChangesAsync();
            await _balanceService.UpdateBalanceAsync(userId);

            return (true, "Транзакцію успішно оновлено");
        }

        public async Task<(bool Success, string Message)> DeleteTransactionAsync(int id, string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return (false, "Користувач не автентифікований");

            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (transaction == null)
                return (false, "Транзакцію не знайдено");

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            await _balanceService.UpdateBalanceAsync(userId);

            return (true, "Транзакцію успішно видалено");
        }

        public async Task<List<Transaction>> GetRecentTransactionsAsync(string? userId, int count = 10)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<Transaction>();

            return await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.DateCreated)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Transaction>> GetTransactionsAsync(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<Transaction>();

            return await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.DateCreated)
                .ToListAsync();
        }
    }
}