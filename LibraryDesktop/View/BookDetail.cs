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
        #region Fields
        private int _bookId;
        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private readonly IRatingService _ratingService;
        private Book? _currentBook;
        private List<Chapter> _chapters = new List<Chapter>();
        private User? _currentUser;

        // Theme
        private bool _isDarkMode = false;

        // Rating state
        private int _selectedRating = 0;
        private bool _hasExistingRating = false;

        private bool _isFavorite = false;

        #endregion

        #region Constructor and Setup
        public BookDetail(IBookService bookService, IUserService userService, IRatingService ratingService)
        {
            InitializeComponent();
            _bookService = bookService;
            _userService = userService;
            _ratingService = ratingService;
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
                    lblViewCount.Text = $"Views: {(_currentBook.ViewCount + 1):N0}";
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

                string content = await _bookService.GetChapterContentAsync(selectedChapter.GitHubContentUrl);

                if (!string.IsNullOrEmpty(content) && !content.StartsWith("Error loading"))
                {
                    rtbContent.Text = content;
                    MessageBox.Show("Story downloaded successfully!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        }

        // ENSURE THIS METHOD HAS ASYNC:
        private async void btnFavorite_Click(object sender, EventArgs e) // ALREADY HAS ASYNC
        {
            if (_currentUser == null)
            {
                MessageBox.Show("Please log in to add the book to favorites!", "Notification",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnFavorite.Enabled = false;
                btnFavorite.Text = "Adding...";

                bool success = await _userService.AddToFavoritesAsync(_currentUser.UserId, _bookId);

                if (success)
                {
                    _isFavorite = true;
                    UpdateFavoriteButtonUI();
                    MessageBox.Show("Book added to favorites!", "Notification",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Book is already in favorites!", "Notification",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _isFavorite = true;
                    UpdateFavoriteButtonUI();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding to favorites: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"btnFavorite_Click Error: {ex}");

                btnFavorite.Text = "❤️ Add to Favorites";
                btnFavorite.Enabled = true;
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
        }

        private async Task LoadBookInfoAsync()
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
        }

        private void UpdateFavoriteButtonUI()
        {
            if (_isFavorite)
            {
                // HIDE BUTTON WHEN ALREADY FAVORITED (will be removed from MyBooks.cs)
                btnFavorite.Visible = false;
            }
            else
            {
                // SHOW BUTTON WHEN NOT YET FAVORITED
                btnFavorite.Visible = true;
                btnFavorite.Text = "❤️ Add to Favorites";
                btnFavorite.Enabled = true;

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
                cmbChapters.Items.Clear();

                if (_currentBook?.Chapters != null && _currentBook.Chapters.Any())
                {
                    _chapters = _currentBook.Chapters.OrderBy(c => c.ChapterNumber).ToList();

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
                Debug.WriteLine($"Error loading average rating: {ex.Message}");
            }
        }

        private async Task LoadUserRatingAsync()
        {
            try
            {
                if (_currentUser != null)
                {
                    var userRating = await _ratingService.GetUserRatingAsync(_currentUser.UserId, _bookId);

                    if (userRating != null)
                    {
                        _selectedRating = userRating.RatingValue;
                        _hasExistingRating = true;

                        if (lblCurrentRating != null)
                            lblCurrentRating.Text = $"Your Rating: {_selectedRating} stars";

                        if (starButtons != null)
                        {
                            for (int i = 0; i < 5; i++)
                                starButtons[i].ForeColor = i < _selectedRating ? Color.Gold : Color.LightGray;
                        }

                        if (txtComment != null)
                            txtComment.Text = userRating.Review ?? string.Empty;

                        if (btnSubmitRating != null)
                            btnSubmitRating.Text = "Update Rating";

                        if (btnDeleteRating != null)
                            btnDeleteRating.Visible = true;
                    }
                    else
                    {
                        ResetRatingToDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user rating: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetRatingToDefault()
        {
            _selectedRating = 0;
            _hasExistingRating = false;

            if (lblCurrentRating != null)
                lblCurrentRating.Text = "No rating submitted.";

            if (starButtons != null)
            {
                for (int i = 0; i < 5; i++)
                    starButtons[i].ForeColor = Color.LightGray;
            }

            if (btnSubmitRating != null)
                btnSubmitRating.Text = "Submit Rating";

            if (btnDeleteRating != null)
                btnDeleteRating.Visible = false;
        }

        private async void StarButton_Click(object? sender, EventArgs e)
        {
            try
            {
                var clickedButton = sender as Guna.UI2.WinForms.Guna2Button;

                if (clickedButton != null && starButtons != null)
                {
                    int ratingValue = Array.IndexOf(starButtons, clickedButton) + 1;

                    if (_selectedRating == ratingValue)
                    {
                        _selectedRating = 0;
                        if (lblCurrentRating != null)
                            lblCurrentRating.Text = _hasExistingRating ? "Click a star to change rating" : "No rating submitted.";
                        if (btnSubmitRating != null)
                            btnSubmitRating.Text = _hasExistingRating ? "Update Rating" : "Submit Rating";
                    }
                    else
                    {
                        _selectedRating = ratingValue;
                        if (lblCurrentRating != null)
                            lblCurrentRating.Text = $"Your Rating: {_selectedRating} stars";
                        if (btnSubmitRating != null)
                            btnSubmitRating.Text = _hasExistingRating ? "Update Rating" : "Submit Rating";
                    }

                    for (int i = 0; i < 5; i++)
                        starButtons[i].ForeColor = i < _selectedRating ? Color.Gold : Color.LightGray;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting rating: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnSubmitRating_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_currentUser == null)
                {
                    MessageBox.Show("Please log in to rate the book!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_selectedRating <= 0)
                {
                    MessageBox.Show("Please select a star rating!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string comment = txtComment?.Text?.Trim() ?? string.Empty;

                if (btnSubmitRating != null)
                {
                    btnSubmitRating.Enabled = false;
                    btnSubmitRating.Text = "Submitting...";
                }

                await _ratingService.AddOrUpdateRatingAsync(_currentUser.UserId, _bookId, _selectedRating, comment);

                MessageBox.Show("Thank you for rating the book!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (lblCurrentRating != null)
                    lblCurrentRating.Text = $"Your Rating: {_selectedRating} stars";

                _hasExistingRating = true;
                if (btnDeleteRating != null)
                    btnDeleteRating.Visible = true;

                await LoadAverageRatingAsync();
                await LoadAllCommentsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error submitting rating: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (btnSubmitRating != null)
                {
                    btnSubmitRating.Enabled = true;
                    btnSubmitRating.Text = "Submit Rating";
                }
            }
        }

        private async void BtnDeleteRating_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_currentUser == null)
                {
                    MessageBox.Show("Please log in!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to delete this rating?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes) return;

                if (btnDeleteRating != null)
                {
                    btnDeleteRating.Enabled = false;
                    btnDeleteRating.Text = "Deleting...";
                }

                await _ratingService.DeleteRatingAsync(_currentUser.UserId, _bookId);

                MessageBox.Show("Rating deleted successfully!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ResetRatingToDefault();

                if (txtComment != null)
                    txtComment.Text = string.Empty;

                await LoadAverageRatingAsync();
                await LoadAllCommentsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting rating: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (btnDeleteRating != null)
                {
                    btnDeleteRating.Enabled = true;
                    btnDeleteRating.Text = "Delete Rating";
                }
            }
        }

        private void UpdateRatingPanelVisibility()
        {
            if (pnlRating != null)
            {
                pnlRating.Visible = _currentUser != null;

                if (_currentUser == null && lblCurrentRating != null)
                {
                    lblCurrentRating.Text = "Log in to rate the book";
                    lblCurrentRating.ForeColor = Color.FromArgb(120, 120, 120);
                }
            }
        }

        private void AddStarHoverEffects()
        {
            if (starButtons == null) return;

            for (int i = 0; i < starButtons.Length; i++)
            {
                int starIndex = i;

                starButtons[i].MouseEnter += (s, e) =>
                {
                    if (_currentUser == null) return;

                    for (int j = 0; j <= starIndex; j++)
                        if (starButtons[j] != null) starButtons[j].ForeColor = Color.Gold;

                    for (int j = starIndex + 1; j < 5; j++)
                        if (starButtons[j] != null) starButtons[j].ForeColor = Color.LightGray;
                };

                starButtons[i].MouseLeave += (s, e) =>
                {
                    if (_currentUser == null) return;

                    for (int j = 0; j < 5; j++)
                        if (starButtons[j] != null) starButtons[j].ForeColor = j < _selectedRating ? Color.Gold : Color.LightGray;
                };
            }
        }
        #endregion

        #region Comments Methods (CLEANED UP)
        private async Task LoadAllCommentsAsync()
        {
            try
            {
                InitializeCommentsUI();

                var allRatings = await _ratingService.GetBookRatingsAsync(_bookId);

                if (allRatings?.Any() == true)
                {
                    foreach (var rating in allRatings.OrderByDescending(r => r.CreatedDate))
                    {
                        var commentPanel = CreateCommentPanel(rating);
                        flpComments?.Controls.Add(commentPanel);
                    }

                    if (lblCommentsTitle != null)
                        lblCommentsTitle.Text = $"All Reviews ({allRatings.Count()})";
                }
                else
                {
                    var noCommentsLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
                    noCommentsLabel.Text = "No reviews for this book yet.";
                    noCommentsLabel.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
                    noCommentsLabel.ForeColor = Color.FromArgb(120, 120, 120);
                    noCommentsLabel.Location = new Point(5, 5);
                    noCommentsLabel.AutoSize = true;
                    flpComments?.Controls.Add(noCommentsLabel);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading comments: {ex.Message}");
            }
        }

        private void InitializeCommentsUI()
        {
            if (pnlAllComments == null)
            {
                pnlAllComments = new Guna.UI2.WinForms.Guna2Panel();
                lblCommentsTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
                flpComments = new FlowLayoutPanel();
            }

            pnlAllComments.Controls.Clear();
            pnlAllComments.SuspendLayout();

            // Configure panel
            pnlAllComments.BorderColor = Color.FromArgb(224, 224, 224);
            pnlAllComments.BorderThickness = 1;
            pnlAllComments.FillColor = Color.White;
            pnlAllComments.BorderRadius = 8;

            pnlContent.AutoScroll = true;
            rtbContent.Dock = DockStyle.None;
            rtbContent.Location = new Point(17, 20);
            rtbContent.Size = new Size(970, 500);
            rtbContent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            pnlAllComments.Location = new Point(17, 540);
            pnlAllComments.Size = new Size(970, 300);
            pnlAllComments.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlContent.Controls.Add(pnlAllComments);

            // Title
            lblCommentsTitle.Text = "All Reviews";
            lblCommentsTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblCommentsTitle.ForeColor = Color.FromArgb(0, 120, 215);
            lblCommentsTitle.Location = new Point(10, 10);
            lblCommentsTitle.AutoSize = true;
            pnlAllComments.Controls.Add(lblCommentsTitle);

            // Flow layout
            flpComments.FlowDirection = FlowDirection.TopDown;
            flpComments.WrapContents = false;
            flpComments.AutoScroll = true;
            flpComments.Location = new Point(10, 40);
            flpComments.Size = new Size(920, 250);
            flpComments.BackColor = Color.White;
            pnlAllComments.Controls.Add(flpComments);

            pnlAllComments.ResumeLayout();
        }

        private Panel CreateCommentPanel(Rating rating)
        {
            var commentPanel = new Panel();
            commentPanel.Size = new Size(900, 100);
            commentPanel.BorderStyle = BorderStyle.FixedSingle;
            commentPanel.Margin = new Padding(5, 5, 5, 10);

            // Theme colors
            commentPanel.BackColor = _isDarkMode ? Color.FromArgb(45, 45, 45) : Color.FromArgb(248, 249, 250);

            // User name
            var userLabel = new Label();
            userLabel.Text = $"{rating.User?.Username ?? "Anonymous"}";
            userLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            userLabel.Location = new Point(15, 10);
            userLabel.Size = new Size(200, 20);
            userLabel.ForeColor = _isDarkMode ? Color.FromArgb(220, 220, 220) : Color.FromArgb(52, 58, 64);
            commentPanel.Controls.Add(userLabel);

            // Star rating display
            var starsLabel = new Label();
            starsLabel.Text = new string('★', rating.RatingValue) + new string('☆', 5 - rating.RatingValue);
            starsLabel.Font = new Font("Segoe UI", 12F);
            starsLabel.ForeColor = Color.FromArgb(255, 193, 7); // Gold color
            starsLabel.Location = new Point(230, 8);
            starsLabel.Size = new Size(100, 22);
            commentPanel.Controls.Add(starsLabel);

            // Date
            var dateLabel = new Label();
            dateLabel.Text = rating.CreatedDate.ToString("dd/MM/yyyy HH:mm");
            dateLabel.Font = new Font("Segoe UI", 9F);
            dateLabel.Location = new Point(680, 10);
            dateLabel.Size = new Size(160, 18);
            dateLabel.ForeColor = _isDarkMode ? Color.FromArgb(150, 150, 150) : Color.FromArgb(108, 117, 125);
            commentPanel.Controls.Add(dateLabel);

            // Comment text
            if (!string.IsNullOrEmpty(rating.Review))
            {
                var commentLabel = new Label();
                commentLabel.Text = rating.Review.Length > 200 ?
                    rating.Review.Substring(0, 200) + "..." : rating.Review;
                commentLabel.Font = new Font("Segoe UI", 9F);
                commentLabel.Location = new Point(15, 35);
                commentLabel.Size = new Size(860, 50);
                commentLabel.AutoEllipsis = true;
                commentLabel.ForeColor = _isDarkMode ? Color.FromArgb(180, 180, 180) : Color.FromArgb(73, 80, 87);
                commentPanel.Controls.Add(commentLabel);

                // Tooltip for full comment
                if (rating.Review.Length > 200)
                {
                    var tooltip = new ToolTip();
                    tooltip.SetToolTip(commentLabel, rating.Review);
                }
            }

            return commentPanel;
        }
        #endregion

        #region Theme Methods
        private void ApplyTheme()
        {
            if (_isDarkMode)
            {
                ApplyDarkTheme();
                btnToggleTheme.Text = "☀️ Light Mode";
                btnToggleTheme.FillColor = Color.FromArgb(255, 193, 7); // Yellow
            }
            else
            {
                ApplyLightTheme();
                btnToggleTheme.Text = "🌙 Dark Mode";
                btnToggleTheme.FillColor = Color.FromArgb(95, 39, 205); // Purple
            }
        }

        private void ApplyDarkTheme()
        {
            // Main content area
            rtbContent.BackColor = Color.FromArgb(30, 30, 30);
            rtbContent.ForeColor = Color.FromArgb(220, 220, 220);
            pnlContent.FillColor = Color.FromArgb(40, 40, 40);

            // Top panel
            pnlTop.FillColor = Color.FromArgb(35, 35, 35);
            lblBookTitle.ForeColor = Color.FromArgb(220, 220, 220);
            lblAuthor.ForeColor = Color.FromArgb(180, 180, 180);
            lblStatus.ForeColor = Color.FromArgb(180, 180, 180);
            lblTotalChapters.ForeColor = Color.FromArgb(180, 180, 180);
            lblViewCount.ForeColor = Color.FromArgb(180, 180, 180);

            // Description text box
            txtDescription.FillColor = Color.FromArgb(45, 45, 45);
            txtDescription.ForeColor = Color.FromArgb(200, 200, 200);
            txtDescription.BorderColor = Color.FromArgb(70, 70, 70);

            // Right panel
            pnlRight.FillColor = Color.FromArgb(35, 35, 35);
            lblChapters.ForeColor = Color.FromArgb(220, 220, 220);

            // ComboBox
            cmbChapters.FillColor = Color.FromArgb(45, 45, 45);
            cmbChapters.ForeColor = Color.FromArgb(200, 200, 200);
            cmbChapters.BorderColor = Color.FromArgb(70, 70, 70);

            // Main form
            this.BackColor = Color.FromArgb(25, 25, 25);
            pnlMain.FillColor = Color.FromArgb(30, 30, 30);

            // Rating panel
            ApplyDarkThemeToRating();

            // Comments panel
            ApplyDarkThemeToComments();
            if (btnFavorite.Visible && !_isFavorite)
            {
                btnFavorite.FillColor = Color.FromArgb(185, 28, 28); // Dark red for dark mode
            }
        }

        private void ApplyLightTheme()
        {
            // Main content area
            rtbContent.BackColor = Color.FromArgb(250, 250, 250);
            rtbContent.ForeColor = Color.FromArgb(50, 50, 50);
            pnlContent.FillColor = Color.White;

            // Top panel
            pnlTop.FillColor = Color.FromArgb(250, 250, 250);
            lblBookTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblAuthor.ForeColor = Color.FromArgb(100, 100, 100);
            lblStatus.ForeColor = Color.FromArgb(100, 100, 100);
            lblTotalChapters.ForeColor = Color.FromArgb(100, 100, 100);
            lblViewCount.ForeColor = Color.FromArgb(100, 100, 100);

            // Description text box
            txtDescription.FillColor = Color.White;
            txtDescription.ForeColor = Color.FromArgb(50, 50, 50);
            txtDescription.BorderColor = Color.FromArgb(213, 218, 223);

            // Right panel
            pnlRight.FillColor = Color.FromArgb(240, 240, 240);
            lblChapters.ForeColor = Color.FromArgb(50, 50, 50);

            // ComboBox
            cmbChapters.FillColor = Color.White;
            cmbChapters.ForeColor = Color.FromArgb(68, 88, 112);
            cmbChapters.BorderColor = Color.FromArgb(213, 218, 223);

            // Main form
            this.BackColor = Color.FromArgb(240, 240, 240);
            pnlMain.FillColor = Color.White;

            // Rating panel
            ApplyLightThemeToRating();

            // Comments panel
            ApplyLightThemeToComments();
            if (btnFavorite.Visible && !_isFavorite)
            {
                btnFavorite.FillColor = Color.FromArgb(220, 53, 69); // Normal red for light mode
            }
        }

        private void ApplyDarkThemeToRating()
        {
            if (pnlRating != null)
            {
                pnlRating.FillColor = Color.FromArgb(40, 40, 40);
                pnlRating.BorderColor = Color.FromArgb(70, 70, 70);

                if (lblRatingTitle != null)
                    lblRatingTitle.ForeColor = Color.FromArgb(100, 149, 237);

                if (lblCurrentRating != null)
                    lblCurrentRating.ForeColor = Color.FromArgb(200, 200, 200);

                if (lblAverageRating != null)
                    lblAverageRating.ForeColor = Color.FromArgb(255, 140, 0);

                if (txtComment != null)
                {
                    txtComment.FillColor = Color.FromArgb(45, 45, 45);
                    txtComment.ForeColor = Color.FromArgb(200, 200, 200);
                    txtComment.BorderColor = Color.FromArgb(70, 70, 70);
                }
            }
        }

        private void ApplyLightThemeToRating()
        {
            if (pnlRating != null)
            {
                pnlRating.FillColor = Color.White;
                pnlRating.BorderColor = Color.FromArgb(224, 224, 224);

                if (lblRatingTitle != null)
                    lblRatingTitle.ForeColor = Color.FromArgb(0, 120, 215);

                if (lblCurrentRating != null)
                    lblCurrentRating.ForeColor = Color.FromArgb(50, 50, 50);

                if (lblAverageRating != null)
                    lblAverageRating.ForeColor = Color.FromArgb(255, 140, 0);

                if (txtComment != null)
                {
                    txtComment.FillColor = Color.White;
                    txtComment.ForeColor = Color.FromArgb(50, 50, 50);
                    txtComment.BorderColor = Color.FromArgb(224, 224, 224);
                }
            }
        }

        private void ApplyDarkThemeToComments()
        {
            if (pnlAllComments != null)
            {
                pnlAllComments.FillColor = Color.FromArgb(40, 40, 40);
                pnlAllComments.BorderColor = Color.FromArgb(70, 70, 70);

                if (lblCommentsTitle != null)
                    lblCommentsTitle.ForeColor = Color.FromArgb(100, 149, 237);

                if (flpComments != null)
                {
                    flpComments.BackColor = Color.FromArgb(40, 40, 40);

                    // Update existing comment panels
                    foreach (Control control in flpComments.Controls)
                    {
                        if (control is Panel commentPanel)
                        {
                            commentPanel.BackColor = Color.FromArgb(45, 45, 45);
                            ApplyThemeToCommentLabels(commentPanel, true);
                        }
                    }
                }
            }
        }

        private void ApplyLightThemeToComments()
        {
            if (pnlAllComments != null)
            {
                pnlAllComments.FillColor = Color.White;
                pnlAllComments.BorderColor = Color.FromArgb(224, 224, 224);

                if (lblCommentsTitle != null)
                    lblCommentsTitle.ForeColor = Color.FromArgb(0, 120, 215);

                if (flpComments != null)
                {
                    flpComments.BackColor = Color.White;

                    // Update existing comment panels
                    foreach (Control control in flpComments.Controls)
                    {
                        if (control is Panel commentPanel)
                        {
                            commentPanel.BackColor = Color.FromArgb(248, 249, 250);
                            ApplyThemeToCommentLabels(commentPanel, false);
                        }
                    }
                }
            }
        }

        private void ApplyThemeToCommentLabels(Panel commentPanel, bool isDark)
        {
            foreach (Control childControl in commentPanel.Controls)
            {
                if (childControl is Label label)
                {
                    if (isDark)
                    {
                        if (label.Font.Bold)
                            label.ForeColor = Color.FromArgb(220, 220, 220);
                        else if (label.Text.Contains("★"))
                            label.ForeColor = Color.FromArgb(255, 193, 7);
                        else if (label.Text.Contains("/"))
                            label.ForeColor = Color.FromArgb(150, 150, 150);
                        else
                            label.ForeColor = Color.FromArgb(180, 180, 180);
                    }
                    else
                    {
                        if (label.Font.Bold)
                            label.ForeColor = Color.FromArgb(52, 58, 64);
                        else if (label.Text.Contains("★"))
                            label.ForeColor = Color.FromArgb(255, 193, 7);
                        else if (label.Text.Contains("/"))
                            label.ForeColor = Color.FromArgb(108, 117, 125);
                        else
                            label.ForeColor = Color.FromArgb(73, 80, 87);
                    }
                }
            }
        }
        #endregion

        #region Reading Mode
        private void rtbContent_DoubleClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(rtbContent.Text) &&
                !rtbContent.Text.Contains("Loading") &&
                !rtbContent.Text.Contains("Select 'Download Story'") &&
                !rtbContent.Text.Contains("Error loading"))
            {
                ShowReadingMode();
            }
        }

        private void ShowReadingMode()
        {
            // Focus to reading area and scroll to top
            rtbContent.Focus();
            rtbContent.SelectionStart = 0;
            rtbContent.ScrollToCaret();

            // Improve reading experience with larger font size
            rtbContent.Font = new Font("Segoe UI", 12f);

            // Apply color based on current theme
            if (_isDarkMode)
            {
                rtbContent.BackColor = Color.FromArgb(25, 25, 25);
                rtbContent.ForeColor = Color.FromArgb(230, 230, 230);
            }
            else
            {
                rtbContent.BackColor = Color.FromArgb(255, 255, 255);
                rtbContent.ForeColor = Color.FromArgb(40, 40, 40);
            }

            MessageBox.Show("Switched to reading mode! Double-click to return to normal mode.",
                "Reading Mode", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region Helper Methods
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
        #endregion
    }
}