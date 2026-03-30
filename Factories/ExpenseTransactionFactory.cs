using TrackMyCash.Models;

namespace TrackMyCash.Factories
{
    public class ExpenseTransactionFactory : TransactionFactory
    {
        public override Transaction CreateTransaction()
        {
            return new Transaction { Type = "Expense" };
        }
    }
}