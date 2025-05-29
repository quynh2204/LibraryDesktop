using Microsoft.EntityFrameworkCore;
using LibraryDesktop.Data.Interfaces;
using LibraryDesktop.Models;

namespace LibraryDesktop.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(LibraryDbContext context) : base(context) { }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }        public async Task<User?> GetUserWithSettingsAsync(int userId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _dbSet.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<int> GetUserCoinsAsync(int userId)
        {
            var user = await _dbSet.FirstOrDefaultAsync(u => u.UserId == userId);
            return user?.Coins ?? 0;
        }

        public async Task UpdateUserCoinsAsync(int userId, int coins)
        {
            var user = await _dbSet.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null)
            {
                user.Coins += coins;
                await SaveChangesAsync();
            }
        }
    }

    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(LibraryDbContext context) : base(context) { }

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            return await _dbSet
                .Where(c => c.IsActive)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryWithBooksAsync(int categoryId)
        {
            return await _dbSet
                .Include(c => c.Books)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }
    }

    public class BookRepository : Repository<Book>, IBookRepository
    {
        public BookRepository(LibraryDbContext context) : base(context) { }

        public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .Where(b => b.CategoryId == categoryId && b.Status == BookStatus.Published)
                .Include(b => b.Category)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetPopularBooksAsync(int count = 10)
        {
            return await _dbSet
                .Where(b => b.Status == BookStatus.Published)
                .Include(b => b.Category)
                .OrderByDescending(b => b.ViewCount)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
        {
            return await _dbSet
                .Where(b => b.Status == BookStatus.Published &&
                           (b.Title.Contains(searchTerm) || 
                            b.Author.Contains(searchTerm) ||
                            (b.Description != null && b.Description.Contains(searchTerm))))
                .Include(b => b.Category)
                .OrderByDescending(b => b.ViewCount)
                .ToListAsync();
        }

        public async Task<Book?> GetBookWithChaptersAsync(int bookId)
        {
            return await _dbSet
                .Include(b => b.Chapters.OrderBy(c => c.ChapterNumber))
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.BookId == bookId);
        }

        public async Task<Book?> GetBookWithDetailsAsync(int bookId)
        {
            return await _dbSet
                .Include(b => b.Category)
                .Include(b => b.Ratings)
                .Include(b => b.Chapters.OrderBy(c => c.ChapterNumber))
                .FirstOrDefaultAsync(b => b.BookId == bookId);
        }

        public async Task IncrementViewCountAsync(int bookId)
        {
            var book = await GetByIdAsync(bookId);
            if (book != null)
            {
                book.ViewCount++;
                await UpdateAsync(book);
                await SaveChangesAsync();
            }
        }
    }

    public class ChapterRepository : Repository<Chapter>, IChapterRepository
    {
        public ChapterRepository(LibraryDbContext context) : base(context) { }

        public async Task<IEnumerable<Chapter>> GetChaptersByBookIdAsync(int bookId)
        {
            return await _dbSet
                .Where(c => c.BookId == bookId)
                .OrderBy(c => c.ChapterNumber)
                .ToListAsync();
        }

        public async Task<Chapter?> GetChapterByNumberAsync(int bookId, int chapterNumber)
        {
            return await _dbSet
                .Include(c => c.Book)
                .FirstOrDefaultAsync(c => c.BookId == bookId && c.ChapterNumber == chapterNumber);
        }

        public async Task<Chapter?> GetNextChapterAsync(int bookId, int currentChapterNumber)
        {
            return await _dbSet
                .Where(c => c.BookId == bookId && c.ChapterNumber > currentChapterNumber)
                .OrderBy(c => c.ChapterNumber)
                .FirstOrDefaultAsync();
        }

        public async Task<Chapter?> GetPreviousChapterAsync(int bookId, int currentChapterNumber)
        {
            return await _dbSet
                .Where(c => c.BookId == bookId && c.ChapterNumber < currentChapterNumber)
                .OrderByDescending(c => c.ChapterNumber)
                .FirstOrDefaultAsync();
        }
    }
}
