using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using Aspose.BarCode.BarCodeRecognition;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Xml.Linq;
using System.Data.SqlClient;
using static QLyRV.Database;
using static QLyRV.Account;
using static QLyRV.AdminForm;
using System.Web;

namespace QLyRV
{
    public partial class Camera : Form
    {
        private FilterInfoCollection cameras;
        private VideoCaptureDevice camera;
        public static int type;
        private String MaGT, MaDS;

        public static string maQN, cccd, dv, ht, cv, cccdDan, htDan;

        private String date = DateTime.Now.ToString("yyyy-MM-dd :g");
        public Camera()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            

            cameras = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo info in cameras)
            {
                comboBox1.Items.Add(info.Name);
            }
            comboBox1.SelectedIndex = 0;
        }

        private void Camera_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            camera = new VideoCaptureDevice(cameras[comboBox1.SelectedIndex].MonikerString);
            camera.NewFrame += new NewFrameEventHandler(Video_NewFrame);
            camera.Start();
        }

        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Capture the frame as a Bitmap
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            StopCam();
            // Process the frame to read a barcode
            if(tabControl1.SelectedIndex == 0)
            {
                ReadQR(bitmap);
            }
            else
            {
                ReadDan(bitmap);
            }

            // Dispose of the bitmap to free up memory
            bitmap.Dispose();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {

        }

        private void ReadDan(Bitmap bitmap)
        {
            var readDan = new BarcodeReader();
            var result = readDan.Decode(bitmap);
            if (result != null)
            {
                StopCam();
                if (result != null)
                {
                    StopCam();
                    string[] parts = result.Text.Split('|');

                    if (parts.Length >= 0)
                    {

                        if(textBox4.Text == "")
                        {
                            textBox4.Text = parts[4];
                            textBox3.Text = parts[0];

                            cccd = textBox4.Text;
                            ht = textBox3.Text;

                            string quan = "select qn.MaQN, qn.HoTen, dv.TenDV from DONVI dv, QUANNHAN qn where qn.CCCD = @cccd and qn.MaDV = dv.MaDV";
                            string connectionString = conn_string;

                            using (SqlConnection conn = new SqlConnection(connectionString))
                            {
                                conn.Open();
                                using (SqlCommand cmd = new SqlCommand(quan, conn))
                                {
                                    cmd.Parameters.AddWithValue("@cccd", cccd);

                                    using(SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        if (reader.Read())
                                        {
                                            textBox4.Text = reader["MaQN"].ToString();
                                            textBox3.Text = reader["HoTen"].ToString();
                                            textBox2.Text = reader["TenDV"].ToString();
                                            maQN = textBox4.Text;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            textBox1.Text = parts[4];
                            textBox5.Text = parts[0];

                            cccdDan = textBox1.Text;
                            htDan = textBox5.Text;
                        }
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string qn = textBox6.Text;
            string quan = "select qn.CCCD, qn.HoTen, dv.TenDV from DONVI dv, QUANNHAN qn where qn.MaQN = @qn and qn.MaDV = dv.MaDV";
            string connectionString = conn_string;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(quan, conn))
                {
                    cmd.Parameters.AddWithValue("@qn", qn);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if(tabControl1.SelectedIndex == 0)
                            {
                                textBox7.Text = textBox6.Text;
                                textBox2.Text = reader["TenDV"].ToString();
                                CCCD_TEXT.Text = reader["CCCD"].ToString();
                                hoten_text.Text = reader["HoTen"].ToString();
                            }
                            else
                            {
                                //textBox2.Text = reader["TenDV"].ToString();
                                textBox4.Text = reader["CCCD"].ToString();
                                textBox3.Text = reader["HoTen"].ToString();
                            }
                        }
                    }
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string ktra = "select Khoa from NHATKYTN  where CCCD_QuanNhan = @cccd and ThoiGianVao = '" + DateTime.Now.ToString() + "'";
            
            string connectionString = conn_string;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand NK = new SqlCommand(ktra, conn))
                {
                    using (SqlDataReader reader = NK.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            
                            if (reader["Khoa"].ToString() == "")
                            {
                                string addNhatKi = "insert into NHATKYTN t values @MaQN, @cccdDan, @TenDan, @TgianV, @TgianR, , @Khoa";
                                SqlCommand add = new SqlCommand(addNhatKi, conn);
                                add.Parameters.AddWithValue("@MaQN", textBox4.Text);
                                //add.Parameters.AddWithValue("@TenQN", textBox3.Text);
                                add.Parameters.AddWithValue("@cccdDan", textBox1.Text);
                                add.Parameters.AddWithValue("@TenDan", textBox5.Text);
                                //add.Parameters.AddWithValue("@NguoiSua", "VB");
                                //add.Parameters.AddWithValue("@ThoiGianSua", DateTime.Now.ToString("yyyy - MMM - d :g"));
                                add.Parameters.AddWithValue("@Khoa", 0);
                                add.Parameters.AddWithValue("@TgianV", DateTime.Now.ToString("yyyy - MMM - d :g"));
                                add.Parameters.AddWithValue("@TgianR", "");
                                add.ExecuteNonQuery();
                            }
                            else
                            {
                                string addNhatKi = "update NHATKYTN set ThoiGianRa = @TgianR, Khoa = 1, ThoiGianSua = @TgianR where MaQN = @qn and CCCD_ThanNhan = @tn";
                                SqlCommand add = new SqlCommand(addNhatKi, conn);
                                add.Parameters.AddWithValue("@qn", textBox4.Text);
                                add.Parameters.AddWithValue("@tn", textBox1.Text);
                                add.Parameters.AddWithValue("@TgianR", DateTime.Now.ToString("yyyy - MMM - d :g"));
                                add.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void ReadQR(Bitmap bitmap)
        {
            var readQR = new BarcodeReader();
            var result = readQR.Decode(bitmap);

            if (result != null)
            {
                StopCam();
                string[] parts = result.Text.Split('|');

                if (parts.Length >= 0)
                {
                    CCCD_TEXT.Text = parts[4]; 
                    hoten_text.Text = parts[0];

                    cccd = CCCD_TEXT.Text;
                    ht = hoten_text.Text;

                    string chucvuQuery = "SELECT cv.ChucVu FROM CHUCVU cv JOIN QUANNHAN qn ON cv.MaCV = qn.MaCV WHERE qn.CCCD = @MaQN";
                    string connectionString = conn_string;

                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();

                            // First query to get ChucVu
                            using (SqlCommand chucvuCommand = new SqlCommand(chucvuQuery, conn))
                            {
                                chucvuCommand.Parameters.AddWithValue("@MaQN", CCCD_TEXT);

                                using (SqlDataReader reader = chucvuCommand.ExecuteReader())
                                {
                                    if (reader.Read() )
                                    {
                                        cv = reader["Chucvu"].ToString();

                                        if (reader["ChucVu"].ToString() == "Học viên")
                                        {
                                            reader.Close();  // Close reader before executing the second query
                                            type = 0;
                                            // Second query to get TenDV
                                            string tenDvQuery = "SELECT dv.TenDV, ds.MaDS, qn.MaQN FROM DANHSACH ds  JOIN QUANNHAN qn ON ds.MaQN = qn.MaQN JOIN DONVI dv ON dv.MaDV = qn.MaDV WHERE ds.ThoiGianRa = @ThoiGianRa AND qn.CCCD = @MaQN";
                                            using (SqlCommand tenDvCommand = new SqlCommand(tenDvQuery, conn))
                                            {
                                                tenDvCommand.Parameters.AddWithValue("@ThoiGianRa", this.date);
                                                tenDvCommand.Parameters.AddWithValue("@MaQN", CCCD_TEXT);

                                                using (SqlDataReader dvReader = tenDvCommand.ExecuteReader())
                                                {
                                                    if (dvReader.Read() && dvReader["MaDS"] != null)
                                                    {
                                                        textBox7.Text = dvReader["MaQN"].ToString();
                                                        donvi_text.Text = dvReader["TenDV"].ToString();
                                                        MaDS = dvReader["MaDS"].ToString();
                                                        //MaGT = reader["MaGT"].ToString();

                                                        dv = donvi_text.Text;
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show("Không có trong danh sách ra ngoài");
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            reader.Close();  // Close reader before executing the second query
                                            //type = 0;
                                            // Second query to get TenDV
                                            string tenDvQuery = "SELECT dv.TenDV, qn.MaQN FROM QUANNHAN qn JOIN DONVI dv ON dv.MaQN = qn.MaQN WHERE qn.CCCD = @MaQN";
                                            using (SqlCommand tenDvCommand = new SqlCommand(tenDvQuery, conn))
                                            {
                                                tenDvCommand.Parameters.AddWithValue("@MaQN", CCCD_TEXT);

                                                using (SqlDataReader dvReader = tenDvCommand.ExecuteReader())
                                                {
                                                    if (dvReader.Read())
                                                    {
                                                        donvi_text.Text = dvReader["TenDV"].ToString();
                                                        textBox7.Text = dvReader["MaQN"].ToString();

                                                        dv = donvi_text.Text;
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show("Không có dữ liệu");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không có dữ liệu"); 
                    }
                }
            }
        }

        private void StopCam()
        {
            if (camera != null && camera.IsRunning)
            {
                camera.SignalToStop();
                camera.WaitForStop();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if(tabControl1.SelectedIndex == 1)
            //{
            //    type = 1;
            //}
            AdminForm af = new AdminForm(1);
            af.Show();
            af.OpenChildForm(new ViPham());    
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string ktra = "select rn.Khoa, ds.STT from NHATKIQN, DANHSACH ds rn where rn.STT_DS = ds.STT and ds.MaQN = '" + textBox7.Text + "' and (ThoiGianRa = '" + DateTime.Now.ToString("yyyy-MM-d") + "' or ThoiGianVao = '" + DateTime.Now.ToString("yyyy-MM-d") + "')";
            string addNhatKi = "insert into NHATKIQN rn values @Khoa, @STT_DS, @ThoiGianRa, @ThoiGianVao, ,";
            string connectionString = conn_string;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand NK = new SqlCommand(ktra, conn))
                {
                    using (SqlDataReader reader = NK.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            SqlCommand add = new SqlCommand(addNhatKi, conn);
                            if (reader["Khoa"].ToString() == "")
                            {
                                if (type == 0)
                                {
                                    add.Parameters.AddWithValue("@STT_DS", reader["STT"].ToString());
                                    add.Parameters.AddWithValue("@ThoiGianRa", DateTime.Now.ToString("yyyy - MMM - d :g"));
                                    add.Parameters.AddWithValue("@ThoiGianVao", "");
                                    add.Parameters.AddWithValue("@Khoa", 0);

                                }
                                else
                                {
                                    add.Parameters.AddWithValue("@STT_DS", reader["STT"].ToString());
                                    add.Parameters.AddWithValue("@ThoiGianRa", "");
                                    add.Parameters.AddWithValue("@ThoiGianVao", DateTime.Now.ToString("yyyy - MMM - d :g"));
                                    add.Parameters.AddWithValue("@Khoa", 1);
                                }
                                add.ExecuteNonQuery();
                            }
                            else if (reader["Khoa"].ToString() == "1")
                            {
                                if (type != 0)
                                {
                                    String ch = "update NHATKIQN set Khoa = 0, ThoiGianRa = @TGR where rn.STT_DS = @STT";
                                    SqlCommand change = new SqlCommand(ch, conn);
                                    change.Parameters.AddWithValue("@TGR", DateTime.Now.ToString("yyyy - MMM - d :g"));
                                    change.Parameters.AddWithValue("@STT", reader["STT"].ToString());
                                    change.ExecuteNonQuery();
                                }
                                else
                                {
                                    add.Parameters.AddWithValue("@STT_DS", reader["STT"].ToString());
                                    add.Parameters.AddWithValue("@ThoiGianRa", DateTime.Now.ToString("yyyy - MMM - d :g"));
                                    add.Parameters.AddWithValue("@ThoiGianVao", "");
                                    add.Parameters.AddWithValue("@Khoa", 0);
                                    add.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                if (type == 0)
                                {
                                    String ch = "update NHATKIQN set Khoa = 1, ThoiGianVao = @TGV where STT_DS = @STT";
                                    SqlCommand change = new SqlCommand(ch, conn);
                                    change.Parameters.AddWithValue("@TGV", DateTime.Now.ToString("yyyy - MMM - d :g"));
                                    change.Parameters.AddWithValue("@STT", reader["STT"].ToString());
                                    change.ExecuteNonQuery();
                                }
                                else
                                {
                                    add.Parameters.AddWithValue("@STT_DS", reader["STT"].ToString());
                                    add.Parameters.AddWithValue("@ThoiGianRa", "");
                                    add.Parameters.AddWithValue("@ThoiGianVao", DateTime.Now.ToString("yyyy - MMM - d :g"));
                                    add.Parameters.AddWithValue("@Khoa", 1);
                                    add.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
