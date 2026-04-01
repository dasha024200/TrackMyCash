using System;
using System.Collections.Generic;
using System.Linq;
using TrackMyCash.Models;

namespace TrackMyCash.Services
{
    public class WeekChartStrategy : IChartStrategy
    {
        public ChartData BuildChart(string? userId, List<Transaction> transactions)
        {
            var today = DateTime.UtcNow.Date;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1); // Monday
            if (startOfWeek > today) startOfWeek = startOfWeek.AddDays(-7);

            var days = Enumerable.Range(0, 7)
                .Select(offset => startOfWeek.AddDays(offset))
                .ToList();

            var labels = new[] { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Нд" };
            var incomeValues = new List<decimal>();
            var expenseValues = new List<decimal>();

            foreach (var day in days)
            {
                var dayTransactions = transactions
                    .Where(t => t.DateCreated.Date == day)
                    .ToList();

                incomeValues.Add(dayTransactions
                    .Where(t => t.Type == "Income")
                    .Sum(t => t.Amount));

                expenseValues.Add(dayTransactions
                    .Where(t => t.Type == "Expense")
                    .Sum(t => t.Amount));
            }

            return new ChartData
            {
                Balance = incomeValues.Sum() - expenseValues.Sum(),
                IncomeDataJson = $"[{string.Join(",", incomeValues)}]",
                ExpenseDataJson = $"[{string.Join(",", expenseValues)}]",
                LabelsJson = $"[{string.Join(",", labels.Select(l => $"\"{l}\""))}]"
            };
        }
    }
}