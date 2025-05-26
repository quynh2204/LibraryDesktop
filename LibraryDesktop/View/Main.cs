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
    public partial class Main : Form
    {
        private readonly IBookService _bookService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;
        private readonly IServiceProvider _serviceProvider;
        private readonly PaymentWebServer _paymentWebServer;
        private User? _currentUser;
        private decimal _currentBalance;
          // Content panel for displaying UserControls
        private Panel? contentPanel;
        
        // Current active UserControl
        private UserControl? currentUserControl;

        public Main(IBookService bookService, 
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
        }

        public async Task InitializeWithUserAsync(User user)
        {
            _currentUser = user;
            await UpdateBalanceDisplayAsync();
            
            // Load Dashboard by default after login
            LoadDashboard();
        }        private void InitializeMainForm()
        {
            // Start the payment web server in the background
            _ = Task.Run(async () =>
            {
                try
                {
                    await _paymentWebServer.StartAsync(NetworkConfiguration.API_SERVER_PORT);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to start payment web server: {ex.Message}", "Warning",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            });

            // Initialize content panel
            InitializeContentPanel();
            
            // Wire up navigation button events
            InitializeNavigationEvents();
            
            // Show login form first
            ShowLogin();
        }private void ShowLogin()
        {
            using (var loginForm = _serviceProvider.GetRequiredService<LoginForm>())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // Login was successful, get the authenticated user
                    // Get the current user from the authentication service
                    _currentUser = loginForm.AuthenticatedUser;
                    
                    if (_currentUser != null)
                    {
                        _ = InitializeWithUserAsync(_currentUser);
                    }
                }
                else
                {
                    // User cancelled login, close application
                    this.Close();
                }
            }
        }

        private void InitializeContentPanel()
        {
            // Create content panel to hold UserControls
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(241, 236, 228)
            };
            
            // Add content panel to main form, making sure it's positioned correctly
            // The navigation panel (guna2Panel1) is docked to the left, so content panel should fill the rest
            this.Controls.Add(contentPanel);
            contentPanel.BringToFront();
        }

        private void InitializeNavigationEvents()
        {
            // Wire up button click events
            btnDashboard.Click += BtnDashboard_Click;
            btnBooks.Click += BtnBooks_Click;
            btnMyBooks.Click += BtnMyBooks_Click;
            btnHistory.Click += BtnHistory_Click;
            btnExchange.Click += BtnExchange_Click;
            btnlogout.Click += BtnLogout_Click;
            pictureBox1.Click += PictureBox1_Click; // Logo click - go to Dashboard
        }

        private void LoadUserControl(UserControl userControl)
        {
            // Clear current content
            contentPanel?.Controls.Clear();
            
            // Dispose previous user control
            currentUserControl?.Dispose();
            
            // Set new user control
            currentUserControl = userControl;
            userControl.Dock = DockStyle.Fill;
            
            // Add to content panel
            contentPanel?.Controls.Add(userControl);
        }

        private void LoadDashboard()
        {
            var dashboard = _serviceProvider.GetRequiredService<Dashboard>();
            LoadUserControl(dashboard);
        }        private void LoadBooks()
        {
            // Create a simple books display UserControl
            var booksControl = new UserControl { Dock = DockStyle.Fill, BackColor = Color.White };
            var label = new Label 
            { 
                Text = "Books View - Coming Soon", 
                Dock = DockStyle.Fill, 
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 16)
            };
            booksControl.Controls.Add(label);
            LoadUserControl(booksControl);
        }

        private void LoadMyBooks()
        {
            var myBooks = _serviceProvider.GetRequiredService<MyBooks>();
            LoadUserControl(myBooks);
        }

        private void LoadHistory()
        {
            var history = _serviceProvider.GetRequiredService<History>();
            LoadUserControl(history);
        }

        private void LoadExchange()
        {
            var exchange = _serviceProvider.GetRequiredService<Exchange>();
            LoadUserControl(exchange);
        }

        // Navigation button event handlers
        private void BtnDashboard_Click(object? sender, EventArgs e)
        {
            LoadDashboard();
        }

        private void BtnBooks_Click(object? sender, EventArgs e)
        {
            LoadBooks();
        }

        private void BtnMyBooks_Click(object? sender, EventArgs e)
        {
            LoadMyBooks();
        }

        private void BtnHistory_Click(object? sender, EventArgs e)
        {
            LoadHistory();
        }

        private void BtnExchange_Click(object? sender, EventArgs e)
        {
            LoadExchange();
        }

        private void BtnLogout_Click(object? sender, EventArgs e)
        {
            // Confirm logout
            var result = MessageBox.Show("Are you sure you want to logout?", "Confirm Logout", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
            if (result == DialogResult.Yes)
            {
                // Clear current user
                _currentUser = null;
                
                // Clear content panel
                contentPanel?.Controls.Clear();
                currentUserControl?.Dispose();
                currentUserControl = null;
                
                // Show login form again
                ShowLogin();
            }
        }

        private void PictureBox1_Click(object? sender, EventArgs e)
        {
            // Logo click - go to Dashboard
            LoadDashboard();
        }        private async Task UpdateBalanceDisplayAsync()
        {
            if (_currentUser != null)
            {
                _currentBalance = await _userService.GetUserBalanceAsync(_currentUser.UserId);
                this.Text = $"Library Desktop - Welcome {_currentUser.Username} (Balance: ${_currentBalance:F2})";
            }
        }

        private async void ShowRechargeForm()
        {
            using (var exchange = _serviceProvider.GetRequiredService<Exchange>())
            {
                if (exchange.ShowDialog() == DialogResult.OK)
                {
                    // Recharge was successful, update user balance display
                    MessageBox.Show("Account recharged successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Refresh user balance from database
                    await UpdateBalanceDisplayAsync();
                }
            }
        }

        // Legacy event handlers (keeping for compatibility with designer)
        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            // Redirect to new logo click handler
            PictureBox1_Click(sender, e);
        }

        private void book1_Load(object sender, EventArgs e)
        {
            // Legacy method - no longer used
        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {
            // Show recharge form when this label is clicked
            ShowRechargeForm();
        }

        private void books_btn_Click(object sender, EventArgs e)
        {
            // Redirect to new books click handler
            BtnBooks_Click(sender, e);
        }
    }
}
