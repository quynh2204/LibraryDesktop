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
            guna2DataGridView1.AutoGenerateColumns = false;
            guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            guna2DataGridView1.MultiSelect = false;
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.AllowUserToAddRows = false;

            // Clear existing columns
            guna2DataGridView1.Columns.Clear();

            // Add columns
            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "BookTitle",
                HeaderText = "Book Title",
                DataPropertyName = "BookTitle",
                Width = 200,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Category",
                HeaderText = "Category",
                DataPropertyName = "Category",
                Width = 120
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ChapterTitle",
                HeaderText = "Chapter",
                DataPropertyName = "ChapterTitle",
                Width = 150
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AccessType",
                HeaderText = "Access Type",
                DataPropertyName = "AccessType",
                Width = 100
            });            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AccessedDate",
                HeaderText = "Date Accessed",
                DataPropertyName = "AccessedDate",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd HH:mm" }
            });

            // Add hidden BookId column for reference
            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "BookId",
                HeaderText = "BookId",
                DataPropertyName = "BookId",
                Visible = false // Hide this column from user view
            });

            // Add hidden HistoryId column for reference
            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "HistoryId",
                HeaderText = "HistoryId", 
                DataPropertyName = "HistoryId",
                Visible = false // Hide this column from user view
            });

            // Add hidden ChapterId column for reference
            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ChapterId",
                HeaderText = "ChapterId",
                DataPropertyName = "ChapterId", 
                Visible = false // Hide this column from user view
            });
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
                });

                guna2DataGridView1.DataSource = displayData.ToList();

                Debug.WriteLine($"Data bound to grid with {displayData.Count()} rows");
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
                var bookId = (int)selectedRow.Cells["BookId"].Value;

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
                    {                        var bookService = serviceProvider.GetRequiredService<IBookService>();
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

            var result = MessageBox.Show(
                "Are you sure you want to clear all reading history? This action cannot be undone.",
                "Clear History Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
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
            }
        }
    }
}
