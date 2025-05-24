using Guna.UI2.WinForms;

namespace LibraryDesktop.View
{
    partial class RegistrationForm
    {        private System.ComponentModel.IContainer components = null;
        private Guna2TextBox txtUsername;
        private Guna2TextBox txtEmail;
        private Guna2TextBox txtPassword;
        private Guna2TextBox txtConfirmPassword;
        private Guna2Button btnRegister;
        private Guna2Button btnCancel;
        private Guna2HtmlLabel lblUsername;
        private Guna2HtmlLabel lblEmail;
        private Guna2HtmlLabel lblPassword;
        private Guna2HtmlLabel lblConfirmPassword;
        private Guna2HtmlLabel lblTitle;        private Guna2Panel mainPanel;
        private Guna2ShadowForm shadowForm;
        private Guna2ControlBox controlBoxClose;
        private Guna2ControlBox controlBoxMinimize;

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
            this.mainPanel = new Guna2Panel();
            this.txtUsername = new Guna2TextBox();
            this.txtEmail = new Guna2TextBox();
            this.txtPassword = new Guna2TextBox();
            this.txtConfirmPassword = new Guna2TextBox();
            this.btnRegister = new Guna2Button();
            this.btnCancel = new Guna2Button();
            this.lblUsername = new Guna2HtmlLabel();
            this.lblEmail = new Guna2HtmlLabel();
            this.lblPassword = new Guna2HtmlLabel();
            this.lblConfirmPassword = new Guna2HtmlLabel();
            this.lblTitle = new Guna2HtmlLabel();
            this.shadowForm = new Guna2ShadowForm(this.components);
            this.controlBoxClose = new Guna2ControlBox();
            this.controlBoxMinimize = new Guna2ControlBox();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            
            // mainPanel
            this.mainPanel.BackColor = System.Drawing.Color.White;
            this.mainPanel.BorderRadius = 20;
            this.mainPanel.Controls.Add(this.btnCancel);
            this.mainPanel.Controls.Add(this.btnRegister);
            this.mainPanel.Controls.Add(this.txtConfirmPassword);
            this.mainPanel.Controls.Add(this.lblConfirmPassword);
            this.mainPanel.Controls.Add(this.txtPassword);
            this.mainPanel.Controls.Add(this.lblPassword);
            this.mainPanel.Controls.Add(this.txtEmail);
            this.mainPanel.Controls.Add(this.lblEmail);
            this.mainPanel.Controls.Add(this.txtUsername);
            this.mainPanel.Controls.Add(this.lblUsername);
            this.mainPanel.Controls.Add(this.lblTitle);
            this.mainPanel.Location = new System.Drawing.Point(30, 30);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(340, 480);
            this.mainPanel.TabIndex = 0;
            
            // lblTitle
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(94, 148, 255);
            this.lblTitle.Location = new System.Drawing.Point(80, 30);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(180, 34);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Register Account";
            
            // lblUsername
            this.lblUsername.BackColor = System.Drawing.Color.Transparent;
            this.lblUsername.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblUsername.ForeColor = System.Drawing.Color.FromArgb(125, 137, 149);
            this.lblUsername.Location = new System.Drawing.Point(50, 80);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(60, 19);
            this.lblUsername.TabIndex = 1;
            this.lblUsername.Text = "Username:";
            
            // txtUsername
            this.txtUsername.BorderRadius = 8;
            this.txtUsername.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtUsername.DefaultText = "";
            this.txtUsername.DisabledState.BorderColor = System.Drawing.Color.FromArgb(208, 208, 208);
            this.txtUsername.DisabledState.FillColor = System.Drawing.Color.FromArgb(226, 226, 226);
            this.txtUsername.DisabledState.ForeColor = System.Drawing.Color.FromArgb(138, 138, 138);
            this.txtUsername.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(138, 138, 138);
            this.txtUsername.FocusedState.BorderColor = System.Drawing.Color.FromArgb(94, 148, 255);
            this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtUsername.HoverState.BorderColor = System.Drawing.Color.FromArgb(94, 148, 255);
            this.txtUsername.Location = new System.Drawing.Point(50, 105);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.PasswordChar = '\0';
            this.txtUsername.PlaceholderText = "Enter your username";
            this.txtUsername.SelectedText = "";
            this.txtUsername.Size = new System.Drawing.Size(240, 36);
            this.txtUsername.TabIndex = 2;
            
