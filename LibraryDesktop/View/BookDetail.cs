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
    // Helper class for chapter information
    public class ChapterInfo
    {
        public int ChapterId { get; set; }
        public int ChapterNumber { get; set; }
        public string ChapterTitle { get; set; } = string.Empty;
        public string GitHubContentUrl { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"Chapter {ChapterNumber}: {ChapterTitle}";
        }
    }

    public partial class BookDetail : Form
    {
        #region Fields
        private int _bookId;
        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private readonly IRatingService _ratingService;
        private readonly IHistoryService _historyService;
        private Book? _currentBook;
        private List<Chapter> _chapters = new List<Chapter>();
        private User? _currentUser;

        // Theme
        private bool _isDarkMode = false;        // Rating state
        private int _selectedRating = 0;
        private bool _hasExistingRating = false;

        private bool _isFavorite = false;

        #endregion

        #region Constructor and Setup
        public BookDetail(IBookService bookService, IUserService userService, IRatingService ratingService, IHistoryService historyService)
        {
            InitializeComponent();
            _bookService = bookService;
            _userService = userService;
            _ratingService = ratingService;
            _historyService = historyService;
        }

        public void SetBookId(int bookId, User? currentUser = null)
        {
            _bookId = bookId;
            _currentUser = currentUser;
            this.Load += BookDetail_Load;
        }
        #endregion

        #region Form Events
        private async void BookDetail_Load(object? sender, EventArgs e)
        {
            if (_bookId <= 0)
            {
                MessageBox.Show("Invalid book ID!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            try
            {
                SetLoadingState(true);
                await LoadBookInfoAsync();
                await LoadChaptersAsync();
                await LoadRatingInfoAsync();
                await LoadAverageRatingAsync();
                await LoadAllCommentsAsync();

                await LoadFavoriteStatusAsync();

                if (_currentBook != null)
                {
                    await _bookService.IncrementViewCountAsync(_bookId);                    
                    // Update the current book object and display with new count
                    _currentBook.ViewCount++;
                    lblViewCount.Text = $"Views: {_currentBook.ViewCount:N0}";
                    
                    System.Diagnostics.Debug.WriteLine($"📊 ViewCount incremented for BookId {_bookId}: {_currentBook.ViewCount}");
                      // Track history - book view
                    if (_currentUser != null)
                    {
                        await _historyService.AddHistoryAsync(_currentUser.UserId, _bookId, null, "View");
                    }
                }

                ApplyTheme(); // Apply default theme
                EnsureProperLayout();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"BookDetail Load Error: {ex}");
            }
            finally
            {
                SetLoadingState(false);
            }
        }        
        
        private async void btnDownloadStory_Click(object sender, EventArgs e)
        {
            if (cmbChapters.SelectedItem == null)
            {
                MessageBox.Show("Please select a chapter to download!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnDownloadStory.Enabled = false;
                btnDownloadStory.Text = "Downloading...";
                rtbContent.Text = "Loading content from GitHub...";

                var selectedChapter = (ChapterInfo)cmbChapters.SelectedItem;

                if (string.IsNullOrEmpty(selectedChapter.GitHubContentUrl))
                {
                    MessageBox.Show("This chapter does not have a content URL!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    rtbContent.Text = "This chapter has no content.";
                    return;
                }

                // Check if book has a price and if user has purchased it or has sufficient coins
                if (_currentBook != null && _currentBook.Price > 0 && _currentUser != null)
                {
                    // Check if user has already purchased this book
                    var userHistory = await _historyService.GetUserHistoryAsync(_currentUser.UserId);
                    bool hasPurchased = userHistory.Any(h => h.BookId == _bookId && h.AccessType == "Purchase");

                    if (!hasPurchased)
                    {
                        // Get current user coins
                        int userCoins = await _userService.GetUserCoinsAsync(_currentUser.UserId);
                        
                        if (userCoins < _currentBook.Price)
                        {
                            MessageBox.Show($"Insufficient coins! You need {_currentBook.Price} coins to access this book, but you only have {userCoins} coins.\n\nPlease recharge your account to continue.", 
                                "Insufficient Coins", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Ask user to confirm purchase
                        var result = MessageBox.Show($"This book costs {_currentBook.Price} coins. Do you want to purchase it?\n\nYour current balance: {userCoins} coins\nAfter purchase: {userCoins - _currentBook.Price} coins", 
                            "Purchase Book", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        
                        if (result == DialogResult.Yes)
                        {
                            try
                            {
                                // Deduct coins from user account
                                int newBalance = userCoins - _currentBook.Price;
                                await _userService.UpdateUserCoinsAsync(_currentUser.UserId, newBalance);
                                
                                // Record the purchase in history
                                await _historyService.AddHistoryAsync(_currentUser.UserId, _bookId, null, "Purchase");
                                  // Update current user object
                                _currentUser.Coins = newBalance;
                                
                                // Trigger real-time sync to update coins display in Main form
                                await ForceMainFormCoinsUpdateAsync();
                                
                                MessageBox.Show($"Book purchased successfully! You now have {newBalance} coins remaining.", 
                                    "Purchase Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception purchaseEx)
                            {
                                MessageBox.Show($"Error processing purchase: {purchaseEx.Message}", "Purchase Error", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        else
                        {
                            return; // User cancelled purchase
                        }
                    }
                }

                string content = await _bookService.GetChapterContentAsync(selectedChapter.GitHubContentUrl);                if (!string.IsNullOrEmpty(content) && !content.StartsWith("Error loading"))
                {
                    rtbContent.Text = content;
                    MessageBox.Show("Story downloaded successfully!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Log chapter read history with chapter name
                    if (_currentUser != null && _historyService != null)
                    {
                        await _historyService.AddHistoryAsync(_currentUser.UserId, _bookId, selectedChapter.ChapterId, "Read");
                    }
                }
                else
                {
                    rtbContent.Text = content.StartsWith("Error loading") ? content : "Content is empty or could not be loaded.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error downloading story: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                rtbContent.Text = "Could not load content. Please try again.";
            }
            finally
            {
                btnDownloadStory.Enabled = true;
                btnDownloadStory.Text = "Download Story";
            }
        }// ENSURE THIS METHOD HAS ASYNC:
        private async void btnFavorite_Click(object sender, EventArgs e) // ALREADY HAS ASYNC
        {
            if (_currentUser == null)
            {
                MessageBox.Show("Please log in to manage favorites!", "Notification",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnFavorite.Enabled = false;
                
                if (_isFavorite)
                {
                    // Remove from favorites
                    btnFavorite.Text = "Removing...";
                    bool success = await _userService.RemoveFromFavoritesAsync(_currentUser.UserId, _bookId);                    if (success)
                    {
                        _isFavorite = false;
                        UpdateFavoriteButtonUI();
                        MessageBox.Show("Book removed from MyBook!", "Notification",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        // Refresh MyBooks form if open
                        await RefreshMyBooksIfOpenAsync();
                    }
                    else
                    {
                        MessageBox.Show("Failed to remove book from favorites!", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // Add to favorites
                    btnFavorite.Text = "Adding...";
                    bool success = await _userService.AddToFavoritesAsync(_currentUser.UserId, _bookId);                    if (success)
                    {
                        _isFavorite = true;
                        UpdateFavoriteButtonUI();
                        MessageBox.Show("Book added to MyBook!", "Notification",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        // Refresh MyBooks form if open
                        await RefreshMyBooksIfOpenAsync();
                    }
                    else
                    {                        MessageBox.Show("Book is already in favorites!", "Notification",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _isFavorite = true;
                        UpdateFavoriteButtonUI();
                        
                        // Refresh MyBooks form if open
                        await RefreshMyBooksIfOpenAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error managing favorites: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"btnFavorite_Click Error: {ex}");

                // Reset button state
                btnFavorite.Enabled = true;
                UpdateFavoriteButtonUI();
            }
        }

        private void btnToggleTheme_Click(object sender, EventArgs e)
        {
            _isDarkMode = !_isDarkMode;
            ApplyTheme();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbChapters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rtbContent.Text != "Loading book information..." &&
                rtbContent.Text != "This story has no chapters." &&
                rtbContent.Text != "Loading content from GitHub...")
            {
                rtbContent.Text = "Select 'Download Story' to read the content of this chapter.";
            }
        }
        #endregion

        #region Data Loading Methods
        private void SetLoadingState(bool isLoading)
        {
            btnDownloadStory.Enabled = !isLoading;
            btnFavorite.Enabled = !isLoading;
            cmbChapters.Enabled = !isLoading;

            if (isLoading)
            {
                rtbContent.Text = "Loading book information...";
            }
        }        private async Task LoadBookInfoAsync()
        {
            try
            {
                _currentBook = await _bookService.GetBookDetailsAsync(_bookId);

                if (_currentBook != null)
                {
                    this.Text = $"Book Details - {_currentBook.Title}";
                    lblBookTitle.Text = _currentBook.Title;
                    lblAuthor.Text = $"Author: {_currentBook.Author}";
                    txtDescription.Text = _currentBook.Description ?? "No description available.";

                    var statusText = GetStatusDisplayText(_currentBook.Status);
                    lblStatus.Text = $"Status: {statusText}";
                    lblTotalChapters.Text = $"Total Chapters: {_currentBook.TotalChapters}";
                    lblViewCount.Text = $"Views: {_currentBook.ViewCount:N0}";

                    // Check purchase status and update button text if book has a price
                    if (_currentBook.Price > 0 && _currentUser != null)
                    {
                        var userHistory = await _historyService.GetUserHistoryAsync(_currentUser.UserId);
                        bool hasPurchased = userHistory.Any(h => h.BookId == _bookId && h.AccessType == "Purchase");
                        
                        if (hasPurchased)
                        {
                            btnDownloadStory.Text = "Download Story";
                            // Could also add a visual indicator that the book is owned
                        }
                        else
                        {
                            btnDownloadStory.Text = $"Purchase & Download ({_currentBook.Price} Coins)";
                        }
                    }
                    else
                    {
                        btnDownloadStory.Text = "Download Story";
                    }
                }
                else
                {
                    MessageBox.Show("Book information not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading book information: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task LoadFavoriteStatusAsync()
        {
            try
            {
                if (_currentUser != null)
                {
                    _isFavorite = await _userService.IsFavoriteAsync(_currentUser.UserId, _bookId);
                }
                else
                {
                    _isFavorite = false;
                }

                UpdateFavoriteButtonUI();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading favorite status: {ex.Message}");
                _isFavorite = false;
                UpdateFavoriteButtonUI();
            }
        }        private void UpdateFavoriteButtonUI()
        {
            btnFavorite.Visible = true;
            btnFavorite.Enabled = true;

            if (_isFavorite)
            {
                // SHOW UNSUBSCRIBE BUTTON WHEN ALREADY FAVORITED
                btnFavorite.Text = "💔 Unsubscribe from MyBook";
                
                // Apply gray color for unsubscribe
                if (_isDarkMode)
                {
                    btnFavorite.FillColor = Color.FromArgb(107, 114, 128); // Gray for dark mode
                }
                else
                {
                    btnFavorite.FillColor = Color.FromArgb(108, 117, 125); // Gray for light mode
                }
            }
            else
            {
                // SHOW ADD BUTTON WHEN NOT YET FAVORITED
                btnFavorite.Text = "❤️ Add to MyBook";
                
                // Apply color based on theme
                if (_isDarkMode)
                {
                    btnFavorite.FillColor = Color.FromArgb(185, 28, 28); // Dark red for dark mode
                }
                else
                {
                    btnFavorite.FillColor = Color.FromArgb(220, 53, 69); // Normal red for light mode
                }
            }
        }

        private async Task LoadChaptersAsync()
        {
            try
            {
                cmbChapters.Items.Clear();                // Get chapters from the book that was loaded with details
                if (_currentBook?.Chapters != null && _currentBook.Chapters.Any())
                {
                    _chapters = _currentBook.Chapters.OrderBy(c => c.ChapterNumber).ToList();

                    // Update TotalChapters if it doesn't match actual chapter count
                    if (_currentBook.TotalChapters != _chapters.Count)
                    {
                        _currentBook.TotalChapters = _chapters.Count;
                        // Update the display to show the correct count
                        lblTotalChapters.Text = $"Tổng số chương: {_currentBook.TotalChapters}";
                        Debug.WriteLine($"Updated TotalChapters from {_currentBook.TotalChapters} to {_chapters.Count}");
                    }

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
                    rtbContent.Text = "Select 'Download Story' to read the content of this chapter.";
                }
                else
                {
                    rtbContent.Text = "This story has no chapters.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading chapter list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetStatusDisplayText(BookStatus status)
        {
            return status switch
            {
                BookStatus.Draft => "Draft",
                BookStatus.Published => "Published",
                BookStatus.Completed => "Completed",
                BookStatus.OnHold => "On Hold",
                BookStatus.Cancelled => "Cancelled",
                _ => "Unknown"
            };
        }
        #endregion

        #region Rating Methods (CLEANED UP)
        private async Task LoadRatingInfoAsync()
        {
            try
            {
                InitializeRatingUI();
                await LoadUserRatingAsync();
                UpdateRatingPanelVisibility();
                AddStarHoverEffects();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading rating information: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeRatingUI()
        {
            pnlRating = new Guna.UI2.WinForms.Guna2Panel();
            lblRatingTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblCurrentRating = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblAverageRating = new Guna.UI2.WinForms.Guna2HtmlLabel();
            starButtons = new Guna.UI2.WinForms.Guna2Button[5];
            txtComment = new Guna.UI2.WinForms.Guna2TextBox();
            btnSubmitRating = new Guna.UI2.WinForms.Guna2Button();
            btnDeleteRating = new Guna.UI2.WinForms.Guna2Button();

            // Setup rating panel
            pnlRating.SuspendLayout();
            pnlRating.Controls.Clear();
            pnlRating.BorderColor = Color.FromArgb(224, 224, 224);
            pnlRating.BorderThickness = 1;
            pnlRating.FillColor = Color.White;
            pnlRating.BorderRadius = 8;
            pnlRating.Location = new Point(17, 430);
            pnlRating.Size = new Size(309, 300);
            pnlRating.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlRight.Controls.Add(pnlRating);

            // Average rating
            lblAverageRating.Text = "No ratings yet";
            lblAverageRating.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblAverageRating.ForeColor = Color.FromArgb(255, 140, 0);
            lblAverageRating.Location = new Point(12, 12);
            lblAverageRating.AutoSize = true;
            pnlRating.Controls.Add(lblAverageRating);

            // Rating title
            lblRatingTitle.Text = "Rate the Book";
            lblRatingTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblRatingTitle.ForeColor = Color.FromArgb(0, 120, 215);
            lblRatingTitle.Location = new Point(10, 34);
            lblRatingTitle.AutoSize = true;
            pnlRating.Controls.Add(lblRatingTitle);

            // Current rating
            lblCurrentRating.Text = "No rating submitted.";
            lblCurrentRating.Font = new Font("Segoe UI", 9F);
            lblCurrentRating.ForeColor = Color.FromArgb(50, 50, 50);
            lblCurrentRating.Location = new Point(10, 65);
            lblCurrentRating.AutoSize = true;
            pnlRating.Controls.Add(lblCurrentRating);

            // Star buttons
            for (int i = 0; i < 5; i++)
            {
                starButtons[i] = new Guna.UI2.WinForms.Guna2Button();
                starButtons[i].Text = "★";
                starButtons[i].Font = new Font("Segoe UI", 14F, FontStyle.Bold);
                starButtons[i].ForeColor = Color.LightGray;
                starButtons[i].FillColor = Color.Transparent;
                starButtons[i].BorderThickness = 0;
                starButtons[i].Size = new Size(35, 35);
                starButtons[i].Location = new Point(10 + i * 40, 95);
                starButtons[i].Click += StarButton_Click;
                starButtons[i].Cursor = Cursors.Hand;
                pnlRating.Controls.Add(starButtons[i]);
            }

            // Comment text box
            txtComment.PlaceholderText = "Enter your review here...";
            txtComment.Font = new Font("Segoe UI", 9F);
            txtComment.ForeColor = Color.FromArgb(50, 50, 50);
            txtComment.BorderColor = Color.FromArgb(224, 224, 224);
            txtComment.BorderRadius = 5;
            txtComment.Multiline = true;
            txtComment.Location = new Point(10, 140);
            txtComment.Size = new Size(289, 60);
            txtComment.ScrollBars = ScrollBars.Vertical;
            pnlRating.Controls.Add(txtComment);

            // Submit button
            btnSubmitRating.Text = "Submit Rating";
            btnSubmitRating.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSubmitRating.ForeColor = Color.White;
            btnSubmitRating.FillColor = Color.FromArgb(0, 120, 215);
            btnSubmitRating.BorderRadius = 5;
            btnSubmitRating.Location = new Point(10, 215);
            btnSubmitRating.Size = new Size(140, 35);
            btnSubmitRating.Cursor = Cursors.Hand;
            btnSubmitRating.Click += BtnSubmitRating_Click;
            pnlRating.Controls.Add(btnSubmitRating);

            // Delete button
            btnDeleteRating.Text = "Delete Rating";
            btnDeleteRating.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnDeleteRating.ForeColor = Color.White;
            btnDeleteRating.FillColor = Color.FromArgb(231, 76, 60);
            btnDeleteRating.BorderRadius = 5;
            btnDeleteRating.Location = new Point(159, 215);
            btnDeleteRating.Size = new Size(140, 35);
            btnDeleteRating.Cursor = Cursors.Hand;
            btnDeleteRating.Visible = false;
            btnDeleteRating.Click += BtnDeleteRating_Click;
            pnlRating.Controls.Add(btnDeleteRating);

            pnlRating.ResumeLayout();
        }

        private async Task LoadAverageRatingAsync()
        {
            try
            {
                var averageRating = await _ratingService.GetAverageRatingAsync(_bookId);

                                if (lblAverageRating != null)
                {
                    lblAverageRating.Text = averageRating > 0 ?
                        $"Average Rating: {averageRating:F1} stars" :
                        "No ratings yet";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading average rating: {ex.Message}");
                if (lblAverageRating != null)
                {
                    lblAverageRating.Text = "Error loading rating";
                }
            }}

        #endregion

        #region Comments UI Methods
        
        private void InitializeCommentsUI()
        {
            if (pnlAllComments != null) return; // Already initialized
            
            // Create comments panel
            pnlAllComments = new Guna.UI2.WinForms.Guna2Panel();
            lblCommentsTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            flpComments = new FlowLayoutPanel();
            
            // Setup comments panel
            pnlAllComments.SuspendLayout();
            pnlAllComments.BorderColor = Color.FromArgb(224, 224, 224);
            pnlAllComments.BorderThickness = 1;
            pnlAllComments.FillColor = Color.White;
            pnlAllComments.BorderRadius = 8;
            pnlAllComments.Size = new Size(994, 300);
            pnlAllComments.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            
            // Setup comments title
            lblCommentsTitle.Text = "Reviews & Comments";
            lblCommentsTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblCommentsTitle.ForeColor = Color.FromArgb(0, 120, 215);
            lblCommentsTitle.Location = new Point(10, 10);
            lblCommentsTitle.AutoSize = true;
            pnlAllComments.Controls.Add(lblCommentsTitle);
            
            // Setup comments flow panel
            flpComments.Location = new Point(10, 40);
            flpComments.Size = new Size(974, 250);
            flpComments.AutoScroll = true;
            flpComments.FlowDirection = FlowDirection.TopDown;
            flpComments.WrapContents = false;
            flpComments.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlAllComments.Controls.Add(flpComments);
            
            pnlAllComments.ResumeLayout();
              // Add comments panel to main panel and position it below rtbContent specifically
            pnlContent.Controls.Add(pnlAllComments);
            
            // Position comments panel below rtbContent with proper spacing
            var rtbBounds = rtbContent.Bounds;
            pnlAllComments.Location = new Point(rtbBounds.X, rtbBounds.Bottom + 20);
            
            // Bring to front to ensure visibility above all other controls
            pnlAllComments.BringToFront();
            pnlAllComments.Visible = true;
            
            // Ensure parent container can accommodate the comments panel
            pnlContent.AutoScroll = true;
        }
          private Panel CreateCommentPanel(Rating rating)
        {
            var panel = new Panel();
            panel.Size = new Size(950, 80);
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Margin = new Padding(0, 0, 0, 10);
            
            // Apply theme-appropriate colors to panel
            if (_isDarkMode)
            {
                panel.BackColor = Color.FromArgb(70, 70, 73);
            }
            else
            {
                panel.BackColor = Color.FromArgb(248, 249, 250);
            }
            
            // User name label
            var lblUser = new Label();
            lblUser.Text = $"User {rating.UserId}"; // You might want to load actual username
            lblUser.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblUser.Location = new Point(10, 5);
            lblUser.AutoSize = true;
            
            // Apply theme-appropriate colors to username
            if (_isDarkMode)
            {
                lblUser.ForeColor = Color.FromArgb(100, 180, 255);
            }
            else
            {
                lblUser.ForeColor = Color.FromArgb(0, 120, 215);
            }
            panel.Controls.Add(lblUser);
            
            // Rating stars
            var lblStars = new Label();
            lblStars.Text = new string('★', rating.RatingValue) + new string('☆', 5 - rating.RatingValue);
            lblStars.Font = new Font("Segoe UI", 10F);
            lblStars.ForeColor = Color.Gold; // Gold color works for both themes
            lblStars.Location = new Point(10, 25);
            lblStars.AutoSize = true;
            panel.Controls.Add(lblStars);
            
            // Date
            var lblDate = new Label();
            lblDate.Text = rating.CreatedDate.ToString("MMM dd, yyyy");
            lblDate.Font = new Font("Segoe UI", 8F);
            lblDate.Location = new Point(panel.Width - 100, 5);
            lblDate.AutoSize = true;
            
            // Apply theme-appropriate colors to date
            if (_isDarkMode)
            {
                lblDate.ForeColor = Color.LightGray;
            }
            else
            {
                lblDate.ForeColor = Color.Gray;
            }
            panel.Controls.Add(lblDate);
            
            // Review text
            var lblReview = new Label();
            lblReview.Text = rating.Review ?? "";
            lblReview.Font = new Font("Segoe UI", 9F);
            lblReview.Location = new Point(10, 45);
            lblReview.Size = new Size(930, 30);
            lblReview.AutoEllipsis = true;
            
            // Apply theme-appropriate colors to review text
            if (_isDarkMode)
            {
                lblReview.ForeColor = Color.White;
            }
            else
            {
                lblReview.ForeColor = Color.FromArgb(50, 50, 50);
            }
            panel.Controls.Add(lblReview);
            
            return panel;}
        
        #endregion

        #region Missing Methods
        private async Task LoadAllCommentsAsync()
        {
            try
            {
                // Initialize comment UI controls if not already done
                InitializeCommentsUI();
                
                // Get all ratings/reviews for this book
                var ratings = await _ratingService.GetBookRatingsAsync(_bookId);
                var commentsWithReviews = ratings.Where(r => !string.IsNullOrWhiteSpace(r.Review)).ToList();
                
                if (flpComments != null)
                {
                    flpComments.Controls.Clear();
                    
                    if (commentsWithReviews.Any())
                    {
                        lblCommentsTitle.Text = $"Reviews & Comments ({commentsWithReviews.Count})";
                        
                        foreach (var rating in commentsWithReviews.OrderByDescending(r => r.CreatedDate))
                        {
                            var commentPanel = CreateCommentPanel(rating);
                            flpComments.Controls.Add(commentPanel);
                        }
                        
                        pnlAllComments.Visible = true;
                    }
                    else
                    {
                        lblCommentsTitle.Text = "No reviews yet";
                        pnlAllComments.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading comments: {ex.Message}");
                if (lblCommentsTitle != null)
                {
                    lblCommentsTitle.Text = "Error loading comments";
                }
            }
        }        private void ApplyTheme()
        {
            // Apply theme based on _isDarkMode
            if (_isDarkMode)
            {
                // Dark mode colors
                this.BackColor = Color.FromArgb(45, 45, 48);
                this.ForeColor = Color.White;
                
                // Apply dark theme to panels
                pnlMain.FillColor = Color.FromArgb(45, 45, 48);
                pnlTop.FillColor = Color.FromArgb(55, 55, 58);
                pnlRight.FillColor = Color.FromArgb(55, 55, 58);
                pnlContent.FillColor = Color.FromArgb(45, 45, 48);
                
                // Apply dark theme to text controls
                rtbContent.BackColor = Color.FromArgb(60, 60, 63);
                rtbContent.ForeColor = Color.White;
                txtDescription.FillColor = Color.FromArgb(60, 60, 63);
                txtDescription.ForeColor = Color.White;
                
                // Apply dark theme to labels
                lblBookTitle.ForeColor = Color.White;
                lblAuthor.ForeColor = Color.LightGray;
                lblStatus.ForeColor = Color.LightGray;
                lblTotalChapters.ForeColor = Color.LightGray;
                lblViewCount.ForeColor = Color.LightGray;
                lblChapters.ForeColor = Color.White;
                
                // Apply dark theme to combobox
                cmbChapters.FillColor = Color.FromArgb(60, 60, 63);
                cmbChapters.ForeColor = Color.White;
                
                // Apply dark theme to rating panel and comments
                if (pnlRating != null)
                {
                    pnlRating.FillColor = Color.FromArgb(60, 60, 63);
                    pnlRating.BorderColor = Color.FromArgb(80, 80, 83);
                    
                    if (lblRatingTitle != null) lblRatingTitle.ForeColor = Color.White;
                    if (lblCurrentRating != null) lblCurrentRating.ForeColor = Color.LightGray;
                    if (lblAverageRating != null) lblAverageRating.ForeColor = Color.FromArgb(255, 140, 0);
                    if (txtComment != null)
                    {
                        txtComment.FillColor = Color.FromArgb(70, 70, 73);
                        txtComment.ForeColor = Color.White;
                        txtComment.BorderColor = Color.FromArgb(100, 100, 103);
                    }
                }
                
                // Apply dark theme to comments panel
                if (pnlAllComments != null)
                {
                    pnlAllComments.FillColor = Color.FromArgb(60, 60, 63);
                    pnlAllComments.BorderColor = Color.FromArgb(80, 80, 83);
                    
                    if (lblCommentsTitle != null) lblCommentsTitle.ForeColor = Color.White;
                    
                    // Update existing comment panels
                    if (flpComments != null)
                    {
                        foreach (Control control in flpComments.Controls)
                        {
                            if (control is Panel commentPanel)
                            {
                                commentPanel.BackColor = Color.FromArgb(70, 70, 73);
                                foreach (Control child in commentPanel.Controls)
                                {
                                    if (child is Label label)
                                    {
                                        if (label.Font.Bold) // Username
                                            label.ForeColor = Color.FromArgb(100, 180, 255);
                                        else if (label.ForeColor == Color.Gold) // Stars
                                            label.ForeColor = Color.Gold;
                                        else if (label.ForeColor == Color.Gray) // Date
                                            label.ForeColor = Color.LightGray;
                                        else // Review text
                                            label.ForeColor = Color.White;
                                    }
                                }
                            }
                        }
                    }
                }
                
                // Update toggle button text
                btnToggleTheme.Text = "☀️ Light Mode";
            }
            else
            {
                // Light mode colors
                this.BackColor = Color.White;
                this.ForeColor = Color.Black;
                
                // Apply light theme to panels
                pnlMain.FillColor = Color.White;
                pnlTop.FillColor = Color.FromArgb(250, 250, 250);
                pnlRight.FillColor = Color.FromArgb(240, 240, 240);
                pnlContent.FillColor = Color.White;
                
                // Apply light theme to text controls
                rtbContent.BackColor = Color.FromArgb(250, 250, 250);
                rtbContent.ForeColor = Color.Black;
                txtDescription.FillColor = Color.White;
                txtDescription.ForeColor = Color.Black;
                
                // Apply light theme to labels
                lblBookTitle.ForeColor = Color.FromArgb(44, 62, 80);
                lblAuthor.ForeColor = Color.FromArgb(100, 100, 100);
                lblStatus.ForeColor = Color.FromArgb(100, 100, 100);
                lblTotalChapters.ForeColor = Color.FromArgb(100, 100, 100);
                lblViewCount.ForeColor = Color.FromArgb(100, 100, 100);
                lblChapters.ForeColor = Color.Black;
                
                // Apply light theme to combobox
                cmbChapters.FillColor = Color.White;
                cmbChapters.ForeColor = Color.FromArgb(68, 88, 112);
                
                // Apply light theme to rating panel and comments
                if (pnlRating != null)
                {
                    pnlRating.FillColor = Color.White;
                    pnlRating.BorderColor = Color.FromArgb(224, 224, 224);
                    
                    if (lblRatingTitle != null) lblRatingTitle.ForeColor = Color.FromArgb(0, 120, 215);
                    if (lblCurrentRating != null) lblCurrentRating.ForeColor = Color.FromArgb(50, 50, 50);
                    if (lblAverageRating != null) lblAverageRating.ForeColor = Color.FromArgb(255, 140, 0);
                    if (txtComment != null)
                    {
                        txtComment.FillColor = Color.White;
                        txtComment.ForeColor = Color.FromArgb(50, 50, 50);
                        txtComment.BorderColor = Color.FromArgb(224, 224, 224);
                    }
                }
                
                // Apply light theme to comments panel
                if (pnlAllComments != null)
                {
                    pnlAllComments.FillColor = Color.White;
                    pnlAllComments.BorderColor = Color.FromArgb(224, 224, 224);
                    
                    if (lblCommentsTitle != null) lblCommentsTitle.ForeColor = Color.FromArgb(44, 62, 80);
                    
                    // Update existing comment panels
                    if (flpComments != null)
                    {
                        foreach (Control control in flpComments.Controls)
                        {
                            if (control is Panel commentPanel)
                            {
                                commentPanel.BackColor = Color.FromArgb(248, 249, 250);
                                foreach (Control child in commentPanel.Controls)
                                {
                                    if (child is Label label)
                                    {
                                        if (label.Font.Bold) // Username
                                            label.ForeColor = Color.FromArgb(0, 120, 215);
                                        else if (label.ForeColor == Color.Gold) // Stars
                                            label.ForeColor = Color.Gold;
                                        else if (label.ForeColor == Color.LightGray || label.ForeColor == Color.Gray) // Date
                                            label.ForeColor = Color.Gray;
                                        else // Review text
                                            label.ForeColor = Color.FromArgb(50, 50, 50);
                                    }
                                }
                            }
                        }
                    }
                }
                      // Update toggle button text
                btnToggleTheme.Text = "🌙 Dark Mode";
            }
            
            // Update favorite button UI to match theme
            UpdateFavoriteButtonUI();
        }

        private void EnsureProperLayout()
        {
            // Ensure proper layout - placeholder
        }        private async Task RefreshMyBooksIfOpenAsync()
        {
            // Find any open MyBooks form and refresh it
            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType().Name == "MyBooks")
                {
                    // Refresh MyBooks form if it has a refresh method
                    // For now, just a placeholder
                    break;
                }
            }
            await Task.CompletedTask;
        }

        private async Task LoadUserRatingAsync()
        {
            if (_currentUser == null) return;

            try
            {
                var userRating = await _ratingService.GetUserRatingAsync(_currentUser.UserId, _bookId);                if (userRating != null)
                {
                    _selectedRating = userRating.RatingValue;
                    _hasExistingRating = true;
                    UpdateStarDisplay();
                    txtComment.Text = userRating.Review ?? "";
                    lblCurrentRating.Text = $"Your rating: {_selectedRating} stars";
                    btnDeleteRating.Visible = true;
                }
                else
                {
                    _selectedRating = 0;
                    _hasExistingRating = false;
                    UpdateStarDisplay();
                    txtComment.Text = "";
                    lblCurrentRating.Text = "No rating submitted.";
                    btnDeleteRating.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user rating: {ex.Message}");
            }
        }

        private void UpdateRatingPanelVisibility()
        {
            if (pnlRating != null)
            {
                pnlRating.Visible = _currentUser != null;
            }
        }

        private void AddStarHoverEffects()
        {
            if (starButtons == null) return;

            for (int i = 0; i < starButtons.Length; i++)
            {
                int starIndex = i; // Capture for closure
                starButtons[i].MouseEnter += (s, e) => {
                    for (int j = 0; j <= starIndex; j++)
                    {
                        starButtons[j].ForeColor = Color.Gold;
                    }
                    for (int j = starIndex + 1; j < starButtons.Length; j++)
                    {
                        starButtons[j].ForeColor = Color.LightGray;
                    }
                };

                starButtons[i].MouseLeave += (s, e) => {
                    UpdateStarDisplay();
                };
            }
        }

        private void UpdateStarDisplay()
        {
            if (starButtons == null) return;

            for (int i = 0; i < starButtons.Length; i++)
            {
                starButtons[i].ForeColor = i < _selectedRating ? Color.Gold : Color.LightGray;
            }
        }        private void StarButton_Click(object sender, EventArgs e)
        {
            if (starButtons == null) return;

            for (int i = 0; i < starButtons.Length; i++)
            {
                if (starButtons[i] == sender)
                {
                    // Toggle functionality: if clicking the same star as current rating, remove the rating
                    if (_selectedRating == i + 1)
                    {
                        _selectedRating = 0; // Remove rating
                    }
                    else
                    {
                        _selectedRating = i + 1; // Set new rating
                    }
                    UpdateStarDisplay();
                    break;
                }
            }
        }

        private async void BtnSubmitRating_Click(object sender, EventArgs e)
        {
            if (_currentUser == null || _selectedRating == 0)
            {
                MessageBox.Show("Please select a rating!", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnSubmitRating.Enabled = false;
                btnSubmitRating.Text = "Submitting...";                var rating = new Rating
                {
                    UserId = _currentUser.UserId,
                    BookId = _bookId,
                    RatingValue = _selectedRating,
                    Review = txtComment.Text?.Trim(),
                    CreatedDate = DateTime.Now
                };                bool success;
                var result = await _ratingService.AddOrUpdateRatingAsync(_currentUser.UserId, _bookId, _selectedRating, txtComment.Text?.Trim());
                success = result != null;                if (success)
                {
                    _hasExistingRating = true;
                    lblCurrentRating.Text = $"Your rating: {_selectedRating} stars";
                    btnDeleteRating.Visible = true;
                    await LoadAverageRatingAsync();
                    await LoadAllCommentsAsync(); // Refresh comments after new rating
                    MessageBox.Show("Rating submitted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to submit rating!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error submitting rating: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSubmitRating.Enabled = true;
                btnSubmitRating.Text = "Submit Rating";
            }
        }

        private async void BtnDeleteRating_Click(object sender, EventArgs e)
        {
            if (_currentUser == null) return;

            var result = MessageBox.Show("Are you sure you want to delete your rating?", "Confirm Delete", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    btnDeleteRating.Enabled = false;
                    btnDeleteRating.Text = "Deleting...";

                    bool success = await _ratingService.DeleteRatingAsync(_currentUser.UserId, _bookId);                    if (success)
                    {
                        _selectedRating = 0;
                        _hasExistingRating = false;
                        UpdateStarDisplay();
                        txtComment.Text = "";
                        lblCurrentRating.Text = "No rating submitted.";
                        btnDeleteRating.Visible = false;
                        await LoadAverageRatingAsync();
                        await LoadAllCommentsAsync(); // Refresh comments after deletion
                        MessageBox.Show("Rating deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete rating!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting rating: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    btnDeleteRating.Enabled = true;
                    btnDeleteRating.Text = "Delete Rating";
                }
            }
        }        private void rtbContent_DoubleClick(object sender, EventArgs e)
        {
            // Placeholder for double-click functionality
        }
        
        /// <summary>
        /// Force immediate real-time coin balance update in Main form
        /// </summary>
        private async Task ForceMainFormCoinsUpdateAsync()
        {
            try
            {
                // Find the Main form from this BookDetail form
                Form? parentForm = this.Owner;
                if (parentForm == null)
                {
                    // Try to find Main form in all open forms
                    parentForm = Application.OpenForms.OfType<Main>().FirstOrDefault();
                }
                
                if (parentForm is Main mainForm)
                {
                    await mainForm.ForceCoinsUpdateAsync();
                    Debug.WriteLine("✅ Successfully triggered real-time coins update in Main form");
                }
                else
                {
                    Debug.WriteLine("⚠️ Could not find Main form to trigger coins update");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error triggering real-time coins update: {ex.Message}");
            }
        }
        #endregion
    }
}