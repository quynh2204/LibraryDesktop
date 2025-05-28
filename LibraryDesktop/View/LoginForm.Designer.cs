using Guna.UI2.WinForms;

namespace LibraryDesktop.View
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;
        private Guna2TextBox txtUsername;
        private Guna2TextBox txtPassword;
        private Guna2Button btnLogin;
        private Guna2Button btnRegister;
        private Guna2HtmlLabel lblUsername;
        private Guna2HtmlLabel lblPassword;
        private Guna2HtmlLabel lblTitle;
        private Guna2Panel mainPanel;
        private Guna2ShadowForm shadowForm;
        private Guna2ControlBox controlBoxClose;

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
            components = new System.ComponentModel.Container();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges11 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges12 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            mainPanel = new Guna2Panel();
            btnRegister = new Guna2Button();
            btnLogin = new Guna2Button();
            txtPassword = new Guna2TextBox();
            lblPassword = new Guna2HtmlLabel();
            txtUsername = new Guna2TextBox();
            lblUsername = new Guna2HtmlLabel();
            lblTitle = new Guna2HtmlLabel();
            shadowForm = new Guna2ShadowForm(components);
            controlBoxClose = new Guna2ControlBox();
            mainPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainPanel
            // 
            mainPanel.BackColor = Color.Transparent;
            mainPanel.BorderRadius = 40;
            mainPanel.BorderStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
            mainPanel.Controls.Add(btnRegister);
            mainPanel.Controls.Add(btnLogin);
            mainPanel.Controls.Add(txtPassword);
            mainPanel.Controls.Add(lblPassword);
            mainPanel.Controls.Add(txtUsername);
            mainPanel.Controls.Add(lblUsername);
            mainPanel.Controls.Add(lblTitle);
            mainPanel.CustomizableEdges = customizableEdges9;
            mainPanel.FillColor = Color.FromArgb(100, 255, 255, 255);
            mainPanel.Location = new Point(49, 51);
            mainPanel.Name = "mainPanel";
            mainPanel.ShadowDecoration.BorderRadius = 40;
            mainPanel.ShadowDecoration.Color = Color.Transparent;
            mainPanel.ShadowDecoration.CustomizableEdges = customizableEdges10;
            mainPanel.ShadowDecoration.Depth = 15;
            mainPanel.ShadowDecoration.Enabled = true;
            mainPanel.Size = new Size(340, 428);
            mainPanel.TabIndex = 0;
            // 
            // btnRegister
            // 
            btnRegister.BorderColor = Color.FromArgb(169, 155, 135);
            btnRegister.BorderRadius = 10;
            btnRegister.BorderThickness = 2;
            btnRegister.CustomizableEdges = customizableEdges1;
            btnRegister.DisabledState.BorderColor = Color.DarkGray;
            btnRegister.DisabledState.CustomBorderColor = Color.DarkGray;
            btnRegister.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnRegister.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnRegister.FillColor = Color.White;
            btnRegister.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnRegister.ForeColor = Color.FromArgb(169, 155, 135);
            btnRegister.Location = new Point(180, 332);
            btnRegister.Name = "btnRegister";
            btnRegister.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnRegister.Size = new Size(110, 40);
            btnRegister.TabIndex = 6;
            btnRegister.Text = "Register";
            btnRegister.Click += btnRegister_Click;
            // 
            // btnLogin
            // 
            btnLogin.BorderRadius = 10;
            btnLogin.CustomizableEdges = customizableEdges3;
            btnLogin.DisabledState.BorderColor = Color.DarkGray;
            btnLogin.DisabledState.CustomBorderColor = Color.DarkGray;
            btnLogin.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnLogin.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnLogin.FillColor = Color.FromArgb(169, 155, 135);
            btnLogin.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(50, 332);
            btnLogin.Name = "btnLogin";
            btnLogin.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnLogin.Size = new Size(110, 40);
            btnLogin.TabIndex = 5;
            btnLogin.Text = "Login";
            btnLogin.Click += btnLogin_Click;
            // 
            // txtPassword
            // 
            txtPassword.BorderRadius = 8;
            txtPassword.Cursor = Cursors.IBeam;
            txtPassword.CustomizableEdges = customizableEdges5;
            txtPassword.DefaultText = "";
            txtPassword.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtPassword.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtPassword.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtPassword.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtPassword.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtPassword.Font = new Font("Segoe UI", 9F);
            txtPassword.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtPassword.Location = new Point(50, 247);
            txtPassword.Margin = new Padding(3, 4, 3, 4);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '●';
            txtPassword.PlaceholderText = "Enter your password";
            txtPassword.SelectedText = "";
            txtPassword.ShadowDecoration.CustomizableEdges = customizableEdges6;
            txtPassword.Size = new Size(240, 36);
            txtPassword.TabIndex = 4;
            // 
            // lblPassword
            // 
            lblPassword.BackColor = Color.Transparent;
            lblPassword.Font = new Font("Segoe UI", 10F);
            lblPassword.ForeColor = Color.FromArgb(125, 137, 149);
            lblPassword.Location = new Point(50, 215);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(78, 25);
            lblPassword.TabIndex = 3;
            lblPassword.Text = "Password:";
            // 
            // txtUsername
            // 
            txtUsername.BorderRadius = 8;
            txtUsername.Cursor = Cursors.IBeam;
            txtUsername.CustomizableEdges = customizableEdges7;
            txtUsername.DefaultText = "";
            txtUsername.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtUsername.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtUsername.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtUsername.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtUsername.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtUsername.Font = new Font("Segoe UI", 9F);
            txtUsername.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtUsername.Location = new Point(50, 135);
            txtUsername.Margin = new Padding(3, 4, 3, 4);
            txtUsername.Name = "txtUsername";
            txtUsername.PlaceholderText = "Enter your username";
            txtUsername.SelectedText = "";
            txtUsername.ShadowDecoration.CustomizableEdges = customizableEdges8;
            txtUsername.Size = new Size(240, 36);
            txtUsername.TabIndex = 2;
            // 
            // lblUsername
            // 
            lblUsername.BackColor = Color.Transparent;
            lblUsername.Font = new Font("Segoe UI", 10F);
            lblUsername.ForeColor = Color.FromArgb(125, 137, 149);
            lblUsername.Location = new Point(50, 103);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(84, 25);
            lblUsername.TabIndex = 1;
            lblUsername.Text = "Username:";
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(169, 155, 135);
            lblTitle.Location = new Point(71, 23);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(189, 43);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Library Login";
            // 
            // shadowForm
            // 
            shadowForm.TargetForm = this;
            // 
            // controlBoxClose
            // 
            controlBoxClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            controlBoxClose.BackColor = Color.Transparent;
            controlBoxClose.BorderColor = Color.Transparent;
            controlBoxClose.BorderRadius = 50;
            controlBoxClose.CustomizableEdges = customizableEdges11;
            controlBoxClose.FillColor = Color.FromArgb(169, 155, 135);
            controlBoxClose.IconColor = SystemColors.Window;
            controlBoxClose.Location = new Point(395, 0);
            controlBoxClose.Name = "controlBoxClose";
            controlBoxClose.ShadowDecoration.BorderRadius = 20;
            controlBoxClose.ShadowDecoration.CustomizableEdges = customizableEdges12;
            controlBoxClose.Size = new Size(43, 46);
            controlBoxClose.TabIndex = 10;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(211, 195, 179);
            ClientSize = new Size(439, 530);
            Controls.Add(controlBoxClose);
            Controls.Add(mainPanel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Library Login";
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            ResumeLayout(false);
        }
    }
}