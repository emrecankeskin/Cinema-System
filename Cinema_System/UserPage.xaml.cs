using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Media;


namespace Cinema_System
{
    /// <summary>
    /// UserPage.xaml etkileşim mantığı
    /// </summary>
    /*
     Hangi salonda hangi saatte olduğunu kontrol etmeli ayrıca saat işini nasıl çözeceğini düşün
     system saatinden sonra filmi kaldırabilir miyim?
     film saat aralığına göre salona film ekleme
     salon tablosu ve reserve edilmiş koltuklar arasında ilişki kur salon_idler ilişkisel olsun theatre pagede başlarken buttonu bu reserve edilmişe göre iptal et
     
     bilet alım bilgilerini göster
     grid column 1 1e listview ekleyip filmleri tarihe göre getir store procedure uygulayabilirsin
     */
    ///
    public partial class UserPage : Page
    {
        SqlConnection conn;
        int user_id;
        public UserPage(int user_id)
        {
            conn = new SqlConnection(CinemaUtils.DATA_BASE_PATH);
            InitializeComponent();
            this.user_id = user_id;
            
            fillComboBox();
            fillTicketGrid();
            fillBeverageGrid();
            fillDataGrid(DateTime.Now);
            getMoney();
            
        }

        private void fillComboBox()
        {
            // sqlden aldığım datalarla dolduracam
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT theatre_name,is_deleted FROM tblTheatre", conn);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                if (Convert.ToBoolean(dr["is_deleted"]) == false)
                {
                    string str = dr["theatre_name"].ToString();
                    cbSalon.Items.Add(str);
                }
            }
            cbSalon.SelectedIndex = 0;
            conn.Close();

        }
        private void fillDataGrid(DateTime time)
        {
            int theatre_id = getTheatreId();
            movieGrid.ItemsSource = null;
            movieGrid.Items.Refresh();
            conn.Open();
            
            string dateTime = time.ToString("yyyy-MM-dd HH:mm:ss");
            // verilen tarihten sonraki tüm filmleri gösterir
            string spSql = "exec up_GetMoviesFromGivenData @date='" + dateTime + "',@theatre_id=@id";

            SqlCommand cmd = new SqlCommand(spSql, conn);
            cmd.Parameters.AddWithValue("@id",theatre_id );
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            movieGrid.ItemsSource = dt.DefaultView;
            conn.Close();

        }

        private void fillTicketGrid()
        {
            // date index : 2 
            DateTime now = DateTime.Now;
            //DateTime parsed = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
            ticketGrid.ItemsSource = null;
            ticketGrid.Items.Refresh();
            conn.Open();
            string spSql = "exec up_GetAllTicketDataById @id=@user_id";

            SqlCommand cmd = new SqlCommand(spSql, conn);
            cmd.Parameters.AddWithValue("@user_id", this.user_id);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            ticketGrid.ItemsSource = dt.DefaultView;
            conn.Close();
        }

        private void fillBeverageGrid()
        {
            beverageGrid.ItemsSource = null;
            beverageGrid.Items.Refresh();
            conn.Open();
            string spSql = "exec up_GetAllBoughtBeveragesDataById @id=@user_id";

            SqlCommand cmd = new SqlCommand(spSql, conn);
            cmd.Parameters.AddWithValue("@user_id", this.user_id);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            beverageGrid.ItemsSource = dt.DefaultView;
            conn.Close();
        }

        private int getTheatreId()
        {
            conn.Open();
            int id = 0;
            //string getTheatreId = "select theatre_id from tblTheatre where theatre_name=@name_str";
            string getTheatreId = "select dbo.fn_getTheatreId(@name_str) as theatre_id";
            SqlCommand getId = new SqlCommand(getTheatreId, conn);
            getId.Parameters.AddWithValue("@name_str",cbSalon.SelectedItem.ToString());
            SqlDataReader dr = getId.ExecuteReader();
            dr.Read();

            id = Convert.ToInt32(dr["theatre_id"]);

            conn.Close();
            return id;

        }
        private void getMoney()
        {
            conn.Open();

            //string cmdStr = "select user_money from tblUser where user_id=@id";
            string cmdStr = "select dbo.fn_getUserMoney(@id) as user_money";
            SqlCommand cmd = new SqlCommand(cmdStr, conn);
            SqlDataReader reader;
            cmd.Parameters.AddWithValue("@id", this.user_id);
            reader = cmd.ExecuteReader();
            reader.Read();
            txtblMoney.Text = reader["user_money"].ToString();


            reader.Close();
            conn.Close();
        }

