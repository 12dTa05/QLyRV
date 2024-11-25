using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data.SqlClient;
using static QLyRV.Database;
using static QLyRV.Account;
using static QLyRV.Success;
using System.Runtime.InteropServices.ComTypes;
using System.Web.UI.WebControls;


namespace QLyRV
{
    public partial class QuanNhan : Form
    {
        public QuanNhan()
        {
            InitializeComponent();

            
        }

        private void QuanNhan_Load(object sender, EventArgs e)
        {
            if (Success.Type != 0 && Success.Type != 2)
            {
                button2.Hide();
                //button2.Hide();
                tabControl1.TabPages.Remove(tabPage1);
            }
            if (Success.Type == 2)
            {
                tabControl1.TabPages.Remove(tabPage1);
                label14.Hide();
                textBox13.Hide();
            }

            if (Success.Type == 0)
            {
                string chucvuQuery = "SELECT MaDV FROM DONVI where TonTai = 1 and MaDVCapTren = 'admin'";
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
            }
            else if(Success.Type == 2)
            {
                string chucvuQuery = "SELECT MaDV FROM DONVI where MaDVCapTren ='" + Account.account + "' and TonTai = 1";
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

            //comboBox1.SelectedIndex = 0;
            dataGridView1.DataSource = GetQN().Tables[0];
        }

        DataSet GetQN()
        {

            DataSet data = new DataSet();

            string query = " SELECT qn.CCCD, qn.MaQN, qn.HoTen, qn.CapBac, qn.MaCV, qn.MaDV, qn.TonTai from QUANNHAN qn where qn.TonTai = @tt and qn.MaDV = @dv ";
            string query1 = " SELECT qn.CCCD, qn.MaQN, qn.HoTen, qn.CapBac, qn.MaCV, qn.MaDV, qn.TonTai from QUANNHAN qn where  qn.TonTai = @tt  ";

            string connectionString = conn_string;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                if(comboBox1.SelectedItem != null)
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@tt", 1);
                        adapter.SelectCommand.Parameters.AddWithValue("@dv", comboBox1.SelectedItem.ToString());
                        // Fill the DataSet
                        adapter.Fill(data);
                    }
                }
                else
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query1, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@tt", 1);
                        //adapter.SelectCommand.Parameters.AddWithValue("@dv", comboBox1.SelectedItem.ToString());
                        // Fill the DataSet
                        adapter.Fill(data);
                    }
                }
            }

            return data;
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataSet data = new DataSet();

            string query = " SELECT qn.CCCD, qn.MaQN, qn.HoTen, qn.CapBac, qn.MaCV, qn.MaDV from QUANNHAN qn where qn.MaDV = @dv and qn.TonTai = @tt";

            string connectionString = conn_string;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@tt", 1);
                    adapter.SelectCommand.Parameters.AddWithValue("@dv", comboBox1.SelectedItem.ToString());
                    // Fill the DataSet
                    adapter.Fill(data);
                }
            }
            dataGridView1.DataSource = data.Tables[0];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string add = "insert into QUANNHAN (CCCD, MaQN, HoTen, CapBac, TonTai, MaCV, MaDV) values (@CCCD, @MaQN, @HoTen, @CapBac, @TonTai, @MaCV, @MaDV)";
            string connectionString = conn_string;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(add, conn))
                {
                    cmd.Parameters.AddWithValue("@CCCD", textBox1.Text.ToString());
                    cmd.Parameters.AddWithValue("@MaQN", textBox4.Text.ToString());
                    cmd.Parameters.AddWithValue("@HoTen", textBox2.Text.ToString());
                    cmd.Parameters.AddWithValue("@CapBac", textBox6.Text.ToString());
                    cmd.Parameters.AddWithValue("@MaDV", textBox3.Text.ToString());
                    cmd.Parameters.AddWithValue("@TonTai", 1);
                    cmd.Parameters.AddWithValue("@MaCV", textBox5.Text.ToString());

                    cmd.ExecuteNonQuery();
                }
            }

            dataGridView1.DataSource = GetQN().Tables[0];
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string add = "update QUANNHAN set HoTen = @HoTen, CapBac = @CapBac, TonTai = @TonTai, MaCV = @MaCV, MaDV = @MaDV where MaQN = @MaQN";
            string connectionString = conn_string;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(add, conn))
                {
                    cmd.Parameters.AddWithValue("@MaQN", textBox7.Text.ToString());
                    cmd.Parameters.AddWithValue("@HoTen", textBox11.Text.ToString());
                    cmd.Parameters.AddWithValue("@CapBac", textBox8.Text.ToString());
                    cmd.Parameters.AddWithValue("@MaDV", textBox10.Text.ToString());
                    cmd.Parameters.AddWithValue("@TonTai", textBox13.Text.ToString());
                    cmd.Parameters.AddWithValue("@MaCV", textBox9.Text.ToString());

                    cmd.ExecuteNonQuery();
                }
            }

            dataGridView1.DataSource = GetQN().Tables[0];
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
            if (e.RowIndex >= 0 && e.ColumnIndex == 0)
            {
                // Retrieve the clicked cell
                textBox12.Text = dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();
                textBox7.Text = dataGridView1[e.ColumnIndex +1, e.RowIndex].Value.ToString();
                textBox11.Text = dataGridView1[e.ColumnIndex + 2, e.RowIndex].Value.ToString();
                textBox8.Text = dataGridView1[e.ColumnIndex + 3, e.RowIndex].Value.ToString();
                textBox9.Text = dataGridView1[e.ColumnIndex + 4, e.RowIndex].Value.ToString();
                textBox10.Text = dataGridView1[e.ColumnIndex + 5, e.RowIndex].Value.ToString();
                textBox13.Text = "1";
            }
        }
    }
}
