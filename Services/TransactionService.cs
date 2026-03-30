using TrackMyCash.Data;
using TrackMyCash.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace TrackMyCash.Services
{
    public class TransactionService
    {
        private readonly ApplicationDbContext _context;

        public TransactionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddTransaction(Transaction transaction)
        {
            // Переконуємося, що дата створення заповнена для бази даних
            if (transaction.DateCreated == default)
            {
                transaction.DateCreated = System.DateTime.Now;
            }
            
            _context.Transactions.Add(transaction);
            _context.SaveChanges(); 
        }

        public List<Transaction> GetAllTransactions()
        {
            // Запит до бази йде тільки по DateCreated, яку база знає
            var transactions = _context.Transactions
                .Include(t => t.Category)
                .OrderByDescending(t => t.DateCreated)
                .ToList();

            // Заповнюємо віртуальні поля, щоб вони відображалися в інтерфейсі
            foreach (var t in transactions)
            {
                t.CategoryName = t.Category?.Name ?? "Без категорії";
                t.Date = t.DateCreated; // Копіюємо дату для контролера
            }

            return transactions;
        }
    }
}