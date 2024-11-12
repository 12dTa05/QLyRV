using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static QLyRV.Database;
using static QLyRV.Account;
using static QLyRV.Success;
using System.Runtime.InteropServices;
using System.Data.SqlClient;

namespace QLyRV
{
    public partial class AdminForm : Form
    {
        public static string Nhom;
        bool sidebarExpand;
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
(
int nLeftRect,     // x-coordinate of upper-left corner
int nTopRect,      // y-coordinate of upper-left corner
int nRightRect,    // x-coordinate of lower-right corner
int nBottomRect,   // y-coordinate of lower-right corner
int nWidthEllipse, // height of ellipse
int nHeightEllipse // width of ellipse
);
        public AdminForm(int type)
        {
            //if (type == 1)
            //{
            //    Camera cm = new Camera();
            //    this.Show();
            //    cm.Show();
            //}
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 30, 30));
        }

        private void TrangChu_Load(object sender, EventArgs e)
        {
            if (Success.Type != 0)
            {
                //panel7.Hide();
                rjButton2.Hide();
            }
            if (Success.Type != 1)
            {
                rjButton3.Hide();
            }

            //if(Account.account == "vb")
            //{
            //    this.Show();
            //    Camera cmr = new Camera();
            //    cmr.Show();
            //}
                
        }

        private Form currentFormChild;
        public void OpenChildForm(Form childForm)
        {
            if (currentFormChild != null)
            {
                currentFormChild.Close();
            }
            currentFormChild = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelBody.Controls.Add(childForm);
            panelBody.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            //OpenChildForm(new Add());
        }

        private void sidebarTimer_Tick(object sender, EventArgs e)
        {
            if (sidebarExpand)
            {
                panelMenu.Width -= 10;
                if (panelMenu.Width == panelMenu.MinimumSize.Width)
                {
                    sidebarExpand = false;
                    sidebarTimer.Stop();
                }
            }
            else
            {
                panelMenu.Width += 10;
                if (panelMenu.Width == panelMenu.MaximumSize.Width)
                {
                    sidebarExpand = true;
                    sidebarTimer.Stop();
                }
            }
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            sidebarTimer.Start();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            //OpenChildForm(new LogForm2());

        }

        private void Button4_Click(object sender, EventArgs e)
        {
            OpenChildForm(new NhatKy(Success.Type));
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //Form1 frm = new Form1();
            //this.Hide();
            //frm.Show();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            OpenChildForm(new ThongKe());
        }

        private void Button4_Click_1(object sender, EventArgs e)
        {

        }

        private void Button3_Click_1(object sender, EventArgs e)
        { 
            if(Success.Type != 0 && Success.Type != 2)
            {
                OpenChildForm(new DangKi());
            }
            else
            {
                OpenChildForm(new DuyetDK());  
            }
        }

        private void Button2_Click_1(object sender, EventArgs e)
        {
            OpenChildForm(new QuanNhan());
        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            OpenChildForm(new TaiKhoan());
        }

        private void rjButton3_Click(object sender, EventArgs e)
        {
            OpenChildForm(new ViPham());
        }
    }
}
