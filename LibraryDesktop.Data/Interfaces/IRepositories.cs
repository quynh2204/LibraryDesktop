using LibraryDesktop.Models;

namespace LibraryDesktop.Data.Interfaces
{    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetUserWithSettingsAsync(int userId);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<int> GetUserCoinsAsync(int userId);
        Task UpdateUserCoinsAsync(int userId, int coins);
        Task AddUserCoinsAsync(int userId, int coinsToAdd);
    }

    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
        Task<Category?> GetCategoryWithBooksAsync(int categoryId);
    }

    public interface IBookRepository : IRepository<Book>
    {
        Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId);
        Task<IEnumerable<Book>> GetPopularBooksAsync(int count = 10);
        Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
        Task<Book?> GetBookWithChaptersAsync(int bookId);
        Task<Book?> GetBookWithDetailsAsync(int bookId);
        Task IncrementViewCountAsync(int bookId);
    }

    public interface IChapterRepository : IRepository<Chapter>
    {
        Task<IEnumerable<Chapter>> GetChaptersByBookIdAsync(int bookId);
        Task<Chapter?> GetChapterByNumberAsync(int bookId, int chapterNumber);
        Task<Chapter?> GetNextChapterAsync(int bookId, int currentChapterNumber);
        Task<Chapter?> GetPreviousChapterAsync(int bookId, int currentChapterNumber);
    }

    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<IEnumerable<Payment>> GetUserPaymentsAsync(int userId);
        Task<Payment?> GetPaymentByTokenAsync(string token);
        Task<IEnumerable<Payment>> GetPendingPaymentsAsync();
        Task CompletePaymentAsync(string token);
    }

    public interface IUserFavoriteRepository : IRepository<UserFavorite>
    {
        Task<IEnumerable<UserFavorite>> GetUserFavoritesAsync(int userId);
        Task<bool> IsFavoriteAsync(int userId, int bookId);
        Task AddFavoriteAsync(int userId, int bookId);
        Task RemoveFavoriteAsync(int userId, int bookId);
    }

    public interface IRatingRepository : IRepository<Rating>
    {
        Task<IEnumerable<Rating>> GetBookRatingsAsync(int bookId);
        Task<Rating?> GetUserRatingAsync(int userId, int bookId);
        Task<double> GetAverageRatingAsync(int bookId);
    }    public interface IUserSettingRepository : IRepository<UserSetting>
    {
        Task<UserSetting?> GetByUserIdAsync(int userId);
    }

    public interface IHistoryRepository : IRepository<History>
    {
        Task<IEnumerable<History>> GetUserHistoryAsync(int userId);
        Task<IEnumerable<History>> GetBookHistoryAsync(int bookId);
        Task ClearUserHistoryAsync(int userId);
    }
}
