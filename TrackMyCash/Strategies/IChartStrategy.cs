using System.Collections.Generic;
using TrackMyCash.Models;

namespace TrackMyCash.Services
{
    public interface IChartStrategy
    {
        ChartData BuildChart(string? userId, List<Transaction> transactions);
    }
}