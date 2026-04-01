using System;
using System.Collections.Generic;
using System.Linq;
using TrackMyCash.Models;

namespace TrackMyCash.Services
{
    public class DateChartStrategy : IChartStrategy
    {
        public ChartData BuildChart(string? userId, List<Transaction> transactions)
        {
            var today = DateTime.UtcNow.Date;
            var transactionsToday = transactions
                .Where(t => t.DateCreated.Date == today)
                .ToList();

            var income = transactionsToday
                .Where(t => t.Type == "Income")
                .Sum(t => t.Amount);

            var expense = transactionsToday
                .Where(t => t.Type == "Expense")
                .Sum(t => t.Amount);

            var balance = income - expense;

            return new ChartData
            {
                Balance = balance,
                IncomeDataJson = $"[{income}]",
                ExpenseDataJson = $"[{expense}]",
                LabelsJson = $"[\"{today:dd.MM.yyyy}\"]"
            };
        }
    }
}