using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cinema_System
{
    /// <summary>
    /// MainWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Application.Current.MainWindow = this;
            Loaded += OnMainWindowLoaded;
        }
        public void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            LoginPage page = new LoginPage();
            
            ChangeView(page);
        }


        public void ChangeView(Page view)
        {
            Application.Current.MainWindow.Height = view.Height;
            Application.Current.MainWindow.Width = view.Width;
            MainFrame.NavigationService.Navigate(view);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void MainFrame_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void MainFrame_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                try
                {
                    this.DragMove();
                }
                catch
                {
                    //MessageBox.Show();
                }
            }
        }
    }
}
