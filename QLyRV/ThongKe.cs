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
                    string chucvuQuery = "SELECT MaDV FROM DONVI where TonTai = 1 and MaDVCapTren = 'admin'";
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
                    string chucvuQuery = "SELECT MaDV FROM DONVI where MaDVCapTren ='" + Account.account + "' and TonTai = 1";
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
                comboBox1.Hide();
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

        private void Admin()
        {
            DataSet data1 = new DataSet();
            DataSet data2 = new DataSet();

            if(checkBox1.Checked)
            {
                string query = " SELECT distinct(nk.STT), qn.MaQN, qn.HoTen, qn.MaDV, nk.ThoiGianRa, nk.ThoiGianVao, nk.MaVP from NHATKYQN nk, DANHSACH ds, QUANNHAN qn where nk.ThoiGianVao = @tg1 and ds.STT = nk.STT_DS and  qn.MaQN = ds.MaQN";
                string query1 = "select distinct(nk.MaGhiNhan), qn.MaQN, qn.HoTen, qn.MaDV, nk.ThoiGianRa, nk.ThoiGianVao from NHATKYTN nk, QUANNHAN qn where nk.ThoiGianVao = @tg1 and nk.MaQN = qn.MaQN";

                string connectionString = conn_string;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@tg1", dateTimePicker1.Value.ToString());
                        //adapter.SelectCommand.Parameters.AddWithValue("@tg2", dateTimePicker2.Value.ToString());
                        // Fill the DataSet
                        adapter.Fill(data1);
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query1, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@tg1", dateTimePicker1.Value.ToString());
                        //adapter.SelectCommand.Parameters.AddWithValue("@tg2", dateTimePicker2.Value.ToString());
                        // Fill the DataSet
                        adapter.Fill(data2);
                    }
                }

                dataGridView1.DataSource = data1.Tables[0];
                dataGridView2.DataSource = data2.Tables[0];
            }
            else
            {
                string query = " SELECT distinct(nk.STT), qn.MaQN, qn.HoTen, qn.MaDV, nk.ThoiGianRa, nk.ThoiGianVao, nk.MaVP from NHATKYQN nk, DANHSACH ds, QUANNHAN qn where nk.ThoiGianVao >= @tg1 and nk.ThoiGianVao <= @tg2 and qn.MaQn = ds.MaQN and ds.STT = nk.STT_DS";
                string query1 = " SELECT distinct(nk.MaGhiNhan), qn.MaQN, qn.HoTen, qn.MaDV, nk.ThoiGianRa, nk.ThoiGianVao from NHATKYTN nk, QUANNHAN qn where nk.ThoiGianVao >= @tg1 and nk.ThoiGianVao <= @tg2 and qn.MaQn = nk.MaQN ";

                string connectionString = conn_string;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@tg1", dateTimePicker1.Value.ToString());
                        adapter.SelectCommand.Parameters.AddWithValue("@tg2", dateTimePicker2.Value.ToString());
                        // Fill the DataSet
                        adapter.Fill(data1);
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query1, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@tg1", dateTimePicker1.Value.ToString());
                        adapter.SelectCommand.Parameters.AddWithValue("@tg2", dateTimePicker2.Value.ToString());
                        // Fill the DataSet
                        adapter.Fill(data2);
                    }

                }

                dataGridView1.DataSource = data1.Tables[0];
                dataGridView2.DataSource= data2.Tables[0];
            }

            textBox1.Text = dataGridView1.RowCount.ToString();
            textBox2.Text = dataGridView2.RowCount.ToString();

            int cnt = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Kiểm tra nếu hàng không phải là hàng trống (khi AllowUserToAddRows = true)
                if (!row.IsNewRow)
                {
                    // Lấy giá trị của ô trong cột "Status"
                    object cellValue = row.Cells["MaVP"].Value;

                    // Kiểm tra điều kiện
                    if (cellValue != null)
                    {
                        cnt++;
                    }
                }
            }
            textBox3.Text = cnt.ToString();
        }

        private void TieuDoan()
        {
            DataSet data1 = new DataSet();
            DataSet data2 = new DataSet();

            if (checkBox1.Checked)
            {
                string query = " SELECT distinct(nk.STT), qn.MaQN, qn.HoTen, qn.MaDV, nk.ThoiGianRa, nk.ThoiGianVao, nk.MaVP from NHATKYQN nk, DANHSACH ds, QUANNHAN qn, DONVI dv where nk.ThoiGianVao = @tg1 and ds.STT = nk.STT_DS and  qn.MaQN = ds.MaQN and qn.MaDV = dv.MaDV and dv.MaDVCapTren = @dv ";
                string query1 = "select distinct(nk.MaGhiNhan), qn.MaQN, qn.HoTen, qn.MaDV, nk.ThoiGianRa, nk.ThoiGianVao from NHATKYTN nk, QUANNHAN qn, DONVI dv where nk.ThoiGianVao = @tg1 and nk.MaQN = qn.MaQN and qn.MaDV = dv.MaDV and dv.MaDVCapTren = @dv";

                string connectionString = conn_string;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@tg1", dateTimePicker1.Value.ToString());
                        //adapter.SelectCommand.Parameters.AddWithValue("@tg2", dateTimePicker2.Value.ToString());
                        adapter.SelectCommand.Parameters.AddWithValue("@dv", Account.account);
                        // Fill the DataSet
                        adapter.Fill(data1);
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query1, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@tg1", dateTimePicker1.Value.ToString());
                        //adapter.SelectCommand.Parameters.AddWithValue("@tg2", dateTimePicker2.Value.ToString());
                        adapter.SelectCommand.Parameters.AddWithValue("@dv", Account.account);
                        // Fill the DataSet
                        adapter.Fill(data2);
                    }
                }

                dataGridView1.DataSource = data1.Tables[0];
                dataGridView2.DataSource = data2.Tables[0];
            }
            else
            {
                string query = " SELECT distinct(nk.STT), qn.MaQN, qn.HoTen, qn.MaDV, nk.ThoiGianRa, nk.ThoiGianVao, nk.MaVP from NHATKYQN nk, DANHSACH ds, QUANNHAN qn, DONVI dv where nk.ThoiGianVao >= @tg1 and nk.ThoiGianVao <= @tg2 and qn.MaQN = ds.MaQN and ds.STT = nk.STT_DS and qn.MaDV = dv.MaDV and dv.MaDVCapTren = @dv";
                string query1 = " SELECT distinct(nk.MaGhiNhan), qn.MaQN, qn.HoTen, qn.MaDV, nk.ThoiGianRa, nk.ThoiGianVao from NHATKYTN nk, QUANNHAN qn, DONVI dv where nk.ThoiGianVao >= @tg1 and nk.ThoiGianVao <= @tg2 and qn.MaQN = nk.MaQN and qn.MaDV = dv.MaDV and dv.MaDVCapTren = @dv";

                string connectionString = conn_string;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@tg1", dateTimePicker1.Value.ToString());
                        adapter.SelectCommand.Parameters.AddWithValue("@tg2", dateTimePicker2.Value.ToString());
                        adapter.SelectCommand.Parameters.AddWithValue("@dv", Account.account);
                        // Fill the DataSet
                        adapter.Fill(data1);
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query1, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@tg1", dateTimePicker1.Value.ToString());
                        adapter.SelectCommand.Parameters.AddWithValue("@tg2", dateTimePicker2.Value.ToString());
                        adapter.SelectCommand.Parameters.AddWithValue("@dv", Account.account);
                        // Fill the DataSet
                        adapter.Fill(data2);
                    }

                }
            }
            textBox1.Text = dataGridView1.RowCount.ToString();
            textBox2.Text = dataGridView2.RowCount.ToString();

            int cnt = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Kiểm tra nếu hàng không phải là hàng trống (khi AllowUserToAddRows = true)
                if (!row.IsNewRow)
                {
                    // Lấy giá trị của ô trong cột "Status"
                    object cellValue = row.Cells["MaVP"].Value;

                    // Kiểm tra điều kiện
                    if (cellValue != null)
                    {
                        cnt++;
                    }
                }
            }
            textBox3.Text = cnt.ToString();
        }

        private void DaiDoi()
        {
            DataSet data1 = new DataSet();
            DataSet data2 = new DataSet();

            if (checkBox1.Checked)
            {
                string query = " SELECT distinct(nk.STT), qn.MaQN, qn.HoTen, qn.MaDV, nk.ThoiGianRa, nk.ThoiGianVao, nk.MaVP from NHATKYQN nk, DANHSACH ds, QUANNHAN qn, DONVI dv where nk.ThoiGianVao = @tg1 and ds.STT = nk.STT_DS and  qn.MaQN = ds.MaQN and qn.MaDV = @dv";
                string query1 = "select distinct(nk.MaGhiNhan), qn.MaQN, qn.HoTen, qn.MaDV, nk.ThoiGianRa, nk.ThoiGianVao from NHATKYTN nk, QUANNHAN qn, DONVI dv where nk.ThoiGianVao = @tg1 and nk.MaQN = qn.MaQN and qn.MaDV = @dv";

                string connectionString = conn_string;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@tg1", dateTimePicker1.Value.ToString());
                        //adapter.SelectCommand.Parameters.AddWithValue("@tg2", dateTimePicker2.Value.ToString());
                        adapter.SelectCommand.Parameters.AddWithValue("@dv", Account.account);
                        // Fill the DataSet
                        adapter.Fill(data1);
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query1, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@tg1", dateTimePicker1.Value.ToString());
                        //adapter.SelectCommand.Parameters.AddWithValue("@tg2", dateTimePicker2.Value.ToString());
                        adapter.SelectCommand.Parameters.AddWithValue("@dv", Account.account);
                        // Fill the DataSet
                        adapter.Fill(data2);
                    }
                }

                dataGridView1.DataSource = data1.Tables[0];
                dataGridView2.DataSource = data2.Tables[0];
            }
            else
            {
                string query = " SELECT distinct(nk.STT), qn.MaQN, qn.HoTen, qn.MaDV, nk.ThoiGianRa, nk.ThoiGianVao, nk.MaVP from NHATKYQN nk, DANHSACH ds, QUANNHAN qn, DONVI dv where nk.ThoiGianVao >= @tg1 and nk.ThoiGianVao <= @tg2 and qn.MaQN = ds.MaQN and ds.STT = nk.STT_DS and qn.MaDV = @dv";
                string query1 = " SELECT distinct(nk.MaGhiNhan), qn.MaQN, qn.HoTen, qn.MaDV, nk.ThoiGianRa, nk.ThoiGianVao from NHATKYTN nk, QUANNHAN qn, DONVI dv where nk.ThoiGianVao >= @tg1 and nk.ThoiGianVao <= @tg2 and qn.MaQN = nk.MaQN and qn.MaDV = @dv";

                string connectionString = conn_string;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@tg1", dateTimePicker1.Value.ToString());
                        adapter.SelectCommand.Parameters.AddWithValue("@tg2", dateTimePicker2.Value.ToString());
                        adapter.SelectCommand.Parameters.AddWithValue("@dv", Account.account);
                        // Fill the DataSet
                        adapter.Fill(data1);
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query1, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@tg1", dateTimePicker1.Value.ToString());
                        adapter.SelectCommand.Parameters.AddWithValue("@tg2", dateTimePicker2.Value.ToString());
                        adapter.SelectCommand.Parameters.AddWithValue("@dv", Account.account);
                        // Fill the DataSet
                        adapter.Fill(data2);
                    }

                }

                dataGridView1.DataSource = data1.Tables[0];
                dataGridView2.DataSource = data2.Tables[0];
            }

            textBox1.Text = dataGridView1.RowCount.ToString();
            textBox2.Text = dataGridView2.RowCount.ToString();

            int cnt = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Kiểm tra nếu hàng không phải là hàng trống (khi AllowUserToAddRows = true)
                if (!row.IsNewRow)
                {
                    // Lấy giá trị của ô trong cột "Status"
                    object cellValue = row.Cells["MaVP"].Value;

                    // Kiểm tra điều kiện
                    if (cellValue != null)
                    {
                        cnt++;
                    }
                }
            }
            textBox3.Text = cnt.ToString();
        }

        private void button_Click(object sender, EventArgs e)
        {
            if(Success.Type == 0)
            {
                Admin();
            }
            else if(Success.Type == 1)
            {
                TieuDoan();
            }
            else
            {
                DaiDoi();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Success.Type == 0)
            {
                DataSet data1 = new DataSet();
                DataSet data2 = new DataSet();

                if (checkBox1.Checked)
                {
                    string query = " SELECT distinct(nk.STT), qn.MaQN, qn.HoTen, qn.MaDV, nk.ThoiGianRa, nk.ThoiGianVao, nk.MaVP from NHATKYQN nk, DANHSACH ds, QUANNHAN qn, DONVI dv where nk.ThoiGianVao = @tg1 and ds.STT = nk.STT_DS and  qn.MaQN = ds.MaQN and qn.MaDV = dv.MaDV and dv.MaDVCapTren = @dv ";
                    string query1 = "select distinct(nk.MaGhiNhan), qn.MaQN, qn.HoTen, qn.MaDV, nk.ThoiGianRa, nk.ThoiGianVao from NHATKYTN nk, QUANNHAN qn, DONVI dv where nk.ThoiGianVao = @tg1 and nk.MaQN = qn.MaQN and qn.MaDV = dv.MaDV and dv.MaDVCapTren = @dv";

                    string connectionString = conn_string;
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                        {
                            adapter.SelectCommand.Parameters.AddWithValue("@tg1", dateTimePicker1.Value.ToString());
                            //adapter.SelectCommand.Parameters.AddWithValue("@tg2", dateTimePicker2.Value.ToString());
                            adapter.SelectCommand.Parameters.AddWithValue("@dv", comboBox1.SelectedItem.ToString());
                            // Fill the DataSet
                            adapter.Fill(data1);
                        }
                        using (SqlDataAdapter adapter = new SqlDataAdapter(query1, conn))
                        {
                            adapter.SelectCommand.Parameters.AddWithValue("@tg1", dateTimePicker1.Value.ToString());
                            //adapter.SelectCommand.Parameters.AddWithValue("@tg2", dateTimePicker2.Value.ToString());
                            adapter.SelectCommand.Parameters.AddWithValue("@dv", comboBox1.SelectedItem.ToString());
                            // Fill the DataSet
                            adapter.Fill(data2);
                        }
                    }

                    dataGridView1.DataSource = data1.Tables[0];
                    dataGridView2.DataSource = data2.Tables[0];
                }
                else
                {
                    string query = " SELECT distinct(nk.STT), qn.MaQN, qn.HoTen, qn.MaDV, nk.ThoiGianRa, nk.ThoiGianVao, nk.MaVP from NHATKYQN nk, DANHSACH ds, QUANNHAN qn, DONVI dv where nk.ThoiGianVao >= @tg1 and nk.ThoiGianVao <= @tg2 and qn.MaQN = ds.MaQN and ds.STT = nk.STT_DS and qn.MaDV = dv.MaDV and dv.MaDVCapTren = @dv";
                    string query1 = " SELECT distinct(nk.MaGhiNhan), qn.MaQN, qn.HoTen, qn.MaDV, nk.ThoiGianRa, nk.ThoiGianVao from NHATKYTN nk, QUANNHAN qn, DONVI dv where nk.ThoiGianVao >= @tg1 and nk.ThoiGianVao <= @tg2 and qn.MaQN = nk.MaQN and qn.MaDV = dv.MaDV and dv.MaDVCapTren = @dv";

                    string connectionString = conn_string;
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                        {
                            adapter.SelectCommand.Parameters.AddWithValue("@tg1", dateTimePicker1.Value.ToString());
                            adapter.SelectCommand.Parameters.AddWithValue("@tg2", dateTimePicker2.Value.ToString());
                            adapter.SelectCommand.Parameters.AddWithValue("@dv", comboBox1.SelectedItem.ToString());
                            // Fill the DataSet
                            adapter.Fill(data1);
                        }
                        using (SqlDataAdapter adapter = new SqlDataAdapter(query1, conn))
                        {
                            adapter.SelectCommand.Parameters.AddWithValue("@tg1", dateTimePicker1.Value.ToString());
                            adapter.SelectCommand.Parameters.AddWithValue("@tg2", dateTimePicker2.Value.ToString());
                            adapter.SelectCommand.Parameters.AddWithValue("@dv", comboBox1.SelectedItem.ToString());
                            // Fill the DataSet
                            adapter.Fill(data2);
                        }

                    }
                }
                textBox1.Text = dataGridView1.RowCount.ToString();
                textBox2.Text = dataGridView2.RowCount.ToString();

                int cnt = 0;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    // Kiểm tra nếu hàng không phải là hàng trống (khi AllowUserToAddRows = true)
                    if (!row.IsNewRow)
                    {
                        // Lấy giá trị của ô trong cột "Status"
                        object cellValue = row.Cells["MaVP"].Value;

                        // Kiểm tra điều kiện
                        if (cellValue != null)
                        {
                            cnt++;
                        }
                    }
                }
                textBox3.Text = cnt.ToString();
            }
            else
            {
                DataSet data1 = new DataSet();
                DataSet data2 = new DataSet();

                if (checkBox1.Checked)
                {
                    string query = " SELECT distinct(nk.STT), qn.MaQN, qn.HoTen, qn.MaDV, nk.ThoiGianRa, nk.ThoiGianVao,nk.MaVP from NHATKYQN nk, DANHSACH ds, QUANNHAN qn, DONVI dv where nk.ThoiGianVao = @tg1 and ds.STT = nk.STT_DS and  qn.MaQN = ds.MaQN and qn.MaDV = @dv";
                    string query1 = "select distinct(nk.MaGhiNhan), qn.MaQN, qn.HoTen, qn.MaDV, nk.ThoiGianRa, nk.ThoiGianVao from NHATKYTN nk, QUANNHAN qn, DONVI dv where nk.ThoiGianVao = @tg1 and nk.MaQN = qn.MaQN and qn.MaDV = @dv";

                    string connectionString = conn_string;
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                        {
                            adapter.SelectCommand.Parameters.AddWithValue("@tg1", dateTimePicker1.Value.ToString());
                            //adapter.SelectCommand.Parameters.AddWithValue("@tg2", dateTimePicker2.Value.ToString());
                            adapter.SelectCommand.Parameters.AddWithValue("@dv", comboBox1.SelectedItem.ToString());
                            // Fill the DataSet
                            adapter.Fill(data1);
                        }
                        using (SqlDataAdapter adapter = new SqlDataAdapter(query1, conn))
                        {
                            adapter.SelectCommand.Parameters.AddWithValue("@tg1", dateTimePicker1.Value.ToString());
                            //adapter.SelectCommand.Parameters.AddWithValue("@tg2", dateTimePicker2.Value.ToString());
                            adapter.SelectCommand.Parameters.AddWithValue("@dv", comboBox1.SelectedItem.ToString());
                            // Fill the DataSet
                            adapter.Fill(data2);
                        }
                    }

                    dataGridView1.DataSource = data1.Tables[0];
                    dataGridView2.DataSource = data2.Tables[0];
                }
                else
                {
                    string query = " SELECT distinct(nk.STT), qn.MaQN, qn.HoTen, qn.MaDV, nk.ThoiGianRa, nk.ThoiGianVao, nk.MaVP from NHATKYQN nk, DANHSACH ds, QUANNHAN qn, DONVI dv where nk.ThoiGianVao >= @tg1 and nk.ThoiGianVao <= @tg2 and qn.MaQN = ds.MaQN and ds.STT = nk.STT_DS and qn.MaDV = @dv";
                    string query1 = " SELECT distinct(nk.MaGhiNhan), qn.MaQN, qn.HoTen, qn.MaDV, nk.ThoiGianRa, nk.ThoiGianVao from NHATKYTN nk, QUANNHAN qn, DONVI dv where nk.ThoiGianVao >= @tg1 and nk.ThoiGianVao <= @tg2 and qn.MaQN = nk.MaQN and qn.MaDV = @dv";

                    string connectionString = conn_string;
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                        {
                            adapter.SelectCommand.Parameters.AddWithValue("@tg1", dateTimePicker1.Value.ToString());
                            adapter.SelectCommand.Parameters.AddWithValue("@tg2", dateTimePicker2.Value.ToString());
                            adapter.SelectCommand.Parameters.AddWithValue("@dv", comboBox1.SelectedItem.ToString());
                            // Fill the DataSet
                            adapter.Fill(data1);
                        }
                        using (SqlDataAdapter adapter = new SqlDataAdapter(query1, conn))
                        {
                            adapter.SelectCommand.Parameters.AddWithValue("@tg1", dateTimePicker1.Value.ToString());
                            adapter.SelectCommand.Parameters.AddWithValue("@tg2", dateTimePicker2.Value.ToString());
                            adapter.SelectCommand.Parameters.AddWithValue("@dv", comboBox1.SelectedItem.ToString());
                            // Fill the DataSet
                            adapter.Fill(data2);
                        }

                    }

                    dataGridView1.DataSource = data1.Tables[0];
                    dataGridView2.DataSource = data2.Tables[0];
                }

                textBox1.Text = dataGridView1.RowCount.ToString();
                textBox2.Text = dataGridView2.RowCount.ToString();

                int cnt = 0;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    // Kiểm tra nếu hàng không phải là hàng trống (khi AllowUserToAddRows = true)
                    if (!row.IsNewRow)
                    {
                        // Lấy giá trị của ô trong cột "Status"
                        object cellValue = row.Cells["MaVP"].Value;

                        // Kiểm tra điều kiện
                        if (cellValue != null)
                        {
                            cnt++;
                        }
                    }
                }
                textBox3.Text = cnt.ToString();
            }
        }
    }
}
