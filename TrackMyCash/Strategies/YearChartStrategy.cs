using System.Collections.Generic;
using System.Linq;
using TrackMyCash.Models;

namespace TrackMyCash.Services
{
    public class YearChartStrategy : IChartStrategy
    {
        public ChartData BuildChart(string? userId, List<Transaction> transactions)
        {
            return new ChartData
            {
                Balance = transactions.Sum(t => t.Type == "Income" ? t.Amount : -t.Amount),
                IncomeDataJson = "[5000, 6000, 5500]",
                ExpenseDataJson = "[3000, 4000, 3500]",
                LabelsJson = "[\"Q1\", \"Q2\", \"Q3\"]"
            };
        }
    }
}