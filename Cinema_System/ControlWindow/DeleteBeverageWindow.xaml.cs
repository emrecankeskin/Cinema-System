using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;

namespace Cinema_System.ControlWindow
{
    /// <summary>
    /// DeleteBeverageWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class DeleteBeverageWindow : Window
    {
        SqlConnection conn;
        public DeleteBeverageWindow()
        {
            conn = new SqlConnection(CinemaUtils.DATA_BASE_PATH);
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            conn.Open();
            SqlCommand cmd;
            SqlCommand cmdGetName;
            SqlDataReader read;
            int id = -1;
            int rowCount = 0;
            string sqlStr = "UPDATE tblBeverage " +
                            "set is_deleted='true', name='[deleted]' " +
                            "where beverage_id=@id";

            string sqlGetName = "select name from tblBeverage where beverage_id=@id";
            string bev_name;
            if (string.IsNullOrEmpty(txtbxBeverageId.Text))
            {
                CinemaUtils.errorMessage("PUT VALID ID IN BOX","ERROR");
            }
            else if (!int.TryParse(txtbxBeverageId.Text, out id))
            {
                CinemaUtils.errorMessage("PUT VALID ID INTEGER IN BOX", "ERROR");
            }
            else
            {
                //id = Convert.ToInt16(txtbxMovieId.Text);
                cmd = new SqlCommand(sqlStr, conn);
                cmdGetName = new SqlCommand(sqlGetName, conn);
                cmd.Parameters.AddWithValue("@id", (object)id);
                cmdGetName.Parameters.AddWithValue("@id", (object)id);
                read = cmdGetName.ExecuteReader();
                read.Read();
                bev_name = read["name"].ToString();
                read.Close();
                rowCount = cmd.ExecuteNonQuery();
                if (rowCount > 0 && bev_name != "[deleted]")
                {
                    CinemaUtils.infoMessage("SUCCESSFULLY DELETED", "DELETE");
                }
                else
                {
                    CinemaUtils.errorMessage("COULD NOT DELETE", "DELETE");
                }

                txtbxBeverageId.Text = "";
            }

            conn.Close();
        }
    }
}
