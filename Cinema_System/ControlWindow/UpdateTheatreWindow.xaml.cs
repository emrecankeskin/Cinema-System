using System;
using System.Data.SqlClient;
using System.Windows;


namespace Cinema_System.ControlWindow
{
    /// <summary>
    /// UpdateTheatreWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class UpdateTheatreWindow : Window
    {
        SqlConnection conn;
        int id;
        public UpdateTheatreWindow(int theatreId)
        {
            conn = new SqlConnection(CinemaUtils.DATA_BASE_PATH);
            this.id = theatreId;
            InitializeComponent();
            fillTextBoxes();
        }

        private void btnConfirmClick(object sender, RoutedEventArgs e)
        {
            conn.Open();
            int capacity;
            bool vip;

            string theatreCapacity = txtbxCapacity.Text;
            string theatreName = txtbxName.Text;
            string vipStatus = txtbxVip.Text;

            string spSql = "exec up_UpdateTheatreDetails @theatre_id=@mid, " +
                            "@theatre_name=@name, " +
                            "@theatre_capacity=@capacity,"+
                            "@theatre_vip=@vip";

            SqlCommand cmd = new SqlCommand(spSql, conn);
            if (theatreName == null || 
                theatreCapacity == null || 
                int.TryParse(txtbxCapacity.Text, out capacity) == false ||
                bool.TryParse(txtbxVip.Text,out vip) == false)
            {
                //cinema utilste yeni yapacam
                CinemaUtils.warningMessage("MAKE SURE FILLED EVERY TEXTBOX ");

            }else if (vip && capacity==64)
            {
                CinemaUtils.infoMessage("VIP THEATRE'S CAPACITY IS ONLY 32!","CAPACITY");
            }else if(!vip && capacity == 32)
            {
                CinemaUtils.infoMessage("NON VIP THEATRE'S CAPACITY IS ONLY 64!", "CAPACITY");
            }
            else
            {
                cmd.Parameters.AddWithValue("@mid", this.id);
                cmd.Parameters.AddWithValue("@name", theatreName);
                cmd.Parameters.AddWithValue("@capacity", theatreCapacity);
                cmd.Parameters.AddWithValue("@vip", vipStatus);

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
            string spSql = "exec up_GetAllTheatreDataById @id=@ourVar";

            SqlCommand cmd = new SqlCommand(spSql, conn);
            cmd.Parameters.AddWithValue("@ourVar", (object)this.id);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            txtbxId.Text = this.id.ToString();
            txtbxName.Text = dr["theatre_name"].ToString();
            txtbxCapacity.Text = dr["theatre_capacity"].ToString();
            txtbxVip.Text = dr["theatre_vip"].ToString();

            conn.Close();
        }
    }
}
