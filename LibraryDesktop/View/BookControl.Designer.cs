namespace LibraryDesktop.View
{
    partial class BookControl
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2ShadowPanel1 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            lblChapterCount = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblAuthor = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblBookTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            picBookCover = new Guna.UI2.WinForms.Guna2PictureBox();
            panelPremium = new Guna.UI2.WinForms.Guna2Panel();
            lblPrice = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2ShadowPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picBookCover).BeginInit();
            panelPremium.SuspendLayout();
            SuspendLayout();
            // 
            // guna2ShadowPanel1
            // 
            guna2ShadowPanel1.BackColor = Color.Transparent;
            guna2ShadowPanel1.Controls.Add(lblChapterCount);
            guna2ShadowPanel1.Controls.Add(lblAuthor);
            guna2ShadowPanel1.Controls.Add(lblBookTitle);
            guna2ShadowPanel1.Controls.Add(picBookCover);
            guna2ShadowPanel1.Controls.Add(panelPremium);
            guna2ShadowPanel1.FillColor = Color.White;
            guna2ShadowPanel1.Location = new Point(3, 3);
            guna2ShadowPanel1.Name = "guna2ShadowPanel1";
            guna2ShadowPanel1.Radius = 8;
            guna2ShadowPanel1.ShadowColor = Color.Black;
            guna2ShadowPanel1.ShadowDepth = 50;
            guna2ShadowPanel1.Size = new Size(220, 320);
            guna2ShadowPanel1.TabIndex = 0;
            guna2ShadowPanel1.Click += BookControl_Click;
            // 
            // lblChapterCount
            // 
            lblChapterCount.BackColor = Color.Transparent;
            lblChapterCount.Font = new Font("Segoe UI", 8F);
            lblChapterCount.ForeColor = Color.FromArgb(169, 155, 135);
            lblChapterCount.Location = new Point(15, 287);
            lblChapterCount.Name = "lblChapterCount";
            lblChapterCount.Size = new Size(66, 19);
            lblChapterCount.TabIndex = 4;
            lblChapterCount.Text = "0 Chapters";
            // 
            // lblAuthor
            // 
            lblAuthor.BackColor = Color.Transparent;
            lblAuthor.Font = new Font("Segoe UI", 8F);
            lblAuthor.ForeColor = Color.FromArgb(128, 128, 128);
            lblAuthor.Location = new Point(15, 252);
            lblAuthor.MaximumSize = new Size(190, 20);
            lblAuthor.Name = "lblAuthor";
            lblAuthor.Size = new Size(42, 19);
            lblAuthor.TabIndex = 3;
            lblAuthor.Text = "Author";
            // 
            // lblBookTitle
            // 
            lblBookTitle.BackColor = Color.Transparent;
            lblBookTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblBookTitle.ForeColor = Color.FromArgb(64, 64, 64);
            lblBookTitle.Location = new Point(15, 211);
            lblBookTitle.MaximumSize = new Size(190, 40);
            lblBookTitle.Name = "lblBookTitle";            lblBookTitle.Size = new Size(85, 25);
            lblBookTitle.TabIndex = 2;
            lblBookTitle.Text = "Book Title";
            // 
            // picBookCover
            // 
            picBookCover.BackColor = Color.Transparent;
            picBookCover.BorderRadius = 5;
            picBookCover.CustomizableEdges = customizableEdges5;
            picBookCover.FillColor = Color.FromArgb(240, 240, 240);
            picBookCover.ImageRotate = 0F;
            picBookCover.Location = new Point(15, 45);
            picBookCover.Name = "picBookCover";
            picBookCover.ShadowDecoration.CustomizableEdges = customizableEdges6;            picBookCover.Size = new Size(190, 160);
            picBookCover.SizeMode = PictureBoxSizeMode.StretchImage;
            picBookCover.TabIndex = 1;
            picBookCover.TabStop = false;
            // 
            // panelPremium
            // 
            panelPremium.BackColor = Color.Transparent;
            panelPremium.BorderRadius = 8;
            panelPremium.Controls.Add(lblPrice);
            panelPremium.CustomizableEdges = customizableEdges7;
            panelPremium.FillColor = Color.FromArgb(255, 193, 7);
            panelPremium.Location = new Point(10, 10);
            panelPremium.Name = "panelPremium";
            panelPremium.ShadowDecoration.CustomizableEdges = customizableEdges8;
            panelPremium.Size = new Size(60, 25);
            panelPremium.TabIndex = 0;
            panelPremium.Visible = false;
            // 
            // lblPrice
            // 
            lblPrice.BackColor = Color.Transparent;
            lblPrice.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblPrice.ForeColor = Color.White;
            lblPrice.Location = new Point(5, 3);
            lblPrice.Name = "lblPrice";
            lblPrice.Size = new Size(48, 19);
            lblPrice.TabIndex = 0;
            lblPrice.Text = "0 Coins";
            // 
            // BookControl
            //            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Transparent;
            Controls.Add(guna2ShadowPanel1);
            Cursor = Cursors.Hand;
            Margin = new Padding(10, 10, 10, 10);
            Name = "BookControl";
            Size = new Size(230, 330);
            guna2ShadowPanel1.ResumeLayout(false);
            guna2ShadowPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picBookCover).EndInit();
            panelPremium.ResumeLayout(false);
            panelPremium.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel1;
        private Guna.UI2.WinForms.Guna2Panel panelPremium;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblPrice;
        private Guna.UI2.WinForms.Guna2PictureBox picBookCover;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblBookTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblAuthor;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblChapterCount;
    }
}
