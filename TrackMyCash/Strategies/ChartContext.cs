using System.Collections.Generic;
using TrackMyCash.Models;

namespace TrackMyCash.Services
{
    public class ChartContext
    {
        private IChartStrategy _strategy = new MonthChartStrategy();

        public void SetStrategy(string period)
        {
            _strategy = period.ToLower() switch
            {
                "week" => new WeekChartStrategy(),
                "year" => new YearChartStrategy(),
                "date" => new DateChartStrategy(),
                _ => new MonthChartStrategy()
            };
        }

        public ChartData BuildChart(string? userId, List<Transaction> transactions)
        {
            return _strategy.BuildChart(userId, transactions);
        }
    }

    public class ChartData
    {
        public decimal Balance { get; set; }
        public string IncomeDataJson { get; set; } = "[]";
        public string ExpenseDataJson { get; set; } = "[]";
        public string LabelsJson { get; set; } = "[]";
    }
}