using LibraryDesktop.Data.Interfaces;
using LibraryDesktop.Data.Services;
using LibraryDesktop.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace LibraryDesktop.View
{
    public partial class Home : UserControl
    {
        public event EventHandler<BookSelectedEventArgs>? BookSelected;

        private readonly IUserService? _userService;
        private readonly IAuthenticationService? _authService;
        private IServiceProvider? _serviceProvider;
        private int _currentUserId;
        private IEnumerable<Book> _allBooks = new List<Book>();
        private IEnumerable<Category> _categories = new List<Category>();

        
        private Guna.UI2.WinForms.Guna2ComboBox? _categoryFilterCombo;public Home()
        {
            InitializeComponent();
            CreateSearchAndFilterControls();
            
            // Handle resize events for responsive layout
            this.Resize += Home_Resize;
            flowLayoutPanel1.Resize += FlowLayoutPanel1_Resize;
            if (account1 != null)
            {
                account1.Visible = false;
            }
        }

        public Home(IUserService userService, IAuthenticationService authService) : this()
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public void SetCurrentUserId(int userId)
        {
            _currentUserId = userId;
        }

        private void CreateSearchAndFilterControls()
        {
            if (guna2TextBox1 != null)
            {
                guna2TextBox1.PlaceholderText = "Search books by title or author...";
                guna2TextBox1.KeyDown += SearchTextBox_KeyDown;
                // Add real-time search on text change
                guna2TextBox1.TextChanged += SearchTextBox_TextChanged;
            }

            if (guna2PictureBox1 != null)
            {
                guna2PictureBox1.Click += SearchButton_Click;
            }

            // Create category filter combobox
            CreateCategoryFilter();
        }

        private void CreateCategoryFilter()
        {
            if (guna2ShadowPanel1 != null)
            {
                _categoryFilterCombo = new Guna.UI2.WinForms.Guna2ComboBox
                {
                    Name = "categoryFilterCombo",
                    Location = new Point(675, 23), // Between search box and search button
                    Size = new Size(150, 44),
                    BorderRadius = 15,
                    Font = new Font("Segoe UI", 9F),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                // Add "All Categories" as default option
                _categoryFilterCombo.Items.Add("All Categories");
                _categoryFilterCombo.SelectedIndex = 0;
                _categoryFilterCombo.SelectedIndexChanged += CategoryFilter_SelectedIndexChanged;

                guna2ShadowPanel1.Controls.Add(_categoryFilterCombo);
            }
        }        private void CategoryFilter_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // Trigger immediate search when category is changed
            PerformSearch();
        }
        
        // Real-time search on text change
        private void SearchTextBox_TextChanged(object? sender, EventArgs e)
        {
            PerformSearch();
        }private void PopulateCategoryFilter()
        {
            if (_categoryFilterCombo != null && _categories != null)
            {
                // Clear existing items except "All Categories"
                _categoryFilterCombo.Items.Clear();
                _categoryFilterCombo.Items.Add("All Categories");
                  // Add all categories
                foreach (var category in _categories.OrderBy(c => c.CategoryName))
                {
                    _categoryFilterCombo.Items.Add(category.CategoryName);
                }
                
                _categoryFilterCombo.SelectedIndex = 0;
            }
        }

        private void UpdateWelcomeMessage()
        {
            if (guna2HtmlLabel1 != null && _userService != null && _currentUserId > 0)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        var user = await _userService.GetUserByIdAsync(_currentUserId);
                        if (user != null)
                        {
                            if (this.InvokeRequired)
                            {
                                this.Invoke(() => guna2HtmlLabel1.Text = $"Hello {user.Username}");
                            }
                            else
                            {
                                guna2HtmlLabel1.Text = $"Hello {user.Username}";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error updating welcome message: {ex.Message}");
                    }
                });
            }
        }

        private void SearchTextBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                PerformSearch();
                e.Handled = true;
            }
        }

        private void SearchButton_Click(object? sender, EventArgs e)
        {
            PerformSearch();
        }        private void PerformSearch()
        {
            if (guna2TextBox1 == null) return;

            string searchTerm = guna2TextBox1.Text.Trim();
            var filteredBooks = _allBooks.AsEnumerable();

            // Apply text search filter
            if (!string.IsNullOrEmpty(searchTerm) && searchTerm != "Search")
            {
                filteredBooks = filteredBooks.Where(book => 
                    book.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    book.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                );
            }            // Apply category filter
            if (_categoryFilterCombo != null && _categoryFilterCombo.SelectedIndex > 0)
            {
                string selectedCategory = _categoryFilterCombo.SelectedItem.ToString() ?? "";
                var category = _categories.FirstOrDefault(c => c.CategoryName == selectedCategory);
                if (category != null)
                {
                    filteredBooks = filteredBooks.Where(book => book.CategoryId == category.CategoryId);
                }
            }            UpdateBookControls(filteredBooks);
            
            if (account1 != null)
            {
                account1.Visible = false;
            }
        }

        // Method to initialize with service provider and load books
        public async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            // Check if already loaded
            if (flowLayoutPanel1.Controls.OfType<BookControl>().Any(bc => bc.Tag != null))
            {
                Debug.WriteLine("Home already loaded with books, skipping reload");
                return;
            }

            try
            {
                var bookService = serviceProvider.GetRequiredService<IBookService>();
                var categoryService = serviceProvider.GetRequiredService<ICategoryService>();

                Debug.WriteLine("Loading books and categories from database...");
                var books = await bookService.GetBooksAsync();
                var categories = await categoryService.GetCategoriesAsync();

                _allBooks = books;
                _categories = categories;
                
                Debug.WriteLine($"Found {books.Count()} books and {categories.Count()} categories");                // Ensure UI operations happen on UI thread
                if (this.InvokeRequired)
                {
                    this.Invoke(() => 
                    {
                        PopulateCategoryFilter();
                        UpdateWelcomeMessage();
                        UpdateBookControls(books);
                    });
                }
                else
                {
                    PopulateCategoryFilter();
                    UpdateWelcomeMessage();
                    UpdateBookControls(books);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading books: {ex.Message}");

                // Show error on UI thread
                if (this.InvokeRequired)
                {
                    this.Invoke(() =>
                    {
                        MessageBox.Show($"Error loading books: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
                else
                {
                    MessageBox.Show($"Error loading books: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }        private void UpdateBookControls(IEnumerable<Book> books)
        {
            try
            {
                flowLayoutPanel1.SuspendLayout();
                
                // Clear existing controls
                ClearBookControls();
                
                // Get books to display
                var booksToShow = books.ToList();
                
                // Calculate optimal sizing for 5 books per row
                ConfigureFlowLayoutForOptimalDisplay();
                
                // Create and add new book controls dynamically
                int bookIndex = 0;
                foreach (var book in booksToShow)
                {
                    var bookControl = CreateBookControl(book, bookIndex);
                    flowLayoutPanel1.Controls.Add(bookControl);
                    bookIndex++;
                }

                Debug.WriteLine($"✅ Created {bookIndex} dynamic book controls with optimal spacing (5 per row)");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error updating book controls: {ex.Message}");
                MessageBox.Show($"Error displaying books: {ex.Message}", "Display Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                flowLayoutPanel1.ResumeLayout(true);
            }
        }

        private void ClearBookControls()
        {
            // Dispose existing controls properly to prevent memory leaks
            var existingControls = flowLayoutPanel1.Controls.OfType<BookControl>().ToList();
            foreach (var control in existingControls)
            {
                control.BookClicked -= OnBookClickedHandler;
                flowLayoutPanel1.Controls.Remove(control);
                control.Dispose();
            }
        }        private void ConfigureFlowLayoutForOptimalDisplay()
        {
            // Configure FlowLayoutPanel for 4 books per row with proper spacing
            flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel1.WrapContents = true;
            flowLayoutPanel1.AutoScroll = true;
            
            // Minimal padding for maximum space utilization
            flowLayoutPanel1.Padding = new Padding(5, 5, 5, 5);
            
            // Calculate available width and book control size for 4 per row
            int availableWidth = flowLayoutPanel1.Width - flowLayoutPanel1.Padding.Horizontal;
            int booksPerRow = 4; // Exactly 4 for larger book controls
            int spacing = 4; // Minimal spacing between book controls
            int totalSpacing = spacing * (booksPerRow - 1);
            int bookControlWidth = (availableWidth - totalSpacing + 80) / booksPerRow;
            
            // Ensure minimum width for readability and proper image display
            if (bookControlWidth < 260)
            {
                bookControlWidth = 260;
                booksPerRow = Math.Max(1, (availableWidth - totalSpacing) / bookControlWidth);
            }
            
            Debug.WriteLine($"📐 Layout configured: {booksPerRow} books per row, {bookControlWidth}px width, {spacing}px spacing");
            Debug.WriteLine($"📏 FlowLayoutPanel size: {flowLayoutPanel1.Width}x{flowLayoutPanel1.Height}, Padding: {flowLayoutPanel1.Padding}");
        }private BookControl CreateBookControl(Book book, int index)
        {
            var bookControl = new BookControl(book)
            {
                Name = $"dynamicBook{index}",
                Size = CalculateOptimalBookControlSize(),
                Margin = new Padding(2, 2, 2, 6), // Further reduced margins for tighter spacing
                Tag = book,
                BackColor = Color.Transparent
            };
            
            // Subscribe to click event
            bookControl.BookClicked += OnBookClickedHandler;
            
            Debug.WriteLine($"📚 Created BookControl {index}: Size={bookControl.Size}, Margin={bookControl.Margin}");
            
            return bookControl;
        }private Size CalculateOptimalBookControlSize()
        {
            // Calculate optimal size based on FlowLayoutPanel dimensions
            int availableWidth = flowLayoutPanel1.Width - flowLayoutPanel1.Padding.Horizontal;
            int booksPerRow = 4; // Reduced to 4 books per row for larger size
            int horizontalSpacing = 8; // Total margin per control (4px each side)
            int totalHorizontalSpacing = horizontalSpacing * booksPerRow;
            
            // Calculate width ensuring minimum and maximum constraints
            int bookWidth = Math.Max(260, (availableWidth - totalHorizontalSpacing) / booksPerRow);
            bookWidth = Math.Min(bookWidth, 320); // Adjusted maximum width for 4 per row
              
            // Maintain aspect ratio for book controls (book-like proportions)
            int bookHeight = (int)(bookWidth * 1.7); // Adjusted aspect ratio
            bookHeight = Math.Max(bookHeight, 550); // Adjusted minimum height
            bookHeight = Math.Min(bookHeight, 680); // Adjusted maximum height
            
            Debug.WriteLine($"📐 Calculated BookControl size: {bookWidth}x{bookHeight} (Available: {availableWidth}px)");
            
            return new Size(bookWidth, bookHeight);
        }

        // Event handler method to avoid lambda closure issues
        private void OnBookClickedHandler(object? sender, Book clickedBook)
        {
            OnBookClicked(clickedBook.BookId);
        }

        private void OnBookClicked(int bookId)
        {
            BookSelected?.Invoke(this, new BookSelectedEventArgs(bookId));
        }

        private void OpenBookDetail(int bookId)
        {
            var mainForm = this.FindForm() as Main;
            mainForm?.OpenBookDetail(bookId);
        }

        public void ShowBookList()
        {

            if (flowLayoutPanel1 != null)
            {
                flowLayoutPanel1.Visible = true;
                flowLayoutPanel1.BringToFront();
            }
        }

        public void ShowHomeView()
        {
            if (flowLayoutPanel1 != null)
            {
                flowLayoutPanel1.Visible = true;
                flowLayoutPanel1.BringToFront();
            }
        }

        public void UpdateUsername(string username)
        {
            if (guna2HtmlLabel1 != null)
            {
                guna2HtmlLabel1.Text = username;
            }
        }

        public void ShowExchangeForm()
        {
            if (flowLayoutPanel1 != null)
            {
                flowLayoutPanel1.Visible = false;
            }
        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {
            ShowExchangeForm();
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            ShowHomeView();
        }

        private async void btnAccount_Click(object sender, EventArgs e)
        {
            if (_userService == null || _authService == null)
            {
                MessageBox.Show("Services not initialized!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Sử dụng account control có sẵn từ designer (giả sử tên là account1)
            if (account1.Visible)
            {
                // Ẩn account, hiện lại home content
                account1.Visible = false;
                flowLayoutPanel1.Visible = true;
                flowLayoutPanel1.BringToFront();
            }
            else
            {
                // Hiện account, ẩn home content
                try
                {
                    // Initialize services cho account control từ designer
                    account1.Initialize(_userService, _authService);

                    // Load user data
                    await account1.LoadUserDataAsync(_currentUserId);

                    // Toggle visibility
                    flowLayoutPanel1.Visible = false;
                    account1.Visible = true;
                    account1.BringToFront();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading account: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void account1_Load(object sender, EventArgs e)
        {

        }

        private void Home_Resize(object sender, EventArgs e)
        {
            // Refresh layout when Home control is resized
            RefreshBookControlsLayout();
        }

        private void FlowLayoutPanel1_Resize(object sender, EventArgs e)
        {
            // Refresh layout when FlowLayoutPanel is resized
            RefreshBookControlsLayout();
        }        private void RefreshBookControlsLayout()
        {
            if (flowLayoutPanel1.Controls.Count > 0)
            {
                try
                {
                    flowLayoutPanel1.SuspendLayout();
                    
                    // Reconfigure layout settings first
                    ConfigureFlowLayoutForOptimalDisplay();
                    
                    // Recalculate optimal size for existing controls
                    var optimalSize = CalculateOptimalBookControlSize();
                      foreach (BookControl bookControl in flowLayoutPanel1.Controls.OfType<BookControl>())
                    {
                        var oldSize = bookControl.Size;
                        bookControl.Size = optimalSize;
                        bookControl.Margin = new Padding(2, 2, 2, 6); // Consistent reduced margins
                        
                        // Force BookControl to refresh its image sizing
                        bookControl.Refresh();
                        
                        Debug.WriteLine($"🔄 Resized BookControl from {oldSize} to {optimalSize}");
                    }
                    
                    Debug.WriteLine($"✅ Refreshed layout for {flowLayoutPanel1.Controls.Count} BookControls");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌ Error refreshing layout: {ex.Message}");
                }
                finally
                {
                    flowLayoutPanel1.ResumeLayout(true);
                }
            }
        }

    }

}