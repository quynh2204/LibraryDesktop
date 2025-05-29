using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryDesktop.Data.Services;
using LibraryDesktop.Models;
using System.Diagnostics;

namespace LibraryDesktop.View
{
    public partial class BookDetail : Form
    {
        private int _bookId;
        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private Book? _currentBook;
        private List<Chapter> _chapters = new List<Chapter>();
        private User? _currentUser;

        public BookDetail(IBookService bookService, IUserService userService)
        {
            InitializeComponent();
            _bookService = bookService;
            _userService = userService;
        }

        // Method to set book ID and user after form creation
        public void SetBookId(int bookId, User? currentUser = null)
        {
            _bookId = bookId;
            _currentUser = currentUser;
            this.Load += BookDetail_Load;
        }

        private async void BookDetail_Load(object sender, EventArgs e)
        {
            if (_bookId <= 0)
            {
                MessageBox.Show("ID sách không hợp lệ!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            try
            {
                SetLoadingState(true);

                await LoadBookInfoAsync();
                await LoadChaptersAsync();

                // Increment view count
                if (_currentBook != null)
                {
                    await _bookService.IncrementViewCountAsync(_bookId);
                    lblViewCount.Text = $"Lượt xem: {(_currentBook.ViewCount + 1):N0}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi khởi tạo form: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"BookDetail Load Error: {ex}");
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        private void SetLoadingState(bool isLoading)
        {
            btnDownloadStory.Enabled = !isLoading;
            btnReadStory.Enabled = !isLoading;
            cmbChapters.Enabled = !isLoading;

            if (isLoading)
            {
                rtbContent.Text = "Đang tải thông tin sách...";
            }
        }

        private async Task LoadBookInfoAsync()
        {
            try
            {
                _currentBook = await _bookService.GetBookDetailsAsync(_bookId);

                if (_currentBook != null)
                {
                    // Update form title and labels
                    this.Text = $"Chi tiết truyện - {_currentBook.Title}";
                    lblBookTitle.Text = _currentBook.Title;
                    lblAuthor.Text = $"Tác giả: {_currentBook.Author}";
                    txtDescription.Text = _currentBook.Description ?? "Không có mô tả.";

                    // Update status and additional info
                    var statusText = GetStatusDisplayText(_currentBook.Status);
                    lblStatus.Text = $"Trạng thái: {statusText}";
                    lblTotalChapters.Text = $"Tổng số chương: {_currentBook.TotalChapters}";
                    lblViewCount.Text = $"Lượt xem: {_currentBook.ViewCount:N0}";

                    Debug.WriteLine($"Loaded book: {_currentBook.Title} (ID: {_currentBook.BookId})");
                }
                else
                {
                    MessageBox.Show("Không tìm thấy thông tin sách!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông tin sách: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"LoadBookInfo Error: {ex}");
            }
        }

        private string GetStatusDisplayText(BookStatus status)
        {
            return status switch
            {
                BookStatus.Draft => "Bản nháp",
                BookStatus.Published => "Đã xuất bản",
                BookStatus.Completed => "Hoàn thành",
                BookStatus.OnHold => "Tạm dừng",
                BookStatus.Cancelled => "Đã hủy",
                _ => "Không xác định"
            };
        }

        private async Task LoadChaptersAsync()
        {
            try
            {
                cmbChapters.Items.Clear();

                // Get chapters from the book that was loaded with details
                if (_currentBook?.Chapters != null && _currentBook.Chapters.Any())
                {
                    _chapters = _currentBook.Chapters.OrderBy(c => c.ChapterNumber).ToList();
                    Debug.WriteLine($"Found {_chapters.Count} chapters for book {_bookId}");

                    foreach (var chapter in _chapters)
                    {
                        var chapterInfo = new ChapterInfo
                        {
                            ChapterId = chapter.ChapterId,
                            ChapterNumber = chapter.ChapterNumber,
                            ChapterTitle = chapter.ChapterTitle,
                            GitHubContentUrl = chapter.GitHubContentUrl
                        };
                        cmbChapters.Items.Add(chapterInfo);
                    }
                }

                if (cmbChapters.Items.Count > 0)
                {
                    cmbChapters.SelectedIndex = 0;
                    rtbContent.Text = "Chọn 'Tải truyện' để đọc nội dung chương này.";
                }
                else
                {
                    rtbContent.Text = "Truyện này chưa có chương nào.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách chương: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"LoadChapters Error: {ex}");
            }
        }

        private async void btnDownloadStory_Click(object sender, EventArgs e)
        {
            if (cmbChapters.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn chương để tải!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Update button state
                btnDownloadStory.Enabled = false;
                btnDownloadStory.Text = "Đang tải...";
                rtbContent.Text = "Đang tải nội dung từ GitHub...";

                var selectedChapter = (ChapterInfo)cmbChapters.SelectedItem;

                if (string.IsNullOrEmpty(selectedChapter.GitHubContentUrl))
                {
                    MessageBox.Show("Chương này chưa có URL nội dung!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    rtbContent.Text = "Chương này chưa có nội dung.";
                    return;
                }

                Debug.WriteLine($"Downloading content from: {selectedChapter.GitHubContentUrl}");

                // Use BookService to get chapter content
                string content = await _bookService.GetChapterContentAsync(selectedChapter.GitHubContentUrl);

                if (!string.IsNullOrEmpty(content) && !content.StartsWith("Error loading"))
                {
                    rtbContent.Text = content;
                    MessageBox.Show("Tải truyện thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Debug.WriteLine($"Successfully loaded content, length: {content.Length} characters");
                }
                else
                {
                    rtbContent.Text = content.StartsWith("Error loading") ? content : "Nội dung trống hoặc không thể tải được.";
                    Debug.WriteLine("Content is empty or contains error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải truyện: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                rtbContent.Text = "Không thể tải nội dung. Vui lòng thử lại.";
                Debug.WriteLine($"Download Error: {ex}");
            }
            finally
            {
                btnDownloadStory.Enabled = true;
                btnDownloadStory.Text = "Tải truyện";
            }
        }

        private void btnReadStory_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(rtbContent.Text) ||
                rtbContent.Text.Contains("Đang tải") ||
                rtbContent.Text.Contains("Chọn 'Tải truyện'") ||
                rtbContent.Text.Contains("Error loading"))
            {
                MessageBox.Show("Vui lòng tải nội dung truyện trước khi đọc!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Focus to reading area and scroll to top
            rtbContent.Focus();
            rtbContent.SelectionStart = 0;
            rtbContent.ScrollToCaret();

            // Show reading mode
            ShowReadingMode();
        }

        private void ShowReadingMode()
        {
            // Improve reading experience
            rtbContent.Font = new Font("Segoe UI", 11f);
            rtbContent.BackColor = Color.FromArgb(250, 250, 250);
            rtbContent.ForeColor = Color.FromArgb(50, 50, 50);

            // Optional: You can create a full-screen reading mode here
            MessageBox.Show("Đã chuyển sang chế độ đọc! Sử dụng thanh cuộn để điều hướng.",
                "Chế độ đọc", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbChapters_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Clear content when changing chapters
            if (rtbContent.Text != "Đang tải thông tin sách..." &&
                rtbContent.Text != "Truyện này chưa có chương nào." &&
                rtbContent.Text != "Đang tải nội dung từ GitHub...")
            {
                rtbContent.Text = "Chọn 'Tải truyện' để đọc nội dung chương này.";
            }
        }

        // Helper class for chapter information
        public class ChapterInfo
        {
            public int ChapterId { get; set; }
            public int ChapterNumber { get; set; }
            public string ChapterTitle { get; set; } = string.Empty;
            public string GitHubContentUrl { get; set; } = string.Empty;

            public override string ToString()
            {
                return $"Chương {ChapterNumber}: {ChapterTitle}";
            }
        }
    }
}