            // lblEmail
            this.lblEmail.BackColor = System.Drawing.Color.Transparent;
            this.lblEmail.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblEmail.ForeColor = System.Drawing.Color.FromArgb(125, 137, 149);
            this.lblEmail.Location = new System.Drawing.Point(50, 155);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(34, 19);
            this.lblEmail.TabIndex = 3;
            this.lblEmail.Text = "Email:";
            
            // txtEmail
            this.txtEmail.BorderRadius = 8;
            this.txtEmail.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtEmail.DefaultText = "";
            this.txtEmail.DisabledState.BorderColor = System.Drawing.Color.FromArgb(208, 208, 208);
            this.txtEmail.DisabledState.FillColor = System.Drawing.Color.FromArgb(226, 226, 226);
            this.txtEmail.DisabledState.ForeColor = System.Drawing.Color.FromArgb(138, 138, 138);
            this.txtEmail.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(138, 138, 138);
            this.txtEmail.FocusedState.BorderColor = System.Drawing.Color.FromArgb(94, 148, 255);
            this.txtEmail.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtEmail.HoverState.BorderColor = System.Drawing.Color.FromArgb(94, 148, 255);
            this.txtEmail.Location = new System.Drawing.Point(50, 180);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.PasswordChar = '\0';
            this.txtEmail.PlaceholderText = "Enter your email";
            this.txtEmail.SelectedText = "";
            this.txtEmail.Size = new System.Drawing.Size(240, 36);
            this.txtEmail.TabIndex = 4;
            
            // lblPassword
            this.lblPassword.BackColor = System.Drawing.Color.Transparent;
            this.lblPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblPassword.ForeColor = System.Drawing.Color.FromArgb(125, 137, 149);
            this.lblPassword.Location = new System.Drawing.Point(50, 230);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(56, 19);
            this.lblPassword.TabIndex = 5;
            this.lblPassword.Text = "Password:";
            
            // txtPassword
            this.txtPassword.BorderRadius = 8;
            this.txtPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPassword.DefaultText = "";
            this.txtPassword.DisabledState.BorderColor = System.Drawing.Color.FromArgb(208, 208, 208);
            this.txtPassword.DisabledState.FillColor = System.Drawing.Color.FromArgb(226, 226, 226);
            this.txtPassword.DisabledState.ForeColor = System.Drawing.Color.FromArgb(138, 138, 138);
            this.txtPassword.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(138, 138, 138);
            this.txtPassword.FocusedState.BorderColor = System.Drawing.Color.FromArgb(94, 148, 255);
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPassword.HoverState.BorderColor = System.Drawing.Color.FromArgb(94, 148, 255);
            this.txtPassword.Location = new System.Drawing.Point(50, 255);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '●';
            this.txtPassword.PlaceholderText = "Enter your password";
            this.txtPassword.SelectedText = "";
            this.txtPassword.Size = new System.Drawing.Size(240, 36);
            this.txtPassword.TabIndex = 6;
            
            // lblConfirmPassword
            this.lblConfirmPassword.BackColor = System.Drawing.Color.Transparent;
            this.lblConfirmPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblConfirmPassword.ForeColor = System.Drawing.Color.FromArgb(125, 137, 149);
            this.lblConfirmPassword.Location = new System.Drawing.Point(50, 305);
            this.lblConfirmPassword.Name = "lblConfirmPassword";
            this.lblConfirmPassword.Size = new System.Drawing.Size(98, 19);
            this.lblConfirmPassword.TabIndex = 7;
            this.lblConfirmPassword.Text = "Confirm Password:";
            
