using System;
using System.Data.SqlClient;
using System.Windows;

namespace Cinema_System
{
    /// <summary>
    /// DeleteMovieWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class DeleteMovieWindow : Window
    {
        SqlConnection conn;
        public DeleteMovieWindow()
        {
            conn = new SqlConnection(CinemaUtils.DATA_BASE_PATH);
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            //DialogResult = true;
            conn.Open();
            SqlCommand cmd;
            SqlCommand cmdGetName;
            SqlDataReader read;
            int id = -1;
            int rowCount = 0;
            string sqlStr = "UPDATE tblMovie " +
                            "set is_deleted='true',movie_name='[deleted]' " +
                            "where movie_id=@id";
            string sqlGetName = "select movie_name from tblMovie where movie_id=@id";
            string movie_name;
            if (string.IsNullOrEmpty(txtbxMovieId.Text))
            {
                MessageBox.Show("PUT VALID ID IN BOX");
            }
            else if(!int.TryParse(txtbxMovieId.Text,out id))
            {
                MessageBox.Show("PUT VALID ID IN BOX");
            }
            else
            {
                id = Convert.ToInt16(txtbxMovieId.Text);
                cmd = new SqlCommand(sqlStr,conn);
                cmdGetName = new SqlCommand(sqlGetName,conn);
                cmd.Parameters.AddWithValue("@id", (object)id);
                cmdGetName.Parameters.AddWithValue("@id", (object)id);
                read = cmdGetName.ExecuteReader();
                read.Read();
                movie_name = read["movie_name"].ToString();
                read.Close();
                rowCount = cmd.ExecuteNonQuery();
                if (rowCount > 0 && movie_name != "[deleted]")
                {
                    CinemaUtils.infoMessage("SUCCESSFULLY DELETED", "DELETE");
                }
                else
                {
                    CinemaUtils.errorMessage("COULD NOT DELETE", "DELETE");
                }
                txtbxMovieId.Text = "";
            }

            conn.Close();
            //this.Close();
        }
    }
}
