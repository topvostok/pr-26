using System.Windows;
using System.Windows.Controls;

namespace pg_26.Pages
{

    public partial class Main : Page
    {
        public Main()
        {
            InitializeComponent();
        }
        private void Exit(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.Close();
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.freme.Navigate(new Pages.Ticket(from.Text, to.Text, tuda.SelectedDate, obratno.SelectedDate));
        }
    }
}
