using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static QLyRV.Database;
using static QLyRV.Account;

namespace QLyRV
{
    public partial class DangKi : Form
    {
        public DangKi()
        {
            InitializeComponent();
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy-MM-d";

            dateTimePicker2.MinDate = DateTime.Now;
            dateTimePicker3.MinDate = DateTime.Now;

            dateTimePicker1.Value = DateTime.Now;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = GetNhatKy().Tables[0];
        }

        DataSet GetNhatKy()
        {

            DataSet data = new DataSet();
            DateTime selectedDate = dateTimePicker1.Value.Date; // Only date component

            // Define the start and end of the day
            DateTime startDate = selectedDate;             // Start of selected date at 00:00
            DateTime endDate = selectedDate.AddDays(1);    // Start of the next day at 00:00

            string query = " SELECT MaQN, HinhThucRN, LyDo, DiaDiem, ThoiGianRa, ThoiGianVao, NguoiSua, ThoiGianSua FROM DANHSACH WHERE ThoiGianRa > @StartDate AND ThoiGianRa < @EndDate";

            string connectionString = conn_string;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    // Set the parameters for the date range
                    adapter.SelectCommand.Parameters.AddWithValue("@StartDate", startDate);
                    adapter.SelectCommand.Parameters.AddWithValue("@EndDate", endDate);

                    // Fill the DataSet
                    adapter.Fill(data);
                }
            }

            return data;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string add = "insert into DANHSACH values @Hinhthuc, @Lido, @DiaDiem, @TgianRa, @TgianVao, , , , @NguoiSua, @ThoigianSua, @MaQN";
            string connectionString = conn_string;
            using(SqlConnection conn = new SqlConnection( connectionString ))
            {
                conn.Open();
                using(SqlCommand cmd = new SqlCommand(add, conn))
                {
                    cmd.Parameters.AddWithValue("@HinhThuc", comboBox1.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@Lido", textBox2.Text.ToString());
                    cmd.Parameters.AddWithValue("@DiaDiem", textBox3.Text.ToString());
                    cmd.Parameters.AddWithValue("@TgianRa", dateTimePicker2.Value.ToString() + ' ' + textBox4.Text);
                    cmd.Parameters.AddWithValue("@TgianVao", dateTimePicker3.Value.ToString() + ' ' + textBox5.Text);
                    cmd.Parameters.AddWithValue("@NguoiSua", Account.account);
                    cmd.Parameters.AddWithValue("@ThoiGianSua", DateTime.Now.ToString());
                    cmd.Parameters.AddWithValue("@MaQN", textBox1.Text);

                    cmd.ExecuteNonQuery();
                }
            }

            dataGridView1.DataSource = GetNhatKy().Tables[0];
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }
    }
}
