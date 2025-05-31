using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryDesktop.Data.Services;
using LibraryDesktop.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryDesktop.View
{
    public partial class MyBooks : UserControl
    {
        #region Fields
        private readonly IUserService _userService;
        private readonly IBookService _bookService;
        private readonly IRatingService _ratingService;
        private User? _currentUser;
        private List<UserFavorite> _favorites = new List<UserFavorite>();
        #endregion

        #region Constructor
        public MyBooks()
        {
            InitializeComponent();
            SetupDataGridView();
        }

        // Constructor với dependency injection
        public MyBooks(IUserService userService, IBookService bookService, IRatingService ratingService)
        {
            InitializeComponent();
            _userService = userService;
            _bookService = bookService;
            _ratingService = ratingService;
            SetupDataGridView();
        }

        // Method để set current user
        public void SetCurrentUser(User user)
        {
            _currentUser = user;
        }
        #endregion

        #region Setup Methods
        private void SetupDataGridView()
        {
            // Clear existing columns
            guna2DataGridView1.Columns.Clear();

            // Configure DataGridView
            guna2DataGridView1.AutoGenerateColumns = false;
            guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            guna2DataGridView1.MultiSelect = false;
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.AllowUserToAddRows = false;
            guna2DataGridView1.AllowUserToDeleteRows = false;

            // Add columns
            AddDataGridViewColumns();
        }

        private void AddDataGridViewColumns()
        {
            // Hidden BookId column
            var bookIdColumn = new DataGridViewTextBoxColumn();
            bookIdColumn.Name = "BookId";
            bookIdColumn.HeaderText = "BookId";
            bookIdColumn.DataPropertyName = "BookId";
            bookIdColumn.Visible = false;
            guna2DataGridView1.Columns.Add(bookIdColumn);

            // Title column
            var titleColumn = new DataGridViewTextBoxColumn();
            titleColumn.Name = "Title";
            titleColumn.HeaderText = "Tên sách";
            titleColumn.DataPropertyName = "Title";
            titleColumn.Width = 300;
            titleColumn.DefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            guna2DataGridView1.Columns.Add(titleColumn);

            // Author column
            var authorColumn = new DataGridViewTextBoxColumn();
            authorColumn.Name = "Author";
            authorColumn.HeaderText = "Tác giả";
            authorColumn.DataPropertyName = "Author";
            authorColumn.Width = 200;
            guna2DataGridView1.Columns.Add(authorColumn);

            // Total Chapters column
            var chaptersColumn = new DataGridViewTextBoxColumn();
            chaptersColumn.Name = "TotalChapters";
            chaptersColumn.HeaderText = "Số chương";
            chaptersColumn.DataPropertyName = "TotalChapters";
            chaptersColumn.Width = 120;
            chaptersColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            guna2DataGridView1.Columns.Add(chaptersColumn);

            // Category column
            var categoryColumn = new DataGridViewTextBoxColumn();
            categoryColumn.Name = "CategoryName";
            categoryColumn.HeaderText = "Thể loại";
            categoryColumn.DataPropertyName = "CategoryName";
            categoryColumn.Width = 150;
            guna2DataGridView1.Columns.Add(categoryColumn);

            // Status column
            var statusColumn = new DataGridViewTextBoxColumn();
            statusColumn.Name = "Status";
            statusColumn.HeaderText = "Trạng thái";
            statusColumn.DataPropertyName = "StatusText";
            statusColumn.Width = 120;
            statusColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            guna2DataGridView1.Columns.Add(statusColumn);

            // Added Date column
            var addedDateColumn = new DataGridViewTextBoxColumn();
            addedDateColumn.Name = "AddedDate";
            addedDateColumn.HeaderText = "Ngày thêm";
            addedDateColumn.DataPropertyName = "AddedDateText";
            addedDateColumn.Width = 120;
            addedDateColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            guna2DataGridView1.Columns.Add(addedDateColumn);
        }
        #endregion

        #region Event Handlers
        private async void MyBooks_Load(object sender, EventArgs e)
        {
            if (_currentUser != null && _userService != null)
            {
                await LoadFavoriteBooksAsync();
            }
            else
            {
                ShowNoUserMessage();
            }
        }

        private void guna2DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            bool hasSelection = guna2DataGridView1.SelectedRows.Count > 0;
            read_btn.Enabled = hasSelection;
            delete_btn.Enabled = hasSelection;
        }

        private async void read_btn_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một cuốn sách để đọc!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                read_btn.Enabled = false;
                read_btn.Text = "Đang mở...";

                var selectedRow = guna2DataGridView1.SelectedRows[0];
                var bookId = Convert.ToInt32(selectedRow.Cells["BookId"].Value);

                // Mở BookDetail form
                await OpenBookDetailAsync(bookId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở sách: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                read_btn.Text = "Read";
                read_btn.Enabled = guna2DataGridView1.SelectedRows.Count > 0;
            }
        }

        private async void delete_btn_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một cuốn sách để xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedRow = guna2DataGridView1.SelectedRows[0];
            var bookTitle = selectedRow.Cells["Title"].Value?.ToString();

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa \"{bookTitle}\" khỏi danh sách yêu thích?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    delete_btn.Enabled = false;
                    delete_btn.Text = "Đang xóa...";

                    var bookId = Convert.ToInt32(selectedRow.Cells["BookId"].Value);
                    bool success = await _userService.RemoveFromFavoritesAsync(_currentUser.UserId, bookId);

                    if (success)
                    {
                        MessageBox.Show("Đã xóa sách khỏi danh sách yêu thích!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Reload danh sách
                        await LoadFavoriteBooksAsync();
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa sách. Vui lòng thử lại!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa sách: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    delete_btn.Text = "Delete";
                    delete_btn.Enabled = guna2DataGridView1.SelectedRows.Count > 0;
                }
            }
        }
        #endregion

        #region Data Loading Methods
        private async Task LoadFavoriteBooksAsync()
        {
            try
            {
                SetLoadingState(true);

                _favorites = (await _userService.GetUserFavoritesAsync(_currentUser.UserId)).ToList();

                if (_favorites.Any())
                {
                    var displayData = _favorites.Select(f => new
                    {
                        BookId = f.Book.BookId,
                        Title = f.Book.Title,
                        Author = f.Book.Author,
                        TotalChapters = f.Book.TotalChapters,
                        CategoryName = f.Book.Category?.CategoryName ?? "Không xác định",
                        StatusText = f.Book.Status == BookStatus.Published ? "Đang phát hành" : "Dừng phát hành",
                        AddedDateText = f.AddedDate.ToString("dd/MM/yyyy")
                    }).ToList();

                    guna2DataGridView1.DataSource = displayData;
                    guna2HtmlLabel1.Text = $"My Books ({_favorites.Count} cuốn)";
                }
                else
                {
                    guna2DataGridView1.DataSource = null;
                    guna2HtmlLabel1.Text = "My Books (Trống)";
                    ShowEmptyMessage();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách yêu thích: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                guna2HtmlLabel1.Text = "My Books (Lỗi)";
            }
            finally
            {
                SetLoadingState(false);
            }
        }        private async Task OpenBookDetailAsync(int bookId)
        {
            if (_bookService == null || _ratingService == null)
            {
                MessageBox.Show("Service không khả dụng!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get IHistoryService from the main form's service provider
            var parentForm = this.FindForm();
            if (parentForm is Main mainForm)
            {
                var serviceProvider = mainForm.GetType()
                    .GetField("_serviceProvider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
                    .GetValue(mainForm) as IServiceProvider;

                if (serviceProvider != null)
                {
                    var historyService = serviceProvider.GetRequiredService<IHistoryService>();
                    
                    // Tạo và mở BookDetail form
                    var bookDetailForm = new BookDetail(_bookService, _userService, _ratingService, historyService);
                    bookDetailForm.SetBookId(bookId, _currentUser);

                    bookDetailForm.ShowDialog(parentForm);

                    // Refresh danh sách sau khi đóng BookDetail (trong trường hợp user thêm/xóa favorite từ đó)
                    await LoadFavoriteBooksAsync();
                }
                else
                {
                    MessageBox.Show("Cannot access services!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Cannot find parent form!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Helper Methods
        private void SetLoadingState(bool isLoading)
        {
            read_btn.Enabled = !isLoading && guna2DataGridView1.SelectedRows.Count > 0;
            delete_btn.Enabled = !isLoading && guna2DataGridView1.SelectedRows.Count > 0;
            guna2DataGridView1.Enabled = !isLoading;

            if (isLoading)
            {
                guna2HtmlLabel1.Text = "My Books (Đang tải...)";
            }
        }

        private void ShowNoUserMessage()
        {
            guna2DataGridView1.DataSource = null;
            read_btn.Enabled = false;
            delete_btn.Enabled = false;
        }

        private void ShowEmptyMessage()
        {
            MessageBox.Show("Bạn chưa có sách nào trong danh sách yêu thích!\nHãy thêm sách yêu thích từ trang chính.",
                "Danh sách trống", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Public method để refresh từ bên ngoài
        public async Task RefreshFavoritesAsync()
        {
            if (_currentUser != null && _userService != null)
            {
                await LoadFavoriteBooksAsync();
            }
        }
        #endregion
    }
}