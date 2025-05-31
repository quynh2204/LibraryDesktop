using Guna.UI2.WinForms.Suite;

namespace LibraryDesktop.View
{
    partial class BookDetail
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            CustomizableEdges customizableEdges19 = new CustomizableEdges();
            CustomizableEdges customizableEdges20 = new CustomizableEdges();
            CustomizableEdges customizableEdges1 = new CustomizableEdges();
            CustomizableEdges customizableEdges2 = new CustomizableEdges();
            CustomizableEdges customizableEdges13 = new CustomizableEdges();
            CustomizableEdges customizableEdges14 = new CustomizableEdges();
            CustomizableEdges customizableEdges3 = new CustomizableEdges();
            CustomizableEdges customizableEdges4 = new CustomizableEdges();
            CustomizableEdges customizableEdges5 = new CustomizableEdges();
            CustomizableEdges customizableEdges6 = new CustomizableEdges();
            CustomizableEdges customizableEdges7 = new CustomizableEdges();
            CustomizableEdges customizableEdges8 = new CustomizableEdges();
            CustomizableEdges customizableEdges9 = new CustomizableEdges();
            CustomizableEdges customizableEdges10 = new CustomizableEdges();
            CustomizableEdges customizableEdges11 = new CustomizableEdges();
            CustomizableEdges customizableEdges12 = new CustomizableEdges();
            CustomizableEdges customizableEdges17 = new CustomizableEdges();
            CustomizableEdges customizableEdges18 = new CustomizableEdges();
            CustomizableEdges customizableEdges15 = new CustomizableEdges();
            CustomizableEdges customizableEdges16 = new CustomizableEdges();
            pnlMain = new Guna.UI2.WinForms.Guna2Panel();
            pnlContent = new Guna.UI2.WinForms.Guna2Panel();
            rtbContent = new RichTextBox();
            pnlRight = new Guna.UI2.WinForms.Guna2Panel();
            btnExit = new Guna.UI2.WinForms.Guna2Button();
            btnToggleTheme = new Guna.UI2.WinForms.Guna2Button();
            btnFavorite = new Guna.UI2.WinForms.Guna2Button();
            btnDownloadStory = new Guna.UI2.WinForms.Guna2Button();
            lblChapters = new Guna.UI2.WinForms.Guna2HtmlLabel();
            cmbChapters = new Guna.UI2.WinForms.Guna2ComboBox();
            pnlTop = new Guna.UI2.WinForms.Guna2Panel();
            lblViewCount = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblTotalChapters = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblStatus = new Guna.UI2.WinForms.Guna2HtmlLabel();
            txtDescription = new Guna.UI2.WinForms.Guna2TextBox();
            lblAuthor = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblBookTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            pnlMain.SuspendLayout();
            pnlContent.SuspendLayout();
            pnlRight.SuspendLayout();
            pnlTop.SuspendLayout();
            SuspendLayout();
            // 
            // pnlMain
            // 
            pnlMain.Controls.Add(pnlContent);
            pnlMain.Controls.Add(pnlRight);
            pnlMain.Controls.Add(pnlTop);
            pnlMain.CustomizableEdges = customizableEdges19;
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Location = new Point(0, 0);
            pnlMain.Margin = new Padding(3, 4, 3, 4);
            pnlMain.Name = "pnlMain";
            pnlMain.ShadowDecoration.CustomizableEdges = customizableEdges20;
            pnlMain.Size = new Size(1371, 933);
            pnlMain.TabIndex = 0;
            // 
            // pnlContent
            // 
            pnlContent.Controls.Add(rtbContent);
            pnlContent.CustomizableEdges = customizableEdges1;
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Location = new Point(0, 187);
            pnlContent.Margin = new Padding(3, 4, 3, 4);
            pnlContent.Name = "pnlContent";
            pnlContent.Padding = new Padding(17, 20, 17, 20);
            pnlContent.ShadowDecoration.CustomizableEdges = customizableEdges2;
            pnlContent.Size = new Size(1028, 746);
            pnlContent.TabIndex = 2;
            // 
            // rtbContent
            // 
            rtbContent.BackColor = Color.FromArgb(250, 250, 250);
            rtbContent.BorderStyle = BorderStyle.None;
            rtbContent.Dock = DockStyle.Fill;
            rtbContent.Font = new Font("Segoe UI", 10F);
            rtbContent.Location = new Point(17, 20);
            rtbContent.Margin = new Padding(3, 4, 3, 4);
            rtbContent.Name = "rtbContent";
            rtbContent.ReadOnly = true;
            rtbContent.Size = new Size(994, 706);
            rtbContent.TabIndex = 0;
            rtbContent.Text = "Select a chapter and click 'Download Story' to read the content.";
            rtbContent.DoubleClick += rtbContent_DoubleClick;
            // 
            // pnlRight
            // 
            pnlRight.Controls.Add(btnExit);
            pnlRight.Controls.Add(btnToggleTheme);
            pnlRight.Controls.Add(btnFavorite);
            pnlRight.Controls.Add(btnDownloadStory);
            pnlRight.Controls.Add(lblChapters);
            pnlRight.Controls.Add(cmbChapters);
            pnlRight.CustomizableEdges = customizableEdges13;
            pnlRight.Dock = DockStyle.Right;
            pnlRight.FillColor = Color.FromArgb(240, 240, 240);
            pnlRight.Location = new Point(1028, 187);
            pnlRight.Margin = new Padding(3, 4, 3, 4);
            pnlRight.Name = "pnlRight";
            pnlRight.Padding = new Padding(17, 20, 17, 20);
            pnlRight.ShadowDecoration.CustomizableEdges = customizableEdges14;
            pnlRight.Size = new Size(343, 746);
            pnlRight.TabIndex = 1;
            // 
            // btnExit
            // 
            btnExit.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnExit.BorderRadius = 8;
            btnExit.CustomizableEdges = customizableEdges3;
            btnExit.DisabledState.BorderColor = Color.DarkGray;
            btnExit.DisabledState.CustomBorderColor = Color.DarkGray;
            btnExit.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnExit.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnExit.FillColor = Color.FromArgb(231, 76, 60);
            btnExit.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnExit.ForeColor = Color.White;
            btnExit.Location = new Point(15, 360);
            btnExit.Margin = new Padding(3, 4, 3, 4);
            btnExit.Name = "btnExit";
            btnExit.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnExit.Size = new Size(309, 60);
            btnExit.TabIndex = 4;
            btnExit.Text = "Exit";
            btnExit.Click += btnExit_Click;
            // 
            // btnToggleTheme
            // 
            btnToggleTheme.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnToggleTheme.BorderRadius = 8;
            btnToggleTheme.CustomizableEdges = customizableEdges5;
            btnToggleTheme.DisabledState.BorderColor = Color.DarkGray;
            btnToggleTheme.DisabledState.CustomBorderColor = Color.DarkGray;
            btnToggleTheme.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnToggleTheme.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);            btnToggleTheme.FillColor = Color.FromArgb(95, 39, 205);
            btnToggleTheme.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnToggleTheme.ForeColor = Color.White;
            btnToggleTheme.Location = new Point(15, 293);
            btnToggleTheme.Margin = new Padding(3, 4, 3, 4);
            btnToggleTheme.Name = "btnToggleTheme";
            btnToggleTheme.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnToggleTheme.Size = new Size(309, 45);
            btnToggleTheme.TabIndex = 5;
            btnToggleTheme.Text = "◐ Toggle Theme";
            btnToggleTheme.Click += btnToggleTheme_Click;
            // 
            // btnFavorite
            // 
            btnFavorite.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnFavorite.BorderRadius = 8;
            btnFavorite.CustomizableEdges = customizableEdges7;
            btnFavorite.DisabledState.BorderColor = Color.DarkGray;
            btnFavorite.DisabledState.CustomBorderColor = Color.DarkGray;
            btnFavorite.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnFavorite.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnFavorite.FillColor = Color.FromArgb(220, 53, 69);
            btnFavorite.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnFavorite.ForeColor = Color.White;
            btnFavorite.Location = new Point(15, 126);
            btnFavorite.Margin = new Padding(3, 4, 3, 4);
            btnFavorite.Name = "btnFavorite";
            btnFavorite.ShadowDecoration.CustomizableEdges = customizableEdges8;
            btnFavorite.Size = new Size(309, 60);
            btnFavorite.TabIndex = 3;
            btnFavorite.Text = "❤️ Add to Favorites";
            btnFavorite.Click += btnFavorite_Click;
            // 
            // btnDownloadStory
            // 
            btnDownloadStory.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnDownloadStory.BorderRadius = 8;
            btnDownloadStory.CustomizableEdges = customizableEdges9;
            btnDownloadStory.DisabledState.BorderColor = Color.DarkGray;
            btnDownloadStory.DisabledState.CustomBorderColor = Color.DarkGray;
            btnDownloadStory.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnDownloadStory.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnDownloadStory.FillColor = Color.FromArgb(52, 152, 219);
            btnDownloadStory.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnDownloadStory.ForeColor = Color.White;
            btnDownloadStory.Location = new Point(17, 211);
            btnDownloadStory.Margin = new Padding(3, 4, 3, 4);
            btnDownloadStory.Name = "btnDownloadStory";
            btnDownloadStory.ShadowDecoration.CustomizableEdges = customizableEdges10;
            btnDownloadStory.Size = new Size(305, 60);
            btnDownloadStory.TabIndex = 2;
            btnDownloadStory.Text = "Download Story";
            btnDownloadStory.Click += btnDownloadStory_Click;
            // 
            // lblChapters
            // 
            lblChapters.BackColor = Color.Transparent;
            lblChapters.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblChapters.Location = new Point(17, 20);
            lblChapters.Margin = new Padding(3, 4, 3, 4);
            lblChapters.Name = "lblChapters";
            lblChapters.Size = new Size(166, 27);
            lblChapters.TabIndex = 1;
            lblChapters.Text = "Chapter List:";
            // 
            // cmbChapters
            // 
            cmbChapters.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbChapters.BackColor = Color.Transparent;
            cmbChapters.BorderRadius = 8;
            cmbChapters.CustomizableEdges = customizableEdges11;
            cmbChapters.DrawMode = DrawMode.OwnerDrawFixed;
            cmbChapters.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbChapters.FocusedColor = Color.FromArgb(94, 148, 255);
            cmbChapters.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            cmbChapters.Font = new Font("Segoe UI", 9F);
            cmbChapters.ForeColor = Color.FromArgb(68, 88, 112);
            cmbChapters.ItemHeight = 30;
            cmbChapters.Location = new Point(17, 67);
            cmbChapters.Margin = new Padding(3, 4, 3, 4);
            cmbChapters.Name = "cmbChapters";
            cmbChapters.ShadowDecoration.CustomizableEdges = customizableEdges12;
            cmbChapters.Size = new Size(308, 36);
            cmbChapters.TabIndex = 0;
            cmbChapters.SelectedIndexChanged += cmbChapters_SelectedIndexChanged;
            // 
            // pnlTop
            // 
            pnlTop.Controls.Add(lblViewCount);
            pnlTop.Controls.Add(lblTotalChapters);
            pnlTop.Controls.Add(lblStatus);
            pnlTop.Controls.Add(txtDescription);
            pnlTop.Controls.Add(lblAuthor);
            pnlTop.Controls.Add(lblBookTitle);
            pnlTop.CustomizableEdges = customizableEdges17;
            pnlTop.Dock = DockStyle.Top;
            pnlTop.FillColor = Color.FromArgb(250, 250, 250);
            pnlTop.Location = new Point(0, 0);
            pnlTop.Margin = new Padding(3, 4, 3, 4);
            pnlTop.Name = "pnlTop";
            pnlTop.Padding = new Padding(17, 20, 17, 20);
            pnlTop.ShadowDecoration.CustomizableEdges = customizableEdges18;
            pnlTop.Size = new Size(1371, 187);
            pnlTop.TabIndex = 0;
            // 
            // lblViewCount
            // 
            lblViewCount.BackColor = Color.Transparent;
            lblViewCount.Font = new Font("Segoe UI", 9F);
            lblViewCount.ForeColor = Color.FromArgb(100, 100, 100);
            lblViewCount.Location = new Point(17, 147);
            lblViewCount.Margin = new Padding(3, 4, 3, 4);
            lblViewCount.Name = "lblViewCount";
            lblViewCount.Size = new Size(68, 22);
            lblViewCount.TabIndex = 5;
            lblViewCount.Text = "View Count:";
            // 
            // lblTotalChapters
            // 
            lblTotalChapters.BackColor = Color.Transparent;
            lblTotalChapters.Font = new Font("Segoe UI", 9F);
            lblTotalChapters.ForeColor = Color.FromArgb(100, 100, 100);
            lblTotalChapters.Location = new Point(17, 120);
            lblTotalChapters.Margin = new Padding(3, 4, 3, 4);
            lblTotalChapters.Name = "lblTotalChapters";
            lblTotalChapters.Size = new Size(113, 22);
            lblTotalChapters.TabIndex = 4;
            lblTotalChapters.Text = "Total Chapters:";
            // 
            // lblStatus
            // 
            lblStatus.BackColor = Color.Transparent;
            lblStatus.Font = new Font("Segoe UI", 9F);
            lblStatus.ForeColor = Color.FromArgb(100, 100, 100);
            lblStatus.Location = new Point(17, 90);
            lblStatus.Margin = new Padding(3, 4, 3, 4);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(73, 22);
            lblStatus.TabIndex = 3;
            lblStatus.Text = "Status:";
            // 
            // txtDescription
            // 
            txtDescription.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtDescription.BorderRadius = 8;
            txtDescription.Cursor = Cursors.IBeam;
            txtDescription.CustomizableEdges = customizableEdges15;
            txtDescription.DefaultText = "";
            txtDescription.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtDescription.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtDescription.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtDescription.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtDescription.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtDescription.Font = new Font("Segoe UI", 9F);
            txtDescription.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtDescription.Location = new Point(409, 73);
            txtDescription.Margin = new Padding(3, 5, 3, 5);
            txtDescription.Multiline = true;
            txtDescription.Name = "txtDescription";
            txtDescription.PlaceholderText = "Story Description...";
            txtDescription.ReadOnly = true;
            txtDescription.ScrollBars = ScrollBars.Vertical;
            txtDescription.SelectedText = "";
            txtDescription.ShadowDecoration.CustomizableEdges = customizableEdges16;
            txtDescription.Size = new Size(945, 94);
            txtDescription.TabIndex = 2;
            // 
            // lblAuthor
            // 
            lblAuthor.BackColor = Color.Transparent;
            lblAuthor.Font = new Font("Segoe UI", 10F);
            lblAuthor.ForeColor = Color.FromArgb(100, 100, 100);
            lblAuthor.Location = new Point(17, 60);
            lblAuthor.Margin = new Padding(3, 4, 3, 4);
            lblAuthor.Name = "lblAuthor";
            lblAuthor.Size = new Size(61, 25);
            lblAuthor.TabIndex = 1;
            lblAuthor.Text = "Author:";
            // 
            // lblBookTitle
            // 
            lblBookTitle.BackColor = Color.Transparent;
            lblBookTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblBookTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblBookTitle.Location = new Point(17, 20);
            lblBookTitle.Margin = new Padding(3, 4, 3, 4);
            lblBookTitle.Name = "lblBookTitle";
            lblBookTitle.Size = new Size(141, 39);
            lblBookTitle.TabIndex = 0;
            lblBookTitle.Text = "Book Title";
            // 
            // BookDetail
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1371, 933);
            ControlBox = false;
            Controls.Add(pnlMain);
            Font = new Font("Segoe UI", 9F);
            Margin = new Padding(3, 4, 3, 4);
            MinimumSize = new Size(1140, 784);
            Name = "BookDetail";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            pnlMain.ResumeLayout(false);
            pnlContent.ResumeLayout(false);
            pnlRight.ResumeLayout(false);
            pnlRight.PerformLayout();
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel pnlMain;
        private Guna.UI2.WinForms.Guna2Panel pnlTop;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblBookTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblAuthor;
        private Guna.UI2.WinForms.Guna2TextBox txtDescription;
        private Guna.UI2.WinForms.Guna2Panel pnlRight;
        private Guna.UI2.WinForms.Guna2ComboBox cmbChapters;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblChapters;
        private Guna.UI2.WinForms.Guna2Button btnDownloadStory;
        private Guna.UI2.WinForms.Guna2Button btnFavorite;
        private Guna.UI2.WinForms.Guna2Button btnToggleTheme;
        private Guna.UI2.WinForms.Guna2Button btnExit;
        private Guna.UI2.WinForms.Guna2Panel pnlContent;
        private System.Windows.Forms.RichTextBox rtbContent;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblStatus;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTotalChapters;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblViewCount;

        // Rating UI Controls - ONLY DECLARED HERE
        private Guna.UI2.WinForms.Guna2Panel? pnlRating;
        private Guna.UI2.WinForms.Guna2HtmlLabel? lblRatingTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel? lblCurrentRating;
        private Guna.UI2.WinForms.Guna2HtmlLabel? lblAverageRating;
        private Guna.UI2.WinForms.Guna2Button[]? starButtons;
        private Guna.UI2.WinForms.Guna2TextBox? txtComment;
        private Guna.UI2.WinForms.Guna2Button? btnSubmitRating;
        private Guna.UI2.WinForms.Guna2Button? btnDeleteRating;

        // Comments display UI Controls - ONLY DECLARED HERE
        private Guna.UI2.WinForms.Guna2Panel? pnlAllComments;
        private Guna.UI2.WinForms.Guna2HtmlLabel? lblCommentsTitle;
        private FlowLayoutPanel? flpComments;
    }
}