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

namespace pg_26.Elements
{
    /// <summary>
    /// Логика взаимодействия для Avia_Itm.xaml
    /// </summary>
    public partial class Avia_Itm : UserControl
    {
        public Avia_Itm()
        {
            InitializeComponent();
        }
        public void SetData(decimal price, string from, string to, DateTime time_start, TimeSpan time_way)
        {
            TimeSpan duration = time_way;

            Price.Text = price.ToString();
            From.Text = from;
            To.Text = to;
            TimeStart.Text = time_start.ToString("HH:mm");
            TimeWay.Text = $"Время в пути: {time_way}";
            DateStart.Text = time_start.ToString("dd MMMM yyyy");
            string[] ArriveDate = time_start.Add(duration).ToString("dd.MMMM.yyyy HH:mm").Split(' ');
            TimeArrival.Text = ArriveDate[1];
            DateEnd.Text = ArriveDate[0].Replace('.', ' ');
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
