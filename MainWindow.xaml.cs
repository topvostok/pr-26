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

            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            {
                MessageBox.Show("Укажите города отправления и назначения", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return ticketsClasses;
            }

            string connectionString = "server=127.0.0.1;port=3306;uid=root;pwd=;database=Airlines;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    var parts = new List<string>();

                    // Билеты ТУДА
                    if (tuda.HasValue)
                    {
                        parts.Add("SELECT price, `from`, `to`, time_start, time_way " +
                                  "FROM Tickets " +
                                  "WHERE `from` = @from AND `to` = @to AND DATE(time_start) = @date_tuda");
                    }

                    // Билеты ОБРАТНО — используем @from2 и @to2
                    if (obratno.HasValue)
                    {
                        parts.Add("SELECT price, `from`, `to`, time_start, time_way " +
                                  "FROM Tickets " +
                                  "WHERE `from` = @to2 AND `to` = @from2 AND DATE(time_start) = @date_obratno");
                    }

                    // Если даты не выбраны
                    if (!tuda.HasValue && !obratno.HasValue)
                    {
                        parts.Add("SELECT price, `from`, `to`, time_start, time_way " +
                                  "FROM Tickets " +
                                  "WHERE (`from` = @from AND `to` = @to) OR (`from` = @to AND `to` = @from)");
                    }

                    string query = string.Join(" UNION ALL ", parts) + " ORDER BY time_start";

                    MySqlCommand command = new MySqlCommand(query, connection);

                    command.Parameters.AddWithValue("@from", from);
                    command.Parameters.AddWithValue("@to", to);

                    // Отдельные параметры для обратного рейса
                    if (obratno.HasValue)
                    {
                        command.Parameters.AddWithValue("@from2", from);
                        command.Parameters.AddWithValue("@to2", to);
                        command.Parameters.AddWithValue("@date_obratno", obratno.Value.ToString("yyyy-MM-dd"));
                    }

                    if (tuda.HasValue)
                        command.Parameters.AddWithValue("@date_tuda", tuda.Value.ToString("yyyy-MM-dd"));

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            decimal price = reader.IsDBNull(0) ? 0m : reader.GetDecimal(0);
                            string fromCity = reader.IsDBNull(1) ? "" : reader.GetString(1);
                            string toCity = reader.IsDBNull(2) ? "" : reader.GetString(2);
                            DateTime timeStart = DateTime.MinValue;
                            TimeSpan timeWay = TimeSpan.Zero;

                            if (!reader.IsDBNull(3))
                            {
                                try { timeStart = reader.GetDateTime(3); }
                                catch
                                {
                                    if (reader.GetFieldType(3) == typeof(TimeSpan))
                                        timeStart = DateTime.Today.Add(reader.GetTimeSpan(3));
                                }
                            }

                            if (!reader.IsDBNull(4))
                                timeWay = reader.GetTimeSpan(4);

                            ticketsClasses.Add(new TicketClass(price, fromCity, toCity, timeStart, timeWay));
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return ticketsClasses;
        }
    }
}