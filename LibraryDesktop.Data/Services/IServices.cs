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

    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int categoryId);
        Task<Category?> GetCategoryWithBooksAsync(int categoryId);
    }public interface IPaymentService
    {
        Task<Payment> CreatePaymentAsync(int userId, int amount, string description = "");
        Task<string> GenerateQRCodeAsync(Payment payment);
        string GenerateQRCodeForPayment(string paymentToken, int amount, int? userId = null);
        Task<bool> CompletePaymentAsync(string token);
        Task<bool> CreateAndCompletePaymentAsync(int userId, int amount, string token, string description = "");
        Task<IEnumerable<Payment>> GetUserPaymentsAsync(int userId);
        Task<Payment?> GetPaymentByTokenAsync(string token);
        
        // Event to notify khi payment completed
        event EventHandler<PaymentCompletedEventArgs>? PaymentCompleted;    }
    
    // Event args cho payment completed
    public class PaymentCompletedEventArgs : EventArgs
    {
        public int UserId { get; set; }
        public int Amount { get; set; }
        public string PaymentToken { get; set; } = string.Empty;
        public DateTime CompletedAt { get; set; }
    }    
    public interface IUserService
    {
        Task<User?> GetUserWithSettingsAsync(int userId);
        Task<User?> GetUserByIdAsync(int userId);
        Task<string?> GetUsernameByIdAsync(int userId);        Task<int> GetUserCoinsAsync(int userId);
        Task UpdateUserCoinsAsync(int userId, int coins);
        Task UpdateUserSettingsAsync(int userId, ThemeMode theme, int fontSize);
        Task<bool> AddToFavoritesAsync(int userId, int bookId);
        Task<bool> RemoveFromFavoritesAsync(int userId, int bookId);
        Task<bool> IsFavoriteAsync(int userId, int bookId);
        Task<IEnumerable<UserFavorite>> GetUserFavoritesAsync(int userId);
    }    public interface IRatingService
    {
        Task<IEnumerable<Rating>> GetBookRatingsAsync(int bookId);
        Task<Rating?> GetUserRatingAsync(int userId, int bookId);
        Task<double> GetAverageRatingAsync(int bookId);
        Task<Rating> AddOrUpdateRatingAsync(int userId, int bookId, int ratingValue, string? review = null);
        Task<bool> DeleteRatingAsync(int userId, int bookId);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> UpdateAvatarUrlAsync(int userId, string avatarUrl);
        Task<User?> GetUserByUsernameAsync(string username);
    }

    public interface IGitHubContentService
    {
        Task<string> GetContentAsync(string url);
        bool IsValidGitHubRawUrl(string url);
        string ConvertToRawUrl(string githubUrl);
    }
}
