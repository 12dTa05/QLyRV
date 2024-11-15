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
using static QLyRV.Success;
using static QLyRV.Database;
using static QLyRV.Account;
using System.Web.UI.WebControls;

namespace QLyRV
{
    public partial class DuyetDK : Form
    {
        public DuyetDK()
        {
            InitializeComponent();
        }

        private void DuyetDK_Load(object sender, EventArgs e)
        {
            //dataGridView1.DataSource = GetDuyet().Tables[0];
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
                string chucvuQuery = "SELECT MaDV FROM DONVI where MaDVCapTren ='" + Account.account +"' and TonTai = 1";
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
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 0)
            {
                // Retrieve the clicked cell
                textBox1.Text = dataGridView1[e.ColumnIndex + 1, e.RowIndex].Value.ToString();
                textBox2.Text = dataGridView1[e.ColumnIndex + 2, e.RowIndex].Value.ToString();
                textBox3.Text = dataGridView1[e.ColumnIndex + 3, e.RowIndex].Value.ToString();
                textBox4.Text = dataGridView1[e.ColumnIndex + 4, e.RowIndex].Value.ToString();
                textBox5.Text = dataGridView1[e.ColumnIndex + 5, e.RowIndex].Value.ToString();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(Success.Type != 0)
            {
                dataGridView1.DataSource = GetDuyet().Tables[0];
            }
            else
            {
                DataSet data = new DataSet();
                DateTime selectedDate = dateTimePicker1.Value.Date; // Only date component

                // Define the start and end of the day
                DateTime startDate = selectedDate;             // Start of selected date at 00:00
                DateTime endDate = selectedDate.AddDays(2);    // Start of the next day at 00:00

                string query = " SELECT ds.MaQN, qn.HoTen, qn.MaDV, ds.HinhThucRV, ds.LyDo, ds.DiaDiem, ds.ThoiGianRa, ds.ThoiGianVao FROM DANHSACH ds, QUANNHAN qn, DONVI dv WHERE  ds.ThoiGianRa > @StartDate AND ds.ThoiGianRa < @EndDate and ds.MaQN = qn.MaQN and qn.MaDV = dv.MaDV and dv.MaDVCapTren = @DonVi";

                string connectionString = conn_string;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        // Set the parameters for the date range
                        adapter.SelectCommand.Parameters.AddWithValue("@StartDate", startDate);
                        adapter.SelectCommand.Parameters.AddWithValue("@EndDate", endDate);
                        adapter.SelectCommand.Parameters.AddWithValue("@DonVi", comboBox1.SelectedItem.ToString());

                        // Fill the DataSet
                        adapter.Fill(data);
                    }
                }
                dataGridView1.DataSource = data.Tables[0];
            }
        }

        DataSet GetDuyet()
        {

            DataSet data = new DataSet();
            DateTime selectedDate = dateTimePicker1.Value.Date; // Only date component

            // Define the start and end of the day
            DateTime startDate = selectedDate;             // Start of selected date at 00:00
            DateTime endDate = selectedDate.AddDays(2);    // Start of the next day at 00:00

            string query = " SELECT ds.MaQN, qn.HoTen, qn.MaDV, ds.HinhThucRV, ds.LyDo, ds.DiaDiem, ds.ThoiGianRa, ds.ThoiGianVao FROM DANHSACH ds, QUANNHAN qn WHERE qn.MaDV = @DonVi and qn.MaQN = ds.MaQN and ds.ThoiGianRa > @StartDate AND ds.ThoiGianRa < @EndDate";

            string connectionString = conn_string;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    // Set the parameters for the date range
                    adapter.SelectCommand.Parameters.AddWithValue("@StartDate", startDate);
                    adapter.SelectCommand.Parameters.AddWithValue("@EndDate", endDate);
                    adapter.SelectCommand.Parameters.AddWithValue("@DonVi", comboBox1.SelectedItem.ToString());

                    // Fill the DataSet
                    adapter.Fill(data);
                }
            }

            return data;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string chucvuQuery = "delete from DANHSACH where MaQN = @MaQN";
            string connectionString = conn_string;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // First query to get ChucVu
                using (SqlCommand cmd = new SqlCommand(chucvuQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@MaQN", textBox1.Text);
                }
            }
            dataGridView1.DataSource = GetDuyet().Tables[0];
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedItem != null)
            {
                if (Success.Type != 0)
                {
                    dataGridView1.DataSource = GetDuyet().Tables[0];
                }
                else
                {
                    DataSet data = new DataSet();
                    DateTime selectedDate = dateTimePicker1.Value.Date; // Only date component

                    // Define the start and end of the day
                    DateTime startDate = selectedDate;             // Start of selected date at 00:00
                    DateTime endDate = selectedDate.AddDays(2);    // Start of the next day at 00:00

                    string query = " SELECT ds.MaQN, qn.HoTen, qn.MaDV, ds.HinhThucRV, ds.LyDo, ds.DiaDiem, ds.ThoiGianRa, ds.ThoiGianVao FROM DANHSACH ds, QUANNHAN qn, DONVI dv WHERE  ds.ThoiGianRa > @StartDate AND ds.ThoiGianRa < @EndDate and ds.MaQN = qn.MaQN and qn.MaDV = dv.MaDV and dv.MaDVCapTren = @DonVi";

                    string connectionString = conn_string;
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                        {
                            // Set the parameters for the date range
                            adapter.SelectCommand.Parameters.AddWithValue("@StartDate", startDate);
                            adapter.SelectCommand.Parameters.AddWithValue("@EndDate", endDate);
                            adapter.SelectCommand.Parameters.AddWithValue("@DonVi", comboBox1.SelectedItem.ToString());

                            // Fill the DataSet
                            adapter.Fill(data);
                        }
                    }
                    dataGridView1.DataSource = data.Tables[0];
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Đã duyệt");
        }
    }
}
