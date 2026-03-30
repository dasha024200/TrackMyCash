using TrackMyCash.Models;

namespace TrackMyCash.Factories
{
    public abstract class TransactionFactory
    {
        public abstract Transaction CreateTransaction();
    }
}