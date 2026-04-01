using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using TrackMyCash.Data;
using TrackMyCash.Models;
using TrackMyCash.Models.ViewModels;
using TrackMyCash.Services;
using Xunit;

namespace TrackMyCash.Tests
{
    public class ServiceTests
    {
        private static ApplicationDbContext CreateInMemoryContext(string? dbName = null)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            return context;
        }

        private static Mock<UserManager<User>> CreateUserManagerMock()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(
                userStoreMock.Object,
                Mock.Of<Microsoft.Extensions.Options.IOptions<IdentityOptions>>(),
                new PasswordHasher<User>(),
                new List<IUserValidator<User>> { new UserValidator<User>() },
                new List<IPasswordValidator<User>> { new PasswordValidator<User>() },
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<Microsoft.Extensions.Logging.ILogger<UserManager<User>>>())
            { CallBase = true };

            return userManagerMock;
        }

        private static SignInManager<User> CreateSignInManagerMock(UserManager<User> userManager)
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<User>>();

            var signInManagerMock = new Mock<SignInManager<User>>(
                userManager,
                contextAccessor.Object,
                userPrincipalFactory.Object,
                Mock.Of<Microsoft.Extensions.Options.IOptions<IdentityOptions>>(),
                Mock.Of<Microsoft.Extensions.Logging.ILogger<SignInManager<User>>>(),
                Mock.Of<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>(),
                Mock.Of<IUserConfirmation<User>>())
            { CallBase = true };

            return signInManagerMock.Object;
        }

        // AuthService tests

        [Fact]
        public async Task AuthService_RegisterAsync_ReturnsFalse_WhenEmailAlreadyExists()
        {
            var userManagerMock = CreateUserManagerMock();
            userManagerMock.Setup(x => x.FindByEmailAsync("exist@example.com")).ReturnsAsync(new User { Email = "exist@example.com" });

            var signInManager = CreateSignInManagerMock(userManagerMock.Object);
            var service = new AuthService(userManagerMock.Object, signInManager);

            var result = await service.RegisterAsync("exist@example.com", "Password1!");

            Assert.False(result.Success);
            Assert.Equal("Користувач з таким email вже існує", result.Message);
        }

        [Fact]
        public async Task AuthService_RegisterAsync_Succeeds_WhenNewEmail()
        {
            var userManagerMock = CreateUserManagerMock();
            userManagerMock.Setup(x => x.FindByEmailAsync("new@example.com")).ReturnsAsync((User?)null);
            userManagerMock.Setup(x => x.FindByNameAsync("new@example.com")).ReturnsAsync((User?)null);
            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), "Password1!")).ReturnsAsync(IdentityResult.Success);

            var signInManager = CreateSignInManagerMock(userManagerMock.Object);
            var service = new AuthService(userManagerMock.Object, signInManager);

            var result = await service.RegisterAsync("new@example.com", "Password1!");

            Assert.True(result.Success);
            Assert.Equal("Реєстрація пройшла успішно!", result.Message);
        }

        [Fact]
        public async Task AuthService_LoginAsync_ReturnsSuccess_WhenPasswordSignIn成功()
        {
            var userManagerMock = CreateUserManagerMock();
            var signInManagerMock = new Mock<SignInManager<User>>(
                userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                Mock.Of<Microsoft.Extensions.Options.IOptions<IdentityOptions>>(),
                Mock.Of<Microsoft.Extensions.Logging.ILogger<SignInManager<User>>>(),
                Mock.Of<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>(),
                Mock.Of<IUserConfirmation<User>>());

            signInManagerMock.Setup(x => x.PasswordSignInAsync("login@example.com", "Password1!", false, false))
                .ReturnsAsync(SignInResult.Success);

            var service = new AuthService(userManagerMock.Object, signInManagerMock.Object);

            var result = await service.LoginAsync("login@example.com", "Password1!", false);

            Assert.True(result.Success);
            Assert.Equal("Успішний вхід", result.Message);
        }

        [Fact]
        public async Task AuthService_LoginAsync_ReturnsFailure_WhenPasswordSignInFailed()
        {
            var userManagerMock = CreateUserManagerMock();
            var signInManagerMock = new Mock<SignInManager<User>>(
                userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                Mock.Of<Microsoft.Extensions.Options.IOptions<IdentityOptions>>(),
                Mock.Of<Microsoft.Extensions.Logging.ILogger<SignInManager<User>>>(),
                Mock.Of<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>(),
                Mock.Of<IUserConfirmation<User>>());

            signInManagerMock.Setup(x => x.PasswordSignInAsync("login@example.com", "badpwd", false, false))
                .ReturnsAsync(SignInResult.Failed);

            var service = new AuthService(userManagerMock.Object, signInManagerMock.Object);

            var result = await service.LoginAsync("login@example.com", "badpwd", false);

            Assert.False(result.Success);
            Assert.Equal("Невірний email або пароль", result.Message);
        }

        [Fact]
        public async Task AuthService_LogoutAsync_CallsSignOut()
        {
            var userManagerMock = CreateUserManagerMock();
            var signInManagerMock = new Mock<SignInManager<User>>(
                userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                Mock.Of<Microsoft.Extensions.Options.IOptions<IdentityOptions>>(),
                Mock.Of<Microsoft.Extensions.Logging.ILogger<SignInManager<User>>>(),
                Mock.Of<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>(),
                Mock.Of<IUserConfirmation<User>>());

            signInManagerMock.Setup(x => x.SignOutAsync()).Returns(Task.CompletedTask).Verifiable();

            var service = new AuthService(userManagerMock.Object, signInManagerMock.Object);
            await service.LogoutAsync();

            signInManagerMock.Verify(x => x.SignOutAsync(), Times.Once);
        }

        // BalanceService tests

        [Fact]
        public async Task BalanceService_GetBalanceAsync_ReturnsZero_WhenUserIdNull()
        {
            using var context = CreateInMemoryContext();
            var service = new BalanceService(context);

            var balance = await service.GetBalanceAsync(null);

            Assert.Equal(0m, balance);
        }

        [Fact]
        public async Task BalanceService_GetBalanceAsync_CalculatesCorrectly()
        {
            using var context = CreateInMemoryContext();
            context.Transactions.AddRange(new[]
            {
                new Transaction { Amount = 100, Type = "Income", UserId = "1" },
                new Transaction { Amount = 50, Type = "Expense", UserId = "1" },
                new Transaction { Amount = 200, Type = "Income", UserId = "1" },
                new Transaction { Amount = 60, Type = "Expense", UserId = "1" }
            });
            await context.SaveChangesAsync();

            var service = new BalanceService(context);
            var balance = await service.GetBalanceAsync("1");

            Assert.Equal(190m, balance);
        }

        [Fact]
        public async Task BalanceService_UpdateBalanceAsync_DoesNotThrow()
        {
            using var context = CreateInMemoryContext();
            var service = new BalanceService(context);

            await service.UpdateBalanceAsync("1");

            Assert.True(true);
        }

        // YearChartStrategy tests

        [Fact]
        public void YearChartStrategy_BuildChart_Returns12MonthData()
        {
            var strategy = new YearChartStrategy();
            var now = DateTime.UtcNow;
            var transactions = new List<Transaction>
            {
                new Transaction { Type = "Income", Amount = 100, DateCreated = new DateTime(now.Year, 1, 1) },
                new Transaction { Type = "Expense", Amount = 50, DateCreated = new DateTime(now.Year, 1, 2) },
                new Transaction { Type = "Income", Amount = 200, DateCreated = new DateTime(now.Year, 12, 1) }
            };

            var chart = strategy.BuildChart("1", transactions);

            Assert.Equal(12, chart.IncomeDataJson.Trim('[', ']').Split(',').Length);
            Assert.Equal(12, chart.ExpenseDataJson.Trim('[', ']').Split(',').Length);
            Assert.Equal(250m, chart.Balance); // (100+200) - 50 = 250
        }

        [Fact]
        public void YearChartStrategy_BuildChart_NoTransactions_ProducesZeroValues()
        {
            var strategy = new YearChartStrategy();
            var chart = strategy.BuildChart("1", new List<Transaction>());

            Assert.Equal("[0,0,0,0,0,0,0,0,0,0,0,0]", chart.IncomeDataJson);
            Assert.Equal("[0,0,0,0,0,0,0,0,0,0,0,0]", chart.ExpenseDataJson);
            Assert.Equal(0m, chart.Balance);
        }

        // CategoryService tests

        [Fact]
        public async Task CategoryService_GetCategoriesAsync_ReturnsEmpty_WhenUserIdNull()
        {
            using var context = CreateInMemoryContext();
            var service = new CategoryService(context);

            var categories = await service.GetCategoriesAsync(null);

            Assert.Empty(categories);
        }

        [Fact]
        public async Task CategoryService_GetCategoriesAsync_IncludesDefaultAndUserCategories()
        {
            using var context = CreateInMemoryContext();
            context.Categories.AddRange(
                new Category { Name = "Default", Type = "Income", UserId = null, IsDefault = true },
                new Category { Name = "User", Type = "Expense", UserId = "user1", IsDefault = false });
            await context.SaveChangesAsync();

            var service = new CategoryService(context);
            var categories = await service.GetCategoriesAsync("user1");

            Assert.Contains(categories, c => c.Name == "Default");
            Assert.Contains(categories, c => c.Name == "User");
        }

        [Fact]
        public async Task CategoryService_CreateCategoryAsync_ReturnsFailure_WhenUserIdNull()
        {
            using var context = CreateInMemoryContext();
            var service = new CategoryService(context);

            var result = await service.CreateCategoryAsync(new CategoryViewModel { Name = "New", Type = "Income" }, null);

            Assert.False(result.Success);
            Assert.Equal("Користувач не автентифікований", result.Message);
        }

        [Fact]
        public async Task CategoryService_CreateCategoryAsync_Success()
        {
            using var context = CreateInMemoryContext();
            var service = new CategoryService(context);

            var result = await service.CreateCategoryAsync(new CategoryViewModel { Name = "New", Type = "Income" }, "user1");

            var saved = await context.Categories.SingleAsync();
            Assert.True(result.Success);
            Assert.Equal("Категорію успішно створено", result.Message);
            Assert.Equal("New", saved.Name);
        }

        [Fact]
        public async Task CategoryService_GetCategoryByIdAsync_ReturnsNull_WhenUserIdNull()
        {
            using var context = CreateInMemoryContext();
            var service = new CategoryService(context);

            var category = await service.GetCategoryByIdAsync(1, null);
            Assert.Null(category);
        }

        [Fact]
        public async Task CategoryService_GetCategoryByIdAsync_ReturnsCategory_WhenFound()
        {
            using var context = CreateInMemoryContext();
            var inserted = new Category { Name = "Test", Type = "Income", UserId = "user1", IsDefault = false };
            context.Categories.Add(inserted);
            await context.SaveChangesAsync();

            var service = new CategoryService(context);
            var category = await service.GetCategoryByIdAsync(inserted.Id, "user1");

            Assert.NotNull(category);
            Assert.Equal("Test", category.Name);
        }

        [Fact]
        public async Task CategoryService_UpdateCategoryAsync_ReturnsFailure_WhenNotFound()
        {
            using var context = CreateInMemoryContext();
            var service = new CategoryService(context);

            var result = await service.UpdateCategoryAsync(new CategoryViewModel { Id = 999, Name = "X", Type = "Income" }, "user1");

            Assert.False(result.Success);
            Assert.Equal("Категорію не знайдено", result.Message);
        }

        [Fact]
        public async Task CategoryService_UpdateCategoryAsync_Succeeds()
        {
            using var context = CreateInMemoryContext();
            var category = new Category { Name = "Old", Type = "Income", UserId = "user1", IsDefault = false };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var service = new CategoryService(context);
            var result = await service.UpdateCategoryAsync(new CategoryViewModel { Id = category.Id, Name = "New", Type = "Expense" }, "user1");

            Assert.True(result.Success);
            var updated = await context.Categories.FindAsync(category.Id);
            Assert.NotNull(updated);
            Assert.Equal("New", updated!.Name);
            Assert.Equal("Expense", updated.Type);
        }

        [Fact]
        public async Task CategoryService_DeleteCategoryAsync_ReturnsFailure_WhenNotFound()
        {
            using var context = CreateInMemoryContext();
            var service = new CategoryService(context);

            var result = await service.DeleteCategoryAsync(999, "user1");

            Assert.False(result.Success);
            Assert.Equal("Категорію не знайдено", result.Message);
        }

        [Fact]
        public async Task CategoryService_DeleteCategoryAsync_ReturnsFailure_WhenDefaultCategory()
        {
            using var context = CreateInMemoryContext();
            var category = new Category { Name = "Default", Type = "Income", UserId = null, IsDefault = true };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var service = new CategoryService(context);
            var result = await service.DeleteCategoryAsync(category.Id, "user1");

            Assert.False(result.Success);
            Assert.Equal("Неможливо видалити стандартну категорію", result.Message);
        }

        [Fact]
        public async Task CategoryService_DeleteCategoryAsync_ReturnsFailure_WhenTransactionsExist()
        {
            using var context = CreateInMemoryContext();
            var category = new Category { Name = "UserCat", Type = "Income", UserId = "user1", IsDefault = false };
            context.Categories.Add(category);
            await context.SaveChangesAsync();
            context.Transactions.Add(new Transaction { CategoryId = category.Id, UserId = "user1", Type = "Income", Amount = 10m, DateCreated = DateTime.UtcNow });
            await context.SaveChangesAsync();

            var service = new CategoryService(context);
            var result = await service.DeleteCategoryAsync(category.Id, "user1");

            Assert.False(result.Success);
            Assert.Equal("Неможливо видалити категорію з транзакціями", result.Message);
        }

        [Fact]
        public async Task CategoryService_DeleteCategoryAsync_Succeeds_WhenNoTransactions()
        {
            using var context = CreateInMemoryContext();
            var category = new Category { Name = "UserCat", Type = "Income", UserId = "user1", IsDefault = false };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var service = new CategoryService(context);
            var result = await service.DeleteCategoryAsync(category.Id, "user1");

            Assert.True(result.Success);
            Assert.Empty(await context.Categories.Where(c => c.Id == category.Id).ToListAsync());
        }

        // TransactionService tests

        [Fact]
        public async Task TransactionService_CreateTransactionAsync_ReturnsFailure_WhenUserUnAuthenticated()
        {
            using var context = CreateInMemoryContext();
            var balanceService = new BalanceService(context);
            var service = new TransactionService(context, balanceService);

            var result = await service.CreateTransactionAsync(new TransactionViewModel(), null);

            Assert.False(result.Success);
            Assert.Equal("Користувач не автентифікований", result.Message);
        }

        [Fact]
        public async Task TransactionService_CreateTransactionAsync_Succeeds_AndUpdatesBalance()
        {
            using var context = CreateInMemoryContext();
            var balanceService = new BalanceService(context);

            var service = new TransactionService(context, balanceService);
            var model = new TransactionViewModel { Amount = 100m, Type = "Income", Comment = "test", DateCreated = DateTime.UtcNow, CategoryId = 1 };
            var result = await service.CreateTransactionAsync(model, "user1");

            Assert.True(result.Success);
            Assert.Equal("Транзакцію успішно додано", result.Message);
            Assert.Single(context.Transactions);
        }

        [Fact]
        public async Task TransactionService_GetTransactionByIdAsync_ReturnsNull_OnInvalidUser()
        {
            using var context = CreateInMemoryContext();
            var service = new TransactionService(context, new BalanceService(context));

            var result = await service.GetTransactionByIdAsync(1, null);
            Assert.Null(result);
        }

        [Fact]
        public async Task TransactionService_GetTransactionByIdAsync_ReturnsTran_WhenFound()
        {
            using var context = CreateInMemoryContext();
            context.Transactions.Add(new Transaction { Id = 1, Amount = 20, Type = "Expense", UserId = "user1", DateCreated = DateTime.UtcNow });
            await context.SaveChangesAsync();

            var service = new TransactionService(context, new BalanceService(context));
            var tran = await service.GetTransactionByIdAsync(1, "user1");

            Assert.NotNull(tran);
            Assert.Equal(20, tran.Amount);
        }

        [Fact]
        public async Task TransactionService_UpdateTransactionAsync_ReturnsFailure_WhenNotFound()
        {
            using var context = CreateInMemoryContext();
            var service = new TransactionService(context, new BalanceService(context));
            var result = await service.UpdateTransactionAsync(999, new TransactionViewModel(), "user1");

            Assert.False(result.Success);
            Assert.Equal("Транзакцію не знайдено", result.Message);
        }

        [Fact]
        public async Task TransactionService_UpdateTransactionAsync_Succeeds()
        {
            using var context = CreateInMemoryContext();
            var entity = new Transaction { Id = 1, Amount = 10, Type = "Income", UserId = "user1", DateCreated = DateTime.UtcNow, CategoryId = 1 };
            context.Transactions.Add(entity);
            await context.SaveChangesAsync();
            var balanceService = new BalanceService(context);
            var service = new TransactionService(context, balanceService);
            var result = await service.UpdateTransactionAsync(1, new TransactionViewModel { Amount = 15, Type = "Expense", DateCreated = DateTime.UtcNow, CategoryId = 1 }, "user1");

            Assert.True(result.Success);
            Assert.Equal("Транзакцію успішно оновлено", result.Message);
            var updatedTransaction = await context.Transactions.FindAsync(1);
            Assert.NotNull(updatedTransaction);
            Assert.Equal(15, updatedTransaction!.Amount);
        }

        [Fact]
        public async Task TransactionService_DeleteTransactionAsync_ReturnsFailure_WhenNotFound()
        {
            using var context = CreateInMemoryContext();
            var service = new TransactionService(context, new BalanceService(context));
            var result = await service.DeleteTransactionAsync(999, "user1");

            Assert.False(result.Success);
            Assert.Equal("Транзакцію не знайдено", result.Message);
        }

        [Fact]
        public async Task TransactionService_DeleteTransactionAsync_Succeeds()
        {
            using var context = CreateInMemoryContext();
            var entity = new Transaction { Id = 1, Amount = 20, Type = "Income", UserId = "user1", DateCreated = DateTime.UtcNow };
            context.Transactions.Add(entity);
            await context.SaveChangesAsync();

            var balanceService = new BalanceService(context);
            var service = new TransactionService(context, balanceService);

            var result = await service.DeleteTransactionAsync(1, "user1");

            Assert.True(result.Success);
            Assert.Empty(context.Transactions);
        }

        [Fact]
        public async Task TransactionService_GetRecentTransactionsAsync_ReturnsEmpty_WhenUserNull()
        {
            using var context = CreateInMemoryContext();
            var service = new TransactionService(context, new BalanceService(context));

            var result = await service.GetRecentTransactionsAsync(null);
            Assert.Empty(result);
        }

        [Fact]
        public async Task TransactionService_GetRecentTransactionsAsync_ReturnsSortedTransactions()
        {
            using var context = CreateInMemoryContext();
            context.Transactions.AddRange(
                new Transaction { Amount = 10, Type = "Income", UserId = "u1", DateCreated = DateTime.UtcNow.AddDays(-1) },
                new Transaction { Amount = 20, Type = "Income", UserId = "u1", DateCreated = DateTime.UtcNow });
            await context.SaveChangesAsync();

            var service = new TransactionService(context, new BalanceService(context));
            Assert.Equal(2, await context.Transactions.CountAsync());
            var directList = await context.Transactions.Where(t => t.UserId == "u1").ToListAsync();
            Assert.Equal(2, directList.Count);
            var list = await service.GetRecentTransactionsAsync("u1", 2);

            Assert.Equal(2, list.Count);
            Assert.True(list[0].DateCreated >= list[1].DateCreated);
        }

        [Fact]
        public async Task TransactionService_GetTransactionsAsync_ReturnsEmpty_WhenUserNull()
        {
            using var context = CreateInMemoryContext();
            var service = new TransactionService(context, new BalanceService(context));

            var result = await service.GetTransactionsAsync(null);
            Assert.Empty(result);
        }
    }
}
