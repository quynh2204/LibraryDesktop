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
using System.IO;
using Timer = System.Windows.Forms.Timer;

namespace LibraryDesktop.View
{
    public partial class Exchange : UserControl
    {
        private readonly IPaymentService? _paymentService;
        private readonly User? _currentUser;
        private decimal _totalAmount = 0;
        private readonly Dictionary<Guna.UI2.WinForms.Guna2Panel, decimal> _coinPanelValues =
            new Dictionary<Guna.UI2.WinForms.Guna2Panel, decimal>();        // Track how many times each panel has been clicked
        private readonly Dictionary<Guna.UI2.WinForms.Guna2Panel, int> _coinPanelClickCount =
            new Dictionary<Guna.UI2.WinForms.Guna2Panel, int>();
        
        // Track which panels are selected
        private readonly Dictionary<Guna.UI2.WinForms.Guna2Panel, bool> _coinPanelSelected =
            new Dictionary<Guna.UI2.WinForms.Guna2Panel, bool>();
              // UI Controls - Only keep essential controls for popup-based approach
        private TextBox? totalTextBox;
        private Label? lblExchangeRate;

        #region Constructor
        public Exchange(IPaymentService paymentService) : base()
        {
            _paymentService = paymentService;
            InitializeComponent();
            InitializeCoinPanels();
            SetupExchangeUI();
        }        public Exchange(IPaymentService paymentService, User currentUser) : this(paymentService)
        {
            _currentUser = currentUser;

            // Subscribe to payment completion events
            if (_paymentService != null)
            {
                _paymentService.PaymentCompleted += OnPaymentCompleted;
            }
        }
        #endregion

