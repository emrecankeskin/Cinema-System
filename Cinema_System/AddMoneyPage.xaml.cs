using System;
using System.Data.SqlClient;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Cinema_System
{
    /// <summary>
    /// AddMoneyPage.xaml etkileşim mantığı
    /// </summary>
    public partial class AddMoneyPage : Page
    {
        int user_id;
        SqlConnection conn;
        public AddMoneyPage(int user_id)
        {
            InitializeComponent();
            this.user_id = user_id;
            conn = new SqlConnection(CinemaUtils.DATA_BASE_PATH);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;

            main?.ChangeView(new UserPage(this.user_id));
        }

        private void btnConfirmTransaction(object sender, RoutedEventArgs e)
        {
            conn.Open();

            SqlCommand cmdGet;
            SqlCommand cmdSet;
            SqlDataReader reader;
            string getMoney = "select user_money from tblUser where user_id=@id";
            string setMoney = "update tblUser set user_money=@money where user_id=@id";
            string number = txtbxCreditCard.Text;
            string amount = txtbxMoney.Text;

            decimal user_money;
            int lenCard = number.Length;
            int lenMoney = amount.Length;


            if(lenCard < 10)
            {
                CinemaUtils.errorMessage("LENGTH OF CREDIT CARD MUST BE AT LEAST 10", "DECLINED");
                
            }
            else if(lenMoney < 2 || decimal.TryParse(amount,out user_money) == false )
            {
                CinemaUtils.errorMessage("MONEY MUST BE AT LEAST 10TL AND NUMBER", "DECLINED");
            }
            else
            {

                number = reverseString(number);
                if (checkLuhn(number))
                {
                    cmdGet = new SqlCommand(getMoney, conn);
                    cmdSet = new SqlCommand(setMoney, conn);
                    cmdGet.Parameters.AddWithValue("@id", this.user_id);
                    reader = cmdGet.ExecuteReader();
                    reader.Read();
                    user_money += Convert.ToDecimal(reader["user_money"]);
                    reader.Close();

                    cmdSet.Parameters.AddWithValue("@money", user_money);
                    cmdSet.Parameters.AddWithValue("@id", this.user_id);

                    cmdSet.ExecuteNonQuery();
                    CinemaUtils.infoMessage("TRANSACTION COMPLETED", "CARD");
                }
                else
                {
                    CinemaUtils.errorMessage("INVALID CARD TRY AGAIN WITH DIFFERENT CARD", "CARD");
                }
            }

            clear();
            conn.Close();


        }

        private bool checkLuhn(string number)
        {
            int cal = 0, total = 0,lenCard = number.Length;
            for (int i = 0; i < lenCard; i++)
            {
                if (i % 2 == 1)
                {
                    cal = number[i] - '0';
                    cal = cal * 2;
                    if (cal > 9)
                    {
                        cal = cal - 9;
                    }

                    total = total + cal;
                }
                else
                {
                    total = total + number[i] - '0';
                }
            }


            return total % 10 == 0;
        }

        private string reverseString(string number)
        {
            // credit card 4444-4444-4444-4444 ya da 4444 4444 4444 4444 olarak yazılabilir
            // 4543 1478 8243 1591
            StringBuilder sb = new StringBuilder();
            int len = number.Length;
            for(int i = len-1;  i >= 0; i--)
            {
                if (number[i] != ' ' && number[i] != '-')
                {
                    sb.Append(number[i]);
                }
            }

            return sb.ToString();
        }

        private void clear()
        {
            txtbxCreditCard.Text = "";
            txtbxMoney.Text = "";
        }
    }
}
