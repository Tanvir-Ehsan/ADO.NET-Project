using ProductManagement.Entities;
using ProductManagement.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProductManagement
{
    public partial class Purchase : Form
    {
        string conStr =ConfigurationManager.ConnectionStrings["db"].ConnectionString;
        int empId = 200;
        int purchaseId = 0;
        string imgName;
        string filePath = "";
        string folderPath = @"E:\Round_53\Projects\ADO\ProductManagement\ProductManagement\ProductImage\";
        string imagePathFromData;

        public Purchase()
        {
            InitializeComponent();
        }
        private void Purchase_Load(object sender, EventArgs e)
        {
            LoadEmployeeName();
            LoadComboproduct();
            LoadCombosupplier();
            pbProduct.Image = Resources.noimage;
            LoadGridView();
            //btnHide.Visible=false;
        }

        private void LoadCombosupplier()
        {
            string sqlQuery = "SELECT * FROM Supplier_t";
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand(sqlQuery, con);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader, LoadOption.Upsert);
            if (dt != null)
            {
                cmbSupplier.DisplayMember = "SupplierName";
                cmbSupplier.ValueMember = "SupplierId";
                cmbSupplier.DataSource = dt;
            }
            con.Close();
        }

        private void LoadComboproduct()
        {
            string sqlQuery = "SELECT * FROM Product_t";
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand(sqlQuery, con);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            DataTable dt=new DataTable();
            dt.Load(reader, LoadOption.Upsert);
            if (dt != null)
            {
                cmbProductName.DisplayMember = "ProductName";
                cmbProductName.ValueMember = "ProductId";
                cmbProductName.DataSource = dt;
            }           
            con.Close();
        }

        private void LoadEmployeeName()
        {
            string userName = "";
            string sqlQuery = "SELECT EmployeeName FROM Employee_t WHERE EmployeeId='"+empId+"'";
            SqlConnection con=new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand(sqlQuery, con);
            con.Open();
            userName=(cmd.ExecuteScalar()).ToString();
            if(userName=="")
            {
                lblEmployeeName.Text = "Unknown";
            }
            lblEmployeeName.Text = userName;
            con.Close();
        }

        private void btnSaveProduct_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtProduct.Text) == true)
            {
                txtProduct.Focus();
                errorProvider1.SetError(this.txtProduct, "Please Enter productName");
                ClearMethod();
            }
            else
            {
                Product objProduct = new Product();
                objProduct.ProductName= txtProduct.Text;
                string sqlQuery = "Insert INTO Product_t (ProductName) VALUES ('" + objProduct.ProductName + "')";
                SqlConnection con = new SqlConnection(conStr);
                SqlCommand cmd = new SqlCommand(sqlQuery, con);
                con.Open();
                int rowCount = cmd.ExecuteNonQuery();
                if (rowCount > 0)
                {
                    MessageBox.Show("Product added successfully!", "Success",MessageBoxButtons.OK) ;
                    ClearMethod();
                }
                else
                {
                    MessageBox.Show("Product Insertion failed!", "Failure", MessageBoxButtons.OK);
                    ClearMethod();
                }
                con.Close();
                LoadComboproduct();

            }
        }

        private void ClearMethod()
        {
            txtProduct.Text = "";
            txtSupplier.Text = "";
            cmbProductName.Text = "";
            cmbSupplier.Text = "";
            txtVoucherNo.Text = "";
            txtUnitPrice.Text = "";
            txtQuantity.Text = "";
            pbProduct.Image= Resources.noimage;
            ShowTotalAmount();
        }

        private void ShowTotalAmount()
        {

            string sqlQuery = "SELECT SUM(TotalPrice) AS TotalAmount FROM Purchase_t";
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand(sqlQuery, con);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                lblTotalAmount.Text = reader["TotalAmount"].ToString();
            }
            else
            {
                lblTotalAmount.Text = "00.00";
            }
            con.Close();
        }

        private void btnSaveSupplier_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSupplier.Text) == true)
            {
                txtSupplier.Focus();
                errorProvider1.SetError(this.txtSupplier, "Please Enter Supplier Name");
                ClearMethod();
            }
            else
            {
                Supplier objSupplier=new Supplier();
                objSupplier.SupplierName = txtSupplier.Text;
                string sqlQuery = "Insert INTO Supplier_t (SupplierName) VALUES ('" + objSupplier.SupplierName + "')";
                SqlConnection con = new SqlConnection(conStr);
                SqlCommand cmd = new SqlCommand(sqlQuery, con);
                con.Open();
                int rowCount = cmd.ExecuteNonQuery();
                if (rowCount > 0)
                {
                    MessageBox.Show("Supplier added successfully!", "Success", MessageBoxButtons.OK);
                    ClearMethod();
                }
                else
                {
                    MessageBox.Show("Supplier Insertion failed!", "Failure", MessageBoxButtons.OK);
                    ClearMethod();
                }
                con.Close();
                LoadCombosupplier();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SqlTransaction transaction;
            
            Purchase_t objPurchase=new Purchase_t();
            decimal unitPrice = Convert.ToDecimal(txtUnitPrice.Text);
            objPurchase.SupplierId = Convert.ToInt16(cmbSupplier.SelectedValue);
            objPurchase.Quantity = Convert.ToInt16(txtQuantity.Text);
            objPurchase.TotalPrice = unitPrice* objPurchase.Quantity;
            objPurchase.EmployeeId = empId;
            objPurchase.VoucherNo = Convert.ToInt16(txtVoucherNo.Text);
            objPurchase.PruchaseDate = Convert.ToDateTime(dtpPurchaseDate.Text);
            objPurchase.ProductId = Convert.ToInt16(cmbProductName.SelectedValue);
            objPurchase.ImgPath = folderPath + Path.GetFileName(openFileDialog1.FileName);
            string sqlQuery = "Insert INTO Purchase_t VALUES (@VoucherNo,@PruchaseDate,@Quantity,@TotalPrice,@ImgPath,@ProductId,@SupplierId,@EmployeeId)";
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand(sqlQuery, con);
            if (filePath == "")
            {
                cmd.Parameters.AddWithValue("@ImgPath", "No Image Found");
            }
            else
            {
                cmd.CommandType= CommandType.Text;
                cmd.Parameters.AddWithValue("@ImgPath", objPurchase.ImgPath);
                try
                {
                    File.Copy(filePath, Path.Combine(folderPath, Path.GetFileName(filePath)), true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            cmd.Parameters.AddWithValue("@VoucherNo", objPurchase.VoucherNo);
            cmd.Parameters.AddWithValue("@PruchaseDate", objPurchase.PruchaseDate);
            cmd.Parameters.AddWithValue("@Quantity", objPurchase.Quantity);
            cmd.Parameters.AddWithValue("@TotalPrice", objPurchase.TotalPrice);            
            cmd.Parameters.AddWithValue("@ProductId", objPurchase.ProductId);
            cmd.Parameters.AddWithValue("@SupplierId", objPurchase.SupplierId);
            cmd.Parameters.AddWithValue("@EmployeeId", objPurchase.EmployeeId);
            con.Open();
            transaction = con.BeginTransaction();
            int rowCount = cmd.ExecuteNonQuery();
            if(rowCount > 0)
            {
                MessageBox.Show("Purchase added successfully!", "Success", MessageBoxButtons.OK);
                ClearMethod();
                transaction.Commit();
            }
            else
            {
                MessageBox.Show("Purchase Insertion failed!", "Failure", MessageBoxButtons.OK);
                ClearMethod();
                transaction.Rollback();
            }
            
            con.Close();
            LoadGridView();
            ClearMethod();
        }

        private void LoadGridView()
        {
            ShowImageInGridView();
            //string sqlQuery = "SELECT pu.VoucherNo, pu.PurchaseDate, pu.Quantity, pu.TotalPrice, po.ProductName, su.SupplierName, em.EmployeeName FROM Purchase_t pu JOIN Product_t po ON pu.ProductId=po.ProductId JOIN Supplier_t su ON pu.SupplierId=su.SupplierId JOIN Employee_t em ON pu.EmployeeId=em.EmployeeId";
            //SqlConnection con = new SqlConnection(conStr);
            //SqlDataAdapter sda=new SqlDataAdapter(sqlQuery, con);
            //DataTable dt = new DataTable();
            //sda.Fill(dt);
            //dgvPurchase.RowTemplate.Height = 30;
            //dgvPurchase.DataSource = dt;
            //dgvPurchase.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image File(*.jpg; *.png; *.jpeg; *.gif; *.bmp)| *.jpg; *.png; *.jpeg; *.gif; *.bmp|all files|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                imgName=openFileDialog1.SafeFileName;
                pbProduct.Image=new Bitmap(openFileDialog1.FileName);
                filePath=openFileDialog1.FileName;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearMethod();
        }

       

        private void ShowImageInGridView()
        {
            string sqlQuery = "SELECT pu.PurchaseId,pu.VoucherNo, pu.PurchaseDate, pu.Quantity, pu.TotalPrice, po.ProductName, su.SupplierName, em.EmployeeName,pu.ImgPath FROM Purchase_t pu JOIN Product_t po ON pu.ProductId=po.ProductId JOIN Supplier_t su ON pu.SupplierId=su.SupplierId JOIN Employee_t em ON pu.EmployeeId=em.EmployeeId";
            SqlConnection con = new SqlConnection(conStr);
            SqlDataAdapter sda = new SqlDataAdapter(sqlQuery, con);
            DataTable dt = new DataTable();
            con.Open();
            sda.Fill(dt);
            dt.Columns.Add("Picture",Type.GetType("System.Byte[]"));
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    dr["Picture"] = File.ReadAllBytes(dr["ImgPath"].ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            con.Close();
            dgvPurchase.RowTemplate.Height = 52;
            dgvPurchase.DataSource= dt;
            DataGridViewImageColumn dgvImage=new DataGridViewImageColumn();
            dgvImage = (DataGridViewImageColumn)dgvPurchase.Columns[9];
            dgvImage.ImageLayout = DataGridViewImageCellLayout.Stretch;
            dgvPurchase.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        

        private void dgvPurchase_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int cellId = e.RowIndex;
            try
            {
                DataGridViewRow row = dgvPurchase.Rows[cellId];
                lblPurchaseId.Text= row.Cells[0].Value.ToString();
                txtVoucherNo.Text = row.Cells[1].Value.ToString();                
                dtpPurchaseDate.Text = row.Cells[2].Value.ToString();
                txtQuantity.Text = row.Cells[3].Value.ToString();
                decimal totalPrice= Convert.ToDecimal(row.Cells[4].Value.ToString());
                int quantity= Convert.ToInt32(row.Cells[3].Value.ToString());
                decimal unitPrice = totalPrice / quantity;
                txtUnitPrice.Text = unitPrice.ToString();
                cmbProductName.Text = row.Cells[5].Value.ToString();
                cmbSupplier.Text = row.Cells[6].Value.ToString();
                if (imagePathFromData=="No Image")
                {
                    pbProduct.Image = Resources.noimage;
                }
                byte[] data = (byte[])row.Cells[9].Value;
                MemoryStream stream=new MemoryStream(data);
                pbProduct.Image=Image.FromStream(stream);
                imagePathFromData = row.Cells[8].Value.ToString();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(lblPurchaseId.Text))
            {
                Purchase_t objPurchase = new Purchase_t();
                decimal unitPrice = Convert.ToDecimal(txtUnitPrice.Text);
                objPurchase.SupplierId = Convert.ToInt16(cmbSupplier.SelectedValue);
                objPurchase.Quantity = Convert.ToInt16(txtQuantity.Text);
                objPurchase.TotalPrice = unitPrice * objPurchase.Quantity;
                objPurchase.EmployeeId = empId;
                objPurchase.VoucherNo = Convert.ToInt16(txtVoucherNo.Text);
                objPurchase.PruchaseDate = Convert.ToDateTime((dtpPurchaseDate.Text));
                objPurchase.PurchaseId = Convert.ToInt16(lblPurchaseId.Text);
                objPurchase.ProductId = Convert.ToInt16(cmbProductName.SelectedValue);
                objPurchase.ImgPath = folderPath + Path.GetFileName(openFileDialog1.FileName);
                string sqlQuery = "UPDATE Purchase_t SET PurchaseDate = @PruchaseDate, Quantity=@Quantity, SupplierId=@SupplierId,TotalPrice=@TotalPrice,EmployeeId=@EmployeeId,ProductId=@ProductId,ImgPath=@ImgPath WHERE PurchaseId=@PurchaseId";
                SqlConnection con = new SqlConnection(conStr);
                SqlCommand cmd = new SqlCommand(sqlQuery, con);
                if (filePath == "")
                {
                    cmd.Parameters.AddWithValue("@ImgPath", imagePathFromData);
                }
                else
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@ImgPath", objPurchase.ImgPath);
                    try
                    {
                        File.Copy(filePath, Path.Combine(folderPath, Path.GetFileName(filePath)), true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                cmd.Parameters.AddWithValue("@VoucherNo", objPurchase.VoucherNo);
                cmd.Parameters.AddWithValue("@PruchaseDate", objPurchase.PruchaseDate);
                cmd.Parameters.AddWithValue("@Quantity", objPurchase.Quantity);
                cmd.Parameters.AddWithValue("@TotalPrice", objPurchase.TotalPrice);
                cmd.Parameters.AddWithValue("@ProductId", objPurchase.ProductId);
                cmd.Parameters.AddWithValue("@SupplierId", objPurchase.SupplierId);
                cmd.Parameters.AddWithValue("@EmployeeId", objPurchase.EmployeeId);
                cmd.Parameters.AddWithValue("@PurchaseId", objPurchase.PurchaseId);
                con.Open();
                int rowCount = cmd.ExecuteNonQuery();
                if (rowCount > 0)
                {
                    lblPurchaseId.Text = "";
                    MessageBox.Show("Purchase added successfully!", "Success", MessageBoxButtons.OK);
                    ClearMethod();
                }
                else
                {
                    MessageBox.Show("Purchase Insertion failed!", "Failure", MessageBoxButtons.OK);
                    ClearMethod();
                }
                con.Close();
                LoadGridView();
                ClearMethod();
            }
            else
            {
                MessageBox.Show("Please select Purchase Id!", "Warning", MessageBoxButtons.OK);
            }
            
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(lblPurchaseId.Text))
            {
                Purchase_t objPurchase = new Purchase_t();             
                objPurchase.PurchaseId = Convert.ToInt16(lblPurchaseId.Text);                
                string sqlQuery = "DELETE FROM  Purchase_t WHERE PurchaseId=@PurchaseId";
                SqlConnection con = new SqlConnection(conStr);
                SqlCommand cmd = new SqlCommand(sqlQuery, con);      
                cmd.Parameters.AddWithValue("@PurchaseId", objPurchase.PurchaseId);
                con.Open();
                int rowCount = cmd.ExecuteNonQuery();
                if (rowCount > 0)
                {
                    lblPurchaseId.Text = "";
                    MessageBox.Show("Deleted successfully!", "Success", MessageBoxButtons.OK);
                    ClearMethod();
                }
                else
                {
                    MessageBox.Show("Deletion failed!", "Failure", MessageBoxButtons.OK);
                    ClearMethod();
                }
                con.Close();
                LoadGridView();
                ClearMethod();
            }
            else
            {
                MessageBox.Show("Please select Purchase Id!", "Warning", MessageBoxButtons.OK);
            }
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            List<PurchaseViewModel> list = new List<PurchaseViewModel>();
            using (SqlConnection con=new SqlConnection(conStr))
            {
                string SqlQuery = "SELECT pu.PurchaseId,pu.EmployeeId,pu.SupplierId,pu.ProductId,pu.VoucherNo,pu.PurchaseDate, pu.Quantity, pu.TotalPrice, po.ProductName, su.SupplierName,em.EmployeeName,pu.ImgPath FROM Purchase_t pu JOIN Product_t po ON pu.ProductId=po.ProductId JOIN Supplier_t su ON pu.SupplierId=su.SupplierId JOIN Employee_t em ON pu.EmployeeId=em.EmployeeId";
                SqlDataAdapter sda = new SqlDataAdapter(SqlQuery, con);
                DataTable dt = new DataTable();
                con.Open();
                sda.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PurchaseViewModel objPurchase = new PurchaseViewModel();
                    objPurchase.ProductId = Convert.ToInt32(dt.Rows[i]["ProductId"].ToString());
                    objPurchase.SupplierId = Convert.ToInt32(dt.Rows[i]["SupplierId"].ToString());
                    objPurchase.EmployeeId = Convert.ToInt32(dt.Rows[i]["EmployeeId"].ToString());
                    objPurchase.VoucherNo = Convert.ToInt32(dt.Rows[i]["VoucherNo"].ToString());
                    objPurchase.PurchaseDate = Convert.ToDateTime(dt.Rows[i]["PurchaseDate"].ToString());
                    objPurchase.Quantity = Convert.ToInt32(dt.Rows[i]["Quantity"].ToString());
                    objPurchase.TotalPrice = Convert.ToDecimal(dt.Rows[i]["TotalPrice"].ToString());
                    objPurchase.ProductName = dt.Rows[i]["ProductName"].ToString();
                    objPurchase.SupplierName = dt.Rows[i]["SupplierName"].ToString();
                    objPurchase.EmployeeName = dt.Rows[i]["EmployeeName"].ToString();
                    objPurchase.ImgPath = dt.Rows[i]["ImgPath"].ToString();
                    objPurchase.PurchaseId = Convert.ToInt32(dt.Rows[i]["PurchaseId"].ToString());
                    list.Add(objPurchase);
                }
            }
            using (Form1 form=new Form1(list))
            {
                form.ShowDialog();
            }
            
        }
    }
}
