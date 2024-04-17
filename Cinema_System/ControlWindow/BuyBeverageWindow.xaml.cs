using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows;
using System.Windows.Input;


namespace Cinema_System.ControlWindow
{
    /// <summary>
    /// BuyBeverageWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class BuyBeverageWindow : Window
    {
        SqlConnection conn;
        Dictionary<int, BeverageItem> dict;
        int user_id;
        int count = 0;
        int movie_id = 0;

        decimal total_money = 0;
        decimal user_money = 0;


        public BuyBeverageWindow(int user_id,int movie_id,decimal user_money)
        {
            conn = new SqlConnection(CinemaUtils.DATA_BASE_PATH);
            dict = new Dictionary<int, BeverageItem>();
            InitializeComponent();
            fillFoodGrid();
            this.user_id = user_id;
            this.user_money = user_money;
            this.movie_id = movie_id;
        }


        // hashmapleyip iptal ettiğinde bir azaltıp gibi gidilebilir
        private void fillFoodGrid()
        {
            conn.Open();
            string sqlStr = "select name as [Name],price as [Price Of Beverage], stock as [Total in Stock]" +
                           "from tblBeverage " +
                           "where is_deleted=0";
            SqlCommand cmd = new SqlCommand(sqlStr, conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            beverageGrid.ItemsSource = dt.DefaultView;
            conn.Close();
        }

        private void beverageGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if(beverageGrid.SelectedItems.Count == 0)
            {
                return;
            }
            PopUpWindow popup = new PopUpWindow();
            popup.ShowDialog();
           
            var gridSelected = (DataRowView)beverageGrid.SelectedItems[0];
            string name = gridSelected[0].ToString();
            decimal price = Convert.ToDecimal(gridSelected[1]);
            int bev_id = getBeverageId(name);
            this.count = popup.getCount();
            if (checkStock(count, bev_id))
            {
                CinemaUtils.errorMessage("CANT BUY " + name, "NOT ENOUGH BEVERAGE");
                return;
            }
            if (this.count == 0)
            {
                return;
            }

            // şurayı dicten alıp tekrar listboxa yazabilirim bu sayede hem silebilmiş olurum
            total_money = total_money + price;
            boughtListBox.Items.Add(name + " - Count: " + Convert.ToString(this.count)+" - "+Convert.ToString(price*this.count));
            if (dict.ContainsKey(bev_id)) 
            {
                dict[bev_id].count += this.count;
            }
            else
            {
                dict.Add(bev_id, new BeverageItem(name, price, count));
            }
            


        }

        private int getBeverageStock(int id)
        {
            conn.Open();
            string cmdStr = "select stock from tblBeverage where beverage_id=@id";
            SqlCommand cmd = new SqlCommand(cmdStr, conn);
            SqlDataReader reader;
            cmd.Parameters.AddWithValue("@id", id);
            reader = cmd.ExecuteReader();
            reader.Read();

            int a = Convert.ToInt32(reader["stock"]);


            conn.Close();


            return a;
        }
        private string getMovieName(int movie_id)
        {
            conn.Open();

            string cmdStr = "select movie_name from tblMovie where movie_id=@id";
            SqlCommand cmd = new SqlCommand(cmdStr, conn);
            SqlDataReader reader;
            cmd.Parameters.AddWithValue("@id", movie_id);
            reader = cmd.ExecuteReader();
            reader.Read();

            string a = Convert.ToString(reader["movie_name"]);


            conn.Close();


            return a;
        }
        private int getBeverageId(string name)
        {
            conn.Open();
            string cmdStr = "select beverage_id from tblBeverage where name=@na";
            int bev_id = -1;
            SqlCommand cmd = new SqlCommand(cmdStr, conn);
            SqlDataReader reader;
            cmd.Parameters.AddWithValue("@na", name);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                bev_id = Convert.ToInt32(reader["beverage_id"]);
            }

            conn.Close();
            return bev_id;



        }

        private bool checkMoney()
        {
            decimal total = 0;
            foreach(var i in dict)
            {
                if(i.Value.count != 0)
                {
                    total += i.Value.price*i.Value.count;
                }
            }

            return total <= this.user_money;
        }

        private bool checkStock(int count,int bev_id)
        {
            int stock = getBeverageStock(bev_id);


            return stock < count;
        }

        private string parseName(string str)
        {
            StringBuilder sb = new StringBuilder();
            int ptr = 0;
            int len = str.Length;
            while (str[ptr] != ' ' && ptr < len)
            {
                sb.Append(str[ptr]);
                ptr++;
            }

            return sb.ToString();
        }
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            String movie_name = getMovieName(this.movie_id);
            String beverageStr = "INSERT INTO tblBoughtBeverages(beverage_id,price,user_id,count,movie_name) " +
                                 "values(@bid,@price,@user_id,@count,@movie_name)";
            conn.Open();
            if(checkMoney() == false)
            {
                conn.Close();
                CinemaUtils.warningMessage("NOT ENOUGH MONEY FOR BEVERAGE ");
                return;
            }
            SqlCommand cmd = new SqlCommand(beverageStr,conn);
            cmd.Parameters.AddWithValue("@bid", 0);
            cmd.Parameters.AddWithValue("@price", "");
            cmd.Parameters.AddWithValue("@user_id", "");
            cmd.Parameters.AddWithValue("@count", "");
            cmd.Parameters.AddWithValue("@movie_name", "");

            foreach(var i in dict)
            {
                if(i.Value.count > 0)
                {
                    cmd.Parameters["@bid"].Value = i.Key;
                    cmd.Parameters["@price"].Value = i.Value.price * i.Value.count;
                    cmd.Parameters["@user_id"].Value = this.user_id;
                    cmd.Parameters["@count"].Value = i.Value.count;
                    cmd.Parameters["@movie_name"].Value = movie_name;
                    cmd.ExecuteNonQuery();
                }

            }
            conn.Close();
            this.Close();
        }


        // deletes the double-clicked item from listbox
        // it assigns count 0 in dict object
        private void boughtListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        { 
            List<String> list = new List<String>();
            int idx = boughtListBox.SelectedIndex;
            int len = boughtListBox.Items.Count;
            string name = parseName(boughtListBox.SelectedItem.ToString());
            int key = getBeverageId(name);

            dict[key].count = 0;


            for (int i = 0; i< len; i++)
            {
                if(i != idx)
                {
                    list.Add(boughtListBox.Items[i].ToString());
                }
            }

            boughtListBox.Items.Clear();

            foreach(string st in list) {
                boughtListBox.Items.Add(st);
            }
        }
    }

    public class BeverageItem
    {
        public decimal price;
        public int count;
        public string name;
        public BeverageItem(string name,decimal price, int count)
        {
            this.name = name;
            this.price = price;
            this.count = count;
        }

    }

}
