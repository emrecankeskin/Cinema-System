using System.Windows;
using System.Data.SqlClient;


namespace Cinema_System.ControlWindow
{
    /// <summary>
    /// DeleteUserWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class DeleteUserWindow : Window
    {
        SqlConnection conn;
        public DeleteUserWindow()
        {
            conn = new SqlConnection(CinemaUtils.DATA_BASE_PATH);
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            conn.Open();
            SqlCommand cmd;
            SqlCommand getCmd;
            SqlDataReader getReader;
            int id = -1;
            int rowCount = 0;
            string sqlStr = "UPDATE tblUser " +
                            "set is_deleted='true',username=@name " +
                            "where user_id=@id";
            string getUsername = "select username from tblUser where user_id=@id";
            string user_name;

            if (string.IsNullOrEmpty(txtbxUserId.Text))
            {
                MessageBox.Show("PUT VALID ID IN BOX");
            }
            else if (!int.TryParse(txtbxUserId.Text, out id))
            {
                MessageBox.Show("PUT VALID ID IN BOX");
            }
            else
            {
                //id = Convert.ToInt16(txtbxMovieId.Text);
                cmd = new SqlCommand(sqlStr, conn);
                getCmd = new SqlCommand(getUsername, conn);
                cmd.Parameters.AddWithValue("@id", id);
                getCmd.Parameters.AddWithValue("@id", id);
                //changing name to deleted because with that way other users can take deleted username
                cmd.Parameters.AddWithValue("@name", "[deleted]");
                
                getReader = getCmd.ExecuteReader();
                getReader.Read();
                user_name = getReader["username"].ToString();
                getReader.Close();
                rowCount = cmd.ExecuteNonQuery();
                if (rowCount > 0 && user_name != "[deleted]")
                {
                    CinemaUtils.infoMessage("SUCCESSFULLY DELETED", "DELETE");
                }
                else
                {
                    CinemaUtils.errorMessage("COULD NOT DELETE", "DELETE");
                }

                txtbxUserId.Text = "";
            }

            conn.Close();
        }

    }
}
