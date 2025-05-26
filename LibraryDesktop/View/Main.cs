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
    public partial class Main : Form
    {
        private readonly IBookService _bookService;
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
        }        public async Task InitializeWithUserAsync(User user)
        {
            _currentUser = user;
            
            // Load user balance
            _currentBalance = await _userService.GetUserBalanceAsync(user.UserId);
            
            // Update UI with user information
            this.Text = $"Library Desktop - Welcome {user.Username}";
            
            // Update account label if it exists
            if (guna2HtmlLabel1 != null)
            {
                guna2HtmlLabel1.Text = user.Username;
            }
            
            // Start payment web server
            try
            {
                await _paymentWebServer.StartAsync();
                Debug.WriteLine("Payment web server started successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to start payment web server: {ex.Message}");
            }
        }

        public void ShowExchangeForm()
        {
            // Hide current content in flowLayoutPanel1
            if (flowLayoutPanel1 != null)
            {
                flowLayoutPanel1.Visible = false;
            }

            // Create and show Exchange control
            var exchangeControl = _serviceProvider.GetRequiredService<Exchange>();
            exchangeControl.Dock = DockStyle.Fill;
            
            // Add to main panel (or create a content panel if needed)
            this.Controls.Add(exchangeControl);
            exchangeControl.BringToFront();
        }

        public void ShowHomeView()
        {
            // Remove any exchange controls
            var exchangeControls = this.Controls.OfType<Exchange>().ToList();
            foreach (var control in exchangeControls)
            {
                this.Controls.Remove(control);
                control.Dispose();
            }

            // Show the book list again
            if (flowLayoutPanel1 != null)
            {
                flowLayoutPanel1.Visible = true;
                flowLayoutPanel1.BringToFront();
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
            }
            
            base.OnFormClosing(e);
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

        // Add navigation event handlers for tile buttons
        private void guna2GradientTileButton1_Click(object sender, EventArgs e)
        {
            // Books - show home view
            ShowHomeView();
        }

        private void guna2GradientTileButton2_Click(object sender, EventArgs e)
        {
            // Returned Books functionality
            ShowHomeView();
        }

        private void guna2GradientTileButton3_Click(object sender, EventArgs e)
        {
            // Borrowing functionality  
            ShowHomeView();
        }

        private void guna2GradientTileButton4_Click(object sender, EventArgs e)
        {
            // Fine Ticket / Exchange functionality
            ShowExchangeForm();
        }

        private void btn_vaid_Click(object sender, EventArgs e)
        {
            // Account / Exchange functionality
            ShowExchangeForm();
        }
    }
}
