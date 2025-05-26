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
        private decimal _currentBalance;

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
