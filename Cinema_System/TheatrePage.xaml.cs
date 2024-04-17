using Cinema_System.ControlWindow;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace Cinema_System
{
    /// <summary>
    /// TheatrePage.xaml etkileşim mantığı
    /// </summary>
    public partial class TheatrePage : Page
    {
        List<String> bookedSeats;
        SqlConnection conn;
        DateTime date;
        int theatreID;
        int movieID;
        int userID;
        int rowCount;

        decimal ticketPrice;

        int[,] selectedList;
        public TheatrePage(int userID, int theatreID, int movieID, DateTime date, decimal ticketPrice, int rowCount)
        {

            
            conn = new SqlConnection(CinemaUtils.DATA_BASE_PATH);
            bookedSeats = new List<String>();

            //selectedSeats = new List<String>();
            selectedList = new int[8, 8];
            this.userID = userID;
            this.theatreID = theatreID;
            this.movieID = movieID;
            this.ticketPrice = ticketPrice;
            this.date = date;
            this.rowCount = rowCount;

            InitializeComponent();
            fillSeats();
            getUserMoney();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            Button b = (Button)sender;
            // map all the selected files and in the confirm method save all the date to database
            // handleding button click 
            String str = b.Content.ToString();
            int row = Convert.ToUInt16(str[0]) - 65;
            int column = Convert.ToUInt16(str[2]) - 49;
            decimal money;
            if(b.Background == Brushes.Blue)
            {
                selectedList[row, column] = 0;
                b.Background = Brushes.Linen;
                money =  Convert.ToDecimal(txtbxTicketPrice.Text) - this.ticketPrice;
                txtbxTicketPrice.Text = money.ToString();
            }
            else
            {
                selectedList[row, column] = 1;
                b.Background = Brushes.Blue;

                money = Convert.ToDecimal(txtbxTicketPrice.Text) + this.ticketPrice;
                txtbxTicketPrice.Text = money.ToString();
            }

        }
        private void fillSeats()
        {
            conn.Open();

            String cmdStr = "select seat_position " +
                            "from tblTicket " +
                            "where theatre_id=@tid and movie_id=@mid";
            SqlCommand cmd = new SqlCommand(cmdStr,conn);
            cmd.Parameters.AddWithValue("@tid",this.theatreID.ToString());
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
                    
                    }
                    else
                    {
                        btn.Background = Brushes.Linen;
                        btn.Click += Button_Click;
                    }
                    Grid.SetRow(btn, i);
                    Grid.SetColumn(btn, j);
                    GridUp.Children.Add(btn);
                    count++;
                }
            }
            rdr.Close();
            conn.Close();
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;
            
            main?.ChangeView(new UserPage(this.userID));
        }

        private void btnConfirm_click(object sender, RoutedEventArgs e)
        {
            // confirm seat information and add to user's seat list
            // matrixi dolaşıp 1 olanları tblİtkcete insert et
            if(checkIfSelected()  == false) {
                CinemaUtils.infoMessage("CHOOSE SEATS TO CONFIRM", "SEAT");
                return;
            }
            var result = MessageBox.Show("DO YOU WANT TO BUY BEVERAGES ", "BEVERAGES", MessageBoxButton.YesNo, MessageBoxImage.Question);
            decimal beverage = 0;
            if (result == MessageBoxResult.Yes)
            {
                var dialog = new BuyBeverageWindow(this.userID, this.movieID, (Convert.ToDecimal(txtbxMoney.Text)- Convert.ToDecimal(txtbxTicketPrice.Text)));
                dialog.ShowDialog();
            }
            if ((Convert.ToDecimal(txtbxTicketPrice.Text) + beverage) > Convert.ToDecimal(txtbxMoney.Text))
            {
                CinemaUtils.errorMessage("NOT ENOUGH MONEY FOR TICKETS", "TRANSACTION");
                return;
            }
            conn.Open();
            String cmdStr = "INSERT INTO tblTicket(user_id,theatre_id,movie_id,ticket_price,seat_position,ticket_date) "+
                            "values(@user_id,@theatre_id,@movie_id,@ticket_price,@seat_position,@ticket_date)";
            SqlCommand cmd = new SqlCommand(cmdStr,conn);

            cmd.Parameters.AddWithValue("@user_id",this.userID);
            cmd.Parameters.AddWithValue("@theatre_id",this.theatreID);
            cmd.Parameters.AddWithValue("@movie_id",this.movieID);
            cmd.Parameters.AddWithValue("@ticket_price",this.ticketPrice);
            cmd.Parameters.AddWithValue("@seat_position","");
            cmd.Parameters.AddWithValue("@ticket_date",this.date);

            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    if (selectedList[i,j] == 1)
                    {
                        int val = 'A' + i;
                        char c = (char) val;
                        string str = c.ToString()+'-'+(j+1).ToString();
                        cmd.Parameters["@seat_position"].Value = str;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            CinemaUtils.infoMessage("COMPLETED ", "TICKET BUY");
            conn.Close();
            fillSeats();
            getUserMoney();
            clearSelectedList();
            txtbxTicketPrice.Text = "0";
        }

        private void getUserMoney()
        {
            // writo to text box money user's money
            conn.Open();

            string cmdStr = "select user_money from tblUser where user_id=@id";
            SqlCommand cmd = new SqlCommand(cmdStr,conn);
            SqlDataReader reader;
            cmd.Parameters.AddWithValue("@id", this.userID);
            reader = cmd.ExecuteReader();
            reader.Read();


            txtbxMoney.Text = reader["user_money"].ToString();

            conn.Close();


        }
        private void clearSelectedList()
        {
            for(int i = 0; i < 8; i++)
            {
                for(int j =0; j < 8; j++)
                {
                    selectedList[i, j] = 0;
                }
            }
        }
        private bool checkIfSelected()
        {
            // check if there is selected seat for confirm button
            int count = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (selectedList[i, j] != 0) {
                        count++;
                       
                    
                    }
                }
            }

            return count != 0;
        }
    }
}
