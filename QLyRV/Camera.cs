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
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Threading;
using FFmpeg.AutoGen;
using Vlc.DotNet.Forms;
using System.Drawing.Drawing2D;

namespace QLyRV
{
    public partial class Camera : Form
    {
        private FilterInfoCollection cameras;
        private VideoCaptureDevice camera;
        public static int type = 1;
        private String MaGT, MaDS, B64;
        private Bitmap bitmap = null;
        private string cmd = @"..\..\API.py";
        private string rtspUrl = "rtsp://admin:12345678a@192.168.0.144:554/0";

        public static string maQN, cccd, dv, ht, cv, cccdDan, htDan, stt;

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

        private async void Camera_Load(object sender, EventArgs e)
        {
            string rtspUrl = "rtsp://admin:12345678a@192.168.0.144:554/1";

            // Example of initializing FFmpeg for RTSP stream (if you use FFmpeg.AutoGen)
            //ffmpeg.avdevice_register_all();
            //ffmpeg.avformat_network_init();
            //ffmpeg.avcodec_register_all();

            camera = new VideoCaptureDevice(cameras[comboBox1.SelectedIndex].MonikerString);
            //camera = new VideoCaptureDevice(rtspUrl);
            camera.NewFrame += new NewFrameEventHandler(Video_NewFrame);
            camera.Start();

            //RunCommand(cmd);

            //await RunPythonScript(cmd);
        }

        private async Task RunPythonScript(string scriptPath)
        {
            await Task.Run(() =>
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "python",             // Python executable
                        Arguments = scriptPath,          // Path to Python script
                        RedirectStandardOutput = true,   // Capture output
                        RedirectStandardError = true,    // Capture errors
                        UseShellExecute = false,         // Required for redirection
                        CreateNoWindow = true            // Don't create a console window
                    }
                };

                process.Start();

                //string output = process.StandardOutput.ReadToEnd();
                //string error = process.StandardError.ReadToEnd();

                //process.WaitForExit();

                //// Optionally log output or error
                //if (!string.IsNullOrEmpty(output))
                //{
                //    Console.WriteLine("[OUTPUT] " + output);
                //}

                //if (!string.IsNullOrEmpty(error))
                //{
                //    Console.Error.WriteLine("[ERROR] " + error);
                //}

