using System;
using System.Data.SqlClient;
using System.Windows;


namespace Cinema_System.ControlWindow
{
    /// <summary>
    /// UpdateUserWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class UpdateUserWindow : Window
    {
        SqlConnection conn;
        int id;
        public UpdateUserWindow(int id)
        {
            conn = new SqlConnection(CinemaUtils.DATA_BASE_PATH);
            this.id = id;
            InitializeComponent();
            fillTextBoxes();

        }

        private void btnConfirmClick(object sender, RoutedEventArgs e)
        {

            string sName;
            string sSurname;
            string sUsername;
            string sPassword;
            int type;
            decimal amount;
           

            string spSql = "exec up_UpdateUserDetails @user_id=@mid," +
                            "@name=@sName," +
                            "@surname=@sSurname," +
                            "@username=@sUsername," +
                            "@password=@sPassword," +
                            "@user_type=@type," +
                            "@user_money=@money";

            SqlCommand cmd = new SqlCommand(spSql, conn);

            if (txtbxName.Text == null || txtbxSurname.Text == null || 
                txtbxUsername.Text == null 
                || txtbxPassword.Password == null 
                || txtbxUsertype.Text == null 
                || int.TryParse(txtbxUsertype.Text,out type) == false 
                || decimal.TryParse(txtbxUserMoney.Text, out amount) == false
                )
            {
                CinemaUtils.warningMessage("MAKE SURE FILLED EVERY TEXTBOX WITH RIGHT DATA TYPES");

            }else if (checkIfUsernameExists(txtbxUsername.Text))
            {
                CinemaUtils.warningMessage("USERNAME IS ALREADY TAKEN CHANGE THE USERNAME");
            }
            else
            {
                conn.Open();
                sName = txtbxName.Text;
                sSurname = txtbxSurname.Text;
                sUsername = txtbxUsername.Text;
                sPassword = txtbxPassword.Password;
                type = Convert.ToInt16(txtbxUsertype.Text);

                cmd.Parameters.AddWithValue("@mid", this.id);
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

        private void fillTextBoxes()
        {
            conn.Open();
            string spSql = "exec up_GetAllUserDataById @id=@ourVar";

            SqlCommand cmd = new SqlCommand(spSql, conn);
            cmd.Parameters.AddWithValue("@ourVar", (object)this.id);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            txtbxId.Text = this.id.ToString();
            txtbxName.Text = dr["name"].ToString();
            txtbxSurname.Text = dr["surname"].ToString();
            txtbxUsername.Text = dr["username"].ToString();
            txtbxPassword.Password = dr["password"].ToString();
            txtbxUsertype.Text = dr["user_type"].ToString();
            txtbxUserMoney.Text = dr["user_money"].ToString();

            conn.Close();
        }

        private bool checkIfUsernameExists(string username)
        {
            conn.Open();
            string sqlStr = "select * from tblUser where username=@usr";
            SqlCommand cmd = new SqlCommand(sqlStr, conn);
            SqlDataReader reader;

            cmd.Parameters.AddWithValue("@usr", username);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                conn.Close();
                return true;
            }

            conn.Close() ;
            return false;


        }
    }
}
