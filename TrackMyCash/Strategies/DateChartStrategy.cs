using System.Collections.Generic;
using System.Linq;
using TrackMyCash.Models;

namespace TrackMyCash.Services
{
    public class DateChartStrategy : IChartStrategy
    {
        public ChartData BuildChart(string? userId, List<Transaction> transactions)
        {
            return new ChartData
            {
                Balance = transactions.Sum(t => t.Type == "Income" ? t.Amount : -t.Amount),
                IncomeDataJson = "[100]",
                ExpenseDataJson = "[80]",
                LabelsJson = "[\"Сьогодні\"]"
            };
        }
    }
}