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
    public partial class NhatKy : Form
    {
        public int Type;
        public NhatKy(int type)
        {
            InitializeComponent();
            this.Type = type;
        }

        private void NhatKy_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy-MM-d";
            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = "yyyy-MM-d";

            //dateTimePicker1.MaxDate = DateTime.Now;
            //dateTimePicker2.MaxDate = DateTime.Now;

            dateTimePicker1.Value = DateTime.Now;

            label2.Hide();
            label3.Hide();
            label6.Hide();
            label4.Hide();
            //dateTimePicker1.Hide();
            dateTimePicker2.Hide();
            dateTimePicker4.Hide();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                label2.Show();
                label3.Show();
                //dateTimePicker1.Show();
                dateTimePicker2.Show();
            }
            else
            {
                label2.Hide();
                label3.Hide();
                //dateTimePicker1.Hide();
                dateTimePicker2.Hide();
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = GetNhatKy().Tables[0];
        }

        DataSet GetNhatKy()
        {

            DataSet data = new DataSet();
            string query = "";
            if(this.Type == 0 && this.Type == 1)
            {
                if (checkBox1.Checked)
                    query = "select lvp.TenVP as ViPham, qn.MaQN, qn.HoTen, qn.MaDV, ds.HinhThucRV, rn.ThoiGianRa, rn.ThoiGianVao from NHATKYQN rn, DANHSACH ds, QUANNHAN qn, VIPHAM vp, LoaiVP lvp where rn.ThoiGianRa >= '" + dateTimePicker1.Value.ToString() + "' and rn.ThoiGianVao <= '" + dateTimePicker2.Value.ToString() + "' and rn.STT_DS = ds.STT and ds.MaQN = qn.MaQN and qn.MaQN = vp.MaQN and vp.LoaiVP = lvp.LoaiVP";
                else                                                                                                  
                    query = "select lvp.TenVP as ViPham, qn.MaQN, qn.HoTen, qn.MaDV, ds.HinhThucRV, rn.ThoiGianRa, rn.ThoiGianVao from NHATKYQN rn, DANHSACH ds, QUANNHAN qn, VIPHAM vp, LoaiVP lvp where rn.ThoiGianVao = '" + dateTimePicker1.Value.ToString() + "' and rn.STT_DS = ds.STT and ds.MaQN = qn.MaQN and qn.MaQN = vp.MaQN and qn.MaDV = dv.MaDV and vp.LoaiVP = lvp.LoaiVP";
            }                                                                                                         
            else if(this.Type == 2)                                                                                   
            {                                                                                                         
                if (checkBox1.Checked)                                                                                
                    query = "select lvp.TenVP as ViPham, qn.MaQN, qn.HoTen, qn.MaDV, ds.HinhThucRV, rn.ThoiGianRa, rn.ThoiGianVao from NHATKYQN rn, DANHSACH ds, QUANNHAN qn, VIPHAM vp, DONVI dv, LoaiVP lvp where rn.ThoiGianRa >= '" + dateTimePicker1.Value.ToString() + "' and rn.ThoiGianVao <= '" + dateTimePicker2.Value.ToString() + "' and rn.STT_DS = ds.STT and ds.MaQN = qn.MaQN and qn.MaQN = vp.MaQN and qn.MaDV = dv.MaDV and dv.MaDVCapTren = '" + Account.account + "' and vp.LoaiVP = lvp.LoaiVP";
                else                                                                                                  
                    query = "select lvp.TenVP as ViPham, qn.MaQN, qn.HoTen, qn.MaDV, ds.HinhThucRV, rn.ThoiGianRa, rn.ThoiGianVao from NHATKYQN rn, DANHSACH ds, QUANNHAN qn, VIPHAM vp, DONVI dv, LoaiVP lvp where rn.ThoiGianVao = '" + dateTimePicker1.Value.ToString() + "' and rn.STT_DS = ds.STT and ds.MaQN = qn.MaQN and qn.MaQN = vp.MaQN and qn.MaDV = dv.MaDV and dv.MaDVCapTren = '" + Account.account + "' and vp.LoaiVP = lvp.LoaiVP";
            }
            else 
            {
                if (checkBox1.Checked)
                    query = "select lvp.TenVP as ViPham, qn.MaQN, qn.HoTen, qn.MaDV, ds.HinhThucRV, rn.ThoiGianRa, rn.ThoiGianVao from NHATKYQN rn, DANHSACH ds, QUANNHAN qn, VIPHAM vp, DONVI dv, LoaiVP lvp where rn.ThoiGianRa >= '" + dateTimePicker1.Value.ToString() + "' and rn.ThoiGianVao <= '" + dateTimePicker2.Value.ToString() + "' and rn.STT_DS = ds.STT and ds.MaQN = qn.MaQN and qn.MaQN = vp.MaQN and qn.MaDV = '" + Account.account + "' and vp.LoaiVP = lvp.LoaiVP";
                else
                    query = "select lvp.TenVP as ViPham, qn.MaQN, qn.HoTen, qn.MaDV, ds.HinhThucRV, rn.ThoiGianRa, rn.ThoiGianVao from NHATKYQN rn, DANHSACH ds, QUANNHAN qn, VIPHAM vp, DONVI dv, LoaiVP lvp where rn.ThoiGianVao = '" + dateTimePicker1.Value.ToString() + "' and rn.STT_DS = ds.STT and ds.MaQN = qn.MaQN and qn.MaQN = vp.MaQN and qn.MaDV = '" + Account.account + "' and vp.LoaiVP = lvp.LoaiVP";
            }


            //MessageBox.Show(this.Type.ToString());
            SqlConnection conn = new SqlConnection(conn_string);
            conn.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
            adapter.Fill(data);

            conn.Close();
            return data;
        }

        DataSet GetKhach()
        {

            DataSet data = new DataSet();
            string query = "";
            if (this.Type == 0 && this.Type == 1)
            {
                if (checkBox1.Checked)
                    query = "select t.MaGhiNhan, t.MaQN, qn.HoTen, qn.MaDV, t.CCCD_ThanNhan, t.HoTen_ThanNhan, t.ThoiGianVao, t.ThoiGianRa from NHATKYTN t, QUANNHAN qn where t.ThoiGianVao >= '" + dateTimePicker1.Value.ToString() + "' and t.ThoiGianVao <= '" + dateTimePicker2.Value.ToString() + "' and t.MaQN = qn.MaQN";
                else
                    query = "select t.MaGhiNhan, t.MaQN, qn.HoTen, qn.MaDV, t.CCCD_ThanNhan, t.HoTen_ThanNhan, t.ThoiGianVao, t.ThoiGianRa from NHATKYTN t, QUANNHAN qn where t.ThoiGianVao = '" + dateTimePicker1.Value.ToString() + "' and t.MaQN = qn.MaQN";
            }
            else if (this.Type == 2)
            {
                if (checkBox1.Checked)
                    query = "select t.MaGhiNhan, t.MaQN, qn.HoTen, qn.MaDV, t.CCCD_ThanNhan, t.HoTen_ThanNhan, t.ThoiGianVao, t.ThoiGianRa from NHATKYTN t, QUANNHAN qn where t.ThoiGianVao >= '" + dateTimePicker1.Value.ToString() + "' and t.ThoiGianVao <= '" + dateTimePicker2.Value.ToString() + "' and t.MaQN = qn.MaQN and qn.MaDV = dv.MaDV and dv.MaDVCapTren = '" + Account.account +"'";
                else
                    query = "select t.MaGhiNhan, t.MaQN, qn.HoTen, qn.MaDV, t.CCCD_ThanNhan, t.HoTen_ThanNhan, t.ThoiGianVao, t.ThoiGianRa from NHATKYTN t, QUANNHAN qn where t.ThoiGianVao = '" + dateTimePicker1.Value.ToString() + "' and t.MaQN = qn.MaQN and qn.MaDV = dv.MaDV and dv.MaDVCapTren = '" + Account.account + "'";
            }
            else
            {
                if (checkBox1.Checked)
                    query = "select t.MaGhiNhan, t.MaQN, qn.HoTen, qn.MaDV, t.CCCD_ThanNhan, t.HoTen_ThanNhan, t.ThoiGianVao, t.ThoiGianRa from NHATKYTN t, QUANNHAN qn where t.ThoiGianVao >= '" + dateTimePicker1.Value.ToString() + "' and t.ThoiGianVao <= '" + dateTimePicker2.Value.ToString() + "' and t.MaQN = qn.MaQN and qn.MaDV = '" + Account.account + "'";
                else
                    query = "select t.MaGhiNhan, t.MaQN, qn.HoTen, qn.MaDV, t.CCCD_ThanNhan, t.HoTen_ThanNhan, t.ThoiGianVao, t.ThoiGianRa from NHATKYTN t, QUANNHAN qn where t.ThoiGianVao = '" + dateTimePicker1.Value.ToString() + "' and t.MaQN = qn.MaQN and qn.MaDV = '" + Account.account + "'";
            }


            //MessageBox.Show(this.Type.ToString());
            SqlConnection conn = new SqlConnection(conn_string);
            conn.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
            adapter.Fill(data);

            conn.Close();
            return data;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                label4.Show();
                label6.Show();
                //dateTimePicker1.Show();
                dateTimePicker4.Show();
            }
            else
            {
                label4.Hide();
                label6.Hide();
                //dateTimePicker1.Hide();
                dateTimePicker4.Hide();
            }
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            dataGridView2.DataSource = GetKhach().Tables[0];
        }
    }
}
