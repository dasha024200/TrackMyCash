namespace TrackMyCash.Models;

public class ExpenseTransaction : Transaction
{
    public ExpenseTransaction()
    {
        Type = "Expense";
    }
}