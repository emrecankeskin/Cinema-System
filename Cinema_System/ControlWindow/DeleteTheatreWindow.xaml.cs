using System.Windows;
using System.Data.SqlClient;

namespace Cinema_System.ControlWindow
{
    /// <summary>
    /// DeleteTheatreWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class DeleteTheatreWindow : Window
    {
        SqlConnection conn;
        public DeleteTheatreWindow()
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
            string sqlStr = "UPDATE tblTheatre " +
                            "set is_deleted='true',theatre_name=@name " +
                            "where theatre_id=@id";
            string sqlGetName = "select theatre_name from tblTheatre where theatre_id=@id";
            string theatre_name;
            if (string.IsNullOrEmpty(txtbxTheatreId.Text))
            {
                MessageBox.Show("PUT VALID ID IN BOX");
            }
            else if (!int.TryParse(txtbxTheatreId.Text, out id))
            {
                MessageBox.Show("PUT VALID ID IN BOX");
            }
            else
            {
                //id = Convert.ToInt16(txtbxMovieId.Text);
                cmd = new SqlCommand(sqlStr, conn);
                cmdGetName = new SqlCommand(sqlGetName, conn);


                cmd.Parameters.AddWithValue("@id", id);
                cmdGetName.Parameters.AddWithValue("@id", id);
                // some of my functions use theatre name to find theatre id to prevent bug in program
                // i set name of the theatre to deleted
                cmd.Parameters.AddWithValue("@name", "[deleted]");

                read = cmdGetName.ExecuteReader();
                read.Read();
                theatre_name = read["theatre_name"].ToString();
                read.Close();

                rowCount = cmd.ExecuteNonQuery();
                if (rowCount > 0 && theatre_name != "[deleted]")
                {
                    CinemaUtils.infoMessage("SUCCESSFULLY DELETED", "DELETE");
                }
                else
                {
                    CinemaUtils.errorMessage("COULD NOT DELETE", "DELETE");
                }

                txtbxTheatreId.Text = "";
            }

            conn.Close();
        }
    }
}
