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
using LibraryDesktop.Data.Interfaces;
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
        private UserControl? currentUserControl;        public Main(IBookService bookService, 
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
            
            // 🔥 Subscribe to payment completed events
            var paymentService = _serviceProvider.GetRequiredService<IPaymentService>();
            paymentService.PaymentCompleted += OnPaymentCompleted;
            
            // Handle form closing to unsubscribe events
            this.FormClosed += (s, e) => 
            {
                try 
                { 
                    paymentService.PaymentCompleted -= OnPaymentCompleted; 
                } 
                catch { /* Ignore */ }
            };
            
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
                    Console.WriteLine("🔄 Attempting to start PaymentWebServer...");
                    await _paymentWebServer.StartAsync(NetworkConfiguration.API_SERVER_PORT);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ PaymentWebServer startup failed: {ex.Message}");
                    MessageBox.Show($"Failed to start payment web server: {ex.Message}", "Warning",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            });

            // Initialize content panel
            InitializeContentPanel();
              // Wire up navigation button events
            InitializeNavigationEvents();
            
            // Don't show login form here - Program.cs already handles authentication
            // The user will be set via InitializeWithUserAsync after successful login
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
        }        private void LoadExchange()
        {
            // Check if user is available
            if (_currentUser == null)
            {
                MessageBox.Show("User session not available. Please log in again.", "Authentication Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create Exchange with current user context
            var paymentService = _serviceProvider.GetRequiredService<IPaymentService>();
            var exchange = new Exchange(paymentService, _currentUser);
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
        }        private void BtnLogout_Click(object? sender, EventArgs e)
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
                
                // Close the main form and restart the application to show login
                this.Hide();
                Application.Restart();
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
        }        private void ShowRechargeForm()
        {
            // Load the Exchange UserControl instead of trying to show it as a dialog
            LoadExchange();
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
          // 🔥 Event handler for payment completion
        private async void OnPaymentCompleted(object? sender, PaymentCompletedEventArgs e)
        {
            try
            {
                // Chỉ cập nhật nếu payment thuộc về current user
                if (_currentUser != null && e.UserId == _currentUser.UserId)
                {
                    Console.WriteLine($"💰 Payment completed for current user! Amount: {e.Amount}, Coins: {e.Amount / 1000}");
                    
                    // Cập nhật balance trên UI thread
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(async () => await UpdateBalanceDisplayAsync()));
                    }
                    else
                    {
                        await UpdateBalanceDisplayAsync();
                    }
                    
                    // Hiển thị notification
                    var coinsAdded = e.Amount / 1000;
                    if (this.InvokeRequired)
                    {
                        this.Invoke(() => 
                        {
                            MessageBox.Show($"🎉 Payment Completed!\n\nAmount: {e.Amount:C}\nCoins Added: {coinsAdded}\n\nYour balance has been updated!", 
                                "Payment Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        });
                    }
                    else
                    {
                        MessageBox.Show($"🎉 Payment Completed!\n\nAmount: {e.Amount:C}\nCoins Added: {coinsAdded}\n\nYour balance has been updated!", 
                            "Payment Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error handling payment completion: {ex.Message}");
            }        }
    }
}
