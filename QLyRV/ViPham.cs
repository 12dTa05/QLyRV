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
using static QLyRV.Camera;
using static QLyRV.Database;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace QLyRV
{
    public partial class ViPham : Form
    {
        private int count;
        public ViPham()
        {
            InitializeComponent();
        }

        private void ViPham_Load(object sender, EventArgs e)
        {
            textBox1.Text = Camera.cccd;
            textBox2.Text = Camera.ht;
            textBox3.Text = Camera.dv;
            textBox6.Text = (Count() + 1).ToString();

            dataGridView1.DataSource = GetVP().Tables[0];
        }

        private int Count()
        {
            string c = "SELECT MaVP FROM VIPHAM";
            string connectionString = conn_string;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // First query to get ChucVu
                using (SqlCommand chucvuCommand = new SqlCommand(c, conn))
                {
                    using (SqlDataReader reader = chucvuCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            count = Int32.Parse(reader.GetString(0));
                        }

                    }
                }
            }
            return count;
        }

        DataSet GetVP()
        {

            DataSet data = new DataSet();

            string query = " SELECT qn.MaQN, qn.HoTen, vp.MoTa, vp.GhiChu, vp.ThoiGian from VIPHAM vp, QUANNHAN qn where vp.MaQN = qn.MaQN and vp.ThoiGian = @tg";

            string connectionString = conn_string;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@tg", dateTimePicker1.Value.ToString());
                    // Fill the DataSet
                    adapter.Fill(data);
                }
            }

            return data;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            count = Count();
            string connectionString = conn_string;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                if (Camera.type == 0)
                {
                    string add = "insert into VIPHAM values @MoTa, , @tg, @ghichu, @NguoiSua, @MaQN \n" + "update RANGOAI set MaVP = @c, Khoa = 1, ThoiGianVao = @tg, ThoiGianSua = @tg where Ma = @MaQN";
                    using (SqlCommand cmd = new SqlCommand(add, conn))
                    {
                        cmd.Parameters.AddWithValue("@MoTa", textBox4.Text.ToString());
                        cmd.Parameters.AddWithValue("@tg", DateTime.Now.ToString());
                        cmd.Parameters.AddWithValue("@ghichu", textBox5.Text.ToString());
                        cmd.Parameters.AddWithValue("@NguoiSua", "VB");
                        cmd.Parameters.AddWithValue("@MaQN", textBox1.Text.ToString());
                        cmd.Parameters.AddWithValue("@c", count);

                        cmd.ExecuteNonQuery();
                    }
                }
                else if(Camera.type == 1)
                {
                    string one = "select CCCD_QuanNhan from GHINHANTHAM where CCCD_QuanNhan = @cccd and ThoiGianVao = @t";
                    using (SqlCommand cmd = new SqlCommand(one, conn))                     
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read() && reader.GetString(reader.GetOrdinal("CCCD_QuanNhan")).ToString() != "")
                            {
                                string add = "insert into VIPHAM values @MoTa, , @tg, @ghichu, @NguoiSua, @MaQN \n" + "insert into GHINHANTHAM values @cccdQuan, @htQuan, @cccdDan, @htDan, @tg, , @ghichu, 0, @NguoiSua, @tg, @c";
                                using(SqlCommand cmd1 = new SqlCommand(add, conn))
                                {
                                    cmd.Parameters.AddWithValue("@MoTa", textBox4.Text.ToString());
                                    cmd.Parameters.AddWithValue("@tg", DateTime.Now.ToString());
                                    cmd.Parameters.AddWithValue("@ghichu", textBox5.Text.ToString());
                                    cmd.Parameters.AddWithValue("@NguoiSua", "VB");
                                    cmd.Parameters.AddWithValue("@MaQN", textBox1.Text.ToString());
                                    cmd.Parameters.AddWithValue("@c", count);
                                    cmd.Parameters.AddWithValue("@cccdQuan", textBox1.Text.ToString());
                                    cmd.Parameters.AddWithValue("@htQuan", Camera.ht);
                                    cmd.Parameters.AddWithValue("@cccdDan", Camera.cccdDan);
                                    cmd.Parameters.AddWithValue("@htDan", Camera.htDan);

                                    cmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                string add = "insert into VIPHAM values @MoTa, , @tg, @ghichu, @NguoiSua, @MaQN \n" + "update GHINHANTHAM set MaVP = @c, Khoa = 1, ThoiGianRa = @tg, ThoiGianSua = @tg where CCCD_QuanNhan = @MaQN and CCCD_ThanNhan = @cccdDan";
                                using (SqlCommand cmd1 = new SqlCommand(add, conn))
                                {
                                    cmd.Parameters.AddWithValue("@MoTa", textBox4.Text.ToString());
                                    cmd.Parameters.AddWithValue("@tg", DateTime.Now.ToString());
                                    cmd.Parameters.AddWithValue("@ghichu", textBox5.Text.ToString());
                                    cmd.Parameters.AddWithValue("@NguoiSua", "VB");
                                    cmd.Parameters.AddWithValue("@MaQN", textBox1.Text.ToString());
                                    cmd.Parameters.AddWithValue("@c", count);
                                    cmd.Parameters.AddWithValue("@cccdDan", Camera.cccdDan);

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }  
                    }   
                }
                else
                {
                    string add = "insert into VIPHAM values @MoTa, , @tg, @ghichu, @NguoiSua, @MaQN \n" + "insert into RANGOAI values @MaQN, , @TgianV, , 0, @ns, @TgianS, @vp, , ";
                    using (SqlCommand cmd = new SqlCommand(add, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaQN", textBox1.Text.ToString());
                        cmd.Parameters.AddWithValue("@TgianV", DateTime.Now.ToString());
                        cmd.Parameters.AddWithValue("@ns", "VB");
                        cmd.Parameters.AddWithValue("@TgianS", DateTime.Now.ToString());
                        cmd.Parameters.AddWithValue("@vp", count);

                        cmd.ExecuteNonQuery();
                    }
                }
                
                
            }

            dataGridView1.DataSource = GetVP().Tables[0];
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
        }
    }
}
