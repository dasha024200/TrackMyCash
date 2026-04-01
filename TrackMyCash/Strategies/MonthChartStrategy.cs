using System;
using System.Collections.Generic;
using System.Linq;
using TrackMyCash.Models;

namespace TrackMyCash.Services
{
    public class MonthChartStrategy : IChartStrategy
    {
        public ChartData BuildChart(string? userId, List<Transaction> transactions)
        {
            var now = DateTime.UtcNow;
            var months = Enumerable.Range(0, 6)
                .Select(i => new DateTime(now.Year, now.Month, 1).AddMonths(-5 + i))
                .ToList();

            var labels = months.Select(m => m.ToString("MMMM", new System.Globalization.CultureInfo("uk-UA"))).ToList();
            var incomeValues = new List<decimal>();
            var expenseValues = new List<decimal>();

            foreach (var month in months)
            {
                var monthTransactions = transactions
                    .Where(t => t.DateCreated.Year == month.Year && t.DateCreated.Month == month.Month)
                    .ToList();

                incomeValues.Add(monthTransactions
                    .Where(t => t.Type == "Income")
                    .Sum(t => t.Amount));

                expenseValues.Add(monthTransactions
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