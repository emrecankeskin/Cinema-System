using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
namespace Cinema_System
{
    /// <summary>
    /// RegisterPage.xaml etkileşim mantığı
    /// </summary>
    public partial class RegisterPage : Page
    {
        SqlConnection conn;
        public RegisterPage()
        {
            InitializeComponent();
            conn = new SqlConnection(CinemaUtils.DATA_BASE_PATH);
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;
            main?.Close();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;
            main?.ChangeView(new LoginPage());
        }

        private void btnRegister_click(object sender, RoutedEventArgs e)
        {
            // check if boxes filled 
            // check if username exists
            //messageboxlara simge ve uyarı ekle
            if(boxPassword.Password.Length < 8 || string.IsNullOrWhiteSpace(boxPassword.Password))
            {
                CinemaUtils.warningMessage("PASSWORD MUST BE BIGGER THAN 8");
            }else if(boxUsername.Text.Length < 5 || string.IsNullOrWhiteSpace(boxUsername.Text))
            {
                CinemaUtils.warningMessage("USERNAME LENGTH MUST BE BIGGER THAN 5");
            }
            else if (boxSurname.Text.Length == 0 || string.IsNullOrWhiteSpace(boxSurname.Text))
            {
                CinemaUtils.warningMessage("SURNAME MUST BE FILLED");
            }
            else if (boxName.Text.Length == 0 ||  string.IsNullOrWhiteSpace(boxName.Text))
            {
                CinemaUtils.warningMessage("NAME MUST BE FILLED");
            }
            else
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT USERNAME FROM tblUser WHERE USERNAME = @s1", conn);
                    SqlDataReader reader;
                    cmd.Parameters.AddWithValue("@s1", boxUsername.Text);
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        reader.Close();
                        CinemaUtils.warningMessage("USERNAME ALREADY TAKEN PLEASE CHOOSE ANOTHER USERNAME");
                    }
                    else
                    {
                        // defaul registered type is user
                        // you can add doorsman or admin in adminpage controls not in registereation page
                        
                        reader.Close();
                        cmd = new SqlCommand("INSERT INTO tblUser(username,name,surname,password,user_type,user_money,is_deleted) values(@s1,@s2,@s3,@s4,@s5,@s6,@s7)", conn);
                        cmd.Parameters.AddWithValue("@s1", boxUsername.Text);
                        cmd.Parameters.AddWithValue("@s2", boxName.Text);
                        cmd.Parameters.AddWithValue("@s3", boxSurname.Text);
                        cmd.Parameters.AddWithValue("@s4", boxPassword.Password);
                        cmd.Parameters.AddWithValue("@s5", (object) 1);
                        cmd.Parameters.AddWithValue("@s6", (object) 0);
                        cmd.Parameters.AddWithValue("@s7", false);
                        int a = cmd.ExecuteNonQuery();
                        clear();
                        CinemaUtils.infoMessage("REGISTERATION COMPLETED", "SUCCESS");
                        
                    }
                    conn.Close();
                }catch(Exception ex)
                {
                    CinemaUtils.errorMessage(Name + ": " + ex.Message,"ERROR");
                }

            }

        }
        private void clear()
        {
            boxUsername.Text = "";
            boxSurname.Text = "";
            boxName.Text = "";
            boxPassword.Password = "";

        }
    }
}
