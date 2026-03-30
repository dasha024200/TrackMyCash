namespace TrackMyCash.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Income / Expense
    public bool IsDefault { get; set; }

    public int? UserId { get; set; }
    public User? User { get; set; }

    public List<Transaction> Transactions { get; set; } = new();
}