                //if (process.ExitCode != 0)
                //{
                //    throw new Exception($"Python script exited with code {process.ExitCode}: {error}");
                //}
            });
        }


        public static string ConvertImageToBase64String(Image image)
        {
           

            using (MemoryStream ms = new MemoryStream())
            {

                image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private String EscapeData(String B64)
        {
            int B64_length = B64.Length;
            if (B64_length <= 32000)
            {
                return Uri.EscapeDataString(B64);
            }


            int idx = 0;
            StringBuilder builder = new StringBuilder();
            String substr = B64.Substring(idx, 32000);
            while (idx < B64_length)
            {
                builder.Append(Uri.EscapeDataString(substr));
                idx += 32000;

                if (idx < B64_length)
                {

                    substr = B64.Substring(idx, Math.Min(32000, B64_length - idx));
                }

            }
            return builder.ToString();

        }

        public String sendPOST(String url, String B64)
        {
            try
            {

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 50000;
                var postData = "image=" + EscapeData(B64);

                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                return responseString;
            }
            catch (Exception ex)
            {
                return "Exception" + ex.ToString();
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            //camera = new VideoCaptureDevice(cameras[comboBox1.SelectedIndex].MonikerString);
            ////camera = new VideoCaptureDevice("rtsp://admin:12052003a@192.168.0.144:554/onvif1");
            //camera.NewFrame += new NewFrameEventHandler(Video_NewFrame);
            //camera.Start();
            
            if (!camera.IsRunning)
            {
                camera.Start();
                button6.Text = "CATCH";
            }
            else
            {
                //var process = new Process
                //{
                //    StartInfo = new ProcessStartInfo
                //    {
                //        FileName = "python", // Nếu trên Linux/MacOS, thay bằng "bash"
                //        Arguments = cmd, // Chạy lệnh và thoát
                //        RedirectStandardOutput = true, // Chuyển hướng đầu ra
                //        RedirectStandardError = true, // Chuyển hướng lỗi
                //        UseShellExecute = false, // Không sử dụng shell bên ngoài
                //        CreateNoWindow = true // Không tạo cửa sổ mới
                //    }
                //};

                //process.Start();
                //

                StopCam();
                
                //RunCommand(cmd);
                
                //RunCommand(cmd);
                if (tabControl1.SelectedIndex == 0)
                {
                    //StopCam();
                    ReadQR(bitmap);
                }
                else
                {
                    //StopCam();
                    ReadDan(bitmap);
                }

                //process.Kill();
                button6.Text = "START";
            }
        }

        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            
            bitmap = (Bitmap)eventArgs.Frame.Clone();
            pictureBox2.Image?.Dispose();
            pictureBox2.Image = bitmap;

            // Process the frame to read a barcode
            

            //if(tabControl1.SelectedIndex == 0)
            //{
            //    //StopCam();
            //    ReadQR(bitmap);
            //}
            //else
            //{
            //    //StopCam();
            //    ReadDan(bitmap);
            //}

            // Dispose of the bitmap to free up memory
            //bitmap.Dispose();
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
            string result = null;

            try
            {
                var readDan = new BarcodeReader();
                //{
                //    AutoRotate = true, // Tự xoay ảnh nếu cần
                //    TryInverted = true // Thử đọc mã QR với độ tương phản đảo ngược
                //}; 
                result = readDan.Decode(bitmap).ToString();
            }
            catch
            {
                MessageBox.Show("Khong doc duoc QR");
            }
            //MessageBox.Show(result.ToString());

            //B64 = ConvertImageToBase64String(pictureBox2.Image);
            //// Goi len server va tra ve ket qua
            //String server_ip = "127.0.0.1";
            ////String retStr2 = sendPOST_2("http://" + server_ip + ":5000/confirm", B64, B64_2);
            //var result = sendPOST("http://" + server_ip + ":5000/translate", B64);

            if (result != null)
            {
                //StopCam();
                if (result != null)
                {
                    StopCam();
                    string[] parts = result.ToString().Split('|');

                    if (parts.Length >= 0)
                    {

                        if(textBox4.Text == "")
                        {
                            textBox4.Text = parts[0];
                            textBox3.Text = parts[2];

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
            string quan = "select qn.CCCD, qn.HoTen, qn.MaCV, dv.TenDV from DONVI dv, QUANNHAN qn where qn.MaQN = @qn and qn.MaDV = dv.MaDV";
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
                                donvi_text.Text = reader["TenDV"].ToString();
                                CCCD_TEXT.Text = reader["CCCD"].ToString();
                                hoten_text.Text = reader["HoTen"].ToString();

                                if (reader["MaCV"].ToString() == "hv")
                                {
                                    type = 0;
                                    // Second query to get TenDV
                                    string tenDvQuery = "SELECT ds.STT FROM DANHSACH ds  JOIN QUANNHAN qn ON ds.MaQN = qn.MaQN WHERE ds.ThoiGianRa = @ThoiGianRa AND qn.CCCD = @MaQN";
                                    using (SqlCommand tenDvCommand = new SqlCommand(tenDvQuery, conn))
                                    {
                                        tenDvCommand.Parameters.AddWithValue("@ThoiGianRa", DateTime.Now.ToString());
                                        tenDvCommand.Parameters.AddWithValue("@MaQN", CCCD_TEXT.Text);

                                        using (SqlDataReader dvReader = tenDvCommand.ExecuteReader())
                                        {
                                            if (dvReader.Read() && dvReader["STT"] != null)
                                            { 
                                                MaDS = dvReader["STT"].ToString();
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
                            }
                            else
                            {
                                textBox2.Text = reader["TenDV"].ToString();
                                textBox4.Text = textBox6.Text;
                                textBox3.Text = reader["HoTen"].ToString();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Khong co du lieu");
                        }
                    }
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string ktra = "select Khoa from NHATKYTN  where MaQN = @cccd and ThoiGianVao = '" + DateTime.Now.ToString() + "'";
            
            string connectionString = conn_string;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand NK = new SqlCommand(ktra, conn))
                {
                    NK.Parameters.AddWithValue("@cccd", textBox4.Text);
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
            //if (bitmap == null)
            //{
            //    MessageBox.Show("Khong the doc QR");
            //}
            string result = null;
            var readQR = new BarcodeReader();
            var _result = readQR.Decode(bitmap);
            
            if(_result == null)
            {
                MessageBox.Show("Khong the doc ma QR");
            }
            else
            {
                result = _result.ToString();
            }
            
            //{
            //    AutoRotate = true, // Tự xoay ảnh nếu cần
            //    TryInverted = true // Thử đọc mã QR với độ tương phản đảo ngược
            //}; 
            

            //B64 = ConvertImageToBase64String(pictureBox2.Image);
            //// Goi len server va tra ve ket qua
            //String server_ip = "127.0.0.1";
            ////String retStr2 = sendPOST_2("http://" + server_ip + ":5000/confirm", B64, B64_2);
            //var result = sendPOST("http://" + server_ip + ":5000/translate", B64);

            //MessageBox.Show(result.ToString());

            if (result != null)
            {
                //StopCam();
                string[] parts = result.ToString().Split('|');

                if (parts.Length >= 0)
                {
                    CCCD_TEXT.Text = parts[0]; 
                    hoten_text.Text = parts[2];

                    cccd = CCCD_TEXT.Text;
                    ht = hoten_text.Text;

                    string chucvuQuery = "SELECT MaCV FROM QUANNHAN  WHERE CCCD = @MaQN and TonTai = 1";
                    string connectionString = conn_string;

                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();

                            // First query to get ChucVu
                            using (SqlCommand chucvuCommand = new SqlCommand(chucvuQuery, conn))
                            {
                                chucvuCommand.Parameters.AddWithValue("@MaQN", CCCD_TEXT.Text);

                                using (SqlDataReader reader = chucvuCommand.ExecuteReader())
                                {
                                    if (reader.Read() )
                                    {
                                        cv = reader["MaCV"].ToString();
                                        //MessageBox.Show(cv);
                                        reader.Close();
                                        if (cv == "hv")
                                        {
                                              // Close reader before executing the second query
                                            type = 0;
                                            // Second query to get TenDV
                                            string tenDvQuery = "SELECT dv.TenDV, ds.STT, qn.MaQN FROM DANHSACH ds  JOIN QUANNHAN qn ON ds.MaQN = qn.MaQN JOIN DONVI dv ON dv.MaDV = qn.MaDV WHERE ds.ThoiGianRa = @ThoiGianRa AND qn.CCCD = @MaQN";
                                            try
                                            {
                                                using (SqlCommand tenDvCommand = new SqlCommand(tenDvQuery, conn))
                                                {
                                                    tenDvCommand.Parameters.AddWithValue("@ThoiGianRa", DateTime.Now.ToString());
                                                    tenDvCommand.Parameters.AddWithValue("@MaQN", CCCD_TEXT.Text);

                                                    using (SqlDataReader dvReader = tenDvCommand.ExecuteReader())
                                                    {
                                                        if (dvReader.Read() && dvReader["STT"] != null)
                                                        {
                                                            textBox7.Text = dvReader["MaQN"].ToString();
                                                            donvi_text.Text = dvReader["TenDV"].ToString();
                                                            MaDS = dvReader["STT"].ToString();
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
                                            catch(Exception ex)
                                            {
                                                MessageBox.Show(ex.ToString());
                                            }
                                        }
                                        else
                                        {
                                            //reader.Close();  // Close reader before executing the second query
                                            type = 1;
                                            // Second query to get TenDV
                                            string tenDvQuery = "SELECT dv.TenDV, qn.MaQN , ds.STT FROM DANHSACH ds join QUANNHAN qn on ds.MaQN = qn.MaQN JOIN DONVI dv ON qn.MADV = dv.MaDV WHERE qn.CCCD = @MaQN";
                                            using (SqlCommand tenDvCommand = new SqlCommand(tenDvQuery, conn))
                                            {
                                                tenDvCommand.Parameters.AddWithValue("@MaQN", CCCD_TEXT.Text);

                                                using (SqlDataReader dvReader = tenDvCommand.ExecuteReader())
                                                {
                                                    if (dvReader.Read())
                                                    {
                                                        donvi_text.Text = dvReader["TenDV"].ToString();
                                                        textBox7.Text = dvReader["MaQN"].ToString();
                                                        MaDS = dvReader["STT"].ToString();

                                                        dv = donvi_text.Text;
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show("Hay thu lai");
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
            string ktra = "select rn.STT from NHATKYQN rn, DANHSACH ds where rn.STT_DS = ds.STT and ds.MaQN = '" + textBox7.Text + "' and (rn.ThoiGianRa = '" + DateTime.Now.ToString("yyyy-MM-d") + "' or rn.ThoiGianVao = '" + DateTime.Now.ToString("yyyy-MM-d") + "')";
            string connectionString = conn_string;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand NK = new SqlCommand(ktra, conn))
                {
                    using (SqlDataReader reader = NK.ExecuteReader())
                    {
                        //if(tabControl1.SelectedIndex == 1)
                        //{
                        //    type = 1;
                        //}
                        if (reader.Read())
                        {
                            maQN = textBox7.Text;
                            stt = reader["STT"].ToString();
                        }
                    }
                }
            }
            AdminForm af = new AdminForm(1);
            af.Show();
            af.OpenChildForm(new ViPham());    
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string ktra = "select rn.Khoa, rn.STT from NHATKYQN rn, DANHSACH ds where rn.STT_DS = ds.STT and ds.MaQN = '" + textBox7.Text + "' and (rn.ThoiGianRa = '" + DateTime.Now.ToString("yyyy-MM-d") + "' or rn.ThoiGianVao = '" + DateTime.Now.ToString("yyyy-MM-d") + "')";
            string addNhatKi = "insert into NHATKYQN (Khoa, STT_DS, ThoiGianRa, ThoiGianVao) values (@Khoa, @STT_DS, @ThoiGianRa, @ThoiGianVao)";
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
                            //MessageBox.Show(type.ToString());
                            //MessageBox.Show("Checked");
                            SqlCommand add = new SqlCommand(addNhatKi, conn);
                            if (reader["Khoa"].ToString() == "True")
                            {
                                
                                if (type != 0)
                                {
                                    String ch = "update NHATKYQN set Khoa = 0, ThoiGianRa = @TGR where STT = @STT";
                                    SqlCommand change = new SqlCommand(ch, conn);
                                    change.Parameters.AddWithValue("@TGR", DateTime.Now);
                                    change.Parameters.AddWithValue("@STT", Int32.Parse(reader["STT"].ToString()));

                                    reader.Close();

                                    change.ExecuteNonQuery();
                                    MessageBox.Show("Done");
                                }
                                else
                                {
                                    add.Parameters.AddWithValue("@STT_DS", MaDS);
                                    add.Parameters.AddWithValue("@ThoiGianRa", DateTime.Now);
                                    add.Parameters.AddWithValue("@ThoiGianVao", "");
                                    add.Parameters.AddWithValue("@Khoa", 0);

                                    reader.Close();

                                    add.ExecuteNonQuery();
                                    MessageBox.Show("Done");
                                }
                            }
                            else
                            {
                                reader.Close();
                                if (type == 0)
                                {
                                    String ch = "update NHATKYQN set Khoa = 1, ThoiGianVao = @TGV where STT = @STT";
                                    SqlCommand change = new SqlCommand(ch, conn);
                                    change.Parameters.AddWithValue("@TGV", DateTime.Now);
                                    change.Parameters.AddWithValue("@STT", Int32.Parse(reader["STT"].ToString()));

                                    reader.Close();

                                    change.ExecuteNonQuery();
                                    MessageBox.Show("Done");
                                }
                                else
                                {
                                    add.Parameters.AddWithValue("@STT_DS", MaDS);
                                    add.Parameters.AddWithValue("@ThoiGianRa", "");
                                    add.Parameters.AddWithValue("@ThoiGianVao", DateTime.Now);
                                    add.Parameters.AddWithValue("@Khoa", 1);

                                    reader.Close();

                                    add.ExecuteNonQuery();
                                    MessageBox.Show("Done");
                                }
                            }
                        }
                        else
                        {
                            reader.Close();
                            SqlCommand add = new SqlCommand(addNhatKi, conn);
                            if (type == 0)
                            {
                                add.Parameters.AddWithValue("@STT_DS", MaDS);
                                add.Parameters.AddWithValue("@ThoiGianRa", DateTime.Now);
                                add.Parameters.AddWithValue("@ThoiGianVao", "");
                                add.Parameters.AddWithValue("@Khoa", 0);

                            }
                            else
                            {
                                add.Parameters.AddWithValue("@STT_DS", MaDS);
                                add.Parameters.AddWithValue("@ThoiGianRa", "");
                                add.Parameters.AddWithValue("@ThoiGianVao", DateTime.Now);
                                add.Parameters.AddWithValue("@Khoa", 1);
                            }
                            add.ExecuteNonQuery();
                            MessageBox.Show("Done");
                        }
                    }
                }
            }

            textBox7.Text = "";
            CCCD_TEXT.Text = "";
            hoten_text.Text = "";
            donvi_text.Text = "";
        }
    }
}
