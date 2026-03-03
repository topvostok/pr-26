using System;
using System.Collections.Generic;
using System.Windows;
using pg_26.Classes;
using MySql.Data.MySqlClient;

namespace pg_26
{

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

            string connectionString = "server=127.0.0.1;port=3306;uid=root;pwd=;database=Airlines;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Формируем запрос для поиска билетов "туда"
                    string query = "";

                    if (tuda.HasValue)
                    {
                        // Добавляем билеты "туда" (from -> to) на указанную дату
                        query += "SELECT price, `from`, `to`, time_start, time_way " +
                                "FROM Tickets " +
                                $"WHERE `from` = @from AND `to` = @to AND DATE(time_start) = @date_tuda ";
                    }

                    if (obratno.HasValue)
                    {
                        // Если есть обратные билеты и уже есть запрос, добавляем UNION
                        if (!string.IsNullOrEmpty(query))
                        {
                            query += " UNION ALL ";
                        }

                        // Добавляем билеты "обратно" (to -> from) на указанную дату
                        query += "SELECT price, `from`, `to`, time_start, time_way " +
                                "FROM Tickets " +
                                $"WHERE `from` = @to_back AND `to` = @from_back AND DATE(time_start) = @date_obratno ";
                    }

                    // Если ни одной даты не выбрано, показываем все билеты по направлению
                    if (!tuda.HasValue && !obratno.HasValue)
                    {
                        query = "SELECT price, `from`, `to`, time_start, time_way " +
                               "FROM Tickets " +
                               "WHERE (`from` = @from AND `to` = @to) OR (`from` = @to_back AND `to` = @from_back) ";
                    }

                    // Добавляем сортировку по дате и времени
                    query += " ORDER BY time_start";

                    MySqlCommand command = new MySqlCommand(query, connection);

                    // Добавляем параметры
                    command.Parameters.AddWithValue("@from", from);
                    command.Parameters.AddWithValue("@to", to);
                    command.Parameters.AddWithValue("@to_back", to);
                    command.Parameters.AddWithValue("@from_back", from);

                    if (tuda.HasValue)
                    {
                        command.Parameters.AddWithValue("@date_tuda", tuda.Value.ToString("yyyy-MM-dd"));
                    }

                    if (obratno.HasValue)
                    {
                        command.Parameters.AddWithValue("@date_obratno", obratno.Value.ToString("yyyy-MM-dd"));
                    }

                    using (MySqlDataReader ticket_query = command.ExecuteReader())
                    {
                        while (ticket_query.Read())
                        {
                            decimal price = ticket_query.IsDBNull(0) ? 0m : ticket_query.GetDecimal(0);
                            string priceString = price.ToString("F2");
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