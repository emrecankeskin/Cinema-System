using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using Cinema_System.ControlWindow;

namespace Cinema_System
{
    /// <summary>
    /// DoorsmanPage.xaml etkileşim mantığı
    /// </summary>
    public partial class DoorsmanPage : Page
    {

        SqlConnection conn;
        int user_id;
        public DoorsmanPage(int user_id)
        {
            conn = new SqlConnection(CinemaUtils.DATA_BASE_PATH);
            InitializeComponent();
            refreshGrid();
            this.user_id = user_id;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;

            main?.Close();
        }

        private void btnLogout_click(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;

            main?.ChangeView(new LoginPage());
        }

        private void fillMovieGrid()
        {
            conn.Open();
            string sqlStr = "select movie_id as [Movie ID], movie_name as [Movie Name], movie_date as [Movie Date],theatre_id as [Theatre No],movie_price as [Ticket Price], movie_duration as [Movie Duration] " +
                "from tblMovie " +
                "where is_deleted=0";
            SqlCommand cmd = new SqlCommand(sqlStr, conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            movieGrid.ItemsSource = dt.DefaultView;
            conn.Close();
        }

        private void fillFoodGrid()
        {
            conn.Open();
            string sqlStr = "select beverage_id as [Beverage ID],price as [Price Of Beverage], name as [Name], stock as [Total in Stock] " +
                "from tblBeverage " +
                "where is_deleted=0";
            SqlCommand cmd = new SqlCommand(sqlStr, conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            foodGrid.ItemsSource = dt.DefaultView;
            conn.Close();
        }
        private void fillTheatreGrid()
        {
            conn.Open();
            string sqlStr = "select theatre_id as [Theatre ID], theatre_name as [Name Of Theatre], theatre_capacity as [Capacaity], theatre_vip as [VIP Status] " +
                "from tblTheatre " +
                "where is_deleted=0";
            SqlCommand cmd = new SqlCommand(sqlStr, conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            theatreGrid.ItemsSource = dt.DefaultView;
            conn.Close();

        }
        private void refreshGrid()
        {
            fillMovieGrid();
            fillFoodGrid();
            fillTheatreGrid();
        }

        private void btnDeleteMovie(object sender, RoutedEventArgs e)
        {
            var dialog = new DeleteMovieWindow();
            dialog.ShowDialog();
            refreshGrid();
        }

        private void btnMovieUpdate(object sender, RoutedEventArgs e)
        {
            conn.Open();
            var gridSelected = (DataRowView)movieGrid.SelectedItem;
            if (gridSelected == null)
            {
                CinemaUtils.warningMessage("SELECT A ROW FOR UPDATE");
            }
            else
            {
                int id = Convert.ToInt32(gridSelected[0]);
                var dialog = new UpdateMovieWindow(id);
                dialog.ShowDialog();
            }
            conn.Close();
            refreshGrid();
        }

        private void btnMovieAdd(object sender, RoutedEventArgs e)
        {
            var dialog = new AddMovieWindow(this.user_id);
            dialog.ShowDialog();
            refreshGrid();
        }

        private void btnBeverageDelete(object sender, RoutedEventArgs e)
        {
            var dialog = new DeleteBeverageWindow();
            dialog.ShowDialog();
            refreshGrid();
        }

        private void btnBeverageUpdate(object sender, RoutedEventArgs e)
        {
            var gridSelected = (DataRowView)foodGrid.SelectedItem;
            conn.Open();
            if (gridSelected == null)
            {
                CinemaUtils.warningMessage("SELECT A ROW FOR UPDATE");
            }
            else
            {
                int id = Convert.ToInt32(gridSelected[0]);
                var dialog = new UpdateBeverageWindow(id);
                dialog.ShowDialog();
            }

            conn.Close();
            refreshGrid();
        }

        private void btnBeverageAdd(object sender, RoutedEventArgs e)
        {
            var dialog = new AddBeverageWindow(this.user_id);
            dialog.ShowDialog();
            refreshGrid();
        }

        private void btnDeleteTheatre(object sender, RoutedEventArgs e)
        {
            var dialog = new DeleteTheatreWindow();
            dialog.ShowDialog();
            refreshGrid();
        }

        private void btnUpdateTheatre(object sender, RoutedEventArgs e)
        {
            var gridSelected = (DataRowView)theatreGrid.SelectedItem;
            conn.Open();
            if (gridSelected == null)
            {
                CinemaUtils.warningMessage("SELECT A ROW FOR UPDATE");
            }
            else
            {
                int id = Convert.ToInt32(gridSelected[0]);
                var dialog = new UpdateTheatreWindow(id);
                dialog.ShowDialog();
            }

            conn.Close();
            refreshGrid();
        }

        private void btnTheatreAdd(object sender, RoutedEventArgs e)
        {
            var dialog = new AddTheatreWindow(this.user_id);
            dialog.ShowDialog();
            refreshGrid();
        }

        private void movieGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;
            var gridSelected = (DataRowView)movieGrid.SelectedItems[0];
            int theatreID = Convert.ToInt32(gridSelected[3]);
            int movieID = Convert.ToInt32(gridSelected[0]);
            bool isVip = isTheatreVip(getTheatreName(theatreID));

            if (isVip)
            {
                main.ChangeView(new AdminTheatrePage(theatreID, movieID, 4,this.user_id));
            }
            else
            {
                main.ChangeView(new AdminTheatrePage(theatreID, movieID, 8, this.user_id));
            }
        }

        private bool isTheatreVip(string theatreName)
        {
            conn.Open();
            bool isVip;
            string cmdstr = "select theatre_vip from tblTheatre where theatre_name=@name";
            SqlCommand cmd = new SqlCommand(cmdstr, conn);
            cmd.Parameters.AddWithValue("@name", theatreName);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            isVip = Convert.ToBoolean(dr["theatre_vip"]);

            conn.Close();
            return isVip;

        }
        private string getTheatreName(int theatre_no)
        {
            conn.Open();
            string name;
            string cmdstr = "select theatre_name from tblTheatre where theatre_id=@id";
            SqlCommand cmd = new SqlCommand(cmdstr, conn);
            cmd.Parameters.AddWithValue("@id", theatre_no);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            name = Convert.ToString(dr["theatre_name"]);

            conn.Close();


            return name;
        }

        private void btnFinanceOpen(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;
            main.ChangeView(new FinancePage(1, this.user_id));
        }
    }
}
