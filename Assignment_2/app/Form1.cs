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
        private DataTable supplierTable = new DataTable();
        private OracleTransaction fabricTrans;
        
        public Form1()
        {
            InitializeComponent();
        }

        // Part 0: LOGIN and LOGOUT
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
            this.supplierTable.Clear();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //! Only Close when the connection is open
            if (this.isLogin) 
                this.dbConn.Close();
        }
        // end of LOGIN and LOGOUT

        // Part 1: Search for supplier
        private void supSearchButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.isLogin)
                {
                    string supplierNameTxt = box_search_by_name.Text.ToLower();
                    string supplierIdTxt = box_search_by_code.Text.ToUpper();
                    
                    if (String.IsNullOrEmpty(supplierNameTxt) & !String.IsNullOrEmpty(supplierIdTxt))
                    {
                        // Read and display supplier
                        using (OracleCommand cmd = new OracleCommand(cmd_select_by_code(supplierIdTxt), this.dbConn))
                        {
                            using (OracleDataReader dataReader = cmd.ExecuteReader())
                            {
                                DataTable dataTable = new DataTable();
                                dataTable.Load(dataReader);
                                if (dataTable.Rows.Count == 0)
                                {
                                    throw new Exception("Supplier ID not found!");
                                }
                                data_supplier.DataSource = dataTable;

                                // Read and display supplement
                                using (OracleCommand cmd_supply = new OracleCommand(display_supply_ID(supplierIdTxt), this.dbConn))
                                {
                                    using (OracleDataReader dataReader_supply = cmd_supply.ExecuteReader())
                                    {
                                        DataTable dataTable_supply = new DataTable();
                                        dataTable_supply.Load(dataReader_supply);
                                        if (dataTable_supply.Rows.Count == 0)
                                        {
                                            throw new Exception("No supplement found!");
                                        }
                                        data_supply.DataSource = dataTable_supply;
                                    }
                                }
                            }
                        }
                    }
                    else if (!String.IsNullOrEmpty(supplierNameTxt) & String.IsNullOrEmpty(supplierIdTxt))
                    {
                        // Read and display supplier        
                        using (OracleCommand cmd = new OracleCommand(cmd_select_by_name(supplierNameTxt), this.dbConn))
                        {
                            using (OracleDataReader dataReader = cmd.ExecuteReader())
                            {
                                DataTable dataTable = new DataTable();
                                dataTable.Load(dataReader);
                                if (dataTable.Rows.Count == 0)
                                {
                                    throw new Exception("Supplier name not found!");
                                }
                                data_supplier.DataSource = dataTable;

                                // Read and display supplement
                                using (OracleCommand cmd_supply = new OracleCommand(display_supply_name(supplierNameTxt), this.dbConn))
                                {
                                    using (OracleDataReader dataReader_supply = cmd_supply.ExecuteReader())
                                    {
                                        DataTable dataTable_supply = new DataTable();
                                        dataTable_supply.Load(dataReader_supply);
                                        if (dataTable_supply.Rows.Count == 0)
                                        {
                                            throw new Exception("No supplement found!");
                                        }
                                        data_supply.DataSource = dataTable_supply;
                                    }
                                }
                            }
                        }
                    }
                    else if (!String.IsNullOrEmpty(supplierNameTxt) & !String.IsNullOrEmpty(supplierIdTxt))
                    {
                        // Read and display supplier      
                        using (OracleCommand cmd = new OracleCommand(cmd_select_by_both(supplierNameTxt, supplierIdTxt), this.dbConn))
                        {
                            using (OracleDataReader dataReader = cmd.ExecuteReader())
                            {
                                DataTable dataTable = new DataTable();
                                dataTable.Load(dataReader);
                                if (dataTable.Rows.Count == 0)
                                {
                                    throw new Exception("Supplier not found!");
                                }
                                data_supplier.DataSource = dataTable;

                                // Read and display supplement
                                using (OracleCommand cmd_supply = new OracleCommand(display_supply_both(supplierIdTxt, supplierNameTxt), this.dbConn))
                                {
                                    using (OracleDataReader dataReader_supply = cmd_supply.ExecuteReader())
                                    {
                                        DataTable dataTable_supply = new DataTable();
                                        dataTable_supply.Load(dataReader_supply);
                                        if (dataTable_supply.Rows.Count == 0)
                                        {
                                            throw new Exception("No supplement found!");
                                        }
                                        data_supply.DataSource = dataTable_supply;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Please fill in 1 search box!");
                    }
                }
                else
                {
                    throw new Exception("Please login first!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                data_supplier.DataSource = null;
                data_supply.DataSource = null;
            }
        }

        // search by ID
        private string cmd_select_by_code(string this_ID)
        {
            string cmdText = "";

            //select few attributes
            string select_table = " SELECT DISTINCT S.SupplierCode AS SupplierID, " +
                                 " S.Name AS SupplierName, " +
                                 " SN.PhoneNo AS PhoneNumber ";

            string query_table = " FROM Assignment_2.Supplier S, Assignment_2.SupplierPhoneNo SN ";

            string query_condition = " WHERE S.SupplierCode = SN.ESupplierCode " +
                                 $"AND S.SupplierCode LIKE '%{this_ID}%'  ";

            cmdText = select_table + query_table + query_condition;
            return cmdText;
        }

        // seacrh by name
        private string cmd_select_by_name(string this_name)
        {
            string cmdText = "";

            string select_table = " SELECT DISTINCT S.SupplierCode AS SupplierID, " +
                                 " S.Name AS SupplierName, " +
                                 " SN.PhoneNo AS PhoneNumber ";

            string query_table = " FROM Assignment_2.Supplier S, Assignment_2.SupplierPhoneNo SN ";

            string query_condition = " WHERE S.SupplierCode = SN.ESupplierCode " + 
                                 $" AND LOWER(S.Name) LIKE LOWER('%{this_name}%') ";

            cmdText = select_table + query_table + query_condition;

            return cmdText; 
        }

        private string cmd_select_by_both(string this_name, string this_ID)
        {
            string cmdText = "";

            string select_table = " SELECT DISTINCT S.SupplierCode AS SupplierID, " +
                                 " S.Name AS SupplierName, " +
                                 " SN.PhoneNo AS PhoneNumber ";

            string query_table = " FROM Assignment_2.Supplier S, Assignment_2.SupplierPhoneNo SN ";

            string query_condition = " WHERE S.SupplierCode = SN.ESupplierCode " +
                                 $" AND (LOWER(S.Name) LIKE LOWER('%{this_name}%') " +
                                 $" OR S.SupplierCode LIKE '%{this_ID}%') ";

            cmdText = select_table + query_table + query_condition;

            return cmdText;
        }

        private string display_supply_ID(string this_ID)
        {
            string cmdText = "";

            string select_table = " SELECT DISTINCT " +
                                " B.BoltCode AS BoltCode, " +
                                " B.DateImported AS ImpDate, " +
                                " B.Length AS BoltLength, " + 
                                " B.Quantity AS Quantity, " +
                                " B.PurchasePrice AS PurchasePrice, " +
                                " F.FabricName AS FabricType, " +
                                " S.SupplierCode AS SupplierID, " +
                                " S.Name AS SupplierName";

            string query_table = " FROM Assignment_2.Supplier S, Assignment_2.BoltStock B, Assignment_2.FabricCategory F ";

            string query_condition = " WHERE S.SupplierCode = B.ESupplierCode AND " +
                                     " B.CategoryCode = F.FabricCode AND " +
                                     $" S.SupplierCode LIKE '%{this_ID}%' ";
                
            cmdText = select_table + query_table + query_condition;

            return cmdText;
        }

        private string display_supply_name(string this_name)
        {
            string cmdText = "";

            string select_table = " SELECT DISTINCT " +
                                " B.BoltCode AS BoltCode, " +
                                " B.DateImported AS ImpDate, " +
                                " B.Length AS BoltLength, " +
                                " B.Quantity AS Quantity, " +
                                " B.PurchasePrice AS PurchasePrice, " +
                                " F.FabricName AS FabricType, " +
                                " S.SupplierCode AS SupplierID, " +
                                " S.Name AS SupplierName";

            string query_table = " FROM Assignment_2.Supplier S, Assignment_2.BoltStock B, Assignment_2.FabricCategory F ";

            string query_condition = " WHERE S.SupplierCode = B.ESupplierCode AND " +
                                     " B.CategoryCode = F.FabricCode " +
                                     $" AND LOWER(S.Name) LIKE LOWER('%{this_name}%') ";

            cmdText = select_table + query_table + query_condition;

            return cmdText;
        }

        private string display_supply_both(string this_ID, string this_name)
        {
            string cmdText = "";

            string select_table = " SELECT DISTINCT " +
                                " B.BoltCode AS BoltCode, " +
                                " B.DateImported AS ImpDate, " +
                                " B.Length AS BoltLength, " +
                                " B.Quantity AS Quantity, " +
                                " B.PurchasePrice AS PurchasePrice, " +
                                " F.FabricName AS FabricType, " +
                                " S.SupplierCode AS SupplierID, " +
                                " S.Name AS SupplierName";

            string query_table = " FROM Assignment_2.Supplier S, Assignment_2.BoltStock B, Assignment_2.FabricCategory F ";

            string query_condition = " WHERE S.SupplierCode = B.ESupplierCode AND " +
                                     " B.CategoryCode = F.FabricCode " +
                                     $" AND (LOWER(S.Name) LIKE LOWER('%{this_name}%') " +
                                     $" OR S.SupplierCode LIKE '%{this_ID}%') ";

            cmdText = select_table + query_table + query_condition;

            return cmdText;
        }
        // End of part 1

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

        // Part 2: Add supplier
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (!this.isLogin)
            {
                MessageBox.Show("Please login first!");
                return;
            }
            else
            {
                if(add_text_name.Text == "")
                {
                    MessageBox.Show("Please insert the supplier name!");
                    return;
                }
                else if(add_text_addr.Text == "")
                {
                    MessageBox.Show("Please insert the supplier address!");
                    return;
                }
                else if (add_text_code.Text == "")
                {
                    MessageBox.Show("Please insert the tax code!");
                    return;
                }
                else if(add_text_acc.Text == "")
                {
                    MessageBox.Show("Please insert the bank account!");
                    return;
                }
                else if (add_text_phone.Text == "")
                {
                    MessageBox.Show("Please insert the phone number!");
                    return;
                }
                else if (add_text_essn.Text == "")
                {
                    MessageBox.Show("Please insert the in-charge supplier!");
                    return;
                }
                else //Masterpiece starts from here !
                {
                    string sID = "S" + IdWorker(Convert.ToString(generateSupplierID() + 1));
                    insertSupplier(sID);
                }
            }
        }

        private void insertSupplier(string sID)
        {
            string addSupplier = " INSERT INTO Assignment_2.Supplier VALUES (:SupplierCode, :Address, :Name, :TaxCode, :BankAccount, :SuperEmployeeCode) ";
            string addPhoneNo  = " INSERT INTO Assignment_2.SupplierPhoneNo VALUES (:ESupplierCode, :PhoneNo) ";
            
            using (OracleCommand cmd_add_supplier = new OracleCommand(addSupplier, this.dbConn))
            {
                this.fabricTrans = this.dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                cmd_add_supplier.CommandType = CommandType.Text;
                cmd_add_supplier.Transaction = this.fabricTrans;

                cmd_add_supplier.Parameters.Add("SupplierCode", sID);
                cmd_add_supplier.Parameters.Add("Address", add_text_addr.Text.Trim());
                cmd_add_supplier.Parameters.Add("Name", add_text_name.Text.Trim());
                cmd_add_supplier.Parameters.Add("TaxCode", add_text_code.Text.Trim());
                cmd_add_supplier.Parameters.Add("BankAccount", add_text_acc.Text.Trim());
                cmd_add_supplier.Parameters.Add("SuperEmployeeCode", add_text_essn.Text.Trim());     
                
                try
                {
                    cmd_add_supplier.ExecuteNonQuery();
                    this.fabricTrans.Commit();
                    MessageBox.Show("Successfully added supplier information!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    clear_insert();
                }
                catch (Exception ex)
                {
                    this.fabricTrans.Rollback();
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            using (OracleCommand cmd_add_phone = new OracleCommand(addPhoneNo, this.dbConn))
            {
                this.fabricTrans = this.dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                cmd_add_phone.CommandType = CommandType.Text;
                cmd_add_phone.Transaction = this.fabricTrans;

                cmd_add_phone.Parameters.Add("ESupplierCode", sID);
                cmd_add_phone.Parameters.Add("PhoneNo", add_text_phone.Text.Trim());
                try
                {
                    cmd_add_phone.ExecuteNonQuery();
                    this.fabricTrans.Commit();
                    MessageBox.Show("Phone numbder added!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    clear_insert();
                }
                catch (Exception ex)
                {
                    this.fabricTrans.Rollback();
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private int generateSupplierID()
        {
            string new_ID = " SELECT MAX(CAST(SUBSTR(SupplierCode,3,LENGTH(SupplierCode)) AS INT)) AS MaxID " +
                " FROM Assignment_2.Supplier ";

            using (OracleCommand cmdMaxID = new OracleCommand(new_ID, this.dbConn))
            {
                cmdMaxID.CommandType = CommandType.Text;
                OracleDataReader dr = cmdMaxID.ExecuteReader();
                dr.Read();
                return Convert.ToInt32(dr["MaxID"]);
            }
        }

        private string IdWorker(string num)
        {
            string ID = new String('0', 3 - num.Length);
            return ID + num;
        }

        private void clear_insert()
        {
            // 2 lines are the same
            add_text_name.Text = add_text_acc.Text = add_text_addr.Text = "";
            add_text_code.Text = add_text_essn.Text = "";
        }
        // End of Part 2

        // Part 3:
        private void supplier_find_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.isLogin)
                {
                    MessageBox.Show("Please login first!");
                    return;
                }
                else // Thing starts from here
                {
                    if (supplier_code_box.Text == "")
                    {
                        MessageBox.Show("Please insert the supplier code!");
                        return;
                    }
                    else
                    {
                        string supplierIdTxt = supplier_code_box.Text.ToUpper();

                        if (!String.IsNullOrEmpty(supplierIdTxt))
                        {
                            // Read and display supplier
                            using (OracleCommand cmd = new OracleCommand(cmd_select_by_code_p3(supplierIdTxt), this.dbConn))
                            {
                                using (OracleDataReader dataReader = cmd.ExecuteReader())
                                {
                                    DataTable dataTable = new DataTable();
                                    dataTable.Load(dataReader);
                                    if (dataTable.Rows.Count == 0)
                                    {
                                        throw new Exception("Supplier ID not found!");
                                    }
                                    data_category.DataSource = dataTable;
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Please insert supplier ID!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                data_category.DataSource = null;
            }
        }

        private string cmd_select_by_code_p3(string this_ID)
        {
            string cmdText = "";

            //select few attributes
            string select_table = " SELECT DISTINCT S.SupplierCode AS SupplierCode, " +
                                  " S.Name AS Supplier, F.FabricCode AS FabricCode, " +
                                  "F.FabricName AS FabricType, F.Color AS FabricColor ";

            string query_table = " FROM Assignment_2.FabricCategory F, Assignment_2.Supplier S, Assignment_2.BoltStock B ";

            string query_condition = " WHERE B.CategoryCode = F.FabricCode " +
                                   " AND B.ESupplierCode = S.SupplierCode " +
                                 $" AND S.SupplierCode LIKE '%{this_ID}%'  ";

            cmdText = select_table + query_table + query_condition;

            return cmdText;
        }

        // Part 4: Make report 
        private void report_find_Click(object sender, EventArgs e)
        {
            if (!this.isLogin)
            {
                MessageBox.Show("Please login first!");
                return;
            }

            data_report_upper.DataSource = null;
            data_report_lower.DataSource = null;

            string CID = text_report.Text; //Customer id
            if (CID.Length < 5)
            {
                MessageBox.Show("ID is too short! (Format: CCxxx)");
                return;
            }

            if (CID.Length > 10)
            {
                MessageBox.Show("ID is too long! (Format: CCxxxxxxxx)");
                return;
            }


            string first_2_Char = CID.Substring(0, 2);

            OracleCommand oc1 = new OracleCommand("Assignment_2.pkg_customer_params.set_customerID", this.dbConn);
            oc1.CommandType = CommandType.StoredProcedure;
            oc1.Parameters.Add("p_customerID", OracleDbType.Char, 5).Value = CID;

            OracleCommand oc2;

            if (first_2_Char != "CC")
            {
                MessageBox.Show("Incorrect ID! (Format: CCxxx)");
                return;
            }

            oc1.ExecuteNonQuery();
            oc2 = new OracleCommand("SELECT * FROM Assignment_2.order_detail_views", this.dbConn);
            OracleDataReader dr = oc2.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            data_report_upper.DataSource = dt;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void label1_Click_4(object sender, EventArgs e)
        {

        }

        private void label1_Click_5(object sender, EventArgs e)
        {

        }

        private void add_text_essn_TextChanged(object sender, EventArgs e)
        {

        }

        
    }
}
