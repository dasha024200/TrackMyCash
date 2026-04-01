using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrackMyCash.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TrackMyCash.Services
{
    public class AuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Реєстрація користувача
        public async Task<(bool Success, string Message)> RegisterAsync(string email, string password)
        {
            // Перевіряємо, чи вже існує користувач з таким email або ім'ям
            if (await _userManager.FindByEmailAsync(email) != null || await _userManager.FindByNameAsync(email) != null)
                return (false, "Користувач з таким email вже існує");

            var user = new User 
            { 
                UserName = email, 
                Email = email,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                    return (true, "Реєстрація пройшла успішно!");

                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, string.IsNullOrEmpty(errors) ? "Не вдалося зареєструвати користувача" : errors);
            }
            catch (DbUpdateException)
            {
                return (false, "Користувач з таким email вже існує");
            }
            catch (Exception)
            {
                return (false, "Сталася несподівана помилка. Спробуйте ще раз пізніше.");
            }
        }

        // Логін користувача
        public async Task<(bool Success, string Message)> LoginAsync(string email, string password, bool rememberMe)
        {
            var result = await _signInManager.PasswordSignInAsync(
                email, 
                password, 
                rememberMe, 
                lockoutOnFailure: false);

            return result.Succeeded 
                ? (true, "Успішний вхід") 
                : (false, "Невірний email або пароль");
        }

        // Вихід
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}