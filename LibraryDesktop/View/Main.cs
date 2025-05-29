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
using System.Diagnostics;

namespace LibraryDesktop.View
{    public partial class Main : Form
    {
        private Home _homeControl;

        private readonly IBookService _bookService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;
        private readonly IServiceProvider _serviceProvider;        private readonly IPaymentService _paymentService;
        private readonly PaymentWebServer _paymentWebServer;
        private User? _currentUser;
        private int _currentCoins;
        
        // Content panel for displaying UserControls
        private Panel? contentPanel;
        
        // Current active UserControl
        private UserControl? currentUserControl;        
        public Main(IBookService bookService,
                   IAuthenticationService authenticationService,
                   IUserService userService,
                   IPaymentService paymentService,
                   IServiceProvider serviceProvider,
                   PaymentWebServer paymentWebServer)
        {
            _bookService = bookService;
            _authenticationService = authenticationService;
            _userService = userService;
            _paymentService = paymentService;
            _serviceProvider = serviceProvider;
            _paymentWebServer = paymentWebServer;

            // 🔥 Subscribe to payment completed events
            _paymentService.PaymentCompleted += OnPaymentCompleted;

            // Handle form closing to unsubscribe events
            this.FormClosed += (s, e) =>
            {
                try
                {
                    _paymentService.PaymentCompleted -= OnPaymentCompleted;
                }
                catch { /* Ignore */ }
            };

            InitializeComponent();

            // Initialize the main form properly
            InitializeMainForm();

            // Setup Home control
            _homeControl = _serviceProvider.GetRequiredService<Home>();
            _homeControl.BookSelected += OnBookSelected;
        }// Event handler for when a book is selected from Home control
        private void OnBookSelected(object? sender, BookSelectedEventArgs e)
        {
            OpenBookDetail(e.BookId);
        }

        // Method to open BookDetail form
        public void OpenBookDetail(int bookId)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var bookDetailForm = scope.ServiceProvider.GetRequiredService<BookDetail>();
                    // Pass the bookId and current user to the form
                    bookDetailForm.SetBookId(bookId, _currentUser);
                    bookDetailForm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở chi tiết sách: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"Error opening BookDetail: {ex}");
            }
        }        
        public async Task InitializeWithUserAsync(User user)
        {
            _currentUser = user;
            await UpdateCoinsDisplayAsync();
            _homeControl.SetCurrentUserId(user.UserId);
            // Load Dashboard by default after login (as startup form)
            LoadDashboard();
        }
        private void InitializeMainForm()
        {
            // Start the payment web server  in the background
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
        private async Task UpdateCoinsDisplayAsync()
        {
            if (_currentUser != null)
            {
                try
                {
                    _currentCoins = await _userService.GetUserCoinsAsync(_currentUser.UserId);
                    // Update UI if needed
                    this.Text = $"Library Desktop - Welcome {_currentUser.Username}";
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error updating coins: {ex.Message}");
                }
            }
        }private void InitializeContentPanel()
        {
            // Hide the designer controls - we'll use dynamic content loading
            if (home1 != null)
            {
                home1.Visible = false;
            }
            if (dashboard1 != null)
            {
                dashboard1.Visible = false;
            }

            contentPanel = new Panel
            {
                Location = new Point(148, 102), // After navigation panel and header
                Size = new Size(1220, 752),     // Fill remaining space
                BackColor = Color.FromArgb(241, 236, 228),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
              // Add content panel to main form
            this.Controls.Add(contentPanel);
            contentPanel.BringToFront();
        }

        private void InitializeNavigationEvents()
        {
            // Wire up button click events - only if controls exist
            if (btnHome != null)
                btnHome.Click += BtnHome_Click;
            if (btnDashboard != null)
                btnDashboard.Click += BtnDashboard_Click;
            if (btnMyBooks != null)
                btnMyBooks.Click += BtnMyBooks_Click;
            if (btnHistory != null)
                btnHistory.Click += BtnHistory_Click;
            if (btnExchange != null)
                btnExchange.Click += BtnExchange_Click;
            if (btnlogout != null)
                btnlogout.Click += BtnLogout_Click;
        }        private void LoadUserControl(UserControl userControl)
        {
            // Clear current content
            contentPanel?.Controls.Clear();
            
            // Only dispose if it's not one of our reusable controls
            if (currentUserControl != null && currentUserControl != _homeControl)
            {
                currentUserControl.Dispose();
            }
            
            // Set new user control
            currentUserControl = userControl;
            userControl.Dock = DockStyle.Fill;
            
            // Add to content panel
            contentPanel?.Controls.Add(userControl);
        }private void LoadDashboard()
        {
            var dashboard = _serviceProvider.GetRequiredService<Dashboard>();
            LoadUserControl(dashboard);
        }        private async void LoadHome()
        {
            try
            {
                // Show loading indicator first
                LoadUserControl(_homeControl);
                
                // Only initialize if the Home control hasn't been loaded with data yet
                // Check if flowLayoutPanel1 has any BookControl children
                if (_homeControl.Controls.Find("flowLayoutPanel1", true).FirstOrDefault() is Control flowPanel)
                {
                    var hasBooks = flowPanel.Controls.OfType<BookControl>().Any(bc => bc.Tag != null);
                    if (!hasBooks)
                    {
                        // Initialize Home control with book data on UI thread
                        await _homeControl.InitializeAsync(_serviceProvider);
                    }
                }
                else
                {
                    // Fallback: always initialize if we can't determine the state
                    await _homeControl.InitializeAsync(_serviceProvider);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading Home: {ex.Message}");
                MessageBox.Show($"Error loading Home: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }private void LoadBooks()
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
        }        // Navigation button event handlers
        private void BtnHome_Click(object? sender, EventArgs e)
        {
            LoadHome();
        }

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
        }private void BtnLogout_Click(object? sender, EventArgs e)
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

        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            // Stop payment web server when form closes
            try
            {
                await _paymentWebServer.StopAsync();
                Debug.WriteLine("Payment web server stopped");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error stopping payment web server: {ex.Message}");
            }            base.OnFormClosing(e);
        }

        private void home1_Load(object sender, EventArgs e)
        {

        }        // Property to get current user for child forms
        public User? CurrentUser => _currentUser;
        public int CurrentCoins => _currentCoins;//private void guna2GradientTileButton4_Click(object sender, EventArgs e)


        // 🔥 Event handler for payment completion
        private async void OnPaymentCompleted(object? sender, PaymentCompletedEventArgs e)
        {
            try
            {
                // Chỉ cập nhật nếu payment thuộc về current user
                if (_currentUser != null && e.UserId == _currentUser.UserId)
                {
                    Console.WriteLine($"💰 Payment completed for current user! Amount: {e.Amount}, Coins: {e.Amount / 1000}");
                      // Cập nhật coins trên UI thread
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(async () => await UpdateCoinsDisplayAsync()));
                    }
                    else
                    {
                        await UpdateCoinsDisplayAsync();
                    }
                      // Hiển thị notification
                    var coinsAdded = e.Amount / 1000;
                    if (this.InvokeRequired)
                    {
                        this.Invoke(() => 
                        {
                            MessageBox.Show($"🎉 Payment Completed!\n\nAmount: {e.Amount:C}\nCoins Added: {coinsAdded}\n\nYour coins have been updated!", 
                                "Payment Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        });
                    }
                    else
                    {
                        MessageBox.Show($"🎉 Payment Completed!\n\nAmount: {e.Amount:C}\nCoins Added: {coinsAdded}\n\nYour coins have been updated!", 
                            "Payment Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error handling payment completion: {ex.Message}");
            }
        }
    }    // Event args for book selection
    public class BookSelectedEventArgs : EventArgs
    {
        public int BookId { get; set; }
        public BookSelectedEventArgs(int bookId)
        {
            BookId = bookId;
        }
    }
}