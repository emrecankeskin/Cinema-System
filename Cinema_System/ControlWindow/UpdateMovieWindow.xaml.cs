using System;
using System.Data.SqlClient;
using System.Windows;


namespace Cinema_System.ControlWindow
{
    /// <summary>
    /// UpdateMovieWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class UpdateMovieWindow : Window
    {
        int movie_id;
        SqlConnection conn;
        public UpdateMovieWindow(int movieId)
        {
            conn = new SqlConnection(CinemaUtils.DATA_BASE_PATH);
            this.movie_id = movieId;
            InitializeComponent();
            fillTextboxes();
        }

        private void btnConfirmClick(object sender, RoutedEventArgs e)
        {

            int tId;
            int movie_duration;
            int hour;
            int minute;
            decimal moviePrice;

            string movieName = txtbxMovieName.Text;
            DateTime? movieDate = dateMovie.SelectedDate;
            string theatreId = txtbxTheatreId.Text;
            string movieDuration = txtbxMovieDuration.Text;
            string spSql = "exec up_UpdateMovieDetails @movie_id=@mid," +
                            "@movie_name=@name," +
                            "@movie_date=@date," +
                            "@theatre_id=@tid," +
                            "@movie_price=@price, " +
                            "@movie_duration=@duration";

            SqlCommand cmd = new SqlCommand(spSql, conn);

            if(movieName == null || movieDate == null || theatreId == null ||
                txtbxHour.Text == null || txtbxMinute.Text == null ||
                int.TryParse(txtbxTheatreId.Text, out tId) == false ||
                int.TryParse(txtbxMovieDuration.Text, out movie_duration) == false  ||
                decimal.TryParse(txtbxMoviePrice.Text, out moviePrice) == false ||
                Convert.ToInt16(txtbxHour.Text) > 23 || 
                Convert.ToInt16(txtbxHour.Text) < 0 ||
                int.TryParse(txtbxHour.Text, out hour) == false || 
                int.TryParse(txtbxMinute.Text, out minute) == false ||
                Convert.ToInt16(txtbxMinute.Text) > 59 || 
                Convert.ToInt16(txtbxMinute.Text) < 0 || 
                checkTheatreExist(tId) == false )
                
            {
                //cinema utilste yeni yapacam
                MessageBox.Show("MAKE SURE FILLED EVERY TEXTBOX ");

            }
            else
            {
                
                DateTime temp = (DateTime)movieDate;
                DateTime newTime = new DateTime(temp.Year, temp.Month, temp.Day, hour, minute, 0);
                if (checkMovieCollusion(newTime,movie_duration,tId,movie_id))
                {
                    CinemaUtils.warningMessage("COLLISION BETWEEN ANOTHER MOVIE CHANGE TIME");
                }
                else
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@mid", this.movie_id);
                    cmd.Parameters.AddWithValue("@name", movieName);
                    cmd.Parameters.AddWithValue("@date", newTime);
                    cmd.Parameters.AddWithValue("@tid", theatreId);
                    cmd.Parameters.AddWithValue("@price", moviePrice);
                    cmd.Parameters.AddWithValue("@duration", movieDuration);

                    try
                    {
                        int a = cmd.ExecuteNonQuery();
                        if (a > 0)
                        {
                            CinemaUtils.infoMessage("SUCCESSFULLY UPDATED {" + a.ToString() + "} rows", "DATABASE");
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
               
            

            }


            conn.Close();

    }

        private void fillTextboxes()
        {
            conn.Open();
            string spSql = "exec up_GetAllMovieDataById @id=@ourVar";

            SqlCommand cmd = new SqlCommand(spSql, conn);
            cmd.Parameters.AddWithValue("@ourVar",(object) this.movie_id);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            DateTime time = Convert.ToDateTime(dr["movie_date"]);
            txtbxMovieId.Text = this.movie_id.ToString();
            txtbxMovieName.Text = dr["movie_name"].ToString();
            txtbxTheatreId.Text = dr["theatre_id"].ToString();
            dateMovie.Text = Convert.ToDateTime(dr["movie_date"]).ToString();
            txtbxMovieDuration.Text = dr["movie_duration"].ToString();
            txtbxMoviePrice.Text = dr["movie_price"].ToString();
            txtbxHour.Text = time.Hour.ToString();
            txtbxMinute.Text = time.Minute.ToString();

            conn.Close();
        }
        private bool checkMovieCollusion(DateTime time, int movie_duration, int theatre_id,int movie_id)
        {

            //checks collision given date and other dates 

            conn.Open();
            DateTime finishTime = time;
            finishTime.AddMinutes(movie_duration);
            string sqlStr = "select movie_id from tblMovie " +
                "where (@start_date<=DATEADD(minute,movie_duration,movie_date)) and (@end_date >= movie_date) and (theatre_id=@tid)";
            SqlCommand cmd = new SqlCommand(sqlStr, conn);

            cmd.Parameters.AddWithValue("@start_date", time);
            cmd.Parameters.AddWithValue("@end_date", finishTime);
            cmd.Parameters.AddWithValue("@tid", theatre_id);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if(Convert.ToInt32(reader["movie_id"]) != movie_id)
                {
                    conn.Close();
                    return true;
                }
            }
            conn.Close();
            return false;

        }


        private bool checkTheatreExist(int theatre_id)
        {
            conn.Open();
            string cmdStr = "select * from tblTheatre where theatre_id=@id";
            SqlCommand cmd = new SqlCommand(cmdStr, conn);
            cmd.Parameters.AddWithValue("@id", theatre_id);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                conn.Close();
                return true;
            }
            conn.Close();
            return false;
        }
    }
}
