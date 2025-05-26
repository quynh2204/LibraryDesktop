using Guna.UI2.WinForms;

namespace LibraryDesktop.View
{
    partial class RegistrationForm
    {
        private System.ComponentModel.IContainer components = null;
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
        private Guna2HtmlLabel lblTitle; private Guna2Panel mainPanel;
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
            components = new System.ComponentModel.Container();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges13 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges14 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges11 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges12 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges15 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges16 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges17 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges18 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            mainPanel = new Guna2Panel();
            btnCancel = new Guna2Button();
            btnRegister = new Guna2Button();
            txtConfirmPassword = new Guna2TextBox();
            lblConfirmPassword = new Guna2HtmlLabel();
            txtPassword = new Guna2TextBox();
            lblPassword = new Guna2HtmlLabel();
            txtEmail = new Guna2TextBox();
            lblEmail = new Guna2HtmlLabel();
            txtUsername = new Guna2TextBox();
            lblUsername = new Guna2HtmlLabel();
            lblTitle = new Guna2HtmlLabel();
            shadowForm = new Guna2ShadowForm(components);
            controlBoxClose = new Guna2ControlBox();
            controlBoxMinimize = new Guna2ControlBox();
            mainPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainPanel
            // 
            mainPanel.BackColor = Color.White;
            mainPanel.BorderRadius = 20;
            mainPanel.Controls.Add(btnCancel);
            mainPanel.Controls.Add(btnRegister);
            mainPanel.Controls.Add(txtConfirmPassword);
            mainPanel.Controls.Add(lblConfirmPassword);
            mainPanel.Controls.Add(txtPassword);
            mainPanel.Controls.Add(lblPassword);
            mainPanel.Controls.Add(txtEmail);
            mainPanel.Controls.Add(lblEmail);
            mainPanel.Controls.Add(txtUsername);
            mainPanel.Controls.Add(lblUsername);
            mainPanel.Controls.Add(lblTitle);
            mainPanel.CustomizableEdges = customizableEdges13;
            mainPanel.Location = new Point(40, 86);
            mainPanel.Margin = new Padding(4, 5, 4, 5);
            mainPanel.Name = "mainPanel";
            mainPanel.ShadowDecoration.CustomizableEdges = customizableEdges14;
            mainPanel.Size = new Size(442, 698);
            mainPanel.TabIndex = 0;
            // 
            // btnCancel
            // 
            btnCancel.BorderColor = Color.FromArgb(94, 148, 255);
            btnCancel.BorderRadius = 10;
            btnCancel.BorderThickness = 2;
            btnCancel.CustomizableEdges = customizableEdges1;
            btnCancel.DisabledState.BorderColor = Color.DarkGray;
            btnCancel.DisabledState.CustomBorderColor = Color.DarkGray;
            btnCancel.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnCancel.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnCancel.FillColor = Color.White;
            btnCancel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnCancel.ForeColor = Color.FromArgb(94, 148, 255);
            btnCancel.Location = new Point(240, 600);
            btnCancel.Margin = new Padding(4, 5, 4, 5);
            btnCancel.Name = "btnCancel";
            btnCancel.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnCancel.Size = new Size(147, 62);
            btnCancel.TabIndex = 10;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnRegister
            // 
            btnRegister.BorderRadius = 10;
            btnRegister.CustomizableEdges = customizableEdges3;
            btnRegister.DisabledState.BorderColor = Color.DarkGray;
            btnRegister.DisabledState.CustomBorderColor = Color.DarkGray;
            btnRegister.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnRegister.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnRegister.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnRegister.ForeColor = Color.White;
            btnRegister.Location = new Point(67, 600);
            btnRegister.Margin = new Padding(4, 5, 4, 5);
            btnRegister.Name = "btnRegister";
            btnRegister.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnRegister.Size = new Size(147, 62);
            btnRegister.TabIndex = 9;
            btnRegister.Text = "Register";
            btnRegister.Click += btnRegister_Click;
            // 
            // txtConfirmPassword
            // 
            txtConfirmPassword.BorderRadius = 8;
            txtConfirmPassword.Cursor = Cursors.IBeam;
            txtConfirmPassword.CustomizableEdges = customizableEdges5;
            txtConfirmPassword.DefaultText = "";
            txtConfirmPassword.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtConfirmPassword.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtConfirmPassword.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtConfirmPassword.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtConfirmPassword.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtConfirmPassword.Font = new Font("Segoe UI", 9F);
            txtConfirmPassword.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtConfirmPassword.Location = new Point(67, 508);
            txtConfirmPassword.Margin = new Padding(4, 6, 4, 6);
            txtConfirmPassword.Name = "txtConfirmPassword";
            txtConfirmPassword.PasswordChar = '●';
            txtConfirmPassword.PlaceholderText = "Confirm your password";
            txtConfirmPassword.SelectedText = "";
            txtConfirmPassword.ShadowDecoration.CustomizableEdges = customizableEdges6;
            txtConfirmPassword.Size = new Size(320, 55);
            txtConfirmPassword.TabIndex = 8;
            // 
            // lblConfirmPassword
            // 
            lblConfirmPassword.BackColor = Color.Transparent;
            lblConfirmPassword.Font = new Font("Segoe UI", 10F);
            lblConfirmPassword.ForeColor = Color.FromArgb(125, 137, 149);
            lblConfirmPassword.Location = new Point(67, 469);
            lblConfirmPassword.Margin = new Padding(4, 5, 4, 5);
            lblConfirmPassword.Name = "lblConfirmPassword";
            lblConfirmPassword.Size = new Size(144, 25);
            lblConfirmPassword.TabIndex = 7;
            lblConfirmPassword.Text = "Confirm Password:";
            // 
            // txtPassword
            // 
            txtPassword.BorderRadius = 8;
            txtPassword.Cursor = Cursors.IBeam;
            txtPassword.CustomizableEdges = customizableEdges7;
            txtPassword.DefaultText = "";
            txtPassword.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtPassword.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtPassword.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtPassword.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtPassword.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtPassword.Font = new Font("Segoe UI", 9F);
            txtPassword.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtPassword.Location = new Point(67, 392);
            txtPassword.Margin = new Padding(4, 6, 4, 6);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '●';
            txtPassword.PlaceholderText = "Enter your password";
            txtPassword.SelectedText = "";
            txtPassword.ShadowDecoration.CustomizableEdges = customizableEdges8;
            txtPassword.Size = new Size(320, 55);
            txtPassword.TabIndex = 6;
            // 
            // lblPassword
            // 
            lblPassword.BackColor = Color.Transparent;
            lblPassword.Font = new Font("Segoe UI", 10F);
            lblPassword.ForeColor = Color.FromArgb(125, 137, 149);
            lblPassword.Location = new Point(67, 354);
            lblPassword.Margin = new Padding(4, 5, 4, 5);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(78, 25);
            lblPassword.TabIndex = 5;
            lblPassword.Text = "Password:";
            // 
            // txtEmail
            // 
            txtEmail.BorderRadius = 8;
            txtEmail.Cursor = Cursors.IBeam;
            txtEmail.CustomizableEdges = customizableEdges9;
            txtEmail.DefaultText = "";
            txtEmail.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtEmail.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtEmail.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtEmail.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtEmail.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtEmail.Font = new Font("Segoe UI", 9F);
            txtEmail.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtEmail.Location = new Point(67, 277);
            txtEmail.Margin = new Padding(4, 6, 4, 6);
            txtEmail.Name = "txtEmail";
            txtEmail.PlaceholderText = "Enter your email";
            txtEmail.SelectedText = "";
            txtEmail.ShadowDecoration.CustomizableEdges = customizableEdges10;
            txtEmail.Size = new Size(320, 55);
            txtEmail.TabIndex = 4;
            // 
            // lblEmail
            // 
            lblEmail.BackColor = Color.Transparent;
            lblEmail.Font = new Font("Segoe UI", 10F);
            lblEmail.ForeColor = Color.FromArgb(125, 137, 149);
            lblEmail.Location = new Point(67, 238);
            lblEmail.Margin = new Padding(4, 5, 4, 5);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(48, 25);
            lblEmail.TabIndex = 3;
            lblEmail.Text = "Email:";
            // 
            // txtUsername
            // 
            txtUsername.BorderRadius = 8;
            txtUsername.Cursor = Cursors.IBeam;
            txtUsername.CustomizableEdges = customizableEdges11;
            txtUsername.DefaultText = "";
            txtUsername.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtUsername.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtUsername.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtUsername.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtUsername.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtUsername.Font = new Font("Segoe UI", 9F);
            txtUsername.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtUsername.Location = new Point(67, 162);
            txtUsername.Margin = new Padding(4, 6, 4, 6);
            txtUsername.Name = "txtUsername";
            txtUsername.PlaceholderText = "Enter your username";
            txtUsername.SelectedText = "";
            txtUsername.ShadowDecoration.CustomizableEdges = customizableEdges12;
            txtUsername.Size = new Size(320, 55);
            txtUsername.TabIndex = 2;
            // 
            // lblUsername
            // 
            lblUsername.BackColor = Color.Transparent;
            lblUsername.Font = new Font("Segoe UI", 10F);
            lblUsername.ForeColor = Color.FromArgb(125, 137, 149);
            lblUsername.Location = new Point(67, 123);
            lblUsername.Margin = new Padding(4, 5, 4, 5);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(84, 25);
            lblUsername.TabIndex = 1;
            lblUsername.Text = "Username:";
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.FromArgb(94, 148, 255);
            lblTitle.Location = new Point(107, 46);
            lblTitle.Margin = new Padding(4, 5, 4, 5);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(243, 43);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Register Account";
            // 
            // shadowForm
            // 
            shadowForm.TargetForm = this;
            // 
            // controlBoxClose
            // 
            controlBoxClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            controlBoxClose.BorderColor = Color.Transparent;
            controlBoxClose.BorderRadius = 10;
            controlBoxClose.CustomizableEdges = customizableEdges15;
            controlBoxClose.FillColor = Color.FromArgb(94, 148, 255);
            controlBoxClose.IconColor = Color.White;
            controlBoxClose.Location = new Point(490, 1);
            controlBoxClose.Margin = new Padding(4, 5, 4, 5);
            controlBoxClose.Name = "controlBoxClose";
            controlBoxClose.ShadowDecoration.CustomizableEdges = customizableEdges16;
            controlBoxClose.Size = new Size(40, 40);
            controlBoxClose.TabIndex = 11;
            controlBoxClose.Click += controlBoxClose_Click;
            // 
            // controlBoxMinimize
            // 
            controlBoxMinimize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            controlBoxMinimize.BorderColor = Color.Transparent;
            controlBoxMinimize.BorderRadius = 10;
            controlBoxMinimize.ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox;
            controlBoxMinimize.CustomizableEdges = customizableEdges17;
            controlBoxMinimize.FillColor = Color.FromArgb(94, 148, 255);
            controlBoxMinimize.IconColor = Color.White;
            controlBoxMinimize.Location = new Point(442, 1);
            controlBoxMinimize.Margin = new Padding(4, 5, 4, 5);
            controlBoxMinimize.Name = "controlBoxMinimize";
            controlBoxMinimize.ShadowDecoration.CustomizableEdges = customizableEdges18;
            controlBoxMinimize.Size = new Size(40, 42);
            controlBoxMinimize.TabIndex = 12;
            controlBoxMinimize.Click += controlBoxMinimize_Click;
            // 
            // RegistrationForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 242, 247);
            ClientSize = new Size(533, 831);
            Controls.Add(mainPanel);
            Controls.Add(controlBoxClose);
            Controls.Add(controlBoxMinimize);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 5, 4, 5);
            Name = "RegistrationForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Register - Library Desktop";
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            ResumeLayout(false);
        }
    }
}
