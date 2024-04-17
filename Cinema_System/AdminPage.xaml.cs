using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using Cinema_System.ControlWindow;

namespace Cinema_System
{
    /// <summary>
    /// AdminPage.xaml etkileşim mantığı
    /// </summary>
    public partial class AdminPage : Page
    {
        SqlConnection conn;
        int user_id;
        public AdminPage(int user_id)
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
        private void fillUserGrid()
        {
            conn.Open();

            string sqlStr = "select user_id as [Username ID], name as [Name], surname as [Surname], username as [Username],password as [User Password], user_type as [User Domain],user_money as [Total Money] " +
                            "from tblUser " +
                            "where is_deleted=0";
            SqlCommand cmd = new SqlCommand(sqlStr, conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            userGrid.ItemsSource = dt.DefaultView;
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
            fillUserGrid();
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
            if(gridSelected == null)
            {
                CinemaUtils.warningMessage("SELECT A ROW FOR UPDATE");
            }
            else
            {
                int id =Convert.ToInt32(gridSelected[0]);
                var dialog = new UpdateMovieWindow(id);
                dialog.ShowDialog();
            }
            conn.Close();
            refreshGrid();
        }

        private void btnUserDelete(object sender, RoutedEventArgs e)
        {
            var dialog = new DeleteUserWindow();
            dialog.ShowDialog();
            refreshGrid();
        }

        private void btnUserUpdate(object sender, RoutedEventArgs e)
        {
            conn.Open();
            var gridSelected = (DataRowView)userGrid.SelectedItem;
            if (gridSelected == null)
            {
                CinemaUtils.warningMessage("SELECT A ROW FOR UPDATE");
            }
            else
            {
                int id = Convert.ToInt32(gridSelected[0]);
                var dialog = new UpdateUserWindow(id);
                dialog.ShowDialog();
            }
            conn.Close();
            refreshGrid();

        }

        private void btnUserAdd(object sender, RoutedEventArgs e)
        {
            var dialog = new AddUserWindow();
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


        private void btnMovieAdd(object sender, RoutedEventArgs e)
        {
 
            var dialog = new AddMovieWindow(this.user_id);
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

        private void btnDeleteTheatre(object sender, RoutedEventArgs e)
        {
            var dialog = new DeleteTheatreWindow();
            dialog.ShowDialog();
            refreshGrid();
        }



        private void btnTheatreAdd(object sender, RoutedEventArgs e)
        {
            var dialog = new AddTheatreWindow(this.user_id);
            dialog.ShowDialog();
            refreshGrid();
        }

        private bool isTheatreVip(int id)
        {
            conn.Open();
            bool isVip;
            string cmdStr = "select dbo.fn_getVipStatus(@id) as theatre_vip";
            SqlCommand cmd = new SqlCommand(cmdStr, conn);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            isVip = Convert.ToBoolean(dr["theatre_vip"]);

            conn.Close();
            return isVip;

        }

        private void movieGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // movie id , name , start date , end date , theatre no , duration of release , price
            var main = (MainWindow)Application.Current.MainWindow;
            var gridSelected = (DataRowView)movieGrid.SelectedItems[0];
            int theatreID = Convert.ToInt32(gridSelected[3]);
            int movieID = Convert.ToInt32(gridSelected[0]);
            bool isVip = isTheatreVip((theatreID));

            if (isVip)
            {
                main.ChangeView(new AdminTheatrePage(theatreID, movieID,4,this.user_id));
            }
            else
            {
                main.ChangeView(new AdminTheatrePage(theatreID, movieID,8,this.user_id));
            }
        }

        private void btnFinanceOpen(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;
            main.ChangeView(new FinancePage(0,this.user_id));
        }

        private void btnWorkerInfoOpen(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;
            main.ChangeView(new WorkerInfoPage(this.user_id));
        }

    }
}
