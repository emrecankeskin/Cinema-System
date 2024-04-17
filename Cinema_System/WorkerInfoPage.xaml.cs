using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;


namespace Cinema_System
{
    /// <summary>
    /// WorkerInfoPage.xaml etkileşim mantığı
    /// </summary>
    public partial class WorkerInfoPage : Page
    {
        SqlConnection conn;
        int user_id;
        public WorkerInfoPage(int user_id)
        {
            conn = new SqlConnection(CinemaUtils.DATA_BASE_PATH);
            InitializeComponent();
            this.user_id = user_id;
            fillMovieLogGrid();
            fillTheatreLogGrid();
            fillBeverageLogGrid();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;
            main.ChangeView(new AdminPage(this.user_id));
        }

        private void fillMovieLogGrid()
        {
            conn.Open();
            string cmdStr = "select logger_id as [LOGGER ID],log_info as [INFO], log_time as [TIME STAMP]  from tblSystemLog where log_info like 'MOVIE%'";
            SqlCommand cmd = new SqlCommand(cmdStr, conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            movieLogGrid.ItemsSource = dt.DefaultView;
            conn.Close();
        }

        private void fillTheatreLogGrid()
        {
            conn.Open();
            string cmdStr = "select logger_id as [LOGGER ID],log_info as [INFO], log_time as [TIME STAMP] from tblSystemLog where log_info like 'THEATRE%'";
            SqlCommand cmd = new SqlCommand(cmdStr, conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            theatreLogGrid.ItemsSource = dt.DefaultView;
            conn.Close();
        }

        private void fillBeverageLogGrid()
        {
            conn.Open();
            string cmdStr = "select  logger_id as [LOGGER ID],log_info as [INFO], log_time as [TIME STAMP]  from tblSystemLog where log_info like 'BEVERAGE%'";
            SqlCommand cmd = new SqlCommand(cmdStr, conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            beverageLogGrid.ItemsSource = dt.DefaultView;
            conn.Close();
        }


    }
}
