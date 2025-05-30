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

        public Home()
        {
            InitializeComponent();
            CreateSearchAndFilterControls();
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
                UpdateBookControls(_allBooks);
            }
            else
            {
                var filteredBooks = _allBooks.Where(book =>
                    book.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    book.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                );

                UpdateBookControls(filteredBooks);
            }
        }

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

                Debug.WriteLine($"Found {books.Count()} books and {categories.Count()} categories");

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

                if (this.InvokeRequired)
                {
                    this.Invoke(() =>
                        MessageBox.Show($"Error loading books: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error));
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
            var staticControls = flowLayoutPanel1.Controls.OfType<BookControl>().ToList();
            int bookIndex = 0;

            foreach (var book in books.Take(12))
            {
                BookControl bookControl;

                if (bookIndex < staticControls.Count)
                {
                    bookControl = staticControls[bookIndex];
                }
                else
                {
                    bookControl = new BookControl();
                    flowLayoutPanel1.Controls.Add(bookControl);
                }

                bookControl.SetBook(book);
                bookControl.Tag = book;
                bookControl.BookClicked += (sender, clickedBook) => OnBookClicked(clickedBook.BookId);
                bookIndex++;
            }

            Debug.WriteLine($"Loaded {bookIndex} books successfully to Home view");
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

            if (account1.Visible)
            {
                account1.Visible = false;
                flowLayoutPanel1.Visible = true;
                flowLayoutPanel1.BringToFront();
            }
            else
            {
                try
                {
                    account1.Initialize(_userService, _authService);
                    await account1.LoadUserDataAsync(_currentUserId);
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
            // Optional: Add logic if needed
        }
    }
}
