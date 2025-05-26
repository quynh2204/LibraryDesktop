using LibraryDesktop.Data.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace LibraryDesktop.View
{
    public partial class Exchange : UserControl
    {
        private readonly IPaymentService _paymentService;
        private string? _currentPaymentId;
        private string? _paymentUrl;
        private decimal _totalAmount = 0;
        private Dictionary<Panel, decimal> _coinPanelValues = new Dictionary<Panel, decimal>();
        private Dictionary<Panel, bool> _coinPanelSelected = new Dictionary<Panel, bool>();

        // UI Controls - Add these to your designer or create them programmatically
        private TextBox totalTextBox;
        private PictureBox picQRCode;
        private Label lblInstructions;
        private Button btnCheckPayment;
        private LinkLabel lnkOpenBrowser;
        private Label lblExchangeRate;

        public Exchange()
        {
            InitializeComponent();
            InitializeCoinPanels();
            SetupExchangeUI();
        }

        public Exchange(IPaymentService paymentService) : this()
        {
            _paymentService = paymentService;
        }

        private void InitializeCoinPanels()
        {
            // Initialize coin panel values (assuming these panels exist in your designer)
            _coinPanelValues[coinPanel10] = 10000m;   // 10k VND
            _coinPanelValues[coinPanel20] = 20000m;   // 20k VND
            _coinPanelValues[coinPanel50] = 50000m;   // 50k VND
            _coinPanelValues[coinPanel100] = 100000m; // 100k VND
            _coinPanelValues[coinPanel200] = 200000m; // 200k VND
            _coinPanelValues[coinPanel500] = 500000m; // 500k VND

            // Initialize selection state
            foreach (var panel in _coinPanelValues.Keys)
            {
                _coinPanelSelected[panel] = false;
                SetupCoinPanelStyle(panel, false);
            }
        }

        private void SetupExchangeUI()
        {
            // If these controls don't exist in designer, create them programmatically
            if (totalTextBox == null)
            {
                totalTextBox = new TextBox
                {
                    Location = new Point(20, 300),
                    Size = new Size(200, 30),
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    ReadOnly = true,
                    BackColor = Color.LightYellow,
                    Text = "0 VND",
                    TextAlign = HorizontalAlignment.Center
                };
                this.Controls.Add(totalTextBox);
            }

            if (lblExchangeRate == null)
            {
                lblExchangeRate = new Label
                {
                    Text = "Exchange Rate: 1,000 VND = 1 coin",
                    Location = new Point(20, 20),
                    Size = new Size(300, 25),
                    Font = new Font("Arial", 9, FontStyle.Bold),
                    ForeColor = Color.Blue
                };
                this.Controls.Add(lblExchangeRate);
            }

            if (lblInstructions == null)
            {
                lblInstructions = new Label
                {
                    Text = "Click coin panels to select amounts for exchange. You can select multiple panels.",
                    Location = new Point(20, 50),
                    Size = new Size(500, 40),
                    Font = new Font("Arial", 9)
                };
                this.Controls.Add(lblInstructions);
            }

            if (picQRCode == null)
            {
                picQRCode = new PictureBox
                {
                    Location = new Point(400, 300),
                    Size = new Size(150, 150),
                    BorderStyle = BorderStyle.FixedSingle,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.White
                };
                this.Controls.Add(picQRCode);
            }

            if (btnCheckPayment == null)
            {
                btnCheckPayment = new Button
                {
                    Text = "Check Payment",
                    Location = new Point(400, 460),
                    Size = new Size(150, 30),
                    BackColor = Color.Blue,
                    ForeColor = Color.White,
                    Enabled = false
                };
                btnCheckPayment.Click += BtnCheckPayment_Click;
                this.Controls.Add(btnCheckPayment);
            }

            if (lnkOpenBrowser == null)
            {
                lnkOpenBrowser = new LinkLabel
                {
                    Text = "Open payment page",
                    Location = new Point(400, 500),
                    Size = new Size(150, 20),
                    Visible = false
                };
                lnkOpenBrowser.LinkClicked += LnkOpenBrowser_LinkClicked;
                this.Controls.Add(lnkOpenBrowser);
            }

            // Setup topup_btn if it exists
            if (topup_btn != null)
            {
                topup_btn.Text = "Exchange";
                topup_btn.BackColor = Color.Gray;
                topup_btn.ForeColor = Color.White;
                topup_btn.Enabled = false;
            }

            CalculateTotal();
        }

        private void SetupCoinPanelStyle(Panel panel, bool isSelected)
        {
            if (isSelected)
            {
                panel.BackColor = Color.LightGreen;
                panel.BorderStyle = BorderStyle.Fixed3D;

                // Add selection indicator if not exists
                var indicator = panel.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "SelectionIndicator");
                if (indicator == null)
                {
                    indicator = new Label
                    {
                        Name = "SelectionIndicator",
                        Text = "✓",
                        Font = new Font("Arial", 16, FontStyle.Bold),
                        ForeColor = Color.Green,
                        BackColor = Color.Transparent,
                        Location = new Point(5, 5),
                        Size = new Size(20, 20)
                    };
                    panel.Controls.Add(indicator);
                }
                indicator.Visible = true;
            }
            else
            {
                panel.BackColor = SystemColors.Control;
                panel.BorderStyle = BorderStyle.FixedSingle;

                // Hide selection indicator
                var indicator = panel.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "SelectionIndicator");
                if (indicator != null)
                {
                    indicator.Visible = false;
                }
            }
        }

        private void ToggleCoinPanel(Panel panel)
        {
            if (_coinPanelValues.ContainsKey(panel))
            {
                _coinPanelSelected[panel] = !_coinPanelSelected[panel];
                SetupCoinPanelStyle(panel, _coinPanelSelected[panel]);
                CalculateTotal();
            }
        }

        private void CalculateTotal()
        {
            _totalAmount = _coinPanelSelected
                .Where(kvp => kvp.Value)
                .Sum(kvp => _coinPanelValues[kvp.Key]);

            if (totalTextBox != null)
            {
                if (_totalAmount > 0)
                {
                    decimal coinEquivalent = _totalAmount / 1000;
                    totalTextBox.Text = $"{_totalAmount:N0} VND ({coinEquivalent:N0} coins)";

                    if (topup_btn != null)
                    {
                        topup_btn.Enabled = true;
                        topup_btn.BackColor = Color.Orange;
                        topup_btn.Text = "Exchange";
                    }
                }
                else
                {
                    totalTextBox.Text = "0 VND";

                    if (topup_btn != null)
                    {
                        topup_btn.Enabled = false;
                        topup_btn.BackColor = Color.Gray;
                        topup_btn.Text = "Select Coins";
                    }
                }
            }
        }

        private async void topup_btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (_totalAmount <= 0)
                {
                    MessageBox.Show("Please select at least one coin panel.", "No Amount Selected",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_paymentService == null)
                {
                    MessageBox.Show("Payment service not available.", "Service Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                topup_btn.Enabled = false;
                topup_btn.Text = "Processing...";

                // Create payment description
                var selectedPanels = _coinPanelSelected.Where(kvp => kvp.Value).Select(kvp => kvp.Key);
                var selectedAmounts = selectedPanels.Select(panel => _coinPanelValues[panel]);
                string description = $"Coin Exchange: {string.Join(" + ", selectedAmounts.Select(a => $"{a:N0}"))} VND";

                // Create payment in database
                var payment = await _paymentService.CreatePaymentAsync(1, _totalAmount, description);

                // Generate QR code for payment
                var qrCodeBase64 = await _paymentService.GenerateQRCodeAsync(payment);
                _currentPaymentId = payment.PaymentToken;
                _paymentUrl = payment.PaymentUrl;

                // Convert base64 to image and display
                var qrCodeBytes = Convert.FromBase64String(qrCodeBase64);
                using (var ms = new MemoryStream(qrCodeBytes))
                {
                    picQRCode.Image = Image.FromStream(ms);
                }

                if (lblInstructions != null)
                {
                    lblInstructions.Text = "Scan this QR code or click the link below to complete your coin exchange.";
                }

                btnCheckPayment.Enabled = true;
                lnkOpenBrowser.Visible = true;

                // Disable coin panel selection during payment
                foreach (var panel in _coinPanelValues.Keys)
                {
                    panel.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing exchange: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (topup_btn != null)
                {
                    topup_btn.Enabled = _totalAmount > 0;
                    topup_btn.Text = "Exchange";
                }
            }
        }

        private async void BtnCheckPayment_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_currentPaymentId))
                {
                    MessageBox.Show("No payment in progress.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                btnCheckPayment.Enabled = false;
                btnCheckPayment.Text = "Checking...";

                var payment = await _paymentService.GetPaymentByTokenAsync(_currentPaymentId);

                if (payment?.PaymentStatus == LibraryDesktop.Models.PaymentStatus.Completed)
                {
                    var selectedCount = _coinPanelSelected.Count(kvp => kvp.Value);
                    decimal totalCoins = _totalAmount / 1000;

                    string successMsg = $"Exchange successful!\n{totalCoins:N0} coins have been added to your account.\n\nTransaction completed: {_totalAmount:N0} VND";

                    MessageBox.Show(successMsg, "Exchange Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reset form after successful exchange
                    ResetForm();
                }
                else
                {
                    MessageBox.Show("Payment is still pending. Please complete the payment and try again.",
                        "Payment Pending", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking payment: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCheckPayment.Enabled = true;
                btnCheckPayment.Text = "Check Payment";
            }
        }

        private void LnkOpenBrowser_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentPaymentId))
            {
                string url = $"http://localhost:8080/payment?token={_currentPaymentId}";
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening browser: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ResetForm()
        {
            foreach (var panel in _coinPanelValues.Keys)
            {
                _coinPanelSelected[panel] = false;
                SetupCoinPanelStyle(panel, false);
                panel.Enabled = true;
            }

            _totalAmount = 0;
            _currentPaymentId = null;
            _paymentUrl = null;

            if (totalTextBox != null) totalTextBox.Text = "0 VND";
            if (picQRCode != null) picQRCode.Image = null;
            if (btnCheckPayment != null) btnCheckPayment.Enabled = false;
            if (lnkOpenBrowser != null) lnkOpenBrowser.Visible = false;
            if (lblInstructions != null)
            {
                lblInstructions.Text = "Click coin panels to select amounts for exchange. You can select multiple panels.";
            }

            CalculateTotal();
        }

        private void Exchange_Load(object sender, EventArgs e)
        {
            // Initialize the exchange panel
            if (coinPanel10 != null)
            {
                coinPanel10.Visible = true;
                coinPanel10.BringToFront();
            }
        }

        // Coin panel click events
        private void coinPanel10_Click(object sender, EventArgs e)
        {
            ToggleCoinPanel(coinPanel10);
        }

        private void coinPanel20_Click(object sender, EventArgs e)
        {
            ToggleCoinPanel(coinPanel20);
        }

        private void coinPanel50_Click(object sender, EventArgs e)
        {
            ToggleCoinPanel(coinPanel50);
        }

        private void coinPanel100_Click(object sender, EventArgs e)
        {
            ToggleCoinPanel(coinPanel100);
        }

        private void coinPanel200_Click(object sender, EventArgs e)
        {
            ToggleCoinPanel(coinPanel200);
        }

        private void coinPanel500_Click(object sender, EventArgs e)
        {
            ToggleCoinPanel(coinPanel500);
        }

        internal DialogResult ShowDialog()
        {
            throw new NotImplementedException();
        }
    }
}