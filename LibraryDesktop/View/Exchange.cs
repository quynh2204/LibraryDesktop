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
using System.IO;
using Timer = System.Windows.Forms.Timer;

namespace LibraryDesktop.View
{
    public partial class Exchange : UserControl
    {
        #region Private Fields
        private readonly IPaymentService _paymentService;
        private string? _currentPaymentId;
        private string? _paymentUrl;
        private decimal _totalAmount = 0;

        // Dictionary to store coin panel values - sử dụng Guna2Panel
        private readonly Dictionary<Guna.UI2.WinForms.Guna2Panel, decimal> _coinPanelValues =
            new Dictionary<Guna.UI2.WinForms.Guna2Panel, decimal>();

        // Track how many times each panel has been clicked
        private readonly Dictionary<Guna.UI2.WinForms.Guna2Panel, int> _coinPanelClickCount =
            new Dictionary<Guna.UI2.WinForms.Guna2Panel, int>();

        // UI Controls for payment display
        private PictureBox picQRCode;

        private LinkLabel lnkOpenBrowser;
        #endregion

        #region Constructor
        public Exchange(IPaymentService paymentService) : base()
        {
            _paymentService = paymentService;
            InitializeComponent();
            InitializeCoinPanels();
            SetupExchangeUI();
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
                _coinPanelValues[coinPanel500] = 500000m; // 500k VND

                // Initialize click count và setup events
                foreach (var kvp in _coinPanelValues)
                {
                    var panel = kvp.Key;
                    _coinPanelClickCount[panel] = 0;
                    SetupCoinPanelStyle(panel);

                    // Add click event handler - Guna2Panel có thể nhận click events
                    panel.Click += (sender, e) => AddCoinValue((Guna.UI2.WinForms.Guna2Panel)sender);

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
                // Create additional UI controls for payment process
                CreatePaymentControls();

                // Set initial state
                UpdateTotalDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting up UI: {ex.Message}", "UI Setup Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreatePaymentControls()
        {
            // Create QR Code display
            if (picQRCode == null)
            {
                picQRCode = new PictureBox
                {
                    Size = new Size(200, 200),
                    Location = new Point(451, 475),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BorderStyle = BorderStyle.FixedSingle,
                    Visible = false,
                    BackColor = Color.White
                };
                this.Controls.Add(picQRCode);
            }

            // Create browser link
            if (lnkOpenBrowser == null)
            {
                lnkOpenBrowser = new LinkLabel
                {
                    Text = "Mở trang thanh toán",
                    Location = new Point(434, 674),
                    Size = new Size(165, 20),
                    Visible = false,
                    Font = new Font("Segoe UI", 9F)
                };
                lnkOpenBrowser.LinkClicked += LnkOpenBrowser_LinkClicked;
                this.Controls.Add(lnkOpenBrowser);
            }

  
        }
        #endregion

        #region UI Event Handlers
        private void Panel_MouseEnter(object sender, EventArgs e)
        {
            var panel = sender as Guna.UI2.WinForms.Guna2Panel;
            if (panel != null && panel.Enabled)
            {
                // Slight color change on hover
                panel.FillColor = Color.FromArgb(240, 248, 255); // AliceBlue
            }
        }

        private void Panel_MouseLeave(object sender, EventArgs e)
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
        private async void topup_btn_Click(object sender, EventArgs e)
        {
            try
            {

                if (_paymentService == null)
                {
                    MessageBox.Show("Dịch vụ thanh toán không khả dụng.", "Lỗi dịch vụ",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Disable button and show processing
                topup_btn.Enabled = false;

                // Create payment in database
                var payment = await _paymentService.CreatePaymentAsync(1, _totalAmount);

                // Generate QR code for payment
                var qrCodeBase64 = await _paymentService.GenerateQRCodeAsync(payment);
                _currentPaymentId = payment.PaymentToken;
                _paymentUrl = payment.PaymentUrl;

                // Convert base64 to image and display
                var qrCodeBytes = Convert.FromBase64String(qrCodeBase64);
                using (var ms = new MemoryStream(qrCodeBytes))
                {
                    if (picQRCode.Image != null)
                        picQRCode.Image.Dispose();
                    picQRCode.Image = Image.FromStream(ms);
                }

                // Update UI for payment mode
                picQRCode.Visible = true;
               
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

        private async void BtnCheckPayment_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_currentPaymentId))
                {
                    MessageBox.Show("Không có giao dịch nào đang xử lý.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                btnCheckPayment.Enabled = false;

                var payment = await _paymentService.GetPaymentByTokenAsync(_currentPaymentId);

                if (payment?.PaymentStatus == LibraryDesktop.Models.PaymentStatus.Completed)
                {
                    decimal totalCoins = _totalAmount / 1000;

                    string successMsg = $"Đổi tiền thành công!\n{totalCoins:N0} xu đã được thêm vào tài khoản.\n\nGiao dịch hoàn tất: {_totalAmount:N0} VND";

                    MessageBox.Show(successMsg, "Đổi tiền hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reset form after successful exchange
                    ResetForm();
                }
                else
                {
                    MessageBox.Show("Thanh toán vẫn đang chờ xử lý. Vui lòng hoàn tất thanh toán và thử lại.",
                        "Thanh toán chờ xử lý", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kiểm tra thanh toán: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCheckPayment.Enabled = true;
            }
        }

        private void LnkOpenBrowser_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentPaymentId))
            {
                string url = $"http://192.168.1.4:5500/LibraryDesktop.Data/WebRoot/index.html?token={_currentPaymentId}&amount={_totalAmount}";
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
                    MessageBox.Show($"Lỗi mở trình duyệt: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                _currentPaymentId = null;
                _paymentUrl = null;

                UpdateTotalDisplay();

                if (picQRCode != null)
                {
                    picQRCode.Image?.Dispose();
                    picQRCode.Image = null;
                    picQRCode.Visible = false;
                }
                if (btnCheckPayment != null) btnCheckPayment.Enabled = false;
                if (lnkOpenBrowser != null) lnkOpenBrowser.Visible = false;
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
        }

        // Keep these for Designer compatibility
        private void coinPanel10_Click(object sender, EventArgs e) => AddCoinValue(coinPanel10);
        private void coinPanel20_Click(object sender, EventArgs e) => AddCoinValue(coinPanel20);
        private void coinPanel50_Click(object sender, EventArgs e) => AddCoinValue(coinPanel50);
        private void coinPanel100_Click(object sender, EventArgs e) => AddCoinValue(coinPanel100);
        private void coinPanel200_Click(object sender, EventArgs e) => AddCoinValue(coinPanel200);
        private void coinPanel500_Click(object sender, EventArgs e) => AddCoinValue(coinPanel500);
        #endregion
    }
}