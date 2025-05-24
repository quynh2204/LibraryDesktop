using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryDesktop.Data.Services;
using System.Diagnostics;

namespace LibraryDesktop.View
{
    public partial class RechargeForm : Form
    {
        private readonly IPaymentService _paymentService;
        private string? _currentPaymentId;
        private string? _paymentUrl;

        public RechargeForm(IPaymentService paymentService)
        {
            _paymentService = paymentService;
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            // Set form title and default instructions text
            this.Text = "Recharge Account";
            lblInstructions.Text = "Enter an amount in VND to recharge your account.\n1,000 VND = 1 coin";
            
            // Add tooltip for clarity
            var toolTip = new ToolTip();
            toolTip.SetToolTip(txtAmount, "Enter amount in VND (1,000 VND = 1 coin)");
            toolTip.SetToolTip(btnGenerateQR, "Generate QR code for payment");
            toolTip.SetToolTip(btnCheckPayment, "Check if payment is confirmed");
            
            // Show exchange rate label
            lblExchangeRate.Visible = true;
        }

        private async void btnGenerateQR_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Please enter a valid amount.", "Invalid Amount", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                btnGenerateQR.Enabled = false;
                btnGenerateQR.Text = "Generating...";

                // Create payment in database
                var payment = await _paymentService.CreatePaymentAsync(1, amount, "Account Recharge"); // Using userId = 1 for demo
                
                // Generate QR code for payment
                var qrCodeBase64 = await _paymentService.GenerateQRCodeAsync(payment);
                _currentPaymentId = payment.PaymentToken;
                _paymentUrl = payment.PaymentUrl; // <-- Store the payment URL                // Convert base64 to image and display
                var qrCodeBytes = Convert.FromBase64String(qrCodeBase64);
                using (var ms = new MemoryStream(qrCodeBytes))
                {
                    picQRCode.Image = Image.FromStream(ms);
                }

                lblInstructions.Text = "Scan this QR code or click the link below to open payment page.";
                btnCheckPayment.Enabled = true;
                lnkOpenBrowser.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating QR code: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnGenerateQR.Enabled = true;
                btnGenerateQR.Text = "Generate QR Code";
            }
        }

        private async void btnCheckPayment_Click(object? sender, EventArgs e)
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

                // Check payment status by token
                var payment = await _paymentService.GetPaymentByTokenAsync(_currentPaymentId);
                
                if (payment?.PaymentStatus == Models.PaymentStatus.Completed)
                {
                    MessageBox.Show("Payment successful! Your account has been recharged.", 
                        "Payment Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
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
            }            finally
            {
                btnCheckPayment.Enabled = true;
                btnCheckPayment.Text = "Check Payment";
            }
        }

        private void lnkOpenBrowser_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentPaymentId))
            {
                // Open the payment web page in default browser
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
    }
}