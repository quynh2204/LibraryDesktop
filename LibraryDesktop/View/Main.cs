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
        private Home _homeControl;

        private readonly IBookService _bookService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IPaymentService _paymentService;
        private readonly PaymentWebServer _paymentWebServer;
        private User? _currentUser;
        private decimal _currentBalance;
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

            _paymentWebServer = paymentWebServer;
            InitializeComponent();

            _paymentService = paymentService;

            // Pass the paymentService to the Exchange instance
            exchange1 = new Exchange(_paymentService);

            _homeControl = _serviceProvider.GetRequiredService<Home>();
            _homeControl.Dock = DockStyle.Fill;
            this.Controls.Add(_homeControl);
            _homeControl.BringToFront(); // Hiển thị Home ngay khi mở form

        }
        public async Task InitializeWithUserAsync(User user)
        {
            _currentUser = user;

            // Load user balance
            _currentBalance = await _userService.GetUserBalanceAsync(user.UserId);

            // Update UI
            this.Text = $"Library Desktop - Welcome {user.Username}";

            // Update account label if it exists
            //if (guna2HtmlLabel1 != null)
            //{
            //    guna2HtmlLabel1.Text = user.Username;
            //}

            // Start payment web server
            try
            {
                await _paymentWebServer.StartAsync(NetworkConfiguration.API_SERVER_PORT);
                Debug.WriteLine("Payment web server started successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to start payment web server: {ex.Message}");
            }
        }


        public void ShowHomeView()
        {
            // Xóa các control khác như Exchange (nếu có)
            var exchangeControls = this.Controls.OfType<Exchange>().ToList();
            foreach (var control in exchangeControls)
            {
                this.Controls.Remove(control);
                control.Dispose();
            }

            _homeControl.BringToFront();
            _homeControl.ShowBookList();
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




        // Add navigation event handlers for tile buttons
        private void btnHome_Click(object sender, EventArgs e)
        {
            // Books - show home view
            ShowHomeView();
        }

        private void btnExchange_Click(object sender, EventArgs e)
        {
            // Returned Books functionality
            ShowHomeView();
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            // Borrowing functionality  
            ShowHomeView();
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            ShowHomeView();
        }

        private void home1_Load(object sender, EventArgs e)
        {

        }

        //private void guna2GradientTileButton4_Click(object sender, EventArgs e)
        //{
        //    // Fine Ticket / Exchange functionality
        //    ShowExchangeForm();
        //}

        //private void btn_vaid_Click(object sender, EventArgs e)
        //{
        //    // Account / Exchange functionality
        //    ShowExchangeForm();
        //}
    }
}
