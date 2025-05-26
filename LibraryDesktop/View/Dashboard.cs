using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryDesktop.View
{
    public partial class Dashboard : UserControl
    {
        // Declare Guna2 controls
        private Guna2Panel mainPanel;
        private Guna2Panel headerPanel;
        private Guna2Panel summaryPanel;
        private Guna2Panel chartsPanel;
        private Guna2Panel tablesPanel;

        // Header controls
        private Guna2ComboBox cmbTimePeriod;
        private Guna2Button btnRefresh;
        private Guna2Button btnExport;
        private Label lblUser;
        private Label lblCurrentTime;

        // Summary cards
        private Guna2Panel cardTotalViews;
        private Guna2Panel cardTopCategory;
        private Guna2Panel cardGrowth;
        private Guna2Panel cardActiveBooks;

        public Dashboard()
        {
            InitializeComponent();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            SetupMainLayout();
            SetupHeader();
            SetupSummaryCards();
            SetupChartsArea();
            SetupTablesArea();
            LoadInitialData();
        }

        #region Main Layout Setup
        private void SetupMainLayout()
        {
            // Main container panel
            mainPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250), // Light gray background
                Padding = new Padding(0)
            };
            this.Controls.Add(mainPanel);

            // Header panel
            headerPanel = new Guna2Panel
            {
                Height = 70,
                Dock = DockStyle.Top,
                BackColor = Color.White,
                BorderRadius = 0,
                ShadowDecoration = { Enabled = true, Depth = 5 }
            };
            mainPanel.Controls.Add(headerPanel);

            // Summary cards panel
            summaryPanel = new Guna2Panel
            {
                Height = 140,
                Dock = DockStyle.Top,
                BackColor = Color.Transparent,
                Padding = new Padding(20, 20, 20, 10)
            };
            mainPanel.Controls.Add(summaryPanel);

            // Charts panel
            chartsPanel = new Guna2Panel
            {
                Height = 420,
                Dock = DockStyle.Top,
                BackColor = Color.Transparent,
                Padding = new Padding(20, 10, 20, 10)
            };
            mainPanel.Controls.Add(chartsPanel);

            // Tables panel
            tablesPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(20, 10, 20, 20)
            };
            mainPanel.Controls.Add(tablesPanel);
        }
        #endregion

        #region Header Setup
        private void SetupHeader()
        {
            // Time period dropdown
            cmbTimePeriod = new Guna2ComboBox
            {
                Location = new Point(20, 20),
                Size = new Size(200, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BorderRadius = 6,
                FillColor = Color.FromArgb(245, 246, 248),
                Font = new Font("Segoe UI", 9)
            };

            // Add time period options
            string[] timePeriods = {
                "Hôm nay",
                "Hôm qua",
                "7 ngày qua",
                "30 ngày qua",
                "Tháng này",
                "Tháng trước",
                "3 tháng qua",
                "Năm này"
            };
            cmbTimePeriod.Items.AddRange(timePeriods);
            cmbTimePeriod.SelectedIndex = 2; // Default: "7 ngày qua"
            cmbTimePeriod.SelectedIndexChanged += CmbTimePeriod_SelectedIndexChanged;

            headerPanel.Controls.Add(cmbTimePeriod);

            // Refresh button
            btnRefresh = new Guna2Button
            {
                Text = "🔄 Làm mới",
                Location = new Point(240, 20),
                Size = new Size(120, 30),
                BorderRadius = 6,
                FillColor = Color.FromArgb(46, 134, 171),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.White
            };
            btnRefresh.Click += BtnRefresh_Click;
            headerPanel.Controls.Add(btnRefresh);

            // Export button
            btnExport = new Guna2Button
            {
                Text = "📊 Xuất Excel",
                Location = new Point(380, 20),
                Size = new Size(120, 30),
                BorderRadius = 6,
                FillColor = Color.FromArgb(34, 197, 94),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.White
            };
            btnExport.Click += BtnExport_Click;
            headerPanel.Controls.Add(btnExport);

            // Current time label
            lblCurrentTime = new Label
            {
                Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Location = new Point(520, 20),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(107, 114, 128),
                TextAlign = ContentAlignment.MiddleLeft
            };
            headerPanel.Controls.Add(lblCurrentTime);

            // User info (on the right)
            lblUser = new Label
            {
                Text = "👤 quynh2204",
                Location = new Point(this.Width - 150, 20),
                Size = new Size(120, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(75, 85, 99),
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            headerPanel.Controls.Add(lblUser);

            // Inside the SetupHeader method
            System.Windows.Forms.Timer timeTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            timeTimer.Tick += (s, e) => lblCurrentTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            timeTimer.Start();
        }
        #endregion

        #region Summary Cards Setup
        private void SetupSummaryCards()
        {
            int cardWidth = 320;
            int cardHeight = 100;
            int spacing = 20;

            // Card 1: Total Views
            cardTotalViews = CreateSummaryCard(
                "📈 Tổng lượt xem",
                "12,450",
                "+8.2% so với hôm qua",
                Color.FromArgb(59, 130, 246),
                new Point(0, 0),
                new Size(cardWidth, cardHeight)
            );
            summaryPanel.Controls.Add(cardTotalViews);

            // Card 2: Top Category  
            cardTopCategory = CreateSummaryCard(
                "⭐ Thể loại hàng đầu",
                "Tiểu thuyết",
                "3,542 lượt xem",
                Color.FromArgb(34, 197, 94),
                new Point(cardWidth + spacing, 0),
                new Size(cardWidth, cardHeight)
            );
            summaryPanel.Controls.Add(cardTopCategory);

            // Card 3: Growth Rate
            cardGrowth = CreateSummaryCard(
                "📊 Tăng trưởng",
                "+15.6%",
                "So với tuần trước",
                Color.FromArgb(249, 115, 22),
                new Point((cardWidth + spacing) * 2, 0),
                new Size(cardWidth, cardHeight)
            );
            summaryPanel.Controls.Add(cardGrowth);

            // Card 4: Active Books
            cardActiveBooks = CreateSummaryCard(
                "📚 Sách hoạt động",
                "89",
                "Được xem hôm nay",
                Color.FromArgb(168, 85, 247),
                new Point((cardWidth + spacing) * 3, 0),
                new Size(cardWidth, cardHeight)
            );
            summaryPanel.Controls.Add(cardActiveBooks);
        }

        private Guna2Panel CreateSummaryCard(string title, string value, string subtitle, Color accentColor, Point location, Size size)
        {
            var card = new Guna2Panel
            {
                Location = location,
                Size = size,
                BorderRadius = 12,
                ShadowDecoration = { Enabled = true, Depth = 10 },
                FillColor = Color.White
            };

            // Accent stripe
            var accentStripe = new Panel
            {
                Size = new Size(4, size.Height),
                Location = new Point(0, 0),
                BackColor = accentColor
            };
            card.Controls.Add(accentStripe);

            // Title label
            var lblTitle = new Label
            {
                Text = title,
                Location = new Point(20, 15),
                Size = new Size(size.Width - 40, 20),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(107, 114, 128)
            };
            card.Controls.Add(lblTitle);

            // Value label
            var lblValue = new Label
            {
                Text = value,
                Location = new Point(20, 35),
                Size = new Size(size.Width - 40, 30),
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39)
            };
            card.Controls.Add(lblValue);

            // Subtitle label
            var lblSubtitle = new Label
            {
                Text = subtitle,
                Location = new Point(20, 70),
                Size = new Size(size.Width - 40, 20),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(107, 114, 128)
            };
            card.Controls.Add(lblSubtitle);

            return card;
        }
        #endregion

        #region Charts Area Setup
        private void SetupChartsArea()
        {
            int chartWidth = 420;
            int chartHeight = 380;
            int spacing = 20;

            // Chart 1: Bar Chart Container
            var chartContainer1 = CreateChartContainer(
                "📊 Top 10 Thể loại",
                new Point(0, 0),
                new Size(chartWidth, chartHeight)
            );
            chartsPanel.Controls.Add(chartContainer1);

            // Chart 2: Pie Chart Container
            var chartContainer2 = CreateChartContainer(
                "🥧 Phân bố Thể loại",
                new Point(chartWidth + spacing, 0),
                new Size(chartWidth, chartHeight)
            );
            chartsPanel.Controls.Add(chartContainer2);

            // Chart 3: Line Chart Container
            var chartContainer3 = CreateChartContainer(
                "📈 Xu hướng 30 ngày",
                new Point((chartWidth + spacing) * 2, 0),
                new Size(chartWidth, chartHeight)
            );
            chartsPanel.Controls.Add(chartContainer3);
        }

        private Guna2Panel CreateChartContainer(string title, Point location, Size size)
        {
            var container = new Guna2Panel
            {
                Location = location,
                Size = size,
                BorderRadius = 12,
                ShadowDecoration = { Enabled = true, Depth = 10 },
                FillColor = Color.White,
                Padding = new Padding(15)
            };

            // Title
            var lblTitle = new Label
            {
                Text = title,
                Location = new Point(15, 15),
                Size = new Size(size.Width - 30, 25),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39)
            };
            container.Controls.Add(lblTitle);

            // Placeholder for actual chart
            var chartPlaceholder = new Panel
            {
                Location = new Point(15, 45),
                Size = new Size(size.Width - 30, size.Height - 60),
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            var placeholderLabel = new Label
            {
                Text = "Chart will be here\n(Implement with\nGuna.Charts or\nSystem.Windows.Forms\n.DataVisualization)",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(107, 114, 128)
            };
            chartPlaceholder.Controls.Add(placeholderLabel);
            container.Controls.Add(chartPlaceholder);

            return container;
        }
        #endregion

        #region Tables Area Setup
        private void SetupTablesArea()
        {
            int tableWidth = 650;
            int tableHeight = 350;
            int spacing = 20;

            // Table 1: Top Books
            var tableContainer1 = CreateTableContainer(
                "🏆 Top 20 Sách được xem nhiều nhất",
                CreateTopBooksTable(),
                new Point(0, 0),
                new Size(tableWidth, tableHeight)
            );
            tablesPanel.Controls.Add(tableContainer1);

            // Table 2: Category Stats
            var tableContainer2 = CreateTableContainer(
                "📚 Thống kê Thể loại",
                CreateCategoryStatsTable(),
                new Point(tableWidth + spacing, 0),
                new Size(tableWidth, tableHeight)
            );
            tablesPanel.Controls.Add(tableContainer2);
        }

        private Guna2Panel CreateTableContainer(string title, Guna2DataGridView dataGrid, Point location, Size size)
        {
            var container = new Guna2Panel
            {
                Location = location,
                Size = size,
                BorderRadius = 12,
                ShadowDecoration = { Enabled = true, Depth = 10 },
                FillColor = Color.White,
                Padding = new Padding(15)
            };

            // Title
            var lblTitle = new Label
            {
                Text = title,
                Location = new Point(15, 15),
                Size = new Size(size.Width - 30, 25),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39)
            };
            container.Controls.Add(lblTitle);

            // DataGrid
            dataGrid.Location = new Point(15, 45);
            dataGrid.Size = new Size(size.Width - 30, size.Height - 60);
            container.Controls.Add(dataGrid);

            return container;
        }

        private Guna2DataGridView CreateTopBooksTable()
        {
            var dgv = new Guna2DataGridView
            {
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 40,
                GridColor = Color.FromArgb(231, 229, 228),
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                Font = new Font("Segoe UI", 9)
            };

            // Style headers
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(75, 85, 99);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Style rows
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(17, 24, 39);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(59, 130, 246);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.RowTemplate.Height = 35;

            // Add columns
            dgv.Columns.Add("Rank", "STT");
            dgv.Columns.Add("Title", "Tên Sách");
            dgv.Columns.Add("Category", "Thể Loại");
            dgv.Columns.Add("Author", "Tác Giả");
            dgv.Columns.Add("Views", "Lượt Xem");
            dgv.Columns.Add("LastViewed", "Lần Cuối");

            // Set column widths
            dgv.Columns["Rank"].Width = 50;
            dgv.Columns["Title"].Width = 200;
            dgv.Columns["Category"].Width = 120;
            dgv.Columns["Author"].Width = 120;
            dgv.Columns["Views"].Width = 80;
            dgv.Columns["LastViewed"].Width = 110;

            return dgv;
        }

        private Guna2DataGridView CreateCategoryStatsTable()
        {
            var dgv = new Guna2DataGridView
            {
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 40,
                GridColor = Color.FromArgb(231, 229, 228),
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                Font = new Font("Segoe UI", 9)
            };

            // Style headers và rows giống như trên
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(75, 85, 99);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(17, 24, 39);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(59, 130, 246);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.RowTemplate.Height = 35;

            // Add columns
            dgv.Columns.Add("Rank", "STT");
            dgv.Columns.Add("CategoryName", "Thể Loại");
            dgv.Columns.Add("TotalViews", "Tổng Xem");
            dgv.Columns.Add("Percentage", "Tỷ Lệ %");
            dgv.Columns.Add("BookCount", "Số Sách");
            dgv.Columns.Add("AvgViews", "TB/Sách");

            // Set column widths
            dgv.Columns["Rank"].Width = 50;
            dgv.Columns["CategoryName"].Width = 150;
            dgv.Columns["TotalViews"].Width = 100;
            dgv.Columns["Percentage"].Width = 80;
            dgv.Columns["BookCount"].Width = 80;
            dgv.Columns["AvgViews"].Width = 80;

            return dgv;
        }
        #endregion

        #region Event Handlers
        private void CmbTimePeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDashboardData();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadDashboardData();
            MessageBox.Show("Dữ liệu đã được làm mới!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng xuất Excel sẽ được implement sau!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region Data Loading
        private void LoadInitialData()
        {
            LoadSampleData();
        }

        private void LoadDashboardData()
        {
            // TODO: Implement actual data loading from database
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            // Load sample data cho Top Books table
            var topBooksTable = (Guna2DataGridView)FindControlRecursive(tablesPanel, typeof(Guna2DataGridView));
            if (topBooksTable != null)
            {
                topBooksTable.Rows.Clear();

                // Sample data
                topBooksTable.Rows.Add("1", "Đắc Nhân Tâm", "Tâm lý", "Dale Carnegie", "1,250", "26/05/2025 18:30");
                topBooksTable.Rows.Add("2", "Sherlock Holmes", "Trinh thám", "Arthur Conan Doyle", "980", "26/05/2025 17:45");
                topBooksTable.Rows.Add("3", "Harry Potter", "Phiêu lưu", "J.K. Rowling", "875", "26/05/2025 16:20");
                topBooksTable.Rows.Add("4", "1984", "Tiểu thuyết", "George Orwell", "756", "26/05/2025 15:10");
                topBooksTable.Rows.Add("5", "The Shining", "Kinh dị", "Stephen King", "698", "26/05/2025 14:55");
            }

            // Load sample data cho Category Stats table
            var categoryStatsTable = FindAllControlsOfType<Guna2DataGridView>(tablesPanel)[1];
            if (categoryStatsTable != null)
            {
                categoryStatsTable.Rows.Clear();

                categoryStatsTable.Rows.Add("1", "Tiểu thuyết", "3,542", "28.5%", "125", "28.3");
                categoryStatsTable.Rows.Add("2", "Tâm lý", "2,890", "23.2%", "89", "32.5");
                categoryStatsTable.Rows.Add("3", "Trinh thám", "2,156", "17.3%", "67", "32.2");
                categoryStatsTable.Rows.Add("4", "Phiêu lưu", "1,987", "16.0%", "78", "25.5");
                categoryStatsTable.Rows.Add("5", "Kinh dị", "1,875", "15.0%", "45", "41.7");
            }
        }

        // Helper methods
        private Control FindControlRecursive(Control container, Type type)
        {
            foreach (Control control in container.Controls)
            {
                if (control.GetType() == type)
                    return control;

                var found = FindControlRecursive(control, type);
                if (found != null)
                    return found;
            }
            return null;
        }

        private List<T> FindAllControlsOfType<T>(Control container) where T : Control
        {
            var result = new List<T>();
            foreach (Control control in container.Controls)
            {
                if (control is T)
                    result.Add((T)control);

                result.AddRange(FindAllControlsOfType<T>(control));
            }
            return result;
        }
        #endregion
    }
}

