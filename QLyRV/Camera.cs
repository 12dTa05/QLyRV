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

        public static string cccd, dv, ht, cv, cccdDan, htDan;

        private String date = DateTime.Now.ToString("yyyy-MM-dd");
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

                            string quan = "select dv.TenDV from DONVI dv, QUANNHAN qn where qn.MaQN = @cccd and qn.MaDV = dv.MaDV";
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
                                            textBox2.Text = reader["TenDV"].ToString();
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

        private void button5_Click(object sender, EventArgs e)
        {
            string ktra = "select Khoa from GHINHANTHAM  where CCCD_QuanNhan = @cccd and ThoiGianVao = '" + DateTime.Now.ToString() + "'";
            
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
                                string addNhatKi = "insert into GHINHANTHAM t values @MaQN, @TenQN, @cccdDan, @TenDan, @TgianV, @TgianR, , @Khoa, @NguoiSua, @TgianSua, ";
                                SqlCommand add = new SqlCommand(addNhatKi, conn);
                                add.Parameters.AddWithValue("@MaQN", textBox4.Text);
                                add.Parameters.AddWithValue("@TenQN", textBox3.Text);
                                add.Parameters.AddWithValue("@cccdDan", textBox1.Text);
                                add.Parameters.AddWithValue("@TenDan", textBox5.Text);
                                add.Parameters.AddWithValue("@NguoiSua", "VB");
                                add.Parameters.AddWithValue("@ThoiGianSua", DateTime.Now.ToString("yyyy - MMM - d :g"));
                                add.Parameters.AddWithValue("@Khoa", 0);
                                add.Parameters.AddWithValue("@TgianV", DateTime.Now.ToString("yyyy - MMM - d :g"));
                                add.Parameters.AddWithValue("@TgianR", "");
                                add.ExecuteNonQuery();
                            }
                            else
                            {
                                string addNhatKi = "update GHINHANTHAM set ThoiGianRa = @TgianR, Khoa = 1, ThoiGianSua = @TgianR where CCCD_QuanNhan = @qn and CCCD_ThanNhan = @tn";
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

                    string chucvuQuery = "SELECT cv.ChucVu FROM CHUCVU cv JOIN QUANNHAN qn ON cv.MaCV = qn.MaCV WHERE qn.MaQN = @MaQN";
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
                                            string tenDvQuery = "SELECT dv.TenDV, ds.MaDS FROM DANHSACH ds  JOIN QUANNHAN qn ON ds.MaQN = qn.MaQN JOIN DONVI dv ON dv.MaQN = qn.MaQN WHERE ds.ThoiGianRa = @ThoiGianRa AND ds.MaQN = @MaQN";
                                            using (SqlCommand tenDvCommand = new SqlCommand(tenDvQuery, conn))
                                            {
                                                tenDvCommand.Parameters.AddWithValue("@ThoiGianRa", this.date);
                                                tenDvCommand.Parameters.AddWithValue("@MaQN", CCCD_TEXT);

                                                using (SqlDataReader dvReader = tenDvCommand.ExecuteReader())
                                                {
                                                    if (dvReader.Read())
                                                    {
                                                        donvi_text.Text = reader["TenDV"].ToString();
                                                        MaDS = reader["MaDS"].ToString();
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
                                            type = 0;
                                            // Second query to get TenDV
                                            string tenDvQuery = "SELECT dv.TenDV FROM QUANNHAN qn JOIN DONVI dv ON dv.MaQN = qn.MaQN WHERE ds.MaQN = @MaQN";
                                            using (SqlCommand tenDvCommand = new SqlCommand(tenDvQuery, conn))
                                            {
                                                tenDvCommand.Parameters.AddWithValue("@MaQN", CCCD_TEXT);

                                                using (SqlDataReader dvReader = tenDvCommand.ExecuteReader())
                                                {
                                                    if (dvReader.Read())
                                                    {
                                                        donvi_text.Text = reader["TenDV"].ToString();

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
            if(tabControl1.SelectedIndex == 1)
            {
                type = 1;
            }
            AdminForm af = new AdminForm(1);
            af.Show();
            af.OpenChildForm(new ViPham());    
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string ktra = "select rn.Khoa from RANGOAI rn where rn.MaQN = '" + CCCD_TEXT.Text + "' and (ThoiGianRa = '" + DateTime.Now.ToString() + "' or ThoiGianVao = '" + DateTime.Now.ToString() + "')";
            string addNhatKi = "insert into RANGOAI rn values @MaQN, @ThoiGianRa, @ThoiGianVao, , @Khoa, @NguoiSua, @ThoiGianSua, , @MaGT, @MaDS ";
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
                                    add.Parameters.AddWithValue("@MaQN", CCCD_TEXT.Text);
                                    add.Parameters.AddWithValue("@ThoiGianRa", DateTime.Now.ToString("yyyy - MMM - d :g"));
                                    add.Parameters.AddWithValue("@ThoiGianVao", "");
                                    add.Parameters.AddWithValue("@Khoa", 0);
                                    add.Parameters.AddWithValue("@NguoiSua", "VB");
                                    add.Parameters.AddWithValue("@ThoiGianSua", DateTime.Now.ToString("yyyy - MMM - d :g"));
                                    add.Parameters.AddWithValue("@MaGT", status_text.Text);
                                    add.Parameters.AddWithValue("@MaDS", MaDS);

                                }
                                else
                                {
                                    add.Parameters.AddWithValue("@MaQN", CCCD_TEXT.Text);
                                    add.Parameters.AddWithValue("@ThoiGianRa", "");
                                    add.Parameters.AddWithValue("@ThoiGianVao", DateTime.Now.ToString("yyyy - MMM - d :g"));
                                    add.Parameters.AddWithValue("@Khoa", 1);
                                    add.Parameters.AddWithValue("@NguoiSua", "VB");
                                    add.Parameters.AddWithValue("@ThoiGianSua", DateTime.Now.ToString("yyyy - MMM - d :g"));
                                    add.Parameters.AddWithValue("@MaGT", "");
                                    add.Parameters.AddWithValue("@MaDS", "");
                                }
                                add.ExecuteNonQuery();
                            }
                            else if (reader["Khoa"].ToString() == "1")
                            {
                                if (type != 0)
                                {
                                    String ch = "update RANGOAI set Khoa = 0, ThoiGianRa = @TGR, ThoiGianSua = @TGS where MaQN = @MaQN";
                                    SqlCommand change = new SqlCommand(ch, conn);
                                    change.Parameters.AddWithValue("@TGR", DateTime.Now.ToString("yyyy - MMM - d :g"));
                                    change.Parameters.AddWithValue("@TGS", DateTime.Now.ToString("yyyy - MMM - d :g"));
                                    change.ExecuteNonQuery();
                                }
                                else
                                {
                                    add.Parameters.AddWithValue("@MaQN", CCCD_TEXT.Text);
                                    add.Parameters.AddWithValue("@ThoiGianRa", DateTime.Now.ToString("yyyy - MMM - d :g"));
                                    add.Parameters.AddWithValue("@ThoiGianVao", "");
                                    add.Parameters.AddWithValue("@Khoa", 0);
                                    add.Parameters.AddWithValue("@NguoiSua", "VB");
                                    add.Parameters.AddWithValue("@ThoiGianSua", DateTime.Now.ToString("yyyy - MMM - d :g"));
                                    add.Parameters.AddWithValue("@MaGT", status_text);
                                    add.Parameters.AddWithValue("@MaDS", MaDS);
                                    add.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                if (type == 0)
                                {
                                    String ch = "update RANGOAI set Khoa = 0, ThoiGianRa = @TGR, ThoiGianSua = @TGS where MaQN = @MaQN";
                                    SqlCommand change = new SqlCommand(ch, conn);
                                    change.Parameters.AddWithValue("@TGR", DateTime.Now.ToString("yyyy - MMM - d :g"));
                                    change.Parameters.AddWithValue("@TGS", DateTime.Now.ToString("yyyy - MMM - d :g"));
                                    change.ExecuteNonQuery();
                                }
                                else
                                {
                                    add.Parameters.AddWithValue("@MaQN", CCCD_TEXT.Text);
                                    add.Parameters.AddWithValue("@ThoiGianRa", "");
                                    add.Parameters.AddWithValue("@ThoiGianVao", DateTime.Now.ToString("yyyy - MMM - d :g"));
                                    add.Parameters.AddWithValue("@Khoa", 1);
                                    add.Parameters.AddWithValue("@NguoiSua", "VB");
                                    add.Parameters.AddWithValue("@ThoiGianSua", DateTime.Now.ToString("yyyy - MMM - d :g"));
                                    add.Parameters.AddWithValue("@MaGT", "");
                                    add.Parameters.AddWithValue("@MaDS", "");
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
