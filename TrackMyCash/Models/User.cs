using Microsoft.AspNetCore.Identity;

namespace TrackMyCash.Models;

public class User : IdentityUser
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Category> Categories { get; set; } = new();
    public List<Transaction> Transactions { get; set; } = new();
}