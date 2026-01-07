using Dapper;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using TeknikServisOtomasyon.Models;

namespace TeknikServisOtomasyon.Data.Repositories
{
    public class EmailAyarlariRepository
    {
        private readonly string _connectionString;

        public EmailAyarlariRepository()
        {
            _connectionString = DatabaseConnection.Instance.ConnectionString;
        }

        public async Task<EmailAyarlari?> GetAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = "SELECT * FROM email_ayarlari LIMIT 1";
            return await connection.QueryFirstOrDefaultAsync<EmailAyarlari>(query);
        }

        public async Task SaveAsync(EmailAyarlari ayarlar)
        {
            using var connection = new MySqlConnection(_connectionString);
            
            // Önce mevcut kayıt var mı kontrol et
            var mevcut = await GetAsync();
            
            if (mevcut == null)
            {
                var insertQuery = @"INSERT INTO email_ayarlari 
                    (SmtpServer, SmtpPort, GondericiEmail, GondericiSifre, GondericiAdi, SslKullan, EmailAktif, GuncellemeTarihi) 
                    VALUES (@SmtpServer, @SmtpPort, @GondericiEmail, @GondericiSifre, @GondericiAdi, @SslKullan, @EmailAktif, @GuncellemeTarihi)";
                await connection.ExecuteAsync(insertQuery, ayarlar);
            }
            else
            {
                var updateQuery = @"UPDATE email_ayarlari SET 
                    SmtpServer = @SmtpServer,
                    SmtpPort = @SmtpPort,
                    GondericiEmail = @GondericiEmail,
                    GondericiSifre = @GondericiSifre,
                    GondericiAdi = @GondericiAdi,
                    SslKullan = @SslKullan,
                    EmailAktif = @EmailAktif,
                    GuncellemeTarihi = @GuncellemeTarihi
                    WHERE Id = @Id";
                ayarlar.Id = mevcut.Id;
                await connection.ExecuteAsync(updateQuery, ayarlar);
            }
        }
    }
}
