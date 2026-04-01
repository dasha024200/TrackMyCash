using TrackMyCash.Models;

namespace TrackMyCash.Factories
{
    public class TransactionFactory
    {
        public Transaction Create(string type, decimal amount, int categoryId, string? userId, string? comment)
        {
            return new Transaction
            {
                Type = type,
                Amount = amount,
                CategoryId = categoryId,
                UserId = userId ?? string.Empty,
                Comment = comment,
                DateCreated = DateTime.UtcNow
            };
        }
    }
}