using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static QLyRV.Database;

namespace QLyRV
{
    public partial class Account : Form
    {
        List<Image> images = new List<Image>();
        string[] location = new string[25];

        public static String account, password;
        public Account()
        {
            InitializeComponent();
            location[0] = @"..\..\animation\textbox_user_1.jpg";
            location[1] = @"..\..\animation\textbox_user_2.jpg";
            location[2] = @"..\..\animation\textbox_user_4.jpg";
            location[3] = @"..\..\animation\textbox_user_5.jpg";
            location[4] = @"..\..\animation\textbox_user_6.jpg";
            location[5] = @"..\..\animation\textbox_user_7.jpg";
            location[6] = @"..\..\animation\textbox_user_8.jpg";
            location[7] = @"..\..\animation\textbox_user_9.jpg";
            location[8] = @"..\..\animation\textbox_user_10.jpg";
            location[9] = @"..\..\animation\textbox_user_11.jpg";
            location[10] = @"..\..\animation\textbox_user_12.jpg";
            location[11] = @"..\..\animation\textbox_user_13.jpg";
            location[12] = @"..\..\animation\textbox_user_14.jpg";
            location[13] = @"..\..\animation\textbox_user_15.jpg";
            location[14] = @"..\..\animation\textbox_user_16.jpg";
            location[15] = @"..\..\animation\textbox_user_17.jpg";
            location[16] = @"..\..\animation\textbox_user_18.jpg";
            location[17] = @"..\..\animation\textbox_user_19.jpg";
            location[18] = @"..\..\animation\textbox_user_20.jpg";
            location[19] = @"..\..\animation\textbox_user_21.jpg";
            location[20] = @"..\..\animation\textbox_user_22.jpg";
            location[21] = @"..\..\animation\textbox_user_23.jpg";
            location[22] = @"..\..\animation\textbox_user_24.jpg";
            tounage();
        }

        private void tounage()
        {
            for (int i = 0; i < 23; i++)
            {
                Bitmap bitmap = new Bitmap(location[i]);
                images.Add(bitmap);
            }
            images.Add(QLyRV.Properties.Resources.debut);
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (txtUsername.Text.Length > 0 && txtUsername.Text.Length <= 15)
            {
                pictureBox1.Image = images[txtUsername.Text.Length - 1];
                pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            }
            else if (txtUsername.Text.Length <= 0)
                pictureBox1.Image = Properties.Resources.debut;
            else
                pictureBox1.Image = images[22];
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            Bitmap bmpass = new Bitmap(@"..\..\animation\textbox_password.png");
            pictureBox1.Image = bmpass;
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text.Length > 0)
                pictureBox1.Image = images[txtUsername.Text.Length - 1];
            else
                pictureBox1.Image = Properties.Resources.debut;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            account = txtUsername.Text;
            password = txtPassword.Text;
            string query = "SELECT * FROM TAIKHOAN WHERE TDN = '" + txtUsername.Text + "'";
            try
            {
                SqlConnection conn = new SqlConnection(conn_string);
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.CommandType = CommandType.Text;
                SqlDataReader reader = command.ExecuteReader();
                // Kiểm tra xem có dữ liệu không 
                if (reader.HasRows)
                {
                    // Đọc và hiển thị dữ liệu
                    while (reader.Read())
                    {
                        if (reader.GetString(reader.GetOrdinal("MatKhau")).ToString() == txtPassword.Text && reader.GetBoolean(reader.GetOrdinal("Khoa")).ToString() == "False")
                        {

                            if (reader.GetString(reader.GetOrdinal("MaNhom")).ToString() == "0")
                            {
                                new Success(0).Show();
                            }
                            else if (reader.GetString(reader.GetOrdinal("MaNhom")).ToString() == "1")
                            {
                                new Success(1).Show();
                            }
                            else if (reader.GetString(reader.GetOrdinal("MaNhom")).ToString() == "2")
                            {
                                new Success(2).Show();
                            }
                            else
                            {
                                new Success(3).Show();
                            }
                            this.Hide();

                        }
                        else
                        {
                            new UnSucess().Show();
                            
                        }
                    }
                }
                else
                {
                   new UnSucess().Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thực hiện truy vấn SQL: " + ex.Message);
            }
        }
    }
}
