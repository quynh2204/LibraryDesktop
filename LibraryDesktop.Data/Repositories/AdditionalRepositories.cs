using Microsoft.EntityFrameworkCore;
using LibraryDesktop.Data.Interfaces;
using LibraryDesktop.Models;

namespace LibraryDesktop.Data.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(LibraryDbContext context) : base(context) { }

        public async Task<IEnumerable<Payment>> GetUserPaymentsAsync(int userId)
        {
            return await _dbSet
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        public async Task<Payment?> GetPaymentByTokenAsync(string token)
        {
            return await _dbSet
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PaymentToken == token);
        }

        public async Task<IEnumerable<Payment>> GetPendingPaymentsAsync()
        {
            return await _dbSet
                .Where(p => p.PaymentStatus == PaymentStatus.Pending)
                .Include(p => p.User)
                .OrderBy(p => p.CreatedDate)
                .ToListAsync();
        }        public async Task CompletePaymentAsync(string token)
        {
            var payment = await GetPaymentByTokenAsync(token);
            if (payment != null && payment.PaymentStatus == PaymentStatus.Pending)
            {
                payment.PaymentStatus = PaymentStatus.Completed;
                payment.CompletedDate = DateTime.Now;
                
                // Update user coins directly
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == payment.UserId);
                  if (user != null)
                {
                    user.Coins += (int)(payment.Amount / 1000); // Convert VND to coins (1000 VND = 1 coin)
                }

                await SaveChangesAsync();
            }
        }
    }

    public class UserFavoriteRepository : Repository<UserFavorite>, IUserFavoriteRepository
    {
        public UserFavoriteRepository(LibraryDbContext context) : base(context) { }

        public async Task<IEnumerable<UserFavorite>> GetUserFavoritesAsync(int userId)
        {
            return await _dbSet
                .Where(uf => uf.UserId == userId)
                .Include(uf => uf.Book)
                    .ThenInclude(b => b.Category)
                .OrderByDescending(uf => uf.AddedDate)
                .ToListAsync();
        }

        public async Task<bool> IsFavoriteAsync(int userId, int bookId)
        {
            return await _dbSet
                .AnyAsync(uf => uf.UserId == userId && uf.BookId == bookId);
        }

        public async Task AddFavoriteAsync(int userId, int bookId)
        {
            if (!await IsFavoriteAsync(userId, bookId))
            {
                var favorite = new UserFavorite
                {
                    UserId = userId,
                    BookId = bookId,
                    AddedDate = DateTime.Now
                };
                await AddAsync(favorite);
                await SaveChangesAsync();
            }
        }

        public async Task RemoveFavoriteAsync(int userId, int bookId)
        {
            var favorite = await _dbSet
                .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.BookId == bookId);
            
            if (favorite != null)
            {
                await DeleteAsync(favorite);
                await SaveChangesAsync();
            }
        }
    }

    public class RatingRepository : Repository<Rating>, IRatingRepository
    {
        public RatingRepository(LibraryDbContext context) : base(context) { }

        public async Task<IEnumerable<Rating>> GetBookRatingsAsync(int bookId)
        {
            return await _dbSet
                .Where(r => r.BookId == bookId)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<Rating?> GetUserRatingAsync(int userId, int bookId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => r.UserId == userId && r.BookId == bookId);
        }

        public async Task<double> GetAverageRatingAsync(int bookId)
        {
            var ratings = await _dbSet
                .Where(r => r.BookId == bookId)
                .Select(r => r.RatingValue)
                .ToListAsync();

            return ratings.Any() ? ratings.Average() : 0;
        }
    }

    public class UserSettingRepository : Repository<UserSetting>, IUserSettingRepository
    {
        public UserSettingRepository(LibraryDbContext context) : base(context) { }        public async Task<UserSetting?> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(us => us.UserId == userId);
        }
    }
}
