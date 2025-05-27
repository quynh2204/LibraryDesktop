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
        public Dashboard()
        {
            InitializeComponent();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            SetupDataGridColumns();

            cmbTimePeriod.SelectedIndex = 2; // Default: "7 ngày qua"
            LoadInitialData();
        }

        #region Setup Methods
        private void SetupDataGridColumns()
        {
            // Setup Top Books DataGrid columns
            dgvTopBooks.Columns.Clear();
            dgvTopBooks.Columns.Add("Rank", "STT");
            dgvTopBooks.Columns.Add("Title", "Tên Sách");
            dgvTopBooks.Columns.Add("Category", "Thể Loại");
            dgvTopBooks.Columns.Add("Author", "Tác Giả");
            dgvTopBooks.Columns.Add("Views", "Lượt Xem");
            dgvTopBooks.Columns.Add("LastViewed", "Lần Cuối");

            // Set column widths for Top Books
            dgvTopBooks.Columns["Rank"].Width = 50;
            dgvTopBooks.Columns["Title"].Width = 200;
            dgvTopBooks.Columns["Category"].Width = 120;
            dgvTopBooks.Columns["Author"].Width = 120;
            dgvTopBooks.Columns["Views"].Width = 80;
            dgvTopBooks.Columns["LastViewed"].Width = 110;

            // Setup Category Stats DataGrid columns
            dgvCategoryStats.Columns.Clear();
            dgvCategoryStats.Columns.Add("Rank", "STT");
            dgvCategoryStats.Columns.Add("CategoryName", "Thể Loại");
            dgvCategoryStats.Columns.Add("TotalViews", "Tổng Xem");
            dgvCategoryStats.Columns.Add("Percentage", "Tỷ Lệ %");
            dgvCategoryStats.Columns.Add("BookCount", "Số Sách");
            dgvCategoryStats.Columns.Add("AvgViews", "TB/Sách");

            // Set column widths for Category Stats
            dgvCategoryStats.Columns["Rank"].Width = 50;
            dgvCategoryStats.Columns["CategoryName"].Width = 150;
            dgvCategoryStats.Columns["TotalViews"].Width = 100;
            dgvCategoryStats.Columns["Percentage"].Width = 80;
            dgvCategoryStats.Columns["BookCount"].Width = 80;
            dgvCategoryStats.Columns["AvgViews"].Width = 80;
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

        private void TimeTimer_Tick(object sender, EventArgs e)
        {
            lblCurrentTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
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
            // Load sample data for Top Books table
            dgvTopBooks.Rows.Clear();
            dgvTopBooks.Rows.Add("1", "Đắc Nhân Tâm", "Tâm lý", "Dale Carnegie", "1,250", "26/05/2025 18:30");
            dgvTopBooks.Rows.Add("2", "Sherlock Holmes", "Trinh thám", "Arthur Conan Doyle", "980", "26/05/2025 17:45");
            dgvTopBooks.Rows.Add("3", "Harry Potter", "Phiêu lưu", "J.K. Rowling", "875", "26/05/2025 16:20");
            dgvTopBooks.Rows.Add("4", "1984", "Tiểu thuyết", "George Orwell", "756", "26/05/2025 15:10");
            dgvTopBooks.Rows.Add("5", "The Shining", "Kinh dị", "Stephen King", "698", "26/05/2025 14:55");
            dgvTopBooks.Rows.Add("6", "To Kill a Mockingbird", "Tiểu thuyết", "Harper Lee", "645", "26/05/2025 14:30");
            dgvTopBooks.Rows.Add("7", "The Great Gatsby", "Tiểu thuyết", "F. Scott Fitzgerald", "598", "26/05/2025 13:45");
            dgvTopBooks.Rows.Add("8", "Pride and Prejudice", "Lãng mạn", "Jane Austen", "567", "26/05/2025 13:20");
            dgvTopBooks.Rows.Add("9", "The Catcher in the Rye", "Tiểu thuyết", "J.D. Salinger", "534", "26/05/2025 12:55");
            dgvTopBooks.Rows.Add("10", "Lord of the Flies", "Tiểu thuyết", "William Golding", "489", "26/05/2025 12:30");

            // Load sample data for Category Stats table
            dgvCategoryStats.Rows.Clear();
            dgvCategoryStats.Rows.Add("1", "Tiểu thuyết", "3,542", "28.5%", "125", "28.3");
            dgvCategoryStats.Rows.Add("2", "Tâm lý", "2,890", "23.2%", "89", "32.5");
            dgvCategoryStats.Rows.Add("3", "Trinh thám", "2,156", "17.3%", "67", "32.2");
            dgvCategoryStats.Rows.Add("4", "Phiêu lưu", "1,987", "16.0%", "78", "25.5");
            dgvCategoryStats.Rows.Add("5", "Kinh dị", "1,875", "15.0%", "45", "41.7");
            dgvCategoryStats.Rows.Add("6", "Lãng mạn", "1,234", "9.9%", "38", "32.5");
            dgvCategoryStats.Rows.Add("7", "Khoa học", "987", "7.9%", "29", "34.0");
            dgvCategoryStats.Rows.Add("8", "Lịch sử", "856", "6.9%", "25", "34.2");
            dgvCategoryStats.Rows.Add("9", "Thể thao", "743", "6.0%", "22", "33.8");
            dgvCategoryStats.Rows.Add("10", "Âm nhạc", "621", "5.0%", "18", "34.5");
        }
        #endregion

        private void chartsPanel_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}