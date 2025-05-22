using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryDesktop
{
    public partial class Book : UserControl
    {
        public Book()
        {
            InitializeComponent();
        }

        private void Book_Click(object sender, EventArgs e)
        {
            if (btn_vaid.Visible == false)
            {
                btn_vaid.Visible = true;
                valid.Visible = true;
            }
            else
            {
                btn_vaid.Visible = false;
                valid.Visible = false;
            }
        }

        private void vaild_Cick(object sender, EventArgs e)
        {
            if (btn_vaid.Visible == false)
            {
                btn_vaid.Visible = true;
                valid.Visible = true;
            }
            else
            {
                btn_vaid.Visible = false;
                valid.Visible = false;
            }
        }

        private void soluong_sach_Click(object sender, EventArgs e)
        {

        }
    }
}
