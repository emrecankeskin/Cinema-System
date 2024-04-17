using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;

namespace Cinema_System
{
    /// <summary>
    /// LoginPage.xaml etkileşim mantığı
    /// </summary>
    public partial class LoginPage : Page
    {
        SqlConnection conn = new SqlConnection(CinemaUtils.DATA_BASE_PATH);
        public LoginPage()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;
            main?.ChangeView(new RegisterPage());
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;
            conn.Open();
            String username, query, password;
            username = txtUsername.Text;
            password = txtPassword.Password;
            try
            {

                query = "SELECT * FROM tblUser WHERE username=@s1 AND password=@s2";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader;
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@s1", username);
                cmd.Parameters.AddWithValue("@s2", password);
                reader = cmd.ExecuteReader();
                bool is_deleted;

                // Reads one data or no data because program checks duplicate usernames when register so username is unique
                /*  0 -> admin 
                    1 -> user 
                    2 -> doorsman
                 */

                if (reader.Read())
                {
                    is_deleted = Convert.ToBoolean(reader["is_deleted"]);
                    if(is_deleted == false)
                    {
                        if (Convert.ToInt32(reader["user_type"]) == 0)
                        {
                            //MessageBox.Show("Admin login","LOGIN",MessageBoxButton.OK,MessageBoxImage.Information);
                            main?.ChangeView(new AdminPage(Convert.ToInt32(reader["user_id"])));
                        }
                        else if (Convert.ToInt32(reader["user_type"]) == 1)
                        {
                            //MessageBox.Show("User Login", "LOGIN", MessageBoxButton.OK, MessageBoxImage.Information);
                            main?.ChangeView(new UserPage(Convert.ToInt32(reader["user_id"])));
                        }
                        else if (Convert.ToInt32(reader["user_type"]) == 2)
                        {
                            //MessageBox.Show("Doorsman login", "LOGIN", MessageBoxButton.OK, MessageBoxImage.Information);
                            main?.ChangeView(new DoorsmanPage(Convert.ToInt32(reader["user_id"])));
                        }
                    }
                    else
                    {
                        CinemaUtils.warningMessage("DELETED ACCOUNT");
                    }

           
                }
                else
                {
                    CinemaUtils.warningMessage("INVALID PASSWORD OR USERNAME");
                }
                reader.Close();
            }
            catch(Exception ex)
            {
                CinemaUtils.errorMessage(Name + ": " + ex.Message, "ERROR");
            }
            conn.Close();

        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;
            main?.Close();
            
        }

    }
}
