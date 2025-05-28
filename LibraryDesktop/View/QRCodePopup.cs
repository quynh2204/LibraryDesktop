using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryDesktop.Data.Services;
using LibraryDesktop.Models;
using System.Diagnostics;
using Guna.UI2.WinForms;

namespace LibraryDesktop.View
{
    public partial class QRCodePopup : Form
    {        private readonly IPaymentService _paymentService;
        private readonly string _paymentToken;
        private readonly int _amount;
        private readonly string _description;
        private readonly int _userId;
        private System.Windows.Forms.Timer? _statusTimer;
        private bool _paymentCompleted = false;
        private int _pollCount = 0;
        private const int MAX_POLL_ATTEMPTS = 100; // 5 minutes (100 * 3 seconds)// UI Controls - Guna UI 2 Components
        private Guna2PictureBox? picQRCode;
        private Guna2HtmlLabel? lblTitle;
        private Guna2HtmlLabel? lblAmount;
        private Guna2HtmlLabel? lblDescription;
        private Guna2HtmlLabel? lblInstructions;
        private Guna2Button? btnOpenBrowser;
        private Guna2Button? btnCheckStatus;
        private Guna2Button? btnCancel;
        private Guna2HtmlLabel? lblStatus;
        private Guna2Panel? mainPanel;        public bool PaymentCompleted => _paymentCompleted;        public QRCodePopup(IPaymentService paymentService, string paymentToken, int amount, string description, int userId)
        {
            _paymentService = paymentService;
            _paymentToken = paymentToken;
            _amount = amount;
            _description = description;
            _userId = userId;
            
            Console.WriteLine($"🔵 QRCodePopup created for token: {_paymentToken}, amount: {_amount}, user: {_userId}");
            
            InitializeComponent();
            SetupUI();
            LoadQRCodeAsync();
            
            // Subscribe to payment completion event
            _paymentService.PaymentCompleted += OnPaymentCompleted;
            
            StartStatusPolling();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(500, 650);
            this.Text = "Payment QR Code";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
        }        private void SetupUI()
        {
            // Main Panel
            mainPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.White,
                BorderRadius = 10
            };
            this.Controls.Add(mainPanel);            // Title
            lblTitle = new Guna2HtmlLabel
            {
                Text = "Scan QR Code to Pay",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Location = new Point(20, 20),
                Size = new Size(460, 30),
                BackColor = Color.Transparent
            };
            mainPanel.Controls.Add(lblTitle);            // Amount
            lblAmount = new Guna2HtmlLabel
            {
                Text = $"Amount: {_amount:N0} VND ({((decimal)_amount / 1000):N0} coins)",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Green,
                Location = new Point(20, 60),
                Size = new Size(460, 25),
                BackColor = Color.Transparent
            };
            mainPanel.Controls.Add(lblAmount);

            // Description
            lblDescription = new Guna2HtmlLabel
            {
                Text = _description,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                Location = new Point(20, 90),
                Size = new Size(460, 25),
                BackColor = Color.Transparent
            };
            mainPanel.Controls.Add(lblDescription);

            // QR Code
            picQRCode = new Guna2PictureBox
            {
                Location = new Point(125, 130),
                Size = new Size(250, 250),
                SizeMode = PictureBoxSizeMode.Zoom,
                FillColor = Color.White,
                BorderRadius = 10
            };
            mainPanel.Controls.Add(picQRCode);

            // Instructions
            lblInstructions = new Guna2HtmlLabel
            {
                Text = "1. Scan the QR code with your mobile banking app<br/>2. Or click 'Open in Browser' to pay on this device<br/>3. Complete the payment and click 'Check Status'",
                Font = new Font("Segoe UI", 9),
                Location = new Point(20, 400),
                Size = new Size(460, 60),
                BackColor = Color.Transparent
            };
            mainPanel.Controls.Add(lblInstructions);