        #region Initialization Methods
        private void InitializeCoinPanels()
        {
            try
            {
                // Initialize coin panel values với các Guna2Panel từ Designer
                _coinPanelValues[coinPanel10] = 10000m;   // 10k VND
                _coinPanelValues[coinPanel20] = 20000m;   // 20k VND
                _coinPanelValues[coinPanel50] = 50000m;   // 50k VND
                _coinPanelValues[coinPanel100] = 100000m; // 100k VND
                _coinPanelValues[coinPanel200] = 200000m; // 200k VND
                _coinPanelValues[coinPanel500] = 500000m; // 500k VND                // Initialize click count và setup events
                foreach (var kvp in _coinPanelValues)
                {
                    var panel = kvp.Key;
                    _coinPanelClickCount[panel] = 0;
                    SetupCoinPanelStyle(panel);

                    // Remove existing event handlers to prevent duplication
                    panel.Click -= CoinPanel_Click;
                    panel.MouseEnter -= Panel_MouseEnter;
                    panel.MouseLeave -= Panel_MouseLeave;                    // Add click event handler - Guna2Panel có thể nhận click events
                    panel.Click += CoinPanel_Click;

                    // Make panels look clickable
                    panel.Cursor = Cursors.Hand;

                    // Add hover effects for better UX
                    panel.MouseEnter += Panel_MouseEnter;
                    panel.MouseLeave += Panel_MouseLeave;
                }

                // Initialize total display
                UpdateTotalDisplay();
            }
            
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing coin panels: {ex.Message}", "Initialization Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SetupExchangeUI()
        {
            try
            {
                // Set initial state
                UpdateTotalDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting up UI: {ex.Message}", "UI Setup Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // Setup topup_btn if it exists
            if (topup_btn != null)
            {
                topup_btn.Text = "Exchange";
                topup_btn.BackColor = Color.Gray;
                topup_btn.ForeColor = Color.White;
                topup_btn.Enabled = false;
            }        }
        #endregion

        #region UI Event Handlers
        private void Panel_MouseEnter(object? sender, EventArgs e)
        {
            var panel = sender as Guna.UI2.WinForms.Guna2Panel;
            if (panel != null && panel.Enabled)
            {
                // Slight color change on hover
                panel.FillColor = Color.FromArgb(240, 248, 255); // AliceBlue
            }
        }

        private void Panel_MouseLeave(object? sender, EventArgs e)
        {
            var panel = sender as Guna.UI2.WinForms.Guna2Panel;
            if (panel != null && panel.Enabled)
            {
                panel.FillColor = Color.White; // Back to white
            }
        }

        private void SetupCoinPanelStyle(Guna.UI2.WinForms.Guna2Panel panel)
        {
            // Set default Guna2Panel style
            panel.FillColor = Color.White;
            panel.BorderColor = Color.FromArgb(255, 224, 192);
            panel.BorderThickness = 10;
            panel.BorderRadius = 35;
        }
        #endregion

        #region Core Functionality
        private void AddCoinValue(Guna.UI2.WinForms.Guna2Panel panel)
        {
            try
            {
                if (_coinPanelValues.ContainsKey(panel) && panel.Enabled)
                {
                    // Add the panel's value to the total
                    decimal addedValue = _coinPanelValues[panel];
                    _totalAmount += addedValue;

                    // Increment click count
                    _coinPanelClickCount[panel]++;

                    // Update displays
                    UpdateTotalDisplay();
                    UpdatePanelClickIndicator(panel);

                    // Show visual feedback
                    ShowClickFeedback(panel, addedValue);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding coin value: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdatePanelClickIndicator(Guna.UI2.WinForms.Guna2Panel panel)
        {
            try
            {
                // Remove existing click count label
                var existingLabel = panel.Controls.OfType<Label>()
                    .FirstOrDefault(l => l.Name == "ClickCountLabel");

                if (existingLabel != null)
                    panel.Controls.Remove(existingLabel);

                // Add new click count label if clicked
                if (_coinPanelClickCount[panel] > 0)
                {
                    var clickLabel = new Label
                    {
                        Name = "ClickCountLabel",
                        Text = $"x{_coinPanelClickCount[panel]}",
                        Font = new Font("Segoe UI", 10, FontStyle.Bold),
                        ForeColor = Color.White,
                        BackColor = Color.Red,
                        Location = new Point(panel.Width - 40, 5),
                        Size = new Size(35, 25),
                        TextAlign = ContentAlignment.MiddleCenter,
                        BorderStyle = BorderStyle.FixedSingle
                    };
                    panel.Controls.Add(clickLabel);
                    clickLabel.BringToFront();
                }
            }
            catch (Exception ex)
            {
                // Log error but don't show message to user for UI updates
                System.Diagnostics.Debug.WriteLine($"Error updating panel indicator: {ex.Message}");
            }
        }

        private void ShowClickFeedback(Guna.UI2.WinForms.Guna2Panel panel, decimal addedValue)
        {
            try
            {
                // Brief color change to show the click was registered
                var originalColor = panel.FillColor;
                panel.FillColor = Color.LightGreen;


                var timer = new Timer { Interval = 800 };
                timer.Tick += (s, e) =>
                {
                    panel.FillColor = originalColor;
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing click feedback: {ex.Message}");
            }
        }

        private void UpdateTotalDisplay()
        {
            try
            {
                if (total_txt != null)
                {
                    total_txt.Text = $"{_totalAmount:N0} VND";
                }
                // Update button state
                if (topup_btn != null)
                {
                    if (_totalAmount > 0)
                    {
                        topup_btn.Enabled = true;
                        topup_btn.FillColor = Color.FromArgb(202, 95, 101);
                    }
                    else
                    {
                        topup_btn.Enabled = false;
                        topup_btn.FillColor = Color.Gray;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating total display: {ex.Message}");
            }
        }

        private void ClearTotal()
        {
            try
            {
                _totalAmount = 0;

                foreach (var panel in _coinPanelValues.Keys)
                {
                    _coinPanelClickCount[panel] = 0;
                    UpdatePanelClickIndicator(panel);
                    panel.FillColor = Color.White;
                }

                UpdateTotalDisplay();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error clearing total: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearTotal();
            ResetForm();
        }
        #endregion

        #region Payment Processing
        private void topup_btn_Click(object sender, EventArgs e)
        {
            try
            {

                if (_paymentService == null)
                {
                    MessageBox.Show("Dịch vụ thanh toán không khả dụng.", "Lỗi dịch vụ",
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
                // Disable button and show processing
                topup_btn.Enabled = false;

                // Create payment in database                // Show QRCodePopup instead of inline display
                using (var qrPopup = new QRCodePopup(_paymentService, tempPaymentToken, (int)_totalAmount, description, _currentUser.UserId))
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

                // Disable coin panel selection during payment
                foreach (var panel in _coinPanelValues.Keys)
                {
                    panel.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xử lý đổi tiền: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (topup_btn != null && _totalAmount > 0)
                {
                    topup_btn.Enabled = true;
                }
            }
        }


        private void ResetForm()
        {
            try
            {
                foreach (var panel in _coinPanelValues.Keys)
                {
                    _coinPanelClickCount[panel] = 0;
                    UpdatePanelClickIndicator(panel);
                    panel.Enabled = true;
                    panel.FillColor = Color.White;
                }

                _totalAmount = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi reset form: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Designer Event Handlers (Compatibility)
        private void Exchange_Load(object sender, EventArgs e)
        {
            try
            {
                // Initialize the exchange panel
                if (coinPanel10 != null)
                {
                    coinPanel10.Visible = true;
                    coinPanel10.BringToFront();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi load form: {ex.Message}", "Lỗi tải",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        // Coin panel click events - REMOVED to prevent duplication
        // All clicks are now handled by the generic CoinPanel_Click method
        
        private void coinPanel10_Click(object sender, EventArgs e)
        {
            // Event handler exists for Designer compatibility but does nothing
            // Actual handling is done by CoinPanel_Click
        }

        private void coinPanel20_Click(object sender, EventArgs e)
        {
            // Event handler exists for Designer compatibility but does nothing
            // Actual handling is done by CoinPanel_Click
        }

        private void coinPanel50_Click(object sender, EventArgs e)
        {
            // Event handler exists for Designer compatibility but does nothing
            // Actual handling is done by CoinPanel_Click
        }

        private void coinPanel100_Click(object sender, EventArgs e)
        {
            // Event handler exists for Designer compatibility but does nothing
            // Actual handling is done by CoinPanel_Click
        }

        private void coinPanel200_Click(object sender, EventArgs e)
        {
            // Event handler exists for Designer compatibility but does nothing
            // Actual handling is done by CoinPanel_Click
        }

        private void coinPanel500_Click(object sender, EventArgs e)
        {
            // Event handler exists for Designer compatibility but does nothing
            // Actual handling is done by CoinPanel_Click
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

                MessageBox.Show(successMsg, "Exchange Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);                // Reset form after successful exchange
                ResetForm();
            }
        }
        
        #endregion

        private void CoinPanel_Click(object? sender, EventArgs e)
        {
            if (sender is Guna.UI2.WinForms.Guna2Panel clickedPanel)
            {
                AddCoinValue(clickedPanel);
            }
        }
    }
}