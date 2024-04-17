using System;
using System.Data.SqlClient;
using System.Windows;


namespace Cinema_System.ControlWindow
{
    /// <summary>
    /// AddMovieWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class AddMovieWindow : Window
    {
        SqlConnection conn;
        int user_id;

        public AddMovieWindow(int user_id)
        {
            conn = new SqlConnection(CinemaUtils.DATA_BASE_PATH);
            InitializeComponent();
            this.user_id = user_id;
        }

        private void btnConfirmClick(object sender, RoutedEventArgs e)
        {

            int tId;
            int movie_duration;
            int hour;
            int minute;
            decimal moviePrice;
            
            string movieName = txtbxName.Text;
            string theatreId = txtbxTheatreId.Text;
            string movieDuration = txtbxMovieDuration.Text;
            DateTime? time = dateMovie.SelectedDate;
            

            //@movie_name varchar(50), @movie_date datetime, @theatre_id int, @movie_duration int, @movie_cinema_day
            // if theatre exist check
            string spSql = "exec up_AddMovie @movie_name=@movieName," +
                            "@movie_date=@time," +
                            "@theatre_id=@theatreId," +
                            "@movie_duration=@duration, " +
                            "@movie_price=@price, " +
                            "@movie_adder=@adder_id";

            // add a function that checks interval
            // finishtime in database
            SqlCommand cmd = new SqlCommand(spSql, conn);


            if (movieName == null || theatreId == null || time == null ||
                int.TryParse(txtbxTheatreId.Text, out tId) == false ||
                int.TryParse(txtbxMovieDuration.Text, out movie_duration) == false ||
                decimal.TryParse(txtbxMovieMoney.Text, out moviePrice) == false ||
                Convert.ToInt16(txtbxHour.Text) > 23 || Convert.ToInt16(txtbxHour.Text) < 0 ||
                int.TryParse(txtbxHour.Text, out hour) == false ||
                int.TryParse(txtbxMinute.Text, out minute) == false ||
                Convert.ToInt16(txtbxMinute.Text) > 59 || Convert.ToInt16(txtbxMinute.Text) < 0
                )
            {
                CinemaUtils.warningMessage("MAKE SURE FILLED EVERY TEXTBOX WITH RIGHT VALUES");

            }
            else
            {
                DateTime temp = (DateTime)time;
                DateTime newTime = new DateTime(temp.Year, temp.Month, temp.Day, hour, minute, 0);
                if (checkMovieCollusion(newTime,movie_duration,tId ) == true)
                {
                    CinemaUtils.warningMessage("COLLISION BETWEEN ANOTHER MOVIE CHANGE TIME");

                }
                else
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@movieName", movieName);
                    cmd.Parameters.AddWithValue("@time", newTime);
                    cmd.Parameters.AddWithValue("@theatreId", theatreId);
                    cmd.Parameters.AddWithValue("@duration", movieDuration);
                    cmd.Parameters.AddWithValue("@price", moviePrice);
                    cmd.Parameters.AddWithValue("@adder_id", this.user_id);

                    try
                    {
                        int a = cmd.ExecuteNonQuery();
                        if (a > 0)
                        {
                            CinemaUtils.infoMessage("SUCCESSFULLY ADDED MOVIE [" + movieName + "]", "MOVIE ADD");
                        }
                        else
                        {
                            CinemaUtils.infoMessage("THERE IS NO EFFECTED ROW.", "PROBLEM");
                        }
                    }
                    catch
                    {
                        CinemaUtils.errorMessage("DATABASE ERROR", "");
                    }
                }

            

            }

            clearBoxes();
            conn.Close();
        }


        private void clearBoxes()
        {
            txtbxMovieDuration.Text = "";
            txtbxName.Text = "";
            txtbxTheatreId.Text = "";
            txtbxMovieMoney.Text = "";
            dateMovie.Text = "";
            txtbxHour.Text = "";
            txtbxMinute.Text = "";

        }

        private bool checkMovieCollusion(DateTime time, int movie_duration, int theatre_id)
        {
            //add duration of movie to datetime of movie 

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
                conn.Close();
                return true;
            }
            conn.Close();
            return false;

        }
        //

    }
}
