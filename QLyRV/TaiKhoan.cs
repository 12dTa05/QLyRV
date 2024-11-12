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
using static QLyRV.Database;


namespace QLyRV
{
    public partial class TaiKhoan : Form
    {
        public TaiKhoan()
        {
            InitializeComponent();
        }

        private void TaiKhoan_Load(object sender, EventArgs e)
        {
            string chucvuQuery = "SELECT MaDV FROM DONVI where DaXoa = 0 and Cap != 0";
            string connectionString = conn_string;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // First query to get ChucVu
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

            dataGridView1.DataSource = GetTK().Tables[0];
        }

        DataSet GetTK()
        {

            DataSet data = new DataSet();

            string query = " SELECT tk.TDN, dv.TenDV, tk.MaNhom from TAIKHOAN tk, DONVI dv where tk.TDN = dv.MaDV and tk.Khoa = @tt";

            string connectionString = conn_string;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@tt", 0);
                    // Fill the DataSet
                    adapter.Fill(data);
                }
            }

            return data;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataSet data = new DataSet();

            string query = " SELECT dv.TenDV, tk.TenDN, tk.MaNhom, tk.Khoa from TAIKHOAN tk, DONVI dv where tk.TDN = @MaDV and tk.Khoa = @tt";

            string connectionString = conn_string;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@tt", 0);
                    adapter.SelectCommand.Parameters.AddWithValue("@MaDV", comboBox1.SelectedItem.ToString());
                    // Fill the DataSet
                    adapter.Fill(data);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string add = " insert into DONVI values @TenDN, , @MaTK, 0, @ct" + "\n insert into TAIKHOAN values @TenDN, @MK, @MaNhom, @TonTai";
            string connectionString = conn_string;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(add, conn))
                {
                    cmd.Parameters.AddWithValue("@MaTK", textBox5.Text.ToString());
                    cmd.Parameters.AddWithValue("@TenDN", textBox2.Text.ToString());
                    cmd.Parameters.AddWithValue("@MaNhom", textBox1.Text.ToString());
                    cmd.Parameters.AddWithValue("@MK", textBox3.Text.ToString());
                    cmd.Parameters.AddWithValue("@TonTai", 1);
                    cmd.Parameters.AddWithValue("@ct", textBox6.Text.ToString());

                    cmd.ExecuteNonQuery();
                }
            }

            dataGridView1.DataSource = GetTK().Tables[0];
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox5.Text = "";
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string add = "update TAIKHOAN set Khoa = @TonTai where TDN = @TDN" + "\n update DONVI set DaXoa = 1 where MaDV = @TDN";
            string connectionString = conn_string;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(add, conn))
                {
                    //cmd.Parameters.AddWithValue("@MaQN", textBox12.Text.ToString());
                    cmd.Parameters.AddWithValue("@TDN", textBox8.Text.ToString());
                    //cmd.Parameters.AddWithValue("@MK", textBox4.Text.ToString());
                    //cmd.Parameters.AddWithValue("@MN", textBox9.Text.ToString());
                    cmd.Parameters.AddWithValue("@TonTai", 1);

                    cmd.ExecuteNonQuery();
                }
            }

            dataGridView1.DataSource = GetTK().Tables[0];
            //textBox12.Text = "";
            textBox8.Text = "";
            textBox4.Text = "";
            textBox9.Text = "";
            textBox13.Text = "";
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
            if (e.RowIndex >= 0 && e.ColumnIndex == 0)
            {
                // Retrieve the clicked cell
                //textBox12.Text = dataGridView1[e.ColumnIndex + 1, e.RowIndex].ToString();
                textBox8.Text = dataGridView1[e.ColumnIndex + 0, e.RowIndex].Value.ToString();

                textBox4.Text = dataGridView1[e.ColumnIndex + 1, e.RowIndex].Value.ToString();
                textBox9.Text = dataGridView1[e.ColumnIndex + 2, e.RowIndex].Value.ToString();
            }
        }
    }
}
