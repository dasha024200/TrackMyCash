using System.Collections.Generic;
using System.Linq;
using TrackMyCash.Models;

namespace TrackMyCash.Services
{
    public class MonthChartStrategy : IChartStrategy
    {
        public ChartData BuildChart(string? userId, List<Transaction> transactions)
        {
            return new ChartData
            {
                Balance = transactions.Sum(t => t.Type == "Income" ? t.Amount : -t.Amount),
                IncomeDataJson = "[1200, 800, 1500]",
                ExpenseDataJson = "[500, 900, 700]",
                LabelsJson = "[\"Січень\", \"Лютий\", \"Березень\"]"
            };
        }
    }
}