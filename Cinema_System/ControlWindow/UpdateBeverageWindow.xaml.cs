using System;
using System.Data.SqlClient;
using System.Windows;

namespace Cinema_System.ControlWindow
{
    /// <summary>
    /// UpdateBeverageWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class UpdateBeverageWindow : Window
    {

        int id;
        SqlConnection conn;
        public UpdateBeverageWindow(int id)
        {
            InitializeComponent();
            conn = new SqlConnection(CinemaUtils.DATA_BASE_PATH);
            this.id = id;
            fillTextBoxes();
        }


        private void fillTextBoxes()
        {
            conn.Open();
            string spSql = "exec up_GetAllBeverageDataById @id=@ourVar";

            SqlCommand cmd = new SqlCommand(spSql, conn);
            cmd.Parameters.AddWithValue("@ourVar", (object)this.id);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            txtbxId.Text = this.id.ToString();
            txtbxName.Text = dr["name"].ToString();
            txtbxStock.Text = dr["stock"].ToString();
            txtbxPrice.Text = dr["price"].ToString();

            conn.Close();
        }

        private void btnConfirmClick(object sender, RoutedEventArgs e)
        {
            conn.Open();

            string beverageName;
            decimal beveragePrice;
            string beverageStock;

            int stock;
            decimal price;

            string spSql = "exec up_UpdateBeverageDetails @beverage_id=@mid, " +
                            "@price=@bevPrice, " +
                            "@name=@bevName, "+
                            "@stock=@bevStock";

            SqlCommand cmd = new SqlCommand(spSql, conn);

            

            if (txtbxPrice.Text == null  || txtbxStock.Text == null || txtbxName.Text == null 
                || int.TryParse(txtbxStock.Text,out stock) == false || decimal.TryParse(txtbxPrice.Text, out price) == false)
            {
                //cinema utilste yeni yapacam
                MessageBox.Show("MAKE SURE FILLED EVERY TEXTBOX AND USE RIGHT DATA TYPES ");

            }
            else
            {
                beverageName = txtbxName.Text;
                beveragePrice = Convert.ToDecimal(txtbxPrice.Text);
                beverageStock = txtbxStock.Text;

                cmd.Parameters.AddWithValue("@mid", this.id);
                cmd.Parameters.AddWithValue("@bevPrice", beveragePrice);
                cmd.Parameters.AddWithValue("@bevName", beverageName );
                cmd.Parameters.AddWithValue("@bevStock", beverageStock);

                try
                {
                    int a = cmd.ExecuteNonQuery();
                    if (a > 0)
                    {
                        CinemaUtils.infoMessage("SUCCESSFULLY UPDATED", "DATABASE");
                    }
                    else
                    {
                        CinemaUtils.warningMessage("COULD NOT UPDATE");
                    }
                }
                catch
                {
                    CinemaUtils.infoMessage("DATABASE ERROR", "DATABASE");
                }

            }

            conn.Close();
        }
    }
}
