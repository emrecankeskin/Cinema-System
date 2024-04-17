
using System.Windows;
using System.Data.SqlClient;
using System;


namespace Cinema_System.ControlWindow
{
    /// <summary>
    /// AddTheatreWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class AddTheatreWindow : Window
    {
        SqlConnection conn;
        int user_id = 0;    

        public AddTheatreWindow(int user_id)
        {
            conn = new SqlConnection(CinemaUtils.DATA_BASE_PATH);
            InitializeComponent();
            this.user_id = user_id;
        }

        private void btnConfirmClick(object sender, RoutedEventArgs e)
        {

            int capacity;
            bool vip;

            string theatreCapacity = txtbxCapacity.Text;
            string theatreName = txtbxName.Text;
            string vipStatus = txtbxVip.Text;

            string spSql = "exec up_AddTheatre @theatre_name=@name, " +
                            "@theatre_capacity=@capacity," +
                            "@theatre_vip=@vip," +
                            "@theatre_adder=@adder_id";

            SqlCommand cmd = new SqlCommand(spSql, conn);

            if (theatreName == null ||
                theatreCapacity == null ||
                int.TryParse(txtbxCapacity.Text, out capacity) == false ||
                bool.TryParse(txtbxVip.Text, out vip) == false)
            {
                CinemaUtils.warningMessage("MAKE SURE FILLED EVERY TEXTBOX ");

            }
            else if (vip && capacity == 64)
            {
                CinemaUtils.infoMessage("VIP THEATRE'S CAPACITY IS ONLY 32!", "CAPACITY");
            }
            else if (!vip && capacity == 32)
            {
                CinemaUtils.infoMessage("NON VIP THEATRE'S CAPACITY IS ONLY 64!", "CAPACITY");
            }
            else if (checkIfExist(txtbxName.Text))
            {
                CinemaUtils.warningMessage("THERE IS ALREADY THEATRE NAMED :" + txtbxName.Text);
            }
            else
            {
                conn.Open();
               
                cmd.Parameters.AddWithValue("@name", theatreName);
                cmd.Parameters.AddWithValue("@capacity", theatreCapacity);
                cmd.Parameters.AddWithValue("@vip", vipStatus);
                cmd.Parameters.AddWithValue("@adder_id", this.user_id);

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

            conn.Close();
        }
        private bool checkIfExist(string name)
        {
            conn.Open();
            string str = "select * from tblTheatre where theatre_name=@nam";
            SqlCommand cmd = new SqlCommand(str, conn);
            cmd.Parameters.AddWithValue("@nam", name);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                conn.Close();
                return true;
            }
            conn.Close();
            return false;
        }
        private void clearBoxes()
        {
            txtbxCapacity.Clear();
            txtbxName.Clear();
            txtbxVip.Clear();
        }
    }
}
