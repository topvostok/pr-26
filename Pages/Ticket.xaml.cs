using System;
using System.Collections.Generic;
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

namespace pg_26.Pages
{
    /// <summary>
    /// Логика взаимодействия для Ticket.xaml
    /// </summary>
    public partial class Ticket : Page
    {
        List<Classes.TicketClass> TicketList;
        public Ticket(string from, string to, DateTime? tuda, DateTime? obratno)
        {
            InitializeComponent();

            TicketList = MainWindow.mainWindow.LoadTickets(from, to, tuda, obratno);
            parrent.Children.Clear();
            foreach (var ticket in TicketList)
            {

                var aviaItem = new Elements.Avia_Itm();

                aviaItem.SetData(
                    ticket.price,
                    ticket.from,
                    ticket.to,

                    ticket.time_start,
                    ticket.time_way

                );

                parrent.Children.Add(aviaItem);

            }

            if (TicketList.Count == 0)
            {
                TextBlock noneTickets = new TextBlock
                {
                    Text = "Билетов не найдено",
                    FontSize = 24,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                parrent.Children.Add(noneTickets);
            }
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.freme.Navigate(new Pages.Main());
        }
    }
}
