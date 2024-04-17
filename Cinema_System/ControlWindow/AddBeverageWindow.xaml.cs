using System;
using System.Data.SqlClient;
using System.Windows;


namespace Cinema_System.ControlWindow
{
    /// <summary>
    /// AddBeverageWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class AddBeverageWindow : Window
    {

        SqlConnection conn;
        int user_id;
        public AddBeverageWindow(int user_id)
        {
            conn = new SqlConnection(CinemaUtils.DATA_BASE_PATH);
            InitializeComponent();
            this.user_id = user_id;
        }

        private void btnConfirmClick(object sender, RoutedEventArgs e)
        {

            string beverageName;
            decimal beveragePrice;
            string beverageStock;

            int stock;
            decimal price;

            // calling up_AddBeverage stored procedure
            string spSql = "exec up_AddBeverage @price=@bevPrice, " +
                            "@name=@bevName, " +
                            "@stock=@bevStock, " +
                            "@beverage_adder=@adder_id";

            SqlCommand cmd = new SqlCommand(spSql, conn);


            // Check if text boxes are empy our filled with wrong data types
            if (txtbxPrice.Text == null || 
                txtbxStock.Text == null || 
                txtbxName.Text == null || 
                int.TryParse(txtbxStock.Text, out stock) == false ||
                decimal.TryParse(txtbxPrice.Text, out price) == false)
            {
                CinemaUtils.warningMessage("MAKE SURE FILLED EVERY TEXTBOX AND USE RIGHT DATA TYPES");

            }
            else if (checkIfExist(txtbxName.Text))
            {
                CinemaUtils.warningMessage("BEVERAGE ALREADY EXISTS");
            }
            else
            {   
                conn.Open();
                beverageName = txtbxName.Text;
                beveragePrice = Convert.ToDecimal(txtbxPrice.Text);
                beverageStock = txtbxStock.Text;

                cmd.Parameters.AddWithValue("@bevPrice", beveragePrice);
                cmd.Parameters.AddWithValue("@bevName", beverageName);
                cmd.Parameters.AddWithValue("@bevStock", beverageStock);
                cmd.Parameters.AddWithValue("@adder_id", this.user_id);

                try
                {
                    int a = cmd.ExecuteNonQuery();
                    if (a > 0)
                    {
                        CinemaUtils.infoMessage("SUCCESSFULLY ADDED {" + txtbxName.Text + "}", "ADD");
                    }
                    else
                    {
                        CinemaUtils.warningMessage("COULD NOT ADD {" + txtbxName.Text + "}");
                    }
                }
                catch
                {
                    CinemaUtils.warningMessage("SQL ERROR");
                }

            }
            clearBoxes();
            conn.Close();
        }

        private bool checkIfExist(string name)
        {
            conn.Open();
            string str = "select * from tblBeverage where name=@nam";
            SqlCommand cmd = new SqlCommand(str, conn);
            cmd.Parameters.AddWithValue("@nam", name);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                conn.Close();
                return true;
            }
            conn.Close();
            return false;
        }
        private void clearBoxes()
        {
            txtbxName.Clear();  
            txtbxPrice.Clear();
            txtbxStock.Clear();
        }
    }
}
