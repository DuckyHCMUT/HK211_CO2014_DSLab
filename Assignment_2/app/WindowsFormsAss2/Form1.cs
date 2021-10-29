using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using Oracle.DataAccess.Client;

namespace WindowsFormsAss2
{
    public partial class Form1 : Form
    {
        private OracleConnection dbConn;
        private bool isLogin = false;
        private DataTable fabricTable = new DataTable();
        private OracleTransaction fabricTrans;
        
        public Form1()
        {
            InitializeComponent();
        }

        // LOGIN and LOGOUT
        private void login_Click(object sender, EventArgs e)
        {
            if (isLogin)
            {
                MessageBox.Show("Please log out first!");
                return;
            }

            string connectionStr = "data source=(description ="
            + "(address = (protocol = tcp)(host = " + this.textBox_HostIP.Text + ")"
            + "(port = " + this.textBox_HostPort.Text + "))"
            + "(connect_data ="
            + "(server = dedicated)"
            + "(service_name = xe)));"
            + "user id=" + this.text_user.Text
            + ";password=" + this.text_password.Text + ";";
            this.dbConn = new OracleConnection(connectionStr);
            try
            {
                this.dbConn.Open();
                this.isLogin = true;
                this.label_status.Text = "LOGGED IN AS: " + this.text_user.Text;
                MessageBox.Show("Logged in Successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void logout_Click(object sender, EventArgs e)
        {
            if (this.isLogin) this.dbConn.Close();
            text_user.Text = "";
            text_password.Text = "";
            this.isLogin = false;
            this.label_status.Text = "NOT LOGGED IN";
            this.fabricTable.Clear();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //! Only Close when the connection is open
            if (this.isLogin) 
                this.dbConn.Close();
        }
        // end of LOGIN and LOGOUT


        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void label_status_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged_2(object sender, EventArgs e)
        {

        }

        private void label1_Click_2(object sender, EventArgs e)
        {

        }

        private void searchingPatientInstructionLabel_Quang_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void username_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_3(object sender, EventArgs e)
        {

        }

        private void box_search_by_name_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
