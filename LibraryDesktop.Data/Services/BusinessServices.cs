using LibraryDesktop.Data.Interfaces;
using LibraryDesktop.Models;

namespace LibraryDesktop.Data.Services
{    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IChapterRepository _chapterRepository;
        private readonly IGitHubContentService _gitHubContentService;

        public BookService(
            IBookRepository bookRepository, 
            IChapterRepository chapterRepository,
            IGitHubContentService gitHubContentService)
        {
            _bookRepository = bookRepository;
            _chapterRepository = chapterRepository;
            _gitHubContentService = gitHubContentService;
        }

        public async Task<IEnumerable<Book>> GetBooksAsync()
        {
            return await _bookRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId)
        {
            return await _bookRepository.GetBooksByCategoryAsync(categoryId);
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
        {
            return await _bookRepository.SearchBooksAsync(searchTerm);
        }

        public async Task<Book?> GetBookDetailsAsync(int bookId)
        {
            return await _bookRepository.GetBookWithDetailsAsync(bookId);
        }

        public async Task<string> GetChapterContentAsync(string githubUrl)
        {
            try
            {
                return await _gitHubContentService.GetContentAsync(githubUrl);
            }
            catch (Exception ex)
            {
                return $"Error loading chapter content: {ex.Message}";
            }
        }

        public async Task IncrementViewCountAsync(int bookId)
        {
            await _bookRepository.IncrementViewCountAsync(bookId);
        }
    }

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            return await _categoryRepository.GetActiveCategoriesAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            return await _categoryRepository.GetByIdAsync(categoryId);
        }

        public async Task<Category?> GetCategoryWithBooksAsync(int categoryId)
        {
            return await _categoryRepository.GetCategoryWithBooksAsync(categoryId);
        }
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserSettingRepository _userSettingRepository;
        private readonly IUserFavoriteRepository _userFavoriteRepository;

        public UserService(
            IUserRepository userRepository,
            IUserSettingRepository userSettingRepository,
            IUserFavoriteRepository userFavoriteRepository)
        {
            _userRepository = userRepository;
            _userSettingRepository = userSettingRepository;
            _userFavoriteRepository = userFavoriteRepository;
        }        public async Task<User?> GetUserWithSettingsAsync(int userId)
        {
            return await _userRepository.GetUserWithSettingsAsync(userId);
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<string?> GetUsernameByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.Username;
        }        public async Task<int> GetUserCoinsAsync(int userId)
        {
            return await _userRepository.GetUserCoinsAsync(userId);
        }

        public async Task UpdateUserCoinsAsync(int userId, int coins)
        {
            await _userRepository.UpdateUserCoinsAsync(userId, coins);
        }

        public async Task UpdateUserSettingsAsync(int userId, ThemeMode theme, int fontSize)
        {
            var userSetting = await _userSettingRepository.GetByUserIdAsync(userId);
            if (userSetting != null)
            {
                userSetting.ThemeMode = theme;
                userSetting.FontSize = fontSize;
                await _userSettingRepository.UpdateAsync(userSetting);
                await _userSettingRepository.SaveChangesAsync();
            }
        }

        public async Task<bool> AddToFavoritesAsync(int userId, int bookId)
        {
            try
            {
                await _userFavoriteRepository.AddFavoriteAsync(userId, bookId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveFromFavoritesAsync(int userId, int bookId)
        {
            try
            {
                await _userFavoriteRepository.RemoveFavoriteAsync(userId, bookId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsFavoriteAsync(int userId, int bookId)
        {
            return await _userFavoriteRepository.IsFavoriteAsync(userId, bookId);
        }

        public async Task<IEnumerable<UserFavorite>> GetUserFavoritesAsync(int userId)
        {
            return await _userFavoriteRepository.GetUserFavoritesAsync(userId);
        }
    }

    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;

        public RatingService(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        public async Task<IEnumerable<Rating>> GetBookRatingsAsync(int bookId)
        {
            return await _ratingRepository.GetBookRatingsAsync(bookId);
        }

        public async Task<Rating?> GetUserRatingAsync(int userId, int bookId)
        {
            return await _ratingRepository.GetUserRatingAsync(userId, bookId);
        }

        public async Task<double> GetAverageRatingAsync(int bookId)
        {
            return await _ratingRepository.GetAverageRatingAsync(bookId);
        }

        public async Task<Rating> AddOrUpdateRatingAsync(int userId, int bookId, int ratingValue, string? review = null)
        {
            var existingRating = await _ratingRepository.GetUserRatingAsync(userId, bookId);

            if (existingRating != null)
            {
                // Update existing rating
                existingRating.RatingValue = ratingValue;
                existingRating.Review = review;
                existingRating.UpdatedDate = DateTime.Now;
                
                await _ratingRepository.UpdateAsync(existingRating);
                await _ratingRepository.SaveChangesAsync();
                return existingRating;
            }
            else
            {
                // Create new rating
                var newRating = new Rating
                {
                    UserId = userId,
                    BookId = bookId,
                    RatingValue = ratingValue,
                    Review = review,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                await _ratingRepository.AddAsync(newRating);
                await _ratingRepository.SaveChangesAsync();
                return newRating;
            }
        }

        public async Task<bool> DeleteRatingAsync(int userId, int bookId)
        {
            try
            {
                var rating = await _ratingRepository.GetUserRatingAsync(userId, bookId);
                if (rating != null)
                {
                    await _ratingRepository.DeleteAsync(rating);
                    await _ratingRepository.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
