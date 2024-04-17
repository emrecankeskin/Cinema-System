using System;
using System.Windows;


namespace Cinema_System
{
    public static class CinemaUtils
    {

        public static String DATA_BASE_PATH = "Data Source=DESKTOP-POG05LE;Initial Catalog=CinemaDatabase;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";


        public static void warningMessage(string message)
        {
            MessageBox.Show("[!] "+message, "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);

        }

        public static void infoMessage(string message,string caption)
        {
            MessageBox.Show("[*] "+message , caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void errorMessage(string message,string caption)
        {
            MessageBox.Show("[*] " + message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }



}
