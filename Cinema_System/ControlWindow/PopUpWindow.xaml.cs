using System;
using System.Windows;


namespace Cinema_System.ControlWindow
{
    /// <summary>
    /// PopUpWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class PopUpWindow : Window
    {
        public PopUpWindow()
        {
            InitializeComponent();
        }

        public int getCount()
        {
            int count;
            if(txtbxCount.Text.Length > 0)
            {
                if (int.TryParse(txtbxCount.Text, out count))
                {
                    return Convert.ToInt32(txtbxCount.Text);
                }
                else
                {
                    return 0;
                }
            }


            return 0;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
