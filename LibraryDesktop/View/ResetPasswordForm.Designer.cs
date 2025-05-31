using Guna.UI2.WinForms;

namespace LibraryDesktop.View
{
    partial class ResetPasswordForm
    {
        private System.ComponentModel.IContainer components = null;
        private Guna2TextBox txtUsername;
        private Guna2TextBox txtEmail;
        private Guna2TextBox txtNewPassword;
        private Guna2TextBox txtConfirmPassword;
        private Guna2Button btnResetPassword;
        private Guna2Button btnCancel;
        private Guna2HtmlLabel lblUsername;
        private Guna2HtmlLabel lblEmail;
        private Guna2HtmlLabel lblNewPassword;
        private Guna2HtmlLabel lblConfirmPassword;
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges13 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges14 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges15 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges16 = new Guna.UI2.WinForms.Suite.CustomizableEdges();

            mainPanel = new Guna2Panel();
            btnCancel = new Guna2Button();
            btnResetPassword = new Guna2Button();
            txtConfirmPassword = new Guna2TextBox();
            lblConfirmPassword = new Guna2HtmlLabel();
            txtNewPassword = new Guna2TextBox();
            lblNewPassword = new Guna2HtmlLabel();
            txtEmail = new Guna2TextBox();
            lblEmail = new Guna2HtmlLabel();
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
            mainPanel.Controls.Add(btnCancel);
            mainPanel.Controls.Add(btnResetPassword);
            mainPanel.Controls.Add(txtConfirmPassword);
            mainPanel.Controls.Add(lblConfirmPassword);
            mainPanel.Controls.Add(txtNewPassword);
            mainPanel.Controls.Add(lblNewPassword);
            mainPanel.Controls.Add(txtEmail);
            mainPanel.Controls.Add(lblEmail);
            mainPanel.Controls.Add(txtUsername);
            mainPanel.Controls.Add(lblUsername);
            mainPanel.Controls.Add(lblTitle);
            mainPanel.CustomizableEdges = customizableEdges15;
            mainPanel.FillColor = Color.FromArgb(100, 255, 255, 255);
            mainPanel.Location = new Point(49, 51);
            mainPanel.Name = "mainPanel";
            mainPanel.ShadowDecoration.BorderRadius = 40;
            mainPanel.ShadowDecoration.Color = Color.Transparent;
            mainPanel.ShadowDecoration.CustomizableEdges = customizableEdges16;
            mainPanel.ShadowDecoration.Depth = 15;
            mainPanel.ShadowDecoration.Enabled = true;
            mainPanel.Size = new Size(340, 550);
            mainPanel.TabIndex = 0;
            // 
            // btnCancel
            // 
            btnCancel.BorderColor = Color.FromArgb(169, 155, 135);
            btnCancel.BorderRadius = 10;
            btnCancel.BorderThickness = 2;
            btnCancel.CustomizableEdges = customizableEdges1;
            btnCancel.DisabledState.BorderColor = Color.DarkGray;
            btnCancel.DisabledState.CustomBorderColor = Color.DarkGray;
            btnCancel.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnCancel.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnCancel.FillColor = Color.White;
            btnCancel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnCancel.ForeColor = Color.FromArgb(169, 155, 135);
            btnCancel.Location = new Point(180, 480);
            btnCancel.Name = "btnCancel";
            btnCancel.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnCancel.Size = new Size(110, 40);
            btnCancel.TabIndex = 10;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnResetPassword
            // 
            btnResetPassword.BorderRadius = 10;
            btnResetPassword.CustomizableEdges = customizableEdges3;
            btnResetPassword.DisabledState.BorderColor = Color.DarkGray;
            btnResetPassword.DisabledState.CustomBorderColor = Color.DarkGray;
            btnResetPassword.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnResetPassword.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnResetPassword.FillColor = Color.FromArgb(169, 155, 135);
            btnResetPassword.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnResetPassword.ForeColor = Color.White;
            btnResetPassword.Location = new Point(50, 480);
            btnResetPassword.Name = "btnResetPassword";
            btnResetPassword.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnResetPassword.Size = new Size(110, 40);
            btnResetPassword.TabIndex = 9;
            btnResetPassword.Text = "Reset";
            btnResetPassword.Click += btnResetPassword_Click;
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
            txtConfirmPassword.Location = new Point(50, 425);
            txtConfirmPassword.Margin = new Padding(3, 4, 3, 4);
            txtConfirmPassword.Name = "txtConfirmPassword";
            txtConfirmPassword.PasswordChar = '●';
            txtConfirmPassword.PlaceholderText = "Confirm your new password";
            txtConfirmPassword.SelectedText = "";
            txtConfirmPassword.ShadowDecoration.CustomizableEdges = customizableEdges6;
            txtConfirmPassword.Size = new Size(240, 36);
            txtConfirmPassword.TabIndex = 8;
            // 
            // lblConfirmPassword
            // 
            lblConfirmPassword.BackColor = Color.Transparent;
            lblConfirmPassword.Font = new Font("Segoe UI", 10F);
            lblConfirmPassword.ForeColor = Color.FromArgb(125, 137, 149);
            lblConfirmPassword.Location = new Point(50, 393);
            lblConfirmPassword.Name = "lblConfirmPassword";
            lblConfirmPassword.Size = new Size(142, 25);
            lblConfirmPassword.TabIndex = 7;
            lblConfirmPassword.Text = "Confirm Password:";
            // 
            // txtNewPassword
            // 
            txtNewPassword.BorderRadius = 8;
            txtNewPassword.Cursor = Cursors.IBeam;
            txtNewPassword.CustomizableEdges = customizableEdges7;
            txtNewPassword.DefaultText = "";
            txtNewPassword.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtNewPassword.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtNewPassword.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtNewPassword.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtNewPassword.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtNewPassword.Font = new Font("Segoe UI", 9F);
            txtNewPassword.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtNewPassword.Location = new Point(50, 340);
            txtNewPassword.Margin = new Padding(3, 4, 3, 4);
            txtNewPassword.Name = "txtNewPassword";
            txtNewPassword.PasswordChar = '●';
            txtNewPassword.PlaceholderText = "Enter your new password";
            txtNewPassword.SelectedText = "";
            txtNewPassword.ShadowDecoration.CustomizableEdges = customizableEdges8;
            txtNewPassword.Size = new Size(240, 36);
            txtNewPassword.TabIndex = 6;
            // 
            // lblNewPassword
            // 
            lblNewPassword.BackColor = Color.Transparent;
            lblNewPassword.Font = new Font("Segoe UI", 10F);
            lblNewPassword.ForeColor = Color.FromArgb(125, 137, 149);
            lblNewPassword.Location = new Point(50, 308);
            lblNewPassword.Name = "lblNewPassword";
            lblNewPassword.Size = new Size(115, 25);
            lblNewPassword.TabIndex = 5;
            lblNewPassword.Text = "New Password:";
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
            txtEmail.Location = new Point(50, 255);
            txtEmail.Margin = new Padding(3, 4, 3, 4);
            txtEmail.Name = "txtEmail";
            txtEmail.PlaceholderText = "Enter your email address";
            txtEmail.SelectedText = "";
            txtEmail.ShadowDecoration.CustomizableEdges = customizableEdges10;
            txtEmail.Size = new Size(240, 36);
            txtEmail.TabIndex = 4;
            // 
            // lblEmail
            // 
            lblEmail.BackColor = Color.Transparent;
            lblEmail.Font = new Font("Segoe UI", 10F);
            lblEmail.ForeColor = Color.FromArgb(125, 137, 149);
            lblEmail.Location = new Point(50, 223);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(50, 25);
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
            txtUsername.Location = new Point(50, 170);
            txtUsername.Margin = new Padding(3, 4, 3, 4);
            txtUsername.Name = "txtUsername";
            txtUsername.PlaceholderText = "Enter your username";
            txtUsername.SelectedText = "";
            txtUsername.ShadowDecoration.CustomizableEdges = customizableEdges12;
            txtUsername.Size = new Size(240, 36);
            txtUsername.TabIndex = 2;
            // 
            // lblUsername
            // 
            lblUsername.BackColor = Color.Transparent;
            lblUsername.Font = new Font("Segoe UI", 10F);
            lblUsername.ForeColor = Color.FromArgb(125, 137, 149);
            lblUsername.Location = new Point(50, 138);
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
            lblTitle.Location = new Point(65, 23);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(210, 43);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Reset Password";
            // 
            // shadowForm
            // 
            shadowForm.TargetForm = this;
            // 
            // controlBoxClose
            // 
            controlBoxClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            controlBoxClose.CustomizableEdges = customizableEdges13;
            controlBoxClose.FillColor = Color.Transparent;
            controlBoxClose.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            controlBoxClose.IconColor = Color.FromArgb(169, 155, 135);
            controlBoxClose.Location = new Point(397, 12);
            controlBoxClose.Name = "controlBoxClose";
            controlBoxClose.ShadowDecoration.CustomizableEdges = customizableEdges14;
            controlBoxClose.Size = new Size(30, 29);
            controlBoxClose.TabIndex = 1;
            controlBoxClose.Click += controlBoxClose_Click;
            // 
            // ResetPasswordForm
            //            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(211, 195, 179);
            ClientSize = new Size(439, 651);
            Controls.Add(controlBoxClose);
            Controls.Add(mainPanel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "ResetPasswordForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Reset Password - Library Desktop";
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            ResumeLayout(false);
        }
    }
}
