using LibraryDesktop.Models;
using LibraryDesktop.Data.Services;
using LibraryDesktop.Data.Interfaces;

namespace LibraryDesktop.Data.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly IHistoryRepository _historyRepository;

        public HistoryService(IHistoryRepository historyRepository)
        {
            _historyRepository = historyRepository;
        }

        public async Task<IEnumerable<History>> GetUserHistoryAsync(int userId)
        {
            return await _historyRepository.GetUserHistoryAsync(userId);
        }

        public async Task<History> AddHistoryAsync(int userId, int bookId, int? chapterId = null, string accessType = "View")
        {
            var history = new History
            {
                UserId = userId,
                BookId = bookId,
                ChapterId = chapterId,
                AccessType = accessType,
                AccessedDate = DateTime.Now
            };

            await _historyRepository.AddAsync(history);
            await _historyRepository.SaveChangesAsync();
            return history;
        }

        public async Task<IEnumerable<History>> GetBookHistoryAsync(int bookId)
        {
            return await _historyRepository.GetBookHistoryAsync(bookId);
        }        public async Task<bool> DeleteHistoryAsync(int historyId)
        {
            var history = await _historyRepository.GetByIdAsync(historyId);
            if (history == null) return false;

            await _historyRepository.DeleteAsync(history);
            await _historyRepository.SaveChangesAsync();
            return true;
        }

        public async Task ClearUserHistoryAsync(int userId)
        {
            await _historyRepository.ClearUserHistoryAsync(userId);
        }
    }
}
