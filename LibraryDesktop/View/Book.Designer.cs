namespace LibraryDesktop
{
    partial class Book
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Book));
            guna2ShadowPanel1 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            soluong_sach = new Guna.UI2.WinForms.Guna2HtmlLabel();
            btn_vaid = new Guna.UI2.WinForms.Guna2GradientButton();
            valid = new Guna.UI2.WinForms.Guna2GradientPanel();
            guna2ShadowPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // guna2ShadowPanel1
            // 
            guna2ShadowPanel1.BackColor = Color.Transparent;
            guna2ShadowPanel1.Controls.Add(soluong_sach);
            guna2ShadowPanel1.FillColor = Color.FromArgb(150, 255, 255, 255);
            guna2ShadowPanel1.Location = new Point(118, 232);
            guna2ShadowPanel1.Name = "guna2ShadowPanel1";
            guna2ShadowPanel1.Radius = 4;
            guna2ShadowPanel1.ShadowColor = Color.Black;
            guna2ShadowPanel1.Size = new Size(62, 46);
            guna2ShadowPanel1.TabIndex = 1;
            // 
            // soluong_sach
            // 
            soluong_sach.BackColor = Color.Transparent;
            soluong_sach.Location = new Point(26, 12);
            soluong_sach.Name = "soluong_sach";
            soluong_sach.Size = new Size(11, 22);
            soluong_sach.TabIndex = 2;
            soluong_sach.Text = "0";
            // 
            // btn_vaid
            // 
            btn_vaid.BackColor = Color.Transparent;
            btn_vaid.BorderRadius = 6;
            btn_vaid.CustomizableEdges = customizableEdges1;
            btn_vaid.DisabledState.BorderColor = Color.DarkGray;
            btn_vaid.DisabledState.CustomBorderColor = Color.DarkGray;
            btn_vaid.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btn_vaid.DisabledState.FillColor2 = Color.FromArgb(169, 169, 169);
            btn_vaid.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btn_vaid.FillColor = Color.FromArgb(169, 155, 135);
            btn_vaid.FillColor2 = Color.FromArgb(211, 195, 179);
            btn_vaid.Font = new Font("Segoe UI", 9F);
            btn_vaid.ForeColor = Color.White;
            btn_vaid.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            btn_vaid.Image = Properties.Resources.icons8_book_50__4_1;
            btn_vaid.Location = new Point(144, 0);
            btn_vaid.Name = "btn_vaid";
            btn_vaid.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btn_vaid.Size = new Size(39, 37);
            btn_vaid.TabIndex = 2;
            btn_vaid.Visible = false;
            // 
            // valid
            // 
            valid.BackColor = Color.Transparent;
            valid.CustomizableEdges = customizableEdges3;
            valid.Location = new Point(0, 0);
            valid.Name = "valid";
            valid.ShadowDecoration.CustomizableEdges = customizableEdges4;
            valid.Size = new Size(183, 278);
            valid.TabIndex = 3;
            valid.Click += vaild_Cick;
            // 
            // Book
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            Controls.Add(btn_vaid);
            Controls.Add(guna2ShadowPanel1);
            Controls.Add(valid);
            Name = "Book";
            Size = new Size(183, 278);
            Click += Book_Click;
            guna2ShadowPanel1.ResumeLayout(false);
            guna2ShadowPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel soluong_sach;
        private Guna.UI2.WinForms.Guna2GradientButton btn_vaid;
        private Guna.UI2.WinForms.Guna2GradientPanel valid;
    }
}
