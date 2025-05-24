using LibraryDesktop.Models;

namespace LibraryDesktop.Data.Services
{
    public interface IAuthenticationService
    {
        Task<User?> LoginAsync(string username, string password);
        Task<User?> RegisterAsync(string username, string email, string password);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }

    public interface IBookService
    {
        Task<IEnumerable<Book>> GetBooksAsync();
        Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId);
        Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
        Task<Book?> GetBookDetailsAsync(int bookId);
        Task<string> GetChapterContentAsync(string githubUrl);
        Task IncrementViewCountAsync(int bookId);
    }

    public interface IPaymentService
    {
        Task<Payment> CreatePaymentAsync(int userId, decimal amount, string description = "");
        Task<string> GenerateQRCodeAsync(Payment payment);
        Task<bool> CompletePaymentAsync(string token);
        Task<IEnumerable<Payment>> GetUserPaymentsAsync(int userId);
        Task<Payment?> GetPaymentByTokenAsync(string token);
    }

    public interface IUserService
    {
        Task<User?> GetUserWithSettingsAsync(int userId);
        Task<decimal> GetUserBalanceAsync(int userId);
        Task UpdateUserSettingsAsync(int userId, ThemeMode theme, int fontSize);
        Task<bool> AddToFavoritesAsync(int userId, int bookId);
        Task<bool> RemoveFromFavoritesAsync(int userId, int bookId);
        Task<bool> IsFavoriteAsync(int userId, int bookId);
        Task<IEnumerable<UserFavorite>> GetUserFavoritesAsync(int userId);
    }

    public interface IGitHubContentService
    {
        Task<string> GetContentAsync(string url);
        bool IsValidGitHubRawUrl(string url);
        string ConvertToRawUrl(string githubUrl);
    }
}
