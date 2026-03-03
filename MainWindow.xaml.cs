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
using pg_26.Classes;
using pg_26.Elements;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace pg_26
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow mainWindow;
        public List<TicketClass> ticketsClasses = new List<TicketClass>();

        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;
            freme.Navigate(new Pages.Main());
        }

        public List<TicketClass> LoadTickets(string from, string to, DateTime? tuda, DateTime? obratno)
        {
            ticketsClasses.Clear();

            string connectionString = "server=127.0.0.1;port=3306;uid=root;pwd=;database=Airlines;"; // Изменил database на Airlines

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Базовый запрос для поиска билетов "туда"
                    string query = "SELECT price, `from`, `to`, time_start, time_way " +
                                  "FROM Tickets " +
                                  "WHERE (`from` = @from AND `to` = @to";

                    // Добавляем условие по дате, если она указана
                    if (tuda.HasValue)
                    {
                        query += " AND DATE(time_start) = @date_tuda";
                    }

                    query += ")";

                    // Добавляем запрос для обратных билетов, если указана дата обратно
                    if (obratno.HasValue)
                    {
                        query += " UNION ALL " +
                                "SELECT price, `from`, `to`, time_start, time_way " +
                                "FROM Tickets " +
                                "WHERE (`from` = @to_back AND `to` = @from_back";

                        if (obratno.HasValue)
                        {
                            query += " AND DATE(time_start) = @date_obratno";
                        }

                        query += ")";
                    }

                    MySqlCommand command = new MySqlCommand(query, connection);

                    // Параметры для первого запроса (туда)
                    command.Parameters.AddWithValue("@from", from);
                    command.Parameters.AddWithValue("@to", to);

                    if (tuda.HasValue)
                    {
                        command.Parameters.AddWithValue("@date_tuda", tuda.Value.ToString("yyyy-MM-dd"));
                    }

                    // Параметры для второго запроса (обратно)
                    if (obratno.HasValue)
                    {
                        command.Parameters.AddWithValue("@from_back", to);
                        command.Parameters.AddWithValue("@to_back", from);
                        command.Parameters.AddWithValue("@date_obratno", obratno.Value.ToString("yyyy-MM-dd"));
                    }

                    using (MySqlDataReader ticket_query = command.ExecuteReader())
                    {
                        while (ticket_query.Read())
                        {
                            decimal price = ticket_query.IsDBNull(0) ? 0 : ticket_query.GetDecimal(0);
                            string fromCity = ticket_query.IsDBNull(1) ? "" : ticket_query.GetString(1);
                            string toCity = ticket_query.IsDBNull(2) ? "" : ticket_query.GetString(2);

                            DateTime timeStart = DateTime.MinValue;
                            TimeSpan timeWay = TimeSpan.Zero;

                            if (!ticket_query.IsDBNull(3))
                            {
                                try
                                {
                                    timeStart = ticket_query.GetDateTime(3);
                                }
                                catch
                                {
                                    if (ticket_query.GetFieldType(3) == typeof(TimeSpan))
                                        timeStart = DateTime.Today.Add(ticket_query.GetTimeSpan(3));
                                }
                            }

                            if (!ticket_query.IsDBNull(4))
                            {
                                timeWay = ticket_query.GetTimeSpan(4);
                            }

                            TicketClass tickets = new TicketClass(
                                price,
                                fromCity,
                                toCity,
                                timeStart,
                                timeWay
                            );

                            ticketsClasses.Add(tickets);
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}\n\nПроверьте:\n1. Запущен ли MySQL сервер\n2. Правильность строки подключения\n3. Существует ли база данных Airlines",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }

            return ticketsClasses;
        }
    }
}