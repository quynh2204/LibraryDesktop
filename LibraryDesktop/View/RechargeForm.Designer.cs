using Guna.UI2.WinForms;
using System.Diagnostics;
using System.Windows.Forms;

namespace LibraryDesktop.View
{
    partial class RechargeForm
    {
        private System.ComponentModel.IContainer components = null;
        private Guna.UI2.WinForms.Guna2TextBox txtAmount;
        private Guna.UI2.WinForms.Guna2Button btnGenerateQR;
        private Guna.UI2.WinForms.Guna2Button btnCheckPayment;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblInstructions;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblExchangeRate;
        private Guna.UI2.WinForms.Guna2PictureBox picQRCode;
        private Guna.UI2.WinForms.Guna2Panel mainPanel;
        private System.Windows.Forms.LinkLabel lnkOpenBrowser;
        private Guna.UI2.WinForms.Guna2ControlBox controlBoxClose;
        private Guna.UI2.WinForms.Guna2ControlBox controlBoxMinimize;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mainPanel = new Guna.UI2.WinForms.Guna2Panel();
            this.picQRCode = new Guna.UI2.WinForms.Guna2PictureBox();
            this.lblInstructions = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblExchangeRate = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtAmount = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnGenerateQR = new Guna.UI2.WinForms.Guna2Button();
            this.btnCheckPayment = new Guna.UI2.WinForms.Guna2Button();
            this.lnkOpenBrowser = new System.Windows.Forms.LinkLabel();
            this.controlBoxClose = new Guna.UI2.WinForms.Guna2ControlBox();
            this.controlBoxMinimize = new Guna.UI2.WinForms.Guna2ControlBox();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picQRCode)).BeginInit();
            this.SuspendLayout();
            
            // mainPanel
            this.mainPanel.BorderRadius = 15;
            this.mainPanel.Controls.Add(this.picQRCode);
            this.mainPanel.Controls.Add(this.lblInstructions);
            this.mainPanel.Controls.Add(this.lblExchangeRate);
            this.mainPanel.Controls.Add(this.txtAmount);
            this.mainPanel.Controls.Add(this.btnGenerateQR);
            this.mainPanel.Controls.Add(this.btnCheckPayment);
            this.mainPanel.Controls.Add(this.lnkOpenBrowser);
            this.mainPanel.FillColor = System.Drawing.Color.White;
            this.mainPanel.Location = new System.Drawing.Point(12, 42);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(400, 500);
            this.mainPanel.TabIndex = 0;
            
            // picQRCode
            this.picQRCode.BorderRadius = 10;
            this.picQRCode.Location = new System.Drawing.Point(100, 200);
            this.picQRCode.Name = "picQRCode";
            this.picQRCode.Size = new System.Drawing.Size(200, 200);
            this.picQRCode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picQRCode.TabIndex = 0;
            this.picQRCode.TabStop = false;
            
            // lblInstructions
            this.lblInstructions.BackColor = System.Drawing.Color.Transparent;
            this.lblInstructions.Location = new System.Drawing.Point(40, 30);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new System.Drawing.Size(320, 40);
            this.lblInstructions.TabIndex = 1;
            this.lblInstructions.Text = "Enter an amount in VND to recharge your account.";
            this.lblInstructions.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            
            // lblExchangeRate
            this.lblExchangeRate.BackColor = System.Drawing.Color.Transparent;
            this.lblExchangeRate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblExchangeRate.ForeColor = System.Drawing.Color.SlateBlue;
            this.lblExchangeRate.Location = new System.Drawing.Point(40, 70);
            this.lblExchangeRate.Name = "lblExchangeRate";
            this.lblExchangeRate.Size = new System.Drawing.Size(320, 18);
            this.lblExchangeRate.TabIndex = 2;
            this.lblExchangeRate.Text = "Exchange rate: 1,000 VND = 1 Coin";
            this.lblExchangeRate.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            
            // txtAmount
            this.txtAmount.BorderRadius = 8;
            this.txtAmount.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAmount.DefaultText = "";
            this.txtAmount.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtAmount.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtAmount.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtAmount.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtAmount.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtAmount.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtAmount.ForeColor = System.Drawing.Color.Black;
            this.txtAmount.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtAmount.Location = new System.Drawing.Point(100, 100);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.PasswordChar = '\0';
            this.txtAmount.PlaceholderText = "Enter amount (VND)";
            this.txtAmount.SelectedText = "";
            this.txtAmount.Size = new System.Drawing.Size(200, 40);
            this.txtAmount.TabIndex = 3;
            
            // btnGenerateQR
            this.btnGenerateQR.BorderRadius = 8;
            this.btnGenerateQR.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnGenerateQR.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnGenerateQR.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnGenerateQR.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnGenerateQR.FillColor = System.Drawing.Color.SlateBlue;
            this.btnGenerateQR.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnGenerateQR.ForeColor = System.Drawing.Color.White;
            this.btnGenerateQR.Location = new System.Drawing.Point(100, 150);
            this.btnGenerateQR.Name = "btnGenerateQR";
            this.btnGenerateQR.Size = new System.Drawing.Size(200, 40);
            this.btnGenerateQR.TabIndex = 4;
            this.btnGenerateQR.Text = "Generate QR Code";
            this.btnGenerateQR.Click += new System.EventHandler(this.btnGenerateQR_Click);
            
            // btnCheckPayment
            this.btnCheckPayment.BorderRadius = 8;
            this.btnCheckPayment.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnCheckPayment.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnCheckPayment.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnCheckPayment.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnCheckPayment.Enabled = false;
            this.btnCheckPayment.FillColor = System.Drawing.Color.MediumSeaGreen;
            this.btnCheckPayment.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCheckPayment.ForeColor = System.Drawing.Color.White;
            this.btnCheckPayment.Location = new System.Drawing.Point(100, 420);
            this.btnCheckPayment.Name = "btnCheckPayment";
            this.btnCheckPayment.Size = new System.Drawing.Size(200, 40);
            this.btnCheckPayment.TabIndex = 6;
            this.btnCheckPayment.Text = "Check Payment";
            this.btnCheckPayment.Click += new System.EventHandler(this.btnCheckPayment_Click);
            
            // lnkOpenBrowser
            this.lnkOpenBrowser.AutoSize = true;
            this.lnkOpenBrowser.BackColor = System.Drawing.Color.Transparent;
            this.lnkOpenBrowser.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkOpenBrowser.LinkColor = System.Drawing.Color.SlateBlue;
            this.lnkOpenBrowser.Location = new System.Drawing.Point(124, 410);
            this.lnkOpenBrowser.Name = "lnkOpenBrowser";
            this.lnkOpenBrowser.Size = new System.Drawing.Size(152, 17);
            this.lnkOpenBrowser.TabIndex = 5;
            this.lnkOpenBrowser.TabStop = true;
            this.lnkOpenBrowser.Text = "Open payment in browser";
            this.lnkOpenBrowser.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lnkOpenBrowser.Visible = false;
            this.lnkOpenBrowser.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkOpenBrowser_LinkClicked);
            
            // controlBoxClose
            this.controlBoxClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.controlBoxClose.FillColor = System.Drawing.Color.Transparent;
            this.controlBoxClose.IconColor = System.Drawing.Color.Gray;
            this.controlBoxClose.Location = new System.Drawing.Point(384, 12);
            this.controlBoxClose.Name = "controlBoxClose";
            this.controlBoxClose.Size = new System.Drawing.Size(28, 24);
            this.controlBoxClose.TabIndex = 7;
            
            // controlBoxMinimize
            this.controlBoxMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.controlBoxMinimize.ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox;
            this.controlBoxMinimize.FillColor = System.Drawing.Color.Transparent;
            this.controlBoxMinimize.IconColor = System.Drawing.Color.Gray;
            this.controlBoxMinimize.Location = new System.Drawing.Point(350, 12);
            this.controlBoxMinimize.Name = "controlBoxMinimize";
            this.controlBoxMinimize.Size = new System.Drawing.Size(28, 24);
            this.controlBoxMinimize.TabIndex = 8;
            
            // RechargeForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(424, 554);
            this.Controls.Add(this.controlBoxMinimize);
            this.Controls.Add(this.controlBoxClose);
            this.Controls.Add(this.mainPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RechargeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Recharge Account";
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picQRCode)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
