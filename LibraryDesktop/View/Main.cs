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
{    public partial class Main : Form
    {        private readonly IBookService _bookService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;
        private readonly IServiceProvider _serviceProvider;
        private readonly PaymentWebServer _paymentWebServer;
        private User? _currentUser;
        private decimal _currentBalance;        public Main(IBookService bookService, 
                   IAuthenticationService authenticationService,
                   IUserService userService,
                   IServiceProvider serviceProvider,
                   PaymentWebServer paymentWebServer)
        {
            _bookService = bookService;
            _authenticationService = authenticationService;
            _userService = userService;
            _serviceProvider = serviceProvider;
            _paymentWebServer = paymentWebServer;
            InitializeComponent();
            InitializeMainForm();
        }        private async void InitializeMainForm()
        {
            // Start the payment web server in the background
            _ = Task.Run(async () =>
            {
                try
                {
                    await _paymentWebServer.StartAsync(5000);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to start payment web server: {ex.Message}", "Warning", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            });
              // Show login form first
            ShowLogin();
            
            // Load books after successful login
            if (_currentUser != null)
            {
                await LoadBooksAsync();
                await UpdateBalanceDisplayAsync();
            }
        }

        private void ShowLogin()
        {
            using (var loginForm = _serviceProvider.GetRequiredService<LoginForm>())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // Login was successful, get current user
                    // In a real app, the login form would return the authenticated user
                    // For now, we'll simulate getting the first user from the database
                    _currentUser = new User { UserId = 1, Username = "Demo User" };
                }
                else
                {
                    // User cancelled login, close application
                    this.Close();
                }
            }
        }        private async Task LoadBooksAsync()
        {
            try
            {
                // Load books from the service
                var books = await _bookService.GetBooksAsync();
                
                // Clear existing book controls
                flowLayoutPanel1.Controls.Clear();
                  // Create book controls for each book
                foreach (var book in books)
                {
                    var bookControl = new BookControl();
                    bookControl.SetBook(book);
                    bookControl.BookClicked += OnBookClicked;
                    flowLayoutPanel1.Controls.Add(bookControl);
                }
                
                // Update title to show user info
                await UpdateBalanceDisplayAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading books: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
          private void OnBookClicked(object? sender, LibraryDesktop.Models.Book book)
        {
            MessageBox.Show($"You clicked on: {book.Title}\nAuthor: {book.Author}\nDescription: {book.Description}", 
                "Book Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async Task UpdateBalanceDisplayAsync()
        {
            if (_currentUser != null)
            {
                _currentBalance = await _userService.GetUserBalanceAsync(_currentUser.UserId);
                this.Text = $"Library Desktop - Welcome {_currentUser.Username} (Balance: ${_currentBalance:F2})";
            }
        }

        private async void ShowRechargeForm()
        {
            using (var rechargeForm = _serviceProvider.GetRequiredService<RechargeForm>())
            {
                if (rechargeForm.ShowDialog() == DialogResult.OK)
                {
                    // Recharge was successful, update user balance display
                    MessageBox.Show("Account recharged successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Refresh user balance from database
                    await UpdateBalanceDisplayAsync();
                }
            }
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void book1_Load(object sender, EventArgs e)
        {

        }        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {
            // Show recharge form when this label is clicked
            ShowRechargeForm();
        }
    }
}
