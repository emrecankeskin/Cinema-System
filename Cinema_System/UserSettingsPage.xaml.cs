using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;


namespace Cinema_System
{
    /// <summary>
    /// UserSettingsPage.xaml etkileşim mantığı
    /// </summary>
    public partial class UserSettingsPage : Page
    {
        SqlConnection conn;
        int user_id;
        public UserSettingsPage(int user_id)
        {
            conn = new SqlConnection(CinemaUtils.DATA_BASE_PATH);
            InitializeComponent();
            this.user_id = user_id;
        }

        private void btnConfirm_click(object sender, RoutedEventArgs e)
        {
            if (newBoxPassword.Password.Length < 8 || string.IsNullOrWhiteSpace(newBoxPassword.Password))
            {
                CinemaUtils.warningMessage("NEW PASSWORD MUST BE BIGGER THAN 8");
            }else if (!checkPasswordValidation(oldBoxPassword.Password))
            {
                CinemaUtils.warningMessage("WRONG PASSWORD TRY AGAIN");
            }
            else if (boxSurname.Text.Length == 0 || string.IsNullOrWhiteSpace(boxSurname.Text))
            {
                CinemaUtils.warningMessage("SURNAME MUST BE FILLED");
            }
            else if (boxName.Text.Length == 0 || string.IsNullOrWhiteSpace(boxName.Text))
            {
                CinemaUtils.warningMessage("NAME MUST BE FILLED");
            }
            else
            {
                conn.Open();
                string sqlStr = "update tblUser  " +
                                "set password=@pwd,name=@name, surname=@surname " +
                                "where user_id=@id";
                SqlCommand sqlCommand = new SqlCommand(sqlStr,conn);
                sqlCommand.Parameters.AddWithValue("@pwd", newBoxPassword.Password);
                sqlCommand.Parameters.AddWithValue("@name", boxName.Text);
                sqlCommand.Parameters.AddWithValue("@surname", boxSurname.Text);
                sqlCommand.Parameters.AddWithValue("@id", this.user_id);
                int a = sqlCommand.ExecuteNonQuery();
                if(a > 0)
                {
                    CinemaUtils.infoMessage("SUCCESSFULLY CHANGED INFO", "CHANGE");
                }
                else
                {
                    CinemaUtils.errorMessage("COULDN'T CHANGE INFO", "CHANGE");
                }
                conn.Close();
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;
            main?.ChangeView(new UserPage(this.user_id));
        }


        private bool checkPasswordValidation(string password)
        {
            conn.Open();
            string sqlStr = "select * from tblUser where user_id=@id";
            string pass_sql;
            SqlCommand sqlCommand = new SqlCommand(sqlStr, conn);
            sqlCommand.Parameters.AddWithValue("@id", this.user_id);
            SqlDataReader reader = sqlCommand.ExecuteReader();
            reader.Read();
            pass_sql = reader["password"].ToString();

            conn.Close();


            return password == pass_sql;
        }
    }
}
