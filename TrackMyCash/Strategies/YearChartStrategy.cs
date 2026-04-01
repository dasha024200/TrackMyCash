using System;
using System.Collections.Generic;
using System.Linq;
using TrackMyCash.Models;

namespace TrackMyCash.Services
{
    public class YearChartStrategy : IChartStrategy
    {
        public ChartData BuildChart(string? userId, List<Transaction> transactions)
        {
            var currentYear = DateTime.UtcNow.Year;
            var months = new[] { "Січ", "Лют", "Бер", "Кві", "Тра", "Чер", "Лип", "Сер", "Вер", "Жов", "Лис", "Гру" };
            var incomeValues = new List<decimal>();
            var expenseValues = new List<decimal>();

            for (int month = 1; month <= 12; month++)
            {
                var monthTransactions = transactions
                    .Where(t => t.DateCreated.Year == currentYear && t.DateCreated.Month == month)
                    .ToList();

                incomeValues.Add(monthTransactions.Where(t => t.Type == "Income").Sum(t => t.Amount));
                expenseValues.Add(monthTransactions.Where(t => t.Type == "Expense").Sum(t => t.Amount));
            }

            return new ChartData
            {
                Balance = incomeValues.Sum() - expenseValues.Sum(),
                IncomeDataJson = $"[{string.Join(",", incomeValues)}]",
                ExpenseDataJson = $"[{string.Join(",", expenseValues)}]",
                LabelsJson = $"[{string.Join(",", months.Select(m => $"\"{m}\""))}]"
            };
        }
    }
}