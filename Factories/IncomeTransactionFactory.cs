using TrackMyCash.Models;

namespace TrackMyCash.Factories
{
    public class IncomeTransactionFactory : TransactionFactory
    {
        public override Transaction CreateTransaction()
        {
            return new Transaction { Type = "Income" };
        }
    }
}