using LibraryDesktop.Data.Services;
using LibraryDesktop.Models;
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
{    public partial class Exchange : UserControl
    {
        private readonly IPaymentService? _paymentService;
        private readonly User? _currentUser;
        private int _totalAmount = 0;
        private Dictionary<Panel, int> _coinPanelValues = new Dictionary<Panel, int>();
        private Dictionary<Panel, bool> _coinPanelSelected = new Dictionary<Panel, bool>();

        // UI Controls - Only keep essential controls for popup-based approach
        private TextBox? totalTextBox;
        private Label? lblInstructions;
        private Label? lblExchangeRate;

        public Exchange()
        {
            InitializeComponent();
            InitializeCoinPanels();
            SetupExchangeUI();
        }        public Exchange(IPaymentService paymentService) : this()
        {
            _paymentService = paymentService;
        }        public Exchange(IPaymentService paymentService, User currentUser) : this()
        {
            _paymentService = paymentService;
            _currentUser = currentUser;
            
            // Subscribe to payment completion events
            if (_paymentService != null)
            {
                _paymentService.PaymentCompleted += OnPaymentCompleted;
            }
        }private void InitializeCoinPanels()
        {
            // Initialize coin panel values
            _coinPanelValues[coinPanel10] = 10000;   // 10k VND
            _coinPanelValues[coinPanel20] = 20000;   // 20k VND
            _coinPanelValues[coinPanel50] = 50000;   // 50k VND
            _coinPanelValues[coinPanel100] = 100000; // 100k VND
            _coinPanelValues[coinPanel200] = 200000; // 200k VND
            _coinPanelValues[coinPanel500] = 500000; // 500k VND

            // Initialize selection state
            foreach (var panel in _coinPanelValues.Keys)
            {
                _coinPanelSelected[panel] = false;
                SetupCoinPanelStyle(panel, false);
            }
        }        private void SetupExchangeUI()
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
        }        private void topup_btn_Click(object? sender, EventArgs e)
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

                if (_currentUser == null)
                {
                    MessageBox.Show("User not available.", "User Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Create payment description
                var selectedPanels = _coinPanelSelected.Where(kvp => kvp.Value).Select(kvp => kvp.Key);
                var selectedAmounts = selectedPanels.Select(panel => _coinPanelValues[panel]);
                string description = $"Coin Exchange: {string.Join(" + ", selectedAmounts.Select(a => $"{a:N0}"))} VND";

                // Generate temporary payment token for QR code
                string tempPaymentToken = Guid.NewGuid().ToString();

                // Show QRCodePopup instead of inline display
                using (var qrPopup = new QRCodePopup(_paymentService, tempPaymentToken, _totalAmount, description, _currentUser.UserId))
                {
                    // Show popup as dialog
                    var result = qrPopup.ShowDialog();
                    
                    // Check if payment was completed
                    if (qrPopup.PaymentCompleted)
                    {
                        // Payment was successful - reset form
                        ResetForm();
                        MessageBox.Show("Exchange completed successfully!", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        // User cancelled the payment
                        MessageBox.Show("Payment cancelled.", "Cancelled", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing exchange: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            if (totalTextBox != null) totalTextBox.Text = "0 VND";
                        
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

        private void OnPaymentCompleted(object? sender, PaymentCompletedEventArgs e)
        {
            // This method will be called when a payment is completed
            if (this.InvokeRequired)
            {
                this.Invoke(() => OnPaymentCompleted(sender, e));
                return;
            }

            // Check if this payment relates to our current exchange
            if (_currentUser != null && e.UserId == _currentUser.UserId)
            {
                int totalCoins = e.Amount / 1000;
                string successMsg = $"Exchange successful!\n{totalCoins:N0} coins have been added to your account.\n\nTransaction completed: {e.Amount:N0} VND";

                MessageBox.Show(successMsg, "Exchange Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reset form after successful exchange
                ResetForm();
            }
        }
    }
}