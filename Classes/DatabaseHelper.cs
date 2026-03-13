using System;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace WpfApp1.Classes
{
    /// <summary>
    /// Отдельный класс для управления подключением к MySQL.
    /// Измените ConnectionString под свои настройки сервера.
    /// </summary>
    public static class DatabaseHelper
    {
        // ───── НАСТРОЙКИ ПОДКЛЮЧЕНИЯ ─────────────────────────────
        private const string Server = "127.0.0.1";
        private const string Port = "3306";
        private const string Database = "AlpineRental";
        private const string User = "root";
        private const string Password = "";
        // ─────────────────────────────────────────────────────────

        public static string ConnectionString =>
            $"server={Server};port={Port};uid={User};pwd={Password};database={Database};charset=utf8mb4;";

        /// <summary>Возвращает открытое соединение. Закрывайте через using.</summary>
        public static MySqlConnection GetConnection()
        {
            var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        /// <summary>Проверяет доступность базы данных.</summary>
        public static bool TestConnection(out string error)
        {
            error = string.Empty;
            try
            {
                using (GetConnection()) return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}
