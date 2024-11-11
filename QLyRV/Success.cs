using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLyRV
{
    public partial class Success : Form
    {
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
        private Timer timer;
        public static int Type;
        public Success(int type)
        {
            InitializeComponent();
            Type = type;
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 30, 30));
            // Thiết lập Timer
            timer = new Timer();
            timer.Interval = 5000; // 5 giây
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Success_Load(object sender, EventArgs e)
        {

        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Dừng Timer
            timer.Stop();
            if (Type == 0)
            {
                AdminForm form1 = new AdminForm(Type);
                form1.Show();
            }
            else if (Type == 1)
            {
                AdminForm form1 = new AdminForm(Type);
                form1.Show();
                Camera form2 = new Camera();
                form2.Show();
                //TrangChu form1 = new TrangChu();
                //form1.Show();
            }
            else if (Type == 2)
            {
                AdminForm form1 = new AdminForm(Type);
                form1.Show();
            }
            else
            {
                AdminForm form1 = new AdminForm(Type);
                form1.Show();
            }
            // Đóng Form1
            this.Hide();
        }
        private void rjButton1_Click(object sender, EventArgs e)
        {
            timer.Stop();
            if (Type == 0)
            {
                AdminForm form1 = new AdminForm(Type);
                form1.Show();
            }
            else if (Type == 1)
            {
                AdminForm form1 = new AdminForm(Type);
                form1.Show();
                Camera form2 = new Camera();
                form2.Show();
                //TrangChu form1 = new TrangChu();
                //form1.Show();
            }
            else if (Type == 2)
            {
                AdminForm form1 = new AdminForm(Type);
                form1.Show();
            }
            else
            {
                AdminForm form1 = new AdminForm(Type);
                form1.Show();
            }
            // Đóng Form1
            this.Hide();
        }
    }
}
