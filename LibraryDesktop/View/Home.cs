using Microsoft.Extensions.DependencyInjection;
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
using System.Diagnostics;
using LibraryDesktop.Data.Interfaces;


namespace LibraryDesktop.View
{
    public partial class Home : UserControl
    {
        public event EventHandler<BookSelectedEventArgs>? BookSelected;

        private readonly IUserService _userService;
        private readonly IAuthenticationService _authService;
        private int _currentUserId;


        public Home()
        {
            InitializeComponent();
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


        // Method to initialize with service provider and load books
{    public partial class Home : UserControl
    {        public event EventHandler<BookSelectedEventArgs>? BookSelected;

        private IEnumerable<Book> _allBooks = new List<Book>();
        private IEnumerable<Category> _categories = new List<Category>();
        private IServiceProvider? _serviceProvider;public Home()
        {
            InitializeComponent();
            CreateSearchAndFilterControls();
        }        // Method to create search and filter controls
        private void CreateSearchAndFilterControls()
        {
            // Wire up search functionality
            if (guna2TextBox1 != null)
            {
                guna2TextBox1.PlaceholderText = "Search books by title or author...";
                guna2TextBox1.KeyDown += SearchTextBox_KeyDown;
            }

            if (guna2PictureBox1 != null)
            {
                guna2PictureBox1.Click += SearchButton_Click;
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
        }

        private void PerformSearch()
        {
            if (guna2TextBox1 == null) return;

            string searchTerm = guna2TextBox1.Text.Trim();
            
            if (string.IsNullOrEmpty(searchTerm))
            {
                // Show all books if search is empty
                UpdateBookControls(_allBooks);
            }
            else
            {
                // Filter books by title or author
                var filteredBooks = _allBooks.Where(book => 
                    book.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    book.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                );
                
                UpdateBookControls(filteredBooks);
            }
        }        // Method to initialize with service provider and load books
        public async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            
            // Check if already loaded (prevent reloading)
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
                
                Debug.WriteLine($"Found {books.Count()} books and {categories.Count()} categories");

                // Ensure UI operations happen on UI thread
                if (this.InvokeRequired)
                {
                    this.Invoke(() => UpdateBookControls(books));
                }
                else
                {
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
        }

        private void UpdateBookControls(IEnumerable<Book> books)
        {
            // Clear existing static book controls (keep the layout)
            var staticControls = flowLayoutPanel1.Controls.OfType<BookControl>().ToList();

            // Update existing controls with real data or add new ones
            int bookIndex = 0;
            foreach (var book in books.Take(12)) // Limit to 12 books
            {
                BookControl bookControl;

                if (bookIndex < staticControls.Count)
                {
                    // Use existing control
                    bookControl = staticControls[bookIndex];
                }
                else
                {
                    // Create new control
                    bookControl = new BookControl();
                    flowLayoutPanel1.Controls.Add(bookControl);
                }
                bookControl.SetBook(book);
                bookControl.Tag = book; // Set tag to indicate this control has data
                bookControl.BookClicked += (sender, clickedBook) => OnBookClicked(clickedBook.BookId);
                bookIndex++;
            }

            Debug.WriteLine($"Loaded {bookIndex} books successfully to Home view");
        }

        // Method được gọi khi user click vào một book
        private void OnBookClicked(int bookId)
        {
            BookSelected?.Invoke(this, new BookSelectedEventArgs(bookId));
        }

        // Hoặc nếu bạn muốn mở BookDetail trực tiếp từ Home
        private void OpenBookDetail(int bookId)
        {
            // Tìm Main form parent
            var mainForm = this.FindForm() as Main;
            mainForm?.OpenBookDetail(bookId);
        }

        // Trong Home.cs (UserControl)
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

            // Hiển thị Home view
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
            // Hide current content in flowLayoutPanel1
            if (flowLayoutPanel1 != null)
            {
                flowLayoutPanel1.Visible = false;
            }
        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {
            // Show user account information or exchange
            ShowExchangeForm();
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            // Handle search functionality
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
    }

}