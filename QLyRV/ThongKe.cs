using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static QLyRV.Account;
using static QLyRV.Database;
using static QLyRV.Success;

using System.Data.SqlClient;

namespace QLyRV
{
    public partial class ThongKe : Form
    {
        public ThongKe()
        {
            InitializeComponent();
        }

        private void ThongKe_Load(object sender, EventArgs e)
        {
            label2.Hide();
            label3.Hide();
            dateTimePicker2.Hide();

            string connectionString = conn_string;
            if (Success.Type == 0)
            {
                
                
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string chucvuQuery = "SELECT MaDV FROM DONVI where DaXoa != 1 and Cap != 0";
                    using (SqlCommand chucvuCommand = new SqlCommand(chucvuQuery, conn))
                    {
                        using (SqlDataReader reader = chucvuCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                comboBox1.Items.Add(reader.GetString(0));
                            }

                        }
                    }
                }
            }
            else if (Success.Type == 2)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string chucvuQuery = "SELECT MaDV FROM DONVI where MaDVCapTren ='" + Account.account + "' and DaXoa != 1";
                    using (SqlCommand chucvuCommand = new SqlCommand(chucvuQuery, conn))
                    {
                        using (SqlDataReader reader = chucvuCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                comboBox1.Items.Add(reader.GetString(0));
                            }

                        }
                    }
                }
            }
            else
            {
                comboBox1.Items.Add(Account.account);
            }

            var items = comboBox1.Items.Cast<string>().ToList();
            // Sắp xếp các mục theo độ dài và sau đó theo thứ tự chữ cái
            items = items.OrderBy(item => item.Length).ThenBy(item => item).ToList();
            // Xóa các mục hiện tại trong ComboBox
            comboBox1.Items.Clear();
            // Thêm các mục đã sắp xếp vào lại ComboBox
            foreach (var item in items)
            {
                comboBox1.Items.Add(item);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                label3.Show();
                label2.Show();
                dateTimePicker2.Show();
            }
            else
            {
                label2.Hide();
                label3.Hide();
                dateTimePicker2.Hide();
            }
        }
    }
}
