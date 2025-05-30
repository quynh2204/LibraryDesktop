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
        private readonly IRatingService _ratingService;
        private Book? _currentBook;
        private List<Chapter> _chapters = new List<Chapter>();
        private User? _currentUser;
          // Rating UI Controls
        private Guna.UI2.WinForms.Guna2Panel? pnlRating;
        private Guna.UI2.WinForms.Guna2HtmlLabel? lblRatingTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel? lblCurrentRating;
        private Guna.UI2.WinForms.Guna2Button[]? starButtons;
        private Guna.UI2.WinForms.Guna2TextBox? txtComment;
        private Guna.UI2.WinForms.Guna2Button? btnSubmitRating;
        private Guna.UI2.WinForms.Guna2Button? btnDeleteRating;
        
        // Comments display UI Controls
        private Guna.UI2.WinForms.Guna2Panel? pnlAllComments;
        private Guna.UI2.WinForms.Guna2HtmlLabel? lblCommentsTitle;
        private FlowLayoutPanel? flpComments;
        
        private int _selectedRating = 0;
        private bool _hasExistingRating = false;

        public BookDetail(IBookService bookService, IUserService userService, IRatingService ratingService)
        {
            InitializeComponent();
            _bookService = bookService;
            _userService = userService;
            _ratingService = ratingService;
        }

        // Method to set book ID and user after form creation
        public void SetBookId(int bookId, User? currentUser = null)
        {
            _bookId = bookId;
            _currentUser = currentUser;
            this.Load += BookDetail_Load;
        }

        private async void BookDetail_Load(object? sender, EventArgs e)
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
                SetLoadingState(true);                await LoadBookInfoAsync();
                await LoadChaptersAsync();
                await LoadRatingInfoAsync();
                await LoadAverageRatingAsync(); // Load average rating
                await LoadAllCommentsAsync(); // Load all comments                // Increment view count
                if (_currentBook != null)
                {
                    await _bookService.IncrementViewCountAsync(_bookId);
                    lblViewCount.Text = $"Lượt xem: {(_currentBook.ViewCount + 1):N0}";
                }

                // Ensure proper layout after all loading is complete
                EnsureProperLayout();
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
        }        private async Task LoadRatingInfoAsync()
        {
            try
            {
                // Initialize rating panel and controls
                pnlRating = new Guna.UI2.WinForms.Guna2Panel();
                lblRatingTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
                lblCurrentRating = new Guna.UI2.WinForms.Guna2HtmlLabel();
                starButtons = new Guna.UI2.WinForms.Guna2Button[5];
                txtComment = new Guna.UI2.WinForms.Guna2TextBox();
                btnSubmitRating = new Guna.UI2.WinForms.Guna2Button();                // Set up rating panel - đặt trong pnlRight
                pnlRating.SuspendLayout();
                pnlRating.Controls.Clear();
                pnlRating.BorderColor = Color.FromArgb(224, 224, 224);
                pnlRating.BorderThickness = 1;
                pnlRating.FillColor = Color.White;
                pnlRating.BorderRadius = 8;                // Đặt rating panel trong pnlRight, phía dưới btnExit (btnExit ở Y=293 + height=60 = 353)
                pnlRating.Location = new Point(17, 370);
                pnlRating.Size = new Size(309, 300); // Increased height to accommodate all controls properly
                pnlRating.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                
                // Thêm vào pnlRight thay vì form chính
                pnlRight.Controls.Add(pnlRating);                // Set up rating title label
                lblRatingTitle.Text = "Đánh giá truyện";
                lblRatingTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
                lblRatingTitle.ForeColor = Color.FromArgb(0, 120, 215);
                lblRatingTitle.Location = new Point(10, 34);
                lblRatingTitle.AutoSize = true;
                pnlRating.Controls.Add(lblRatingTitle);

                // Set up current rating label
                lblCurrentRating.Text = "Chưa có đánh giá nào.";
                lblCurrentRating.Font = new Font("Segoe UI", 9F);
                lblCurrentRating.ForeColor = Color.FromArgb(50, 50, 50);
                lblCurrentRating.Location = new Point(10, 65);
                lblCurrentRating.AutoSize = true;
                pnlRating.Controls.Add(lblCurrentRating);

                // Set up star buttons for rating (sắp xếp theo hàng ngang)
                for (int i = 0; i < 5; i++)
                {
                    starButtons[i] = new Guna.UI2.WinForms.Guna2Button();
                    starButtons[i].Text = "★";
                    starButtons[i].Font = new Font("Segoe UI", 14F, FontStyle.Bold);
                    starButtons[i].ForeColor = Color.LightGray;
                    starButtons[i].FillColor = Color.Transparent;
                    starButtons[i].BorderThickness = 0;
                    starButtons[i].Size = new Size(35, 35);                    starButtons[i].Location = new Point(10 + i * 40, 95);
                    starButtons[i].Click += StarButton_Click;
                    starButtons[i].Cursor = Cursors.Hand;
                    pnlRating.Controls.Add(starButtons[i]);
                }

                // Set up comment text box (nhiều dòng)
                txtComment.PlaceholderText = "Nhập nhận xét của bạn ở đây...";
                txtComment.Font = new Font("Segoe UI", 9F);
                txtComment.ForeColor = Color.FromArgb(50, 50, 50);
                txtComment.BorderColor = Color.FromArgb(224, 224, 224);
                txtComment.BorderRadius = 5;
                txtComment.Multiline = true;                txtComment.Location = new Point(10, 140);
                txtComment.Size = new Size(289, 60); // Increased height for better usability
                txtComment.ScrollBars = ScrollBars.Vertical;
                pnlRating.Controls.Add(txtComment);// Set up submit rating button
                btnSubmitRating.Text = "Gửi đánh giá";
                btnSubmitRating.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                btnSubmitRating.ForeColor = Color.White;
                btnSubmitRating.FillColor = Color.FromArgb(0, 120, 215);
                btnSubmitRating.BorderRadius = 5;                btnSubmitRating.Location = new Point(10, 215);
                btnSubmitRating.Size = new Size(140, 35);
                btnSubmitRating.Cursor = Cursors.Hand;
                btnSubmitRating.Click += BtnSubmitRating_Click;
                pnlRating.Controls.Add(btnSubmitRating);

                // Set up delete rating button
                btnDeleteRating = new Guna.UI2.WinForms.Guna2Button();
                btnDeleteRating.Text = "Xóa đánh giá";
                btnDeleteRating.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                btnDeleteRating.ForeColor = Color.White;
                btnDeleteRating.FillColor = Color.FromArgb(231, 76, 60); // Red color
                btnDeleteRating.BorderRadius = 5;
                btnDeleteRating.Location = new Point(159, 215);
                btnDeleteRating.Size = new Size(140, 35);
                btnDeleteRating.Cursor = Cursors.Hand;
                btnDeleteRating.Visible = false; // Initially hidden
                btnDeleteRating.Click += BtnDeleteRating_Click;
                pnlRating.Controls.Add(btnDeleteRating);// Thêm label hướng dẫn
                var lblInstruction = new Guna.UI2.WinForms.Guna2HtmlLabel();
                lblInstruction.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
                lblInstruction.ForeColor = Color.FromArgb(120, 120, 120);                lblInstruction.Location = new Point(210, 105);
                lblInstruction.AutoSize = true;
                pnlRating.Controls.Add(lblInstruction);

                pnlRating.ResumeLayout();

                // Add hover effects for better user experience
                AddStarHoverEffects();
                
                // Update visibility based on login status
                UpdateRatingPanelVisibility();

                // Load existing rating if available
                await LoadUserRatingAsync();

                // Update rating panel visibility based on user login status
                UpdateRatingPanelVisibility();

                // Add hover effects for star buttons
                AddStarHoverEffects();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông tin đánh giá: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"LoadRatingInfo Error: {ex}");
            }
        }

        // Method to load and display average rating
        private async Task LoadAverageRatingAsync()
        {
            try
            {
                var averageRating = await _ratingService.GetAverageRatingAsync(_bookId);
                
                if (pnlRating != null)
                {
                    // Add average rating display
                    var lblAverageRating = new Guna.UI2.WinForms.Guna2HtmlLabel();
                    lblAverageRating.Text = averageRating > 0 ? 
                        $"Đánh giá trung bình: {averageRating:F1} sao" : 
                        "Chưa có đánh giá nào";
                    lblAverageRating.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    lblAverageRating.ForeColor = Color.FromArgb(255, 140, 0); // Orange color                    lblAverageRating.Location = new Point(10, 10);
                    lblAverageRating.AutoSize = true;
                    lblAverageRating.Location = new Point(12, 12);
                    pnlRating.Controls.Add(lblAverageRating);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading average rating: {ex.Message}");
            }
        }

        private async Task LoadUserRatingAsync()
        {
            try
            {
                if (_currentUser != null)
                {                    // Get user's rating for the book
                    var userRating = await _ratingService.GetUserRatingAsync(_currentUser.UserId, _bookId);                    if (userRating != null)
                    {
                        // Set selected rating based on user's rating
                        _selectedRating = userRating.RatingValue;
                        _hasExistingRating = true;
                        
                        if (lblCurrentRating != null)
                        {
                            lblCurrentRating.Text = $"Đánh giá của bạn: {_selectedRating} sao";
                        }

                        // Update star buttons appearance
                        if (starButtons != null)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                starButtons[i].ForeColor = i < _selectedRating ? Color.Gold : Color.LightGray;
                            }
                        }

                        // Set comment text if available
                        if (txtComment != null)
                        {
                            txtComment.Text = userRating.Review ?? string.Empty;
                        }

                        // Update button texts and visibility
                        if (btnSubmitRating != null)
                        {
                            btnSubmitRating.Text = "Cập nhật đánh giá";
                        }
                        if (btnDeleteRating != null)
                        {
                            btnDeleteRating.Visible = true;
                        }
                    }
                    else
                    {
                        // No existing rating - reset to default state
                        _selectedRating = 0;
                        _hasExistingRating = false;
                        
                        if (lblCurrentRating != null)
                        {
                            lblCurrentRating.Text = "Chưa có đánh giá nào.";
                        }
                        if (starButtons != null)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                starButtons[i].ForeColor = Color.LightGray;
                            }
                        }
                        if (btnSubmitRating != null)
                        {
                            btnSubmitRating.Text = "Gửi đánh giá";
                        }
                        if (btnDeleteRating != null)
                        {
                            btnDeleteRating.Visible = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải đánh giá người dùng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"LoadUserRating Error: {ex}");
            }
        }        private async void StarButton_Click(object? sender, EventArgs e)
        {
            try
            {
                // Get the clicked star button
                var clickedButton = sender as Guna.UI2.WinForms.Guna2Button;                if (clickedButton != null && starButtons != null)
                {
                    // Determine the rating value based on the clicked button's position
                    int ratingValue = Array.IndexOf(starButtons, clickedButton) + 1;

                    // If clicking the same star that's already selected, clear the rating
                    if (_selectedRating == ratingValue)
                    {
                        _selectedRating = 0;
                        
                        // Update current rating label
                        if (lblCurrentRating != null)
                        {
                            lblCurrentRating.Text = _hasExistingRating ? "Nhấn sao để thay đổi đánh giá" : "Chưa có đánh giá nào.";
                        }

                        // Update submit button text
                        if (btnSubmitRating != null)
                        {
                            btnSubmitRating.Text = _hasExistingRating ? "Cập nhật đánh giá" : "Gửi đánh giá";
                        }
                    }
                    else
                    {
                        // Update the selected rating
                        _selectedRating = ratingValue;

                        // Update current rating label
                        if (lblCurrentRating != null)
                        {
                            lblCurrentRating.Text = $"Đánh giá của bạn: {_selectedRating} sao";
                        }

                        // Update submit button text
                        if (btnSubmitRating != null)
                        {
                            btnSubmitRating.Text = _hasExistingRating ? "Cập nhật đánh giá" : "Gửi đánh giá";
                        }
                    }

                    // Update star buttons appearance
                    for (int i = 0; i < 5; i++)
                    {
                        starButtons[i].ForeColor = i < _selectedRating ? Color.Gold : Color.LightGray;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chọn đánh giá: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"StarButton_Click Error: {ex}");
            }
        }private async void BtnSubmitRating_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_currentUser == null)
                {
                    MessageBox.Show("Vui lòng đăng nhập để đánh giá truyện!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate rating selection
                if (_selectedRating <= 0)
                {
                    MessageBox.Show("Vui lòng chọn số sao để đánh giá!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get the comment text
                string comment = txtComment?.Text?.Trim() ?? string.Empty;

                // Disable button while submitting
                if (btnSubmitRating != null)
                {
                    btnSubmitRating.Enabled = false;
                    btnSubmitRating.Text = "Đang gửi...";
                }

                // Submit the rating using the rating service
                await _ratingService.AddOrUpdateRatingAsync(_currentUser.UserId, _bookId, _selectedRating, comment);

                MessageBox.Show("Cảm ơn bạn đã đánh giá truyện!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Update current rating display
                if (lblCurrentRating != null)
                {
                    lblCurrentRating.Text = $"Đánh giá của bạn: {_selectedRating} sao";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi đánh giá: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"BtnSubmitRating_Click Error: {ex}");
            }            finally
            {
                // Re-enable button
                if (btnSubmitRating != null)
                {
                    btnSubmitRating.Enabled = true;
                    btnSubmitRating.Text = "Gửi đánh giá";
                }
            }
        }

        private async void BtnDeleteRating_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_currentUser == null)
                {
                    MessageBox.Show("Vui lòng đăng nhập!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Confirm deletion
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa đánh giá này?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    
                if (result != DialogResult.Yes)
                    return;

                // Disable button while deleting
                if (btnDeleteRating != null)
                {
                    btnDeleteRating.Enabled = false;
                    btnDeleteRating.Text = "Đang xóa...";
                }

                // Delete the rating using the rating service
                await _ratingService.DeleteRatingAsync(_currentUser.UserId, _bookId);

                MessageBox.Show("Đã xóa đánh giá thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reset UI to no rating state
                _selectedRating = 0;
                _hasExistingRating = false;

                if (lblCurrentRating != null)
                {
                    lblCurrentRating.Text = "Chưa có đánh giá nào.";
                }

                if (starButtons != null)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        starButtons[i].ForeColor = Color.LightGray;
                    }
                }

                if (txtComment != null)
                {
                    txtComment.Text = string.Empty;
                }

                if (btnSubmitRating != null)
                {
                    btnSubmitRating.Text = "Gửi đánh giá";
                }

                if (btnDeleteRating != null)
                {
                    btnDeleteRating.Visible = false;
                }

                // Reload average rating and comments
                await LoadAverageRatingAsync();
                await LoadAllCommentsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa đánh giá: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"BtnDeleteRating_Click Error: {ex}");
            }
            finally
            {
                // Re-enable button
                if (btnDeleteRating != null)
                {
                    btnDeleteRating.Enabled = true;
                    btnDeleteRating.Text = "Xóa đánh giá";
                }
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

        // Method to show/hide rating panel based on user login status
        private void UpdateRatingPanelVisibility()
        {
            if (pnlRating != null)
            {
                pnlRating.Visible = _currentUser != null;
                
                if (_currentUser == null && lblCurrentRating != null)
                {
                    lblCurrentRating.Text = "Đăng nhập để đánh giá truyện";
                    lblCurrentRating.ForeColor = Color.FromArgb(120, 120, 120);
                }
            }
        }

        // Method to provide visual feedback for star hover effects
        private void AddStarHoverEffects()
        {
            if (starButtons == null) return;

            for (int i = 0; i < starButtons.Length; i++)
            {
                int starIndex = i; // Capture for closure
                
                starButtons[i].MouseEnter += (s, e) =>
                {
                    if (_currentUser == null) return;
                    
                    // Highlight stars up to hovered star
                    for (int j = 0; j <= starIndex; j++)
                    {
                        if (starButtons[j] != null)
                            starButtons[j].ForeColor = Color.Gold;
                    }
                    for (int j = starIndex + 1; j < 5; j++)
                    {
                        if (starButtons[j] != null)
                            starButtons[j].ForeColor = Color.LightGray;
                    }
                };

                starButtons[i].MouseLeave += (s, e) =>
                {
                    if (_currentUser == null) return;
                    
                    // Restore to selected rating
                    for (int j = 0; j < 5; j++)
                    {
                        if (starButtons[j] != null)
                            starButtons[j].ForeColor = j < _selectedRating ? Color.Gold : Color.LightGray;
                    }
                };
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

        // Method to load and display all comments for the book
        private async Task LoadAllCommentsAsync()
        {
            try
            {
                // Initialize comments panel if not exists
                if (pnlAllComments == null)
                {
                    pnlAllComments = new Guna.UI2.WinForms.Guna2Panel();
                    lblCommentsTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
                    flpComments = new FlowLayoutPanel();
                }

                // Clear previous content
                pnlAllComments.Controls.Clear();
                pnlAllComments.SuspendLayout();                // Configure comments panel to be placed in pnlContent (below rtbContent)
                pnlAllComments.BorderColor = Color.FromArgb(224, 224, 224);
                pnlAllComments.BorderThickness = 1;
                pnlAllComments.FillColor = Color.White;
                pnlAllComments.BorderRadius = 8;
                
                // Enable scrolling for pnlContent to accommodate the comments panel
                pnlContent.AutoScroll = true;
                  // Adjust rtbContent to not fill the entire panel
                rtbContent.Dock = DockStyle.None;
                rtbContent.Location = new Point(17, 20);
                rtbContent.Size = new Size(970, 500); // Slightly reduced width to ensure no overlap
                rtbContent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;// Position comments panel below rtbContent with some margin
                pnlAllComments.Location = new Point(17, 540); // rtbContent.Top + rtbContent.Height + margin (20 + 500 + 20)
                pnlAllComments.Size = new Size(970, 300); // Reduced width to avoid horizontal scrolling
                pnlAllComments.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                
                // Add to pnlContent instead of pnlRight
                pnlContent.Controls.Add(pnlAllComments);

                // Configure title label
                lblCommentsTitle.Text = "Tất cả đánh giá";
                lblCommentsTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
                lblCommentsTitle.ForeColor = Color.FromArgb(0, 120, 215);
                lblCommentsTitle.Location = new Point(10, 10);
                lblCommentsTitle.AutoSize = true;
                pnlAllComments.Controls.Add(lblCommentsTitle);                // Configure flow layout panel for comments
                flpComments.FlowDirection = FlowDirection.TopDown;
                flpComments.WrapContents = false;
                flpComments.AutoScroll = true;                flpComments.Location = new Point(10, 40);
                flpComments.Size = new Size(920, 250); // Reduced width to match parent panel
                flpComments.BackColor = Color.White;
                pnlAllComments.Controls.Add(flpComments);

                // Get all ratings for this book
                var allRatings = await _ratingService.GetBookRatingsAsync(_bookId);                if (allRatings?.Any() == true)
                {
                    foreach (var rating in allRatings.OrderByDescending(r => r.CreatedDate))
                    {
                        var commentPanel = CreateCommentPanel(rating);
                        flpComments.Controls.Add(commentPanel);
                    }

                    lblCommentsTitle.Text = $"Tất cả đánh giá ({allRatings.Count()})";
                }
                else
                {
                    var noCommentsLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
                    noCommentsLabel.Text = "Chưa có đánh giá nào cho cuốn sách này.";
                    noCommentsLabel.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
                    noCommentsLabel.ForeColor = Color.FromArgb(120, 120, 120);
                    noCommentsLabel.Location = new Point(5, 5);
                    noCommentsLabel.AutoSize = true;
                    flpComments.Controls.Add(noCommentsLabel);                }

                pnlAllComments.ResumeLayout();
                  // Debug: Ensure the panel is visible and accessible
                pnlAllComments.Visible = true;
                pnlAllComments.BringToFront();
                
                // Force layout update to ensure proper positioning
                pnlContent.PerformLayout();
                pnlAllComments.PerformLayout();
                
                // Debug: Log layout information
                Debug.WriteLine($"pnlContent size: {pnlContent.Size}");
                Debug.WriteLine($"rtbContent location: {rtbContent.Location}, size: {rtbContent.Size}");
                Debug.WriteLine($"pnlAllComments location: {pnlAllComments.Location}, size: {pnlAllComments.Size}");
                Debug.WriteLine($"flpComments location: {flpComments.Location}, size: {flpComments.Size}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading all comments: {ex.Message}");
            }
        }        // Helper method to create individual comment panel
        private Panel CreateCommentPanel(Rating rating)
        {
            var commentPanel = new Panel();
            commentPanel.Size = new Size(900, 100); // Reduced width to fit better
            commentPanel.BorderStyle = BorderStyle.FixedSingle;
            commentPanel.BackColor = Color.FromArgb(248, 249, 250);
            commentPanel.Margin = new Padding(5, 5, 5, 10);

            // User name and rating stars
            var userLabel = new Label();
            userLabel.Text = $"{rating.User?.Username ?? "Ẩn danh"}";
            userLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            userLabel.ForeColor = Color.FromArgb(52, 58, 64);
            userLabel.Location = new Point(15, 10);
            userLabel.Size = new Size(200, 20);
            commentPanel.Controls.Add(userLabel);

            // Star rating display
            var starsLabel = new Label();
            starsLabel.Text = new string('★', rating.RatingValue) + new string('☆', 5 - rating.RatingValue);
            starsLabel.Font = new Font("Segoe UI", 12F);
            starsLabel.ForeColor = Color.FromArgb(255, 193, 7); // Gold color
            starsLabel.Location = new Point(230, 8);
            starsLabel.Size = new Size(100, 22);
            commentPanel.Controls.Add(starsLabel);            // Date
            var dateLabel = new Label();
            dateLabel.Text = rating.CreatedDate.ToString("dd/MM/yyyy HH:mm");
            dateLabel.Font = new Font("Segoe UI", 9F);
            dateLabel.ForeColor = Color.FromArgb(108, 117, 125);
            dateLabel.Location = new Point(680, 10); // Adjusted X position for smaller panel
            dateLabel.Size = new Size(160, 18);
            commentPanel.Controls.Add(dateLabel);

            // Comment text
            if (!string.IsNullOrEmpty(rating.Review))
            {
                var commentLabel = new Label();                commentLabel.Text = rating.Review.Length > 200 ? 
                    rating.Review.Substring(0, 200) + "..." : rating.Review;
                commentLabel.Font = new Font("Segoe UI", 9F);
                commentLabel.ForeColor = Color.FromArgb(73, 80, 87);
                commentLabel.Location = new Point(15, 35);
                commentLabel.Size = new Size(860, 50); // Adjusted width for smaller panel
                commentLabel.AutoEllipsis = true;
                commentPanel.Controls.Add(commentLabel);

                // Tooltip for full comment                if (rating.Review.Length > 200)
                {
                    var tooltip = new ToolTip();
                    tooltip.SetToolTip(commentLabel, rating.Review);
                }
            }            return commentPanel;
        }        // Method to ensure proper layout after all loading is complete
        private void EnsureProperLayout()
        {
            try
            {
                // Ensure pnlContent AutoScroll is enabled
                pnlContent.AutoScroll = true;
                
                // Bring comments panel to front if it exists in pnlContent
                if (pnlAllComments != null && pnlAllComments.Created && pnlAllComments.Parent == pnlContent)
                {
                    pnlAllComments.BringToFront();
                    Debug.WriteLine("Comments panel brought to front");
                }
                
                // Ensure rating panel is visible in pnlRight
                if (pnlRating != null && pnlRating.Created && pnlRating.Parent == pnlRight)
                {
                    pnlRating.BringToFront();
                    Debug.WriteLine("Rating panel brought to front in pnlRight");
                }
                
                // Force refresh of the layout
                pnlContent.PerformLayout();
                pnlRight.PerformLayout();
                this.PerformLayout();
                
                Debug.WriteLine("Layout refresh completed");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in EnsureProperLayout: {ex.Message}");
            }
        }
    }
}