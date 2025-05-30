using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryDesktop.Data.Interfaces;
using LibraryDesktop.Data.Services;
using LibraryDesktop.Models;

namespace LibraryDesktop.View
{    public partial class Dashboard : UserControl
    {
        private readonly IBookService? _bookService;
        private readonly IRatingService? _ratingService;
        private readonly IHistoryService? _historyService;
        private readonly ICategoryService? _categoryService;

        // Parameterless constructor for designer compatibility
        public Dashboard()
        {
            InitializeComponent();
        }

        public Dashboard(IBookService bookService, IRatingService ratingService, 
                        IHistoryService historyService, ICategoryService categoryService)
        {
            _bookService = bookService;
            _ratingService = ratingService;
            _historyService = historyService;
            _categoryService = categoryService;
            InitializeComponent();
        }private async void Dashboard_Load(object sender, EventArgs e)
        {
            SetupDataGridColumns();

            cmbTimePeriod.SelectedIndex = 2; // Default: "7 ngày qua"
            await LoadInitialDataAsync();
        }

        #region Setup Methods
        private void SetupDataGridColumns()
        {
            // Setup Top Books DataGrid columns
            dgvTopBooks.Columns.Clear();
            dgvTopBooks.Columns.Add("Rank", "STT");
            dgvTopBooks.Columns.Add("Title", "Tên Sách");
            dgvTopBooks.Columns.Add("Category", "Thể Loại");
            dgvTopBooks.Columns.Add("Author", "Tác Giả");
            dgvTopBooks.Columns.Add("Views", "Lượt Xem");
            //dgvTopBooks.Columns.Add("LastViewed", "Lần Cuối");

            // Set column widths for Top Books
            dgvTopBooks.Columns["Rank"].Width = 50;
            dgvTopBooks.Columns["Title"].Width = 200;
            dgvTopBooks.Columns["Category"].Width = 120;
            dgvTopBooks.Columns["Author"].Width = 120;
            dgvTopBooks.Columns["Views"].Width = 80;
            //dgvTopBooks.Columns["LastViewed"].Width = 110;

            // Setup Category Stats DataGrid columns
            dgvCategoryStats.Columns.Clear();
            dgvCategoryStats.Columns.Add("Rank", "STT");
            dgvCategoryStats.Columns.Add("CategoryName", "Thể Loại");
            dgvCategoryStats.Columns.Add("TotalViews", "Tổng Xem");
            dgvCategoryStats.Columns.Add("Percentage", "Tỷ Lệ %");
            dgvCategoryStats.Columns.Add("BookCount", "Số Sách");
            dgvCategoryStats.Columns.Add("AvgViews", "TB/Sách");

            // Set column widths for Category Stats
            dgvCategoryStats.Columns["Rank"].Width = 50;
            dgvCategoryStats.Columns["CategoryName"].Width = 150;
            dgvCategoryStats.Columns["TotalViews"].Width = 100;
            dgvCategoryStats.Columns["Percentage"].Width = 80;
            dgvCategoryStats.Columns["BookCount"].Width = 80;
            dgvCategoryStats.Columns["AvgViews"].Width = 80;        }
        #endregion

        #region Event Handlers
        private async void CmbTimePeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            await LoadDashboardDataAsync();
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            await LoadDashboardDataAsync();
            MessageBox.Show("Dữ liệu đã được làm mới!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng xuất Excel sẽ được implement sau!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TimeTimer_Tick(object sender, EventArgs e)
        {
            lblCurrentTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
        #endregion

        #region Data Loading
        private async Task LoadInitialDataAsync()
        {
            await LoadDashboardDataAsync();
        }        private async Task LoadDashboardDataAsync()
        {
            try
            {
                // Check if services are available (not null)
                if (_bookService == null || _ratingService == null || 
                    _historyService == null || _categoryService == null)
                {
                    // Load sample data if services are not available
                    LoadSampleData();
                    return;
                }
                
                // Load real data from database
                await LoadTopBooksAsync();
                await LoadCategoryStatsAsync();
                UpdateTimeLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu Dashboard: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Don't fallback to sample data if we have services - show the error instead
                if (_bookService == null || _ratingService == null || 
                    _historyService == null || _categoryService == null)
                {
                    LoadSampleData();
                }
            }
        }        private async Task LoadTopBooksAsync()
        {
            dgvTopBooks.Rows.Clear();
            
            try
            {
                // Check if services are available
                if (_bookService == null || _ratingService == null || _historyService == null)
                {
                    return;
                }
                
                // Get all books with their categories (fresh data from database)
                var books = await _bookService.GetBooksAsync();
                var bookStats = new List<(Book book, int totalViews, DateTime lastViewed)>();
                
                foreach (var book in books)
                {
                    // Use ViewCount as the primary metric for ranking (real-time data)
                    int totalViews = book.ViewCount;
                    
                    // Get latest view from history (defaulting to book creation date if no history)
                    var bookHistory = await _historyService.GetBookHistoryAsync(book.BookId);
                    var lastViewed = bookHistory.Any() ? 
                        bookHistory.Max(h => h.AccessedDate) : 
                        book.CreatedDate;
                      bookStats.Add((book, totalViews, lastViewed));
                    
                    System.Diagnostics.Debug.WriteLine($"📊 Book '{book.Title}' has {totalViews} views");
                }
                
                // Sort by view count and take top 10
                var topBooks = bookStats
                    .OrderByDescending(bs => bs.totalViews)
                    .Take(10)
                    .ToList();
                  // Populate DataGrid
                for (int i = 0; i < topBooks.Count; i++)
                {
                    var (book, views, lastViewed) = topBooks[i];
                    var category = book.Category?.CategoryName ?? "N/A";
                    
                    dgvTopBooks.Rows.Add(
                        (i + 1).ToString(),
                        book.Title,
                        category,
                        book.Author,
                        views.ToString("#,##0")
                        //lastViewed.ToString("dd/MM/yyyy HH:mm")
                    );
                }
                
                // Update Top 1 Book panel
                if (topBooks.Count > 0)
                {
                    var topBook = topBooks[0];
                    lblValuebook.Text = $"{topBook.book.Title} ({topBook.totalViews:N0} views)";
                }
                else
                {
                    lblValuebook.Text = "No data available";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải top books: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }        private async Task LoadCategoryStatsAsync()
        {
            dgvCategoryStats.Rows.Clear();
            
            try
            {
                // Check if services are available
                if (_bookService == null || _ratingService == null || _categoryService == null)
                {
                    return;
                }
                  // Get all categories and books (fresh data from database)
                var categories = await _categoryService.GetCategoriesAsync();
                var books = await _bookService.GetBooksAsync();
                
                var categoryStats = new List<(Category category, int totalViews, int bookCount, double avgViews)>();
                
                foreach (var category in categories)
                {
                    // Get books in this category
                    var categoryBooks = books.Where(b => b.CategoryId == category.CategoryId).ToList();
                    
                    if (categoryBooks.Count == 0) continue;
                    
                    // Calculate total views for this category using ViewCount (real-time data)
                    int totalViews = categoryBooks.Sum(book => book.ViewCount);
                    
                    double avgViews = categoryBooks.Count > 0 ? (double)totalViews / categoryBooks.Count : 0;
                    
                    categoryStats.Add((category, totalViews, categoryBooks.Count, avgViews));
                    
                    System.Diagnostics.Debug.WriteLine($"📊 Category '{category.CategoryName}' has {totalViews} total views across {categoryBooks.Count} books");
                }
                
                // Calculate total views across all categories for percentage
                var grandTotal = categoryStats.Sum(cs => cs.totalViews);
                
                // Sort by total views and take top 10
                var topCategories = categoryStats
                    .OrderByDescending(cs => cs.totalViews)
                    .Take(10)
                    .ToList();
                  // Populate DataGrid
                for (int i = 0; i < topCategories.Count; i++)
                {
                    var (category, totalViews, bookCount, avgViews) = topCategories[i];
                    var percentage = grandTotal > 0 ? (double)totalViews / grandTotal * 100 : 0;
                    
                    dgvCategoryStats.Rows.Add(
                        (i + 1).ToString(),
                        category.CategoryName,
                        totalViews.ToString("#,##0"),
                        percentage.ToString("F1") + "%",
                        bookCount.ToString(),
                        avgViews.ToString("F1")
                    );
                }
                
                // Update Top 1 Category panel
                if (topCategories.Count > 0)
                {
                    var topCategory = topCategories[0];
                    var percentage = grandTotal > 0 ? (double)topCategory.totalViews / grandTotal * 100 : 0;
                    lblValuecate.Text = $"{topCategory.category.CategoryName} ({percentage:F1}%)";
                }
                else
                {
                    lblValuecate.Text = "No data available";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thống kê thể loại: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateTimeLabel()
        {
            lblCurrentTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private void LoadSampleData()
        {
            // Load sample data for Top Books table
            dgvTopBooks.Rows.Clear();
            dgvTopBooks.Rows.Add("1", "Đắc Nhân Tâm", "Tâm lý", "Dale Carnegie", "1,250", "26/05/2025 18:30");
            dgvTopBooks.Rows.Add("2", "Sherlock Holmes", "Trinh thám", "Arthur Conan Doyle", "980", "26/05/2025 17:45");
            dgvTopBooks.Rows.Add("3", "Harry Potter", "Phiêu lưu", "J.K. Rowling", "875", "26/05/2025 16:20");
            dgvTopBooks.Rows.Add("4", "1984", "Tiểu thuyết", "George Orwell", "756", "26/05/2025 15:10");
            dgvTopBooks.Rows.Add("5", "The Shining", "Kinh dị", "Stephen King", "698", "26/05/2025 14:55");
            dgvTopBooks.Rows.Add("6", "To Kill a Mockingbird", "Tiểu thuyết", "Harper Lee", "645", "26/05/2025 14:30");
            dgvTopBooks.Rows.Add("7", "The Great Gatsby", "Tiểu thuyết", "F. Scott Fitzgerald", "598", "26/05/2025 13:45");
            dgvTopBooks.Rows.Add("8", "Pride and Prejudice", "Lãng mạn", "Jane Austen", "567", "26/05/2025 13:20");
            dgvTopBooks.Rows.Add("9", "The Catcher in the Rye", "Tiểu thuyết", "J.D. Salinger", "534", "26/05/2025 12:55");
            dgvTopBooks.Rows.Add("10", "Lord of the Flies", "Tiểu thuyết", "William Golding", "489", "26/05/2025 12:30");

            // Load sample data for Category Stats table
            dgvCategoryStats.Rows.Clear();
            dgvCategoryStats.Rows.Add("1", "Tiểu thuyết", "3,542", "28.5%", "125", "28.3");
            dgvCategoryStats.Rows.Add("2", "Tâm lý", "2,890", "23.2%", "89", "32.5");
            dgvCategoryStats.Rows.Add("3", "Trinh thám", "2,156", "17.3%", "67", "32.2");
            dgvCategoryStats.Rows.Add("4", "Phiêu lưu", "1,987", "16.0%", "78", "25.5");
            dgvCategoryStats.Rows.Add("5", "Kinh dị", "1,875", "15.0%", "45", "41.7");
            dgvCategoryStats.Rows.Add("6", "Lãng mạn", "1,234", "9.9%", "38", "32.5");
            dgvCategoryStats.Rows.Add("7", "Khoa học", "987", "7.9%", "29", "34.0");
            dgvCategoryStats.Rows.Add("8", "Lịch sử", "856", "6.9%", "25", "34.2");
            dgvCategoryStats.Rows.Add("9", "Thể thao", "743", "6.0%", "22", "33.8");
            dgvCategoryStats.Rows.Add("10", "Âm nhạc", "621", "5.0%", "18", "34.5");
        }
        #endregion

        private void chartsPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        /// <summary>
        /// Refresh dashboard data - can be called from Main form when real-time updates are needed
        /// </summary>
        public async Task RefreshDashboardAsync()
        {
            try
            {
                await LoadDashboardDataAsync();
                
                // Update time label
                UpdateTimeLabel();
                
                System.Diagnostics.Debug.WriteLine("📊 Dashboard refreshed successfully with latest data");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing dashboard: {ex.Message}");
            }
        }
    }
}