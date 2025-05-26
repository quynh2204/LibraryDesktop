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

namespace LibraryDesktop.View
{
    public partial class Home : UserControl
    {
        private readonly IServiceProvider _serviceProvider;

        public Home(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeComponent();
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

            // Create and show Exchange control
            var exchangeControl = _serviceProvider.GetRequiredService<Exchange>();
            exchangeControl.Dock = DockStyle.Fill;

            // Add to main panel (or create a content panel if needed)
            this.Controls.Add(exchangeControl);
            exchangeControl.BringToFront();
        }
        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {
            // Show user account information or exchange
            ShowExchangeForm();
        }
    }
}
