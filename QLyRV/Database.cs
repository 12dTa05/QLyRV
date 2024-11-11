using System;
using System.Data.SqlClient;
using System.Windows.Forms;
//using static WindowsForm.CameraForm;
//using static WindowsForm.LogForm1;
//using static WindowsForm.LoginForm;

namespace QLyRV
{
    public partial class Database : Form
    {
        public string server, database, userid, passwd;
        public static SqlConnection conn;
        public static String conn_string;
        public Database()
        {
            InitializeComponent();
        }

        private void connect_btn_Click(object sender, EventArgs e)
        {
            if (Check_Conn())
            {
                conn_string = $"Server={server}; Database={database}; User Id={userid}; Password={passwd}; Trusted_Connection=True;";
                using (conn = new SqlConnection(conn_string))
                {
                    try
                    {
                        conn.Open();
                        Account frm = new Account();
                        //TrangChu frm = new TrangChu();
                        //Camera frm = new Camera();
                        //CameraForm frm = new CameraForm();
                        this.Hide();
                        frm.Show();
                        conn.Close();
                    }
                    catch (SqlException)
                    {
                        MessageBox.Show("Check your information again");
                    }
                }

            }
            else
            {
                MessageBox.Show("Check your information again");
            }
        }

        public bool Check_Conn()
        {
            server = Server.Text;
            database = Database1.Text;
            userid = UserID.Text;
            passwd = Password.Text;
            if (server == "" || database == "" || userid == "" || passwd == "")
            {
                return false;
            }
            return true;
        }
    }
}