        private bool isTheatreVip(int id)
        {
            conn.Open();
            bool isVip;
            //string cmdstr = "select theatre_vip from tblTheatre where theatre_id=@id";
            string cmdStr = "select dbo.fn_getVipStatus(@id) as theatre_vip";
            SqlCommand cmd = new SqlCommand(cmdStr, conn);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            isVip = Convert.ToBoolean(dr["theatre_vip"]);

            conn.Close();
            return isVip;

        }

        private void closeButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;
            main?.Close();
        }

        private void cbSalon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if(cbSalon.SelectedItem == null || dateSalon.SelectedDate == null)
            {
                MessageBox.Show("PLEASE CHOOSE A THEATRE AND TIME", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DateTime now = DateTime.Now;
            DateTime parsed = new DateTime(now.Year,now.Month,now.Day,now.Hour,0,0);
            object selectedValue = cbSalon.SelectedItem;
            var temp = (DateTime)dateSalon.SelectedDate;
            DateTime selectedTime = new DateTime(temp.Year, temp.Month, temp.Day, now.Hour,now.Minute, now.Second);
   

            if (selectedTime < parsed)
            {
                CinemaUtils.warningMessage("YOU CANNOT CHOOSE PAST DAYS");
            }
            else
            {
                var time = (DateTime)selectedTime;
                fillDataGrid(time);
                
            }

        }

        private void movieGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(movieGrid.SelectedItems.Count  == 0)
            {
                return;
            }
            var gridSelected = (DataRowView)movieGrid.SelectedItems[0];
            var main = (MainWindow)Application.Current.MainWindow;

            int theatreId = Convert.ToInt32(gridSelected[3]);
            int movieId = Convert.ToInt32(gridSelected[0]);
            string movieName = gridSelected[0].ToString();
            //int price = grid
            DateTime startTime = (DateTime)gridSelected[2];
            decimal price = Convert.ToDecimal(gridSelected[4]);

            bool isVip = isTheatreVip(theatreId);
            // seçilen film kullanıcı adı ve seçilen theatreye göre TheatrePage çağır
            // movie id yi seçilene göre yapacağım
            // movie id : 0 , movie name : 1 , start date : 2,  theatre no : 3, price 4, duration 5  
            if (isVip)
            {
                main?.ChangeView(new TheatrePage(this.user_id, theatreId,
                    movieId, startTime, price, 4));
            }
            else
            {
                main?.ChangeView(new TheatrePage(this.user_id, theatreId,
                    movieId, startTime, price, 8));
            }
        }

        private void btnLogout_click(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;

            main?.ChangeView(new LoginPage());
        }

        private void ticketGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ticketGrid.SelectedItems.Count == 0)
            {
                return;
            }
            var gridSelected = (DataRowView)ticketGrid.SelectedItems[0];
            
            var result = MessageBox.Show("DO YOU REALLY WANT TO REFUND", "REFUND", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                conn.Open();
                string cmdStr = "delete from tblTicket where ticket_id=@id";
                SqlCommand cmd = new SqlCommand(cmdStr, conn);
                cmd.Parameters.AddWithValue("@id", Convert.ToInt32(gridSelected[0]));
                cmd.ExecuteNonQuery();

                CinemaUtils.infoMessage("SUCCESSFULLY REFUNDED", "REFUND");

                conn.Close();
                fillTicketGrid();
                getMoney();
            }
            else
            {
                getMoney();
                return;
            }
            

            
        }

        private void btnAddMoney_Click(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;

            main?.ChangeView(new AddMoneyPage(this.user_id));
        }

        private void ticketGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DateTime temp;
            DateTime time;
            DateTime curr = DateTime.Now;
            DataGridRow gridRow = e.Row;
            DataRow row = (gridRow.DataContext as DataRowView).Row;

            temp = Convert.ToDateTime(row[3]);
            time = new DateTime(temp.Year,temp.Month,temp.Day,temp.Hour,temp.Minute,temp.Second);
            
            if(time < curr)
            {
                gridRow.Background = new SolidColorBrush(Colors.IndianRed);
                //gridRow.FontSize = 15;
                gridRow.IsEnabled = false;
            }
            else
            {
                gridRow.Background = new SolidColorBrush(Colors.LightGreen);
                gridRow.Foreground = new SolidColorBrush(Colors.Black);
            }
            //MessageBox.Show(row[2].ToString());
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;

            main?.ChangeView(new UserSettingsPage(this.user_id));
        }
    } 
}
