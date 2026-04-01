using Microsoft.EntityFrameworkCore;
using TrackMyCash.Data;
using TrackMyCash.Models;

namespace TrackMyCash.Services;

public class AuthService
{
    private readonly ApplicationDbContext _context;

    public AuthService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> RegisterAsync(string email, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Email == email))
            return false;

        var user = new User
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            return null;

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

        return isPasswordValid ? user : null;
    }
}