            // Status
            lblStatus = new Guna2HtmlLabel
            {
                Text = "Waiting for payment...",
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.Orange,
                Location = new Point(20, 470),
                Size = new Size(460, 25),
                BackColor = Color.Transparent
            };
            mainPanel.Controls.Add(lblStatus);

            // Open Browser Button
            btnOpenBrowser = new Guna2Button
            {
                Text = "Open in Browser",
                Location = new Point(50, 510),
                Size = new Size(120, 35),
                FillColor = Color.Blue,
                ForeColor = Color.White,
                BorderRadius = 8,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnOpenBrowser.Click += BtnOpenBrowser_Click!;
            mainPanel.Controls.Add(btnOpenBrowser);

            // Check Status Button
            btnCheckStatus = new Guna2Button
            {
                Text = "Check Status",
                Location = new Point(190, 510),
                Size = new Size(120, 35),
                FillColor = Color.Green,
                ForeColor = Color.White,
                BorderRadius = 8,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnCheckStatus.Click += BtnCheckStatus_Click!;
            mainPanel.Controls.Add(btnCheckStatus);

            // Cancel Button
            btnCancel = new Guna2Button
            {
                Text = "Cancel",
                Location = new Point(330, 510),
                Size = new Size(120, 35),
                FillColor = Color.Gray,
                ForeColor = Color.White,
                BorderRadius = 8,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnCancel.Click += BtnCancel_Click!;
            mainPanel.Controls.Add(btnCancel);

            // Handle form closing
            this.FormClosing += QRCodePopup_FormClosing!;
        }
        private void LoadQRCodeAsync()
        {
            try
            {
                if (lblStatus != null)
                {
                    lblStatus.Text = "Generating QR code...";
                    lblStatus.ForeColor = Color.Blue;
                }                // Generate QR code directly without database operations (using real userId)
                var qrCodeBase64 = _paymentService.GenerateQRCodeForPayment(_paymentToken, _amount, _userId);

                // Convert base64 to image and display
                var qrCodeBytes = Convert.FromBase64String(qrCodeBase64);
                using (var ms = new MemoryStream(qrCodeBytes))
                {
                    if (picQRCode != null)
                        picQRCode.Image = Image.FromStream(ms);
                }

                if (lblStatus != null)
                {
                    lblStatus.Text = "QR code ready. Waiting for payment...";
                    lblStatus.ForeColor = Color.Orange;
                }
            }
            catch (Exception ex)
            {
                if (lblStatus != null)
                {
                    lblStatus.Text = $"Error generating QR code: {ex.Message}";
                    lblStatus.ForeColor = Color.Red;
                }
                MessageBox.Show($"Failed to generate QR code: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        private void StartStatusPolling()
        {
            _statusTimer = new System.Windows.Forms.Timer();
            _statusTimer.Interval = 3000; // Check every 3 seconds
            _statusTimer.Tick += async (sender, e) => await CheckPaymentStatusAsync();
            _statusTimer.Start();
        }        private async Task CheckPaymentStatusAsync()
        {
            // Stop timer during check to prevent overlapping calls
            _statusTimer?.Stop();
            
            try
            {
                _pollCount++;
                
                // Check for timeout
                if (_pollCount >= MAX_POLL_ATTEMPTS)
                {
                    if (lblStatus != null)
                    {
                        lblStatus.Text = "Payment check timeout. Please verify payment manually.";
                        lblStatus.ForeColor = Color.Orange;
                    }
                    return; // Don't restart timer
                }
                
                await CheckPaymentStatus();
            }
            finally
            {
                // Restart timer if payment not completed yet and not timed out
                if (!_paymentCompleted && _pollCount < MAX_POLL_ATTEMPTS && _statusTimer != null)
                {
                    _statusTimer.Start();
                }
            }
        }private async Task CheckPaymentStatus()
        {
            try
            {
                // Check if payment was completed via web payment (self-contained approach)
                var payment = await _paymentService.GetPaymentByTokenAsync(_paymentToken);
                
                if (payment?.PaymentStatus == PaymentStatus.Completed && !_paymentCompleted)
                {
                    // Call payment completion directly since this is our payment
                    HandlePaymentCompletion();
                }
            }
            catch (Exception ex)
            {
                // Silently fail for status polling - don't disrupt user experience
                Console.WriteLine($"Status polling error: {ex.Message}");
            }
        }        private void OnPaymentCompleted(object? sender, PaymentCompletedEventArgs e)
        {
            Console.WriteLine($"🔵 PaymentCompleted event received in QRCodePopup. Token: {e?.PaymentToken}, Our token: {_paymentToken}");
            
            // Check if this is our payment - this is critical to prevent wrong popup from closing
            if (e == null || e.PaymentToken != _paymentToken)
            {
                Console.WriteLine($"🔵 Payment event ignored - not our token");
                return;
            }

            Console.WriteLine($"🔵 Processing payment completion for our token");

            // Ensure UI updates happen on main thread
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnPaymentCompleted(sender, e)));
                return;
            }

            // Handle the payment completion
            HandlePaymentCompletion();
        }

        private void HandlePaymentCompletion()
        {
            // Prevent multiple completion handling
            if (_paymentCompleted)
                return;
            
            _statusTimer?.Stop();
            _paymentCompleted = true;
            
            if (lblStatus != null)
            {
                lblStatus.Text = "Payment completed successfully!";
                lblStatus.ForeColor = Color.Green;
            }
            
            // Update button states
            if (btnCheckStatus != null) btnCheckStatus.Enabled = false;
            if (btnOpenBrowser != null) btnOpenBrowser.Enabled = false;
            if (btnCancel != null) btnCancel.Text = "Close";
              
            MessageBox.Show($"Payment successful!\n{((decimal)_amount / 1000):N0} coins have been added to your account.", 
                "Payment Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }private async void BtnCheckStatus_Click(object sender, EventArgs e)
        {
            if (btnCheckStatus != null)
            {
                btnCheckStatus.Enabled = false;
                btnCheckStatus.Text = "Checking...";
            }
            
            try
            {
                await CheckPaymentStatus();
                
                if (!_paymentCompleted && lblStatus != null)
                {
                    lblStatus.Text = "Payment still pending. Please complete payment.";
                    lblStatus.ForeColor = Color.Orange;
                }
            }
            catch (Exception ex)
            {
                if (lblStatus != null)
                {
                    lblStatus.Text = $"Error checking status: {ex.Message}";
                    lblStatus.ForeColor = Color.Red;
                }
            }
            finally
            {
                if (btnCheckStatus != null)
                {
                    btnCheckStatus.Enabled = true;
                    btnCheckStatus.Text = "Check Status";
                }
            }
        }        private void BtnOpenBrowser_Click(object sender, EventArgs e)
        {
            try
            {
                // Generate self-contained URL with embedded payment data
                var qrCodeBase64 = _paymentService.GenerateQRCodeForPayment(_paymentToken, _amount, _userId);
                
                // Extract URL from the QR code generation (same logic as GenerateQRCodeForPayment)
                var paymentData = new
                {
                    token = _paymentToken,
                    amount = _amount,
                    userId = _userId,
                    timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
                };
                
                var paymentDataJson = System.Text.Json.JsonSerializer.Serialize(paymentData);
                var paymentDataBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(paymentDataJson));
                
                string url = $"{NetworkConfiguration.GetLiveServerUrl()}?data={paymentDataBase64}";
                
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
                
                if (lblStatus != null)
                {
                    lblStatus.Text = "Payment page opened in browser...";
                    lblStatus.ForeColor = Color.Blue;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening browser: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }        private void QRCodePopup_FormClosing(object sender, FormClosingEventArgs e)
        {
            _statusTimer?.Stop();
            _statusTimer?.Dispose();
            
            // Unsubscribe from payment event
            _paymentService.PaymentCompleted -= OnPaymentCompleted;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _statusTimer?.Dispose();
                picQRCode?.Image?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
