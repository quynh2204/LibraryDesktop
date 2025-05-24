using LibraryDesktop.Data.Interfaces;

namespace LibraryDesktop.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        ICategoryRepository Categories { get; }
        IBookRepository Books { get; }
        IChapterRepository Chapters { get; }
        IUserFavoriteRepository UserFavorites { get; }
        IRatingRepository Ratings { get; }
        IUserSettingRepository UserSettings { get; }
        IPaymentRepository Payments { get; }
        
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
