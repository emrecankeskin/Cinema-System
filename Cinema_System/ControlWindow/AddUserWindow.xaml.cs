using System;
using System.Data.SqlClient;
using System.Windows;


namespace Cinema_System.ControlWindow
{
    /// <summary>
    /// AddUserWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class AddUserWindow : Window
    {
        SqlConnection conn;
        public AddUserWindow()
        {
            conn = new SqlConnection(CinemaUtils.DATA_BASE_PATH);
            InitializeComponent();
        }

        private void btnConfirmClick(object sender, RoutedEventArgs e)
        {
            conn.Open();

            string sName;
            string sSurname;
            string sUsername;
            string sPassword;

            int type;
            decimal amount;


            string spSql = "exec up_AddUser @name=@sName," +
                            "@surname=@sSurname," +
                            "@username=@sUsername," +
                            "@password=@sPassword," +
                            "@user_type=@type," +
                            "@user_money=@money ";

            SqlCommand cmd = new SqlCommand(spSql, conn);

            if (txtbxName.Text == null || txtbxSurname.Text == null || txtbxUsername.Text == null
                || txtbxPassword.Password == null || txtbxUsertype.Text == null || int.TryParse(txtbxUsertype.Text, out type) == false
                || txtbxUsername.Text.Length < 5 || txtbxPassword.Password.Length < 8 || decimal.TryParse(txtbxUserMoney.Text, out amount) == false)
            {
                CinemaUtils.warningMessage("MAKE SURE FILLED EVERY TEXTBOX WITH RIGHT DATA TYPES");

            }
            else
            {

                SqlCommand cmdUsername = new SqlCommand("SELECT USERNAME FROM tblUser WHERE USERNAME = @s1", conn);
                SqlDataReader reader;
                cmdUsername.Parameters.AddWithValue("@s1", txtbxUsername.Text);
                reader = cmdUsername.ExecuteReader();
                if (reader.Read())
                {
                    reader.Close();
                    MessageBox.Show("[*] USERNAME ALREADY TAKEN PLEASE CHOOSE ANOTHER USERNAME", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
                else
                {
                    reader.Close();
                    sName = txtbxName.Text;
                    sSurname = txtbxSurname.Text;
                    sUsername = txtbxUsername.Text;
                    sPassword = txtbxPassword.Password;

                    type = Convert.ToInt16(txtbxUsertype.Text);
                    amount = Convert.ToDecimal(txtbxUserMoney.Text);

                    cmd.Parameters.AddWithValue("@sName", sName);
                    cmd.Parameters.AddWithValue("@sSurname", sSurname);
                    cmd.Parameters.AddWithValue("@sUsername", sUsername);
                    cmd.Parameters.AddWithValue("@sPassword", sPassword);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@money", amount);

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
                    clearBoxes();
                }

            

            }


            conn.Close();
        }

        private void clearBoxes()
        {
            txtbxName.Clear();
            txtbxPassword.Clear();
            txtbxSurname.Clear();
            txtbxUsername.Clear();
            txtbxUsertype.Clear();
            txtbxUserMoney.Clear();
        }
    }
}
