using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Cinema_System
{
    /// <summary>
    /// FinancePage.xaml etkileşim mantığı
    /// </summary>
    public partial class FinancePage : Page
    {
        SqlConnection conn;
        int user_id = 0;
        int user_type  = 0;

        public FinancePage(int user_type,int user_id)
        {
            InitializeComponent();
            this.conn = new SqlConnection(CinemaUtils.DATA_BASE_PATH);
            this.user_type = user_type;
            this.user_id = user_id;
            fillSoldTicketGrid();
            fillSoldBeveragesGrid();
            fillMostBuyersGrid();
            fillMostBoughtBeverages();
        }


        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if(this.user_type == 0)
            {
                var main = (MainWindow)Application.Current.MainWindow;

                //userpageye dönecek fakat verilen kullanıcı id ile
                main?.ChangeView(new AdminPage(this.user_id));
            }
            else if(this.user_type==1)
            {
                var main = (MainWindow)Application.Current.MainWindow;
                //Doorsmanı idsine göre çağıracam 
                main?.ChangeView(new DoorsmanPage(this.user_id));
            }
        }

        private void fillSoldTicketGrid()
        {
            conn.Open();
            string cmdStr = "select sum(ticket_price) as [Total Money],(select m.movie_date from tblMovie as m where m.movie_id=t.movie_id ) as [Movie Date]," +
                            "(select m.movie_name from tblMovie as m where m.movie_id=t.movie_id) as [Movie Name] " +
                            "from tblTicket as t group by movie_id ";
                            
            SqlCommand cmd = new SqlCommand(cmdStr, conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            soldTicketGrid.ItemsSource = dt.DefaultView;
            conn.Close();
        }

        private void fillSoldBeveragesGrid()
        {
            conn.Open();
            string cmdStr = "select (select name from tblBeverage as b where bb.beverage_id=b.beverage_id) as [Beverage Name], " +
                            "price as [Price], " +
                            "(select name from tblUser as u where u.user_id=bb.user_id) as [Username], " +
                            "count as  [Amonut], movie_name as [Movie Name] " +
                            "from tblBoughtBeverages as bb";
            SqlCommand cmd = new SqlCommand(cmdStr, conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            soldBeverageGrid.ItemsSource = dt.DefaultView;
            conn.Close();
        }

        private void fillMostBuyersGrid()
        {
            conn.Open();
            string cmdStr = "select sum(ticket_price) as [Total Money],(select name from tblUser as u where u.user_id=t.user_id and name != '[deleted]') as [User Name]" +
                            "from tblTicket as t group by user_id ";
            SqlCommand cmd = new SqlCommand( cmdStr, conn);
            SqlDataAdapter adapter = new SqlDataAdapter( cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            mostBuyerGrid.ItemsSource = dt.DefaultView;
            conn.Close();
        }

        private void fillMostBoughtBeverages()
        {
            conn.Open();
            string cmdStr = "select * from vw_GetBeverageTotalSale  where [Beverage Name] != '[deleted]'";

            SqlCommand cmd = new SqlCommand(cmdStr, conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            mostBoughtBeverageGrid.ItemsSource = dt.DefaultView;
            conn.Close();
        }
    }
}
