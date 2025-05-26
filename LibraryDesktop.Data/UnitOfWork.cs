using Microsoft.EntityFrameworkCore.Storage;
using LibraryDesktop.Data.Interfaces;
using LibraryDesktop.Data.Repositories;

namespace LibraryDesktop.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LibraryDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(LibraryDbContext context)
        {
            _context = context;
            
            Users = new UserRepository(_context);
            Categories = new CategoryRepository(_context);
            Books = new BookRepository(_context);
            Chapters = new ChapterRepository(_context);
            UserFavorites = new UserFavoriteRepository(_context);
            Ratings = new RatingRepository(_context);
            UserSettings = new UserSettingRepository(_context);
            Payments = new PaymentRepository(_context);
        }

        public IUserRepository Users { get; }
        public ICategoryRepository Categories { get; }
        public IBookRepository Books { get; }
        public IChapterRepository Chapters { get; }
        public IUserFavoriteRepository UserFavorites { get; }
        public IRatingRepository Ratings { get; }
        public IUserSettingRepository UserSettings { get; }
        public IPaymentRepository Payments { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
