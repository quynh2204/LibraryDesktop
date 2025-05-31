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
using System.Diagnostics;

namespace LibraryDesktop.View
{
    public partial class History : UserControl
    {
        private readonly IHistoryService? _historyService;
        private readonly IBookService? _bookService;
        private readonly IUserService? _userService;
        private User? _currentUser;
        private IEnumerable<LibraryDesktop.Models.History>? _historyData;

        // Parameterless constructor for designer compatibility
        public History()
        {
            InitializeComponent();
        }

        // Constructor for dependency injection
        public History(IHistoryService historyService, IBookService bookService, IUserService userService)
        {
            _historyService = historyService;
            _bookService = bookService;
            _userService = userService;
            InitializeComponent();
            SetupDataGridView();
            this.Load += History_Load;
        }

        public void SetCurrentUser(User user)
        {
            _currentUser = user;
            if (_historyService != null)
            {
                _ = LoadHistoryDataAsync();
            }
        }

        private void SetupDataGridView()
        {
            // Cấu hình cơ bản
            guna2DataGridView1.AutoGenerateColumns = false;
            guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            guna2DataGridView1.MultiSelect = false;
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.AllowUserToAddRows = false;
            guna2DataGridView1.AllowUserToDeleteRows = false;

            // Ngăn user kéo thả và tối ưu hiển thị
            guna2DataGridView1.AllowUserToResizeColumns = false;
            guna2DataGridView1.AllowUserToResizeRows = false;
            guna2DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            guna2DataGridView1.RowHeadersVisible = false;
            guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Clear existing columns
            guna2DataGridView1.Columns.Clear();

            // Add columns với cấu hình tối ưu
            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "BookTitle",
                HeaderText = "Book Title",
                DataPropertyName = "BookTitle",
                FillWeight = 35, // 35% width
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(5, 2, 5, 2)
                }
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Category",
                HeaderText = "Category",
                DataPropertyName = "Category",
                FillWeight = 15, // 15% width
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(2, 2, 2, 2)
                }
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ChapterTitle",
                HeaderText = "Chapter",
                DataPropertyName = "ChapterTitle",
                FillWeight = 25, // 25% width
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(5, 2, 5, 2)
                }
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AccessType",
                HeaderText = "Access Type",
                DataPropertyName = "AccessType",
                FillWeight = 12, // 12% width
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(2, 2, 2, 2)
                }
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AccessedDate",
                HeaderText = "Date Accessed",
                DataPropertyName = "AccessedDate",
                FillWeight = 13, // 13% width
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "yyyy-MM-dd HH:mm",
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(2, 2, 2, 2)
                }
            });

            // Hidden columns cho reference
            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "BookId",
                HeaderText = "BookId",
                DataPropertyName = "BookId",
                Visible = false
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "HistoryId",
                HeaderText = "HistoryId",
                DataPropertyName = "HistoryId",
                Visible = false
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ChapterId",
                HeaderText = "ChapterId",
                DataPropertyName = "ChapterId",
                Visible = false
            });

            // Không cho phép sort
            foreach (DataGridViewColumn column in guna2DataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            // Thêm màu xen kẽ cho các hàng
            guna2DataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
            guna2DataGridView1.DefaultCellStyle.BackColor = Color.White;

            // Cải thiện appearance
            guna2DataGridView1.BorderStyle = BorderStyle.None;
            guna2DataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            guna2DataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            guna2DataGridView1.DefaultCellStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
        }

        private void AdjustRowHeight()
        {
            // Tự động điều chỉnh chiều cao hàng cho text dài
            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                int maxHeight = 35; // Minimum height

                // Kiểm tra BookTitle và ChapterTitle columns (có thể có text dài)
                foreach (string columnName in new[] { "BookTitle", "ChapterTitle" })
                {
                    var cell = row.Cells[columnName];
                    if (cell.Visible && cell.Value != null)
                    {
                        string text = cell.Value.ToString();
                        if (!string.IsNullOrEmpty(text))
                        {
                            Size textSize = TextRenderer.MeasureText(text, guna2DataGridView1.Font,
                                new Size(cell.OwningColumn.Width - 15, 0), TextFormatFlags.WordBreak);
                            int cellHeight = textSize.Height + 15; // Add padding
                            maxHeight = Math.Max(maxHeight, cellHeight);
                        }
                    }
                }

                // Set row height với giới hạn tối đa 80px
                row.Height = Math.Min(maxHeight, 80);
            }
        }

        private async void History_Load(object sender, EventArgs e)
        {
            if (_historyService == null)
            {
                MessageBox.Show("History service is not available.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            await LoadHistoryDataAsync();
        }

        private async Task LoadHistoryDataAsync()
        {
            if (_historyService == null || _currentUser == null)
                return;

            try
            {
                Debug.WriteLine($"Loading history for user: {_currentUser.UserId}");

                var histories = await _historyService.GetUserHistoryAsync(_currentUser.UserId);
                _historyData = histories;

                Debug.WriteLine($"Found {histories.Count()} history records");

                // Transform data for DataGridView
                var displayData = histories.Select(h => new
                {
                    BookTitle = h.Book?.Title ?? "Unknown Book",
                    Category = h.Book?.Category?.CategoryName ?? "Unknown Category",
                    ChapterTitle = h.Chapter?.ChapterTitle ?? "Book Overview",
                    AccessType = h.AccessType,
                    AccessedDate = h.AccessedDate,
                    HistoryId = h.HistoryId,
                    BookId = h.BookId,
                    ChapterId = h.ChapterId
                }).OrderByDescending(x => x.AccessedDate).ToList(); // Sắp xếp theo ngày mới nhất

                guna2DataGridView1.DataSource = displayData;

                // Tự động điều chỉnh chiều cao hàng sau khi bind data
                if (displayData.Any())
                {
                    guna2DataGridView1.Refresh();
                    Application.DoEvents(); // Đảm bảo UI được update
                    AdjustRowHeight();
                }

                Debug.WriteLine($"Data bound to grid with {displayData.Count} rows");

                // Update UI status
                if (!displayData.Any())
                {
                    // Có thể thêm message cho trường hợp không có history
                    Debug.WriteLine("No history records found for user");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading history: {ex.Message}");
                MessageBox.Show($"Error loading history: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void read_btn_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a history item to read.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var selectedRow = guna2DataGridView1.SelectedRows[0];
                var bookIdCell = selectedRow.Cells["BookId"];

                if (bookIdCell.Value == null)
                {
                    MessageBox.Show("Invalid book selection.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var bookId = Convert.ToInt32(bookIdCell.Value);

                Debug.WriteLine($"Opening book with ID: {bookId}");

                // Get the required services to create BookDetail
                var parentForm = this.FindForm();
                if (parentForm is Main mainForm)
                {
                    // Access the service provider through Main form
                    var serviceProvider = mainForm.GetType()
                        .GetField("_serviceProvider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
                        .GetValue(mainForm) as IServiceProvider;

                    if (serviceProvider != null)
                    {
                        var bookService = serviceProvider.GetRequiredService<IBookService>();
                        var userService = serviceProvider.GetRequiredService<IUserService>();
                        var ratingService = serviceProvider.GetRequiredService<IRatingService>();
                        var historyService = serviceProvider.GetRequiredService<IHistoryService>();

                        var bookDetail = new BookDetail(bookService, userService, ratingService, historyService);
                        bookDetail.SetBookId(bookId, _currentUser);
                        bookDetail.ShowDialog();

                        // Add history entry for opening the book
                        if (_historyService != null && _currentUser != null)
                        {
                            await _historyService.AddHistoryAsync(_currentUser.UserId, bookId, null, "View");
                            await LoadHistoryDataAsync(); // Refresh the history
                        }
                    }
                    else
                    {
                        MessageBox.Show("Unable to access required services.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Unable to access parent form.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening book: {ex.Message}");
                MessageBox.Show($"Error opening book: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void clear_btn_Click(object sender, EventArgs e)
        {
            if (_historyService == null || _currentUser == null)
            {
                MessageBox.Show("History service or user information is not available.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kiểm tra xem có history để clear không
            if (_historyData == null || !_historyData.Any())
            {
                MessageBox.Show("No reading history to clear.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                "Are you sure you want to clear all reading history? This action cannot be undone.",
                "Clear History Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2); // Default to No

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Disable buttons during operation
                    clear_btn.Enabled = false;
                    read_btn.Enabled = false;

                    await _historyService.ClearUserHistoryAsync(_currentUser.UserId);
                    await LoadHistoryDataAsync(); // Refresh the display

                    MessageBox.Show("Reading history has been cleared successfully.", "History Cleared",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error clearing history: {ex.Message}");
                    MessageBox.Show($"Error clearing history: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Re-enable buttons
                    clear_btn.Enabled = true;
                    read_btn.Enabled = true;
                }
            }
        }

        // Method để refresh data từ bên ngoài nếu cần
        public async Task RefreshHistoryAsync()
        {
            await LoadHistoryDataAsync();
        }

        // Method để kiểm tra xem có history không
        public bool HasHistory()
        {
            return _historyData != null && _historyData.Any();
        }
    }
}