            // txtConfirmPassword
            this.txtConfirmPassword.BorderRadius = 8;
            this.txtConfirmPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtConfirmPassword.DefaultText = "";
            this.txtConfirmPassword.DisabledState.BorderColor = System.Drawing.Color.FromArgb(208, 208, 208);
            this.txtConfirmPassword.DisabledState.FillColor = System.Drawing.Color.FromArgb(226, 226, 226);
            this.txtConfirmPassword.DisabledState.ForeColor = System.Drawing.Color.FromArgb(138, 138, 138);
            this.txtConfirmPassword.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(138, 138, 138);
            this.txtConfirmPassword.FocusedState.BorderColor = System.Drawing.Color.FromArgb(94, 148, 255);
            this.txtConfirmPassword.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtConfirmPassword.HoverState.BorderColor = System.Drawing.Color.FromArgb(94, 148, 255);
            this.txtConfirmPassword.Location = new System.Drawing.Point(50, 330);
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.PasswordChar = '●';
            this.txtConfirmPassword.PlaceholderText = "Confirm your password";
            this.txtConfirmPassword.SelectedText = "";
            this.txtConfirmPassword.Size = new System.Drawing.Size(240, 36);
            this.txtConfirmPassword.TabIndex = 8;
            
            // btnRegister
            this.btnRegister.BorderRadius = 10;
            this.btnRegister.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnRegister.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnRegister.DisabledState.FillColor = System.Drawing.Color.FromArgb(169, 169, 169);
            this.btnRegister.DisabledState.ForeColor = System.Drawing.Color.FromArgb(141, 141, 141);
            this.btnRegister.FillColor = System.Drawing.Color.FromArgb(94, 148, 255);
            this.btnRegister.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnRegister.ForeColor = System.Drawing.Color.White;
            this.btnRegister.Location = new System.Drawing.Point(50, 390);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(110, 40);
            this.btnRegister.TabIndex = 9;
            this.btnRegister.Text = "Register";
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            
            // btnCancel
            this.btnCancel.BorderRadius = 10;
            this.btnCancel.BorderThickness = 2;
            this.btnCancel.BorderColor = System.Drawing.Color.FromArgb(94, 148, 255);
            this.btnCancel.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnCancel.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnCancel.DisabledState.FillColor = System.Drawing.Color.FromArgb(169, 169, 169);
            this.btnCancel.DisabledState.ForeColor = System.Drawing.Color.FromArgb(141, 141, 141);
            this.btnCancel.FillColor = System.Drawing.Color.White;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(94, 148, 255);
            this.btnCancel.Location = new System.Drawing.Point(180, 390);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 40);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            
            // controlBoxClose
            this.controlBoxClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.controlBoxClose.BorderColor = System.Drawing.Color.Transparent;
            this.controlBoxClose.BorderRadius = 10;
            this.controlBoxClose.FillColor = System.Drawing.Color.FromArgb(94, 148, 255);
            this.controlBoxClose.IconColor = System.Drawing.Color.White;
            this.controlBoxClose.Location = new System.Drawing.Point(370, 10);
            this.controlBoxClose.Name = "controlBoxClose";
            this.controlBoxClose.Size = new System.Drawing.Size(40, 40);
            this.controlBoxClose.TabIndex = 11;
            this.controlBoxClose.Click += new System.EventHandler(this.controlBoxClose_Click);
              // controlBoxMinimize
            this.controlBoxMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.controlBoxMinimize.BorderColor = System.Drawing.Color.Transparent;
            this.controlBoxMinimize.BorderRadius = 10;
            this.controlBoxMinimize.ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox;
            this.controlBoxMinimize.FillColor = System.Drawing.Color.FromArgb(94, 148, 255);
            this.controlBoxMinimize.IconColor = System.Drawing.Color.White;
            this.controlBoxMinimize.Location = new System.Drawing.Point(320, 10);
            this.controlBoxMinimize.Name = "controlBoxMinimize";
            this.controlBoxMinimize.Size = new System.Drawing.Size(40, 40);
            this.controlBoxMinimize.TabIndex = 12;
            this.controlBoxMinimize.Click += new System.EventHandler(this.controlBoxMinimize_Click);
            
            // shadowForm
            this.shadowForm.TargetForm = this;
            
            // RegistrationForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(240, 242, 247);
            this.ClientSize = new System.Drawing.Size(400, 540);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.controlBoxClose);
            this.Controls.Add(this.controlBoxMinimize);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RegistrationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Register - Library Desktop";
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
