using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data.SqlClient;

namespace Cinema_System
{
    /// <summary>
    /// AdminTheatrePage.xaml etkileşim mantığı
    /// </summary>
    /// sadece admin doorsman ya da diğer yetkili biri yükleyebilecek veya tüm bunları silebileek 

    public partial class AdminTheatrePage : Page
    {
        List<String> bookedSeats;
        //List<String> selectedSeats;
        SqlConnection conn;
        int theatreID;
        int movieID;

        int rowCount;
        int user_id;


        int[,] selectedList;
        public AdminTheatrePage(int theatreID,int movieID,int rowCount,int user_id)
        {
            conn = new SqlConnection(CinemaUtils.DATA_BASE_PATH);
            selectedList = new int[8, 8];
            bookedSeats = new List<String>();

            this.theatreID = theatreID;
            this.movieID = movieID;
            this.rowCount = rowCount;
            this.user_id = user_id;
            InitializeComponent();
            fillSeats();
            // hangi salondan alacağımız seçip ona göre öyle gösterecem
            //get list of elements for salon in sql than change clickable and color to red 

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            String str = b.Content.ToString();
            int row = Convert.ToUInt16(str[0]) - 65;
            int column = Convert.ToUInt16(str[2]) - 49;
            if (b.Background == Brushes.Blue)
            {
                selectedList[row, column] = 0;
                b.Background = Brushes.Linen;
            }
            else
            {
                selectedList[row, column] = 1;
                b.Background = Brushes.Blue;
            }

        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            // go back to dashboard
            var main = (MainWindow)Application.Current.MainWindow;

            main?.ChangeView(new AdminPage(this.user_id));
        }


        private void fillSeats()
        {
            conn.Open();
            //String cmdStr = "select seat_position " +
            //                "from tblTicket " +
            //                "where theatre_id=@tid" + this.theatreID.ToString() + " and movie_id=mid" +this.movieID.ToString();
            String cmdStr = "select seat_position " +
                            "from tblTicket " +
                            "where theatre_id=@tid and movie_id=@mid";
            SqlCommand cmd = new SqlCommand(cmdStr, conn);
            cmd.Parameters.AddWithValue("@tid", this.theatreID.ToString());
            cmd.Parameters.AddWithValue("@mid", this.movieID.ToString());
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                bookedSeats.Add(rdr[0].ToString());
            }



            int count = 1;
            //get list of elements for salon in sql than change clickable and color to red 
            for (int i = 1; i <= this.rowCount; ++i)
            {
                for (int j = 1; j <= 8; ++j)
                {
                    Char y = (Char)(Convert.ToUInt16('A') + i - 1);
                    Button btn = new Button();
                    btn.Width = 65;
                    btn.Height = 40;

                    btn.Content = y.ToString() + "-" + j.ToString();
                    btn.Name = "button" + count.ToString();

                    if (bookedSeats.Contains(btn.Content.ToString()))
                    {
                        btn.Background = Brushes.Black;
                        btn.Foreground = Brushes.White;
                        //btn.IsEnabled = false;
                        btn.Click += Button_Click;

                    }
                    else
                    {
                        btn.Background = Brushes.Linen;
                        btn.Click += Button_Click;
                    }
                    Grid.SetRow(btn, i);
                    Grid.SetColumn(btn, j);
                    AdminGrid.Children.Add(btn);
                    count++;
                }
            }
            rdr.Close();
            conn.Close();
        }

        private int getTicketId(string pos)
        {
            conn.Open();
            string cmdStr = "select ticket_id from tblTicket where theatre_id=@tid and movie_id=@mid and seat_position=@position";
            SqlCommand sqlCommand = new SqlCommand(cmdStr, conn);
            sqlCommand.Parameters.AddWithValue("@tid", this.theatreID);
            sqlCommand.Parameters.AddWithValue("@mid", this.movieID);
            sqlCommand.Parameters.AddWithValue("@position", pos);
            SqlDataReader dataReader = sqlCommand.ExecuteReader();
            dataReader.Read();
            if (!dataReader.HasRows)
            {
                conn.Close();
                return -1;
                
            }
            int ticket_id = Convert.ToInt32(dataReader["ticket_id"]);

            conn.Close();

            return ticket_id;

        }

        private void deleteSeat(int ticket_id)
        {
            conn.Open();
            string cmdStr = "delete from tblTicket where ticket_id=@tid";
            SqlCommand sqlCommand = new SqlCommand(cmdStr, conn);
            sqlCommand.Parameters.AddWithValue("@tid", ticket_id.ToString());

            sqlCommand.ExecuteNonQuery();

            conn.Close();

        }
        private void btnDeleteSeat(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (selectedList[i, j] == 1)
                    {
                        int val = 'A' + i;
                        char c = (char)val;
                        string str = c.ToString() + '-' + (j + 1).ToString();
                        int ticket_id = getTicketId(str);
                        if(ticket_id != -1)
                        {
                            deleteSeat(ticket_id);
                        }else
                        {
                            CinemaUtils.warningMessage(str + " IS NOT BOOKED");
                        }
                        fillSeats();
                    }
                }
            }



        }

    }
}
