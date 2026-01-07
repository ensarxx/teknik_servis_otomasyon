using MySql.Data.MySqlClient;
using System.Data;

namespace TeknikServisOtomasyon.Data
{
    public class DatabaseConnection
    {
        private static DatabaseConnection? _instance;
        private readonly string _connectionString;

        private DatabaseConnection()
        {
            // Connection string - MySQL için
            // Pwd= kısmına MySQL şifrenizi yazın
            _connectionString = "Server=localhost;Port=3306;Database=teknik_servis_db;Uid=root;Pwd=root;Charset=utf8mb4;";
        }

        public static DatabaseConnection Instance
        {
            get
            {
                _instance ??= new DatabaseConnection();
                return _instance;
            }
        }

        public string ConnectionString => _connectionString;

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public bool TestConnection()
        {
            try
            {
                using var connection = GetConnection();
                connection.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void UpdateConnectionString(string server, string database, string userId, string password)
        {
            // Yeni bir instance oluşturmak yerine, bağlantı bilgilerini güncelle
            var newConnectionString = $"Server={server};Database={database};Uid={userId};Pwd={password};CharSet=utf8mb4;";
            _instance = new DatabaseConnection(newConnectionString);
        }

        private DatabaseConnection